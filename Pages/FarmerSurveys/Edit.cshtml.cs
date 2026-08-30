using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheBestBean.Data;
using TheBestBean.Models;
using TheBestBean.Services;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace TheBestBean.Pages.FarmerSurveys
{
    public class EditModel : PageModel
    {
        private readonly TheBestBeanContext _context;
        private readonly FarmProfileService _farmProfileService;

        public EditModel(TheBestBeanContext context, FarmProfileService farmProfileService)
        {
            _context = context;
            _farmProfileService = farmProfileService;
        }

        [BindProperty]
        public FarmerSurvey FarmerSurvey { get; set; } = default!;

        [BindProperty]
        public IFormFile? Upload { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var farmersurvey = await _context.FarmerSurveys.FirstOrDefaultAsync(m => m.Id == id);
            if (farmersurvey == null)
            {
                return NotFound();
            }
            FarmerSurvey = farmersurvey;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // optional new photo
            if (Upload != null && Upload.Length > 0)
            {
                var uploadsRoot = Path.Combine("wwwroot", "uploads", "surveys");
                Directory.CreateDirectory(uploadsRoot);
                var ext = Path.GetExtension(Upload.FileName).ToLower();
                var allowed = new[] { ".webp", ".webp", ".webp", ".webp" };
                if (!allowed.Contains(ext))
                {
                    ModelState.AddModelError(string.Empty, "Invalid image type. Use JPG, PNG, or WEBP.");
                    return Page();
                }
                var fileName = $"survey_{Guid.NewGuid():N}{ext}";
                var filePath = Path.Combine(uploadsRoot, fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    await Upload.CopyToAsync(stream);
                }
                FarmerSurvey.SurveyPhotoPath = $"/uploads/surveys/{fileName}";
            }

            _context.Attach(FarmerSurvey).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                
                // Update farm profile from modified survey
                await _farmProfileService.UpdateFarmProfileFromSurvey(FarmerSurvey.Id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FarmerSurveyExists(FarmerSurvey.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Details", new { id = FarmerSurvey.Id });
        }

        private bool FarmerSurveyExists(int id)
        {
            return (_context.FarmerSurveys?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
