using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TheBestBean.Data;
using TheBestBean.Models;

namespace TheBestBean.Pages
{
    public class PostModel : PageModel
    {
        private readonly TheBestBeanContext _context;

        public PostModel(TheBestBeanContext context)
        {
            _context = context;
        }

        public BlogPost BlogPost { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var post = await _context.BlogPosts
                .FirstOrDefaultAsync(m => m.Slug == slug && m.IsPublished);

            if (post == null)
            {
                return NotFound();
            }

            BlogPost = post;
            return Page();
        }
    }
}
