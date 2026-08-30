using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TheBestBean.Data;
using TheBestBean.Models;

namespace TheBestBean.Pages.FarmProfiles
{
    public class EditModel : PageModel
    {
        private readonly TheBestBeanContext _context;

        public EditModel(TheBestBeanContext context)
        {
            _context = context;
        }

        [BindProperty]
        public FarmProfile FarmProfile { get; set; } = default!;

        [BindProperty]
        public List<IFormFile> Uploads { get; set; } = new List<IFormFile>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fp = await _context.FarmProfiles.FirstOrDefaultAsync(m => m.Id == id);
            if (fp == null)
            {
                return NotFound();
            }
            FarmProfile = fp;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Append any new uploads
            var existing = new List<string>();
            if (!string.IsNullOrWhiteSpace(FarmProfile.FarmPhotos))
            {
                try { existing = JsonSerializer.Deserialize<List<string>>(FarmProfile.FarmPhotos) ?? new List<string>(); }
                catch { existing = new List<string>(); }
            }

            if (Uploads != null && Uploads.Any())
            {
                var uploadsRoot = Path.Combine("wwwroot", "uploads", "farms");
                Directory.CreateDirectory(uploadsRoot);
                var allowed = new[] { ".webp", ".webp", ".webp", ".webp" };
                foreach (var file in Uploads)
                {
                    if (file == null || file.Length == 0) continue;
                    var ext = Path.GetExtension(file.FileName).ToLower();
                    if (!allowed.Contains(ext)) continue;
                    var fileName = $"farm_{Guid.NewGuid():N}{ext}";
                    var filePath = Path.Combine(uploadsRoot, fileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await file.CopyToAsync(stream);
                    }
                    existing.Add($"/uploads/farms/{fileName}");
                }
            }

            FarmProfile.FarmPhotos = JsonSerializer.Serialize(existing);
            FarmProfile.LastUpdated = DateTime.Now;

            _context.Attach(FarmProfile).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToPage("./Profile", new { id = FarmProfile.Id });
        }
    }
}


