using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TheBestBean.Data;
using TheBestBean.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace TheBestBean.Pages.FarmProfiles
{
    public class ProfileModel : PageModel
    {
        private readonly TheBestBeanContext _context;

        public ProfileModel(TheBestBeanContext context)
        {
            _context = context;
        }

        public FarmProfile FarmProfile { get; set; } = default!;

        [BindProperty]
        public IFormFile? Upload { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var farmProfile = await _context.FarmProfiles
                .Include(fp => fp.RelatedSurvey)
                .FirstOrDefaultAsync(m => m.Id == id && m.IsActive);

            if (farmProfile == null)
            {
                return NotFound();
            }

            FarmProfile = farmProfile;
            return Page();
        }

        public List<string> ParseFarmPhotos()
        {
            if (string.IsNullOrEmpty(FarmProfile.FarmPhotos))
            {
                return new List<string>();
            }

            try
            {
                return JsonSerializer.Deserialize<List<string>>(FarmProfile.FarmPhotos) ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        public async Task<IActionResult> OnPostUploadAsync(int id)
        {
            var farm = await _context.FarmProfiles.FirstOrDefaultAsync(f => f.Id == id);
            if (farm == null) return NotFound();

            if (Upload != null && Upload.Length > 0)
            {
                var uploadsRoot = Path.Combine("wwwroot", "uploads", "farms");
                Directory.CreateDirectory(uploadsRoot);
                var ext = Path.GetExtension(Upload.FileName).ToLower();
                var allowed = new[] { ".webp", ".webp", ".webp", ".webp" };
                if (!allowed.Contains(ext))
                {
                    TempData["UploadError"] = "Unsupported image type.";
                    return RedirectToPage(new { id });
                }
                var fileName = $"farm_{Guid.NewGuid():N}{ext}";
                var filePath = Path.Combine(uploadsRoot, fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    await Upload.CopyToAsync(stream);
                }
                var list = new List<string>();
                try { list = string.IsNullOrWhiteSpace(farm.FarmPhotos) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(farm.FarmPhotos) ?? new List<string>(); }
                catch { list = new List<string>(); }
                list.Add($"/uploads/farms/{fileName}");
                farm.FarmPhotos = JsonSerializer.Serialize(list);
                farm.LastUpdated = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(new { id });
        }
    }
}




