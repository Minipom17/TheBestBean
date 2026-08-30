using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TheBestBean.Data;
using TheBestBean.Models;

namespace TheBestBean.Pages.Admin.Blog
{
    public class EditModel : PageModel
    {
        private readonly TheBestBeanContext _context;

        public EditModel(TheBestBeanContext context)
        {
            _context = context;
        }

        [BindProperty]
        public BlogPost BlogPost { get; set; } = default!;

        public bool IsNew => HttpContext.Request.RouteValues["id"] == null;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                BlogPost = new BlogPost
                {
                    CreatedAt = DateTime.UtcNow,
                    IsPublished = false,
                    Author = User.Identity?.Name ?? "Admin"
                };
                return Page();
            }

            var post = await _context.BlogPosts.FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            BlogPost = post;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (BlogPost.Id == 0)
            {
                if (BlogPost.IsPublished)
                {
                    BlogPost.PublishedAt = DateTime.UtcNow;
                }
                _context.BlogPosts.Add(BlogPost);
            }
            else
            {
                var existingPost = await _context.BlogPosts.FirstOrDefaultAsync(m => m.Id == BlogPost.Id);
                if (existingPost == null)
                {
                    return NotFound();
                }

                existingPost.Title = BlogPost.Title;
                existingPost.Slug = BlogPost.Slug;
                existingPost.Content = BlogPost.Content;
                existingPost.ImageUrl = BlogPost.ImageUrl;
                existingPost.Author = BlogPost.Author;
                existingPost.UpdatedAt = DateTime.UtcNow;
                
                if (BlogPost.IsPublished && !existingPost.IsPublished)
                {
                    existingPost.PublishedAt = DateTime.UtcNow;
                }
                existingPost.IsPublished = BlogPost.IsPublished;
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
        
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var post = await _context.BlogPosts.FindAsync(id);
            if (post != null)
            {
                _context.BlogPosts.Remove(post);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage("./Index");
        }
    }
}
