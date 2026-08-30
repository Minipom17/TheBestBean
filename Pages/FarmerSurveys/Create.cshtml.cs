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
    public class CreateModel : PageModel
    {
        private readonly TheBestBeanContext _context;
        private readonly FarmProfileService _farmProfileService;

        public CreateModel(TheBestBeanContext context, FarmProfileService farmProfileService)
        {
            _context = context;
            _farmProfileService = farmProfileService;
        }

        public IActionResult OnGet()
        {
            FarmerSurvey = new FarmerSurvey { SurveyDate = DateTime.Today }; // Pre-fill date
            return Page();
        }

        [BindProperty]
        public FarmerSurvey FarmerSurvey { get; set; } = default!;

        [BindProperty]
        public IFormFile? Upload { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.FarmerSurveys == null || FarmerSurvey == null)
            {
                return Page();
            }

            // handle optional photo upload
            if (Upload != null && Upload.Length > 0)
            {
                var uploadsRoot = Path.Combine("wwwroot", "uploads", "surveys");
                Directory.CreateDirectory(uploadsRoot);
                var ext = Path.GetExtension(Upload.FileName);
                var allowed = new[] { ".webp", ".webp", ".webp", ".webp" };
                if (!allowed.Contains(ext.ToLower()))
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

            _context.FarmerSurveys.Add(FarmerSurvey);
            await _context.SaveChangesAsync();

            // Auto-create farm profile from survey
            await _farmProfileService.CreateFarmProfileFromSurvey(FarmerSurvey.Id);

            return RedirectToPage("./Index");
        }
    }
}
