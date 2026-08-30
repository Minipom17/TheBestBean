using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TheBestBean.Data;
using TheBestBean.Models;

namespace TheBestBean.Pages_Admin_Experiences
{
    public class CreateModel : PageModel
    {
        private readonly TheBestBean.Data.TheBestBeanContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostEnvironment;

        public CreateModel(TheBestBean.Data.TheBestBeanContext context, Microsoft.AspNetCore.Hosting.IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        [BindProperty]
        public IFormFile? ImageUpload { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Experience Experience { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (ImageUpload != null && ImageUpload.Length > 0)
            {
                var fileName = Path.GetFileNameWithoutExtension(ImageUpload.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(ImageUpload.FileName);
                var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "Media");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageUpload.CopyToAsync(fileStream);
                }
                Experience.ImageUrl = "/Media/" + fileName;
            }

            _context.Experiences.Add(Experience);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
