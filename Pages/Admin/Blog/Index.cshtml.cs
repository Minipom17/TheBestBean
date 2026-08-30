using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TheBestBean.Data;
using TheBestBean.Models;
using System.Linq;

namespace TheBestBean.Pages.Admin.Blog
{
    public class IndexModel : PageModel
    {
        private readonly TheBestBeanContext _context;

        public IndexModel(TheBestBeanContext context)
        {
            _context = context;
        }

        public IList<BlogPost> BlogPosts { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.BlogPosts != null)
            {
                BlogPosts = await _context.BlogPosts
                    .OrderByDescending(b => b.CreatedAt)
                    .ToListAsync();
            }
        }
    }
}
