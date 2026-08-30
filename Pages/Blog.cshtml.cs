using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TheBestBean.Data;
using TheBestBean.Models;

namespace TheBestBean.Pages
{
    public class BlogModel : PageModel
    {
        private readonly TheBestBeanContext _context;

        public BlogModel(TheBestBeanContext context)
        {
            _context = context;
        }

        public IList<BlogPost> BlogPosts { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.BlogPosts != null)
            {
                BlogPosts = await _context.BlogPosts
                    .Where(b => b.IsPublished)
                    .OrderByDescending(b => b.PublishedAt)
                    .ToListAsync();
            }
            else
            {
                BlogPosts = new List<BlogPost>();
            }
        }
    }
}
