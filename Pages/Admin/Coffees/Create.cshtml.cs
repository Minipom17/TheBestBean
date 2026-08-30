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

namespace TheBestBean.Pages_Admin_Coffees
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
        ViewData["CoffeeFarmId"] = new SelectList(_context.CoffeeFarm, "Id", "Name");
        ViewData["CoffeeRegionId"] = new SelectList(_context.CoffeeRegion, "Id", "Name");
        ViewData["OriginCountryId"] = new SelectList(_context.OriginCountry, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public CoffeeBean CoffeeBean { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            
            if (ImageUpload != null)
            {
                var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "Media");
                Directory.CreateDirectory(uploadsFolder);
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageUpload.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageUpload.CopyToAsync(fileStream);
                }
                
                CoffeeBean.ImageUrl = "/Media/" + uniqueFileName;
            }
            
            _context.CoffeeBean.Add(CoffeeBean);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
