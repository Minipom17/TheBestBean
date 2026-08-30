using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheBestBean.Data;
using TheBestBean.Models;

namespace TheBestBean.Pages_Admin_Coffees
{
    public class EditModel : PageModel
    {
        private readonly TheBestBean.Data.TheBestBeanContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostEnvironment;

        public EditModel(TheBestBean.Data.TheBestBeanContext context, Microsoft.AspNetCore.Hosting.IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        [BindProperty]
        public CoffeeBean CoffeeBean { get; set; } = default!;

        [BindProperty]
        public IFormFile? ImageUpload { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coffeebean =  await _context.CoffeeBean.FirstOrDefaultAsync(m => m.Id == id);
            if (coffeebean == null)
            {
                return NotFound();
            }
            CoffeeBean = coffeebean;
           ViewData["CoffeeFarmId"] = new SelectList(_context.CoffeeFarm, "Id", "Name");
           ViewData["CoffeeRegionId"] = new SelectList(_context.CoffeeRegion, "Id", "Name");
           ViewData["OriginCountryId"] = new SelectList(_context.OriginCountry, "Id", "Name");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
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
            
            _context.Attach(CoffeeBean).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CoffeeBeanExists(CoffeeBean.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool CoffeeBeanExists(int id)
        {
            return _context.CoffeeBean.Any(e => e.Id == id);
        }
    }
}
