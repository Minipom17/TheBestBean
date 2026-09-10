using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Threading.Tasks;
using TheBestBean.Data;
using System.Xml.Linq;
using System.Linq;

namespace TheBestBean.Controllers
{
    [Route("sitemap.xml")]
    public class SitemapController : Controller
    {
        private readonly TheBestBeanContext _context;

        public SitemapController(TheBestBeanContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XNamespace imageNs = "http://www.google.com/schemas/sitemap-image/1.1";
            var urlset = new XElement(xmlns + "urlset",
                new XAttribute(XNamespace.Xmlns + "image", imageNs.NamespaceName));

            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";

            // Static pages
            var staticPages = new[]
            {
                ("/", "1.0", "weekly"),
                ("/Coffee", "0.9", "weekly"),
                ("/Experiences", "0.9", "weekly"),
                ("/BrewMethods", "0.8", "monthly"),
                ("/Wholesale", "0.8", "monthly"),
                ("/Blog", "0.8", "weekly"),
                ("/About", "0.7", "monthly"),
                ("/Resources", "0.7", "monthly"),
                ("/Subscriptions", "0.7", "monthly"),
                ("/SocialCoffee", "0.7", "monthly"),
                ("/OriginMap", "0.7", "monthly"),
                ("/Learn", "0.6", "monthly"),
                ("/BeanAtlas", "0.6", "monthly"),
                ("/RoastScience", "0.6", "monthly"),
                ("/CoffeeAlchemy", "0.6", "monthly"),
                ("/Newbie", "0.6", "monthly")
            };
            foreach (var (page, priority, changefreq) in staticPages)
            {
                urlset.Add(new XElement(xmlns + "url",
                    new XElement(xmlns + "loc", baseUrl + page),
                    new XElement(xmlns + "changefreq", changefreq),
                    new XElement(xmlns + "priority", priority)
                ));
            }

            // Products (CoffeeBeans)
            var products = await _context.CoffeeBean.ToListAsync();
            foreach (var product in products)
            {
                urlset.Add(new XElement(xmlns + "url",
                    new XElement(xmlns + "loc", $"{baseUrl}/Product/{product.Id}"),
                    new XElement(xmlns + "changefreq", "monthly"),
                    new XElement(xmlns + "priority", "0.8")
                ));
            }

            // Experiences (public URLs are /Tour/{id}, not /Experiences/{id})
            var experiences = await _context.Experiences
                .Where(e => e.Category != "Archived")
                .ToListAsync();
            foreach (var exp in experiences)
            {
                var loc = $"{baseUrl}/Tour/{exp.Id}";
                var urlEl = new XElement(xmlns + "url",
                    new XElement(xmlns + "loc", loc),
                    new XElement(xmlns + "changefreq", "weekly"),
                    new XElement(xmlns + "priority", "0.9")
                );
                if (!string.IsNullOrWhiteSpace(exp.ImageUrl))
                {
                    var imageLoc = exp.ImageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                        ? exp.ImageUrl
                        : baseUrl + (exp.ImageUrl.StartsWith('/') ? exp.ImageUrl : "/" + exp.ImageUrl);
                    urlEl.Add(new XElement(imageNs + "image",
                        new XElement(imageNs + "loc", imageLoc)));
                }
                urlset.Add(urlEl);
            }

            // Blog Posts
            var posts = await _context.BlogPosts.Where(p => p.IsPublished).ToListAsync();
            foreach (var post in posts)
            {
                urlset.Add(new XElement(xmlns + "url",
                    new XElement(xmlns + "loc", $"{baseUrl}/Post/{post.Slug}"),
                    new XElement(xmlns + "changefreq", "monthly"),
                    new XElement(xmlns + "priority", "0.7")
                ));
            }

            var doc = new XDocument(new XDeclaration("1.0", "UTF-8", null), urlset);

            return Content(doc.Declaration.ToString() + "\n" + doc.ToString(), "application/xml", Encoding.UTF8);
        }
    }
}
