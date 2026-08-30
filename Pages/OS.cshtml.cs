using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TheBestBean.Data;
using TheBestBean.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace TheBestBean.Pages
{
    [IgnoreAntiforgeryToken]
    public class OSModel : PageModel
    {
        private readonly ILogger<OSModel> _logger;
        private readonly TheBestBeanContext _context;

        public OSModel(ILogger<OSModel> logger, TheBestBeanContext context)
        {
            _logger = logger;
            _context = context;
        }

        public void OnGet()
        {
            // No additional logic needed for static content
        }

        // ========== INVENTORY API HANDLERS ==========

        public async Task<IActionResult> OnGetInventoryAsync()
        {
            var inventory = await _context.BeanInventories
                .Where(b => b.IsActive)
                .OrderByDescending(b => b.CreatedDate)
                .Select(b => new
                {
                    id = b.Id,
                    name = b.Name,
                    country = b.Country ?? "",
                    farmer = b.Farmer ?? "",
                    process = b.Process ?? "",
                    totalKg = b.TotalKg,
                    costPerKg = b.CostPerKg
                })
                .ToListAsync();

            return new JsonResult(inventory);
        }

        public async Task<IActionResult> OnPostSaveInventoryAsync()
        {
            if (!Request.HasFormContentType)
                return BadRequest("Form data is required");
            
            var form = await Request.ReadFormAsync();
            var name = form["name"].ToString();
            
            if (string.IsNullOrEmpty(name))
                return BadRequest("Name is required");

            var idStr = form["id"].ToString() ?? "";
            int id = int.TryParse(idStr, out var parsedId) ? parsedId : 0;

            BeanInventory bean;
            if (id > 0)
            {
                bean = await _context.BeanInventories.FindAsync(id);
                if (bean == null)
                    return NotFound();
            }
            else
            {
                bean = new BeanInventory();
                _context.BeanInventories.Add(bean);
            }

            bean.Name = name;
            bean.Country = form["country"].ToString();
            bean.Farmer = form["farmer"].ToString();
            bean.Process = form["process"].ToString();
            
            if (decimal.TryParse(form["totalKg"].ToString(), out var totalKg))
                bean.TotalKg = totalKg;
            
            if (decimal.TryParse(form["costPerKg"].ToString(), out var costPerKg))
                bean.CostPerKg = costPerKg;
            else
                bean.CostPerKg = null;

            bean.HasCertificates = form["hasCertificates"].ToString() == "true";
            
            // Handle certificate file upload
            var certificateFile = form.Files["certificate"];
            if (certificateFile != null && certificateFile.Length > 0)
            {
                var ext = Path.GetExtension(certificateFile.FileName).ToLower();
                var allowedExtensions = new[] { ".pdf", ".webp", ".webp", ".webp" };
                if (!allowedExtensions.Contains(ext))
                {
                    return BadRequest("Certificate must be a PDF, PNG, or JPEG file");
                }
                
                var uploadsRoot = Path.Combine("wwwroot", "uploads", "certificates");
                Directory.CreateDirectory(uploadsRoot);
                var fileName = $"cert_{Guid.NewGuid():N}{ext}";
                var filePath = Path.Combine(uploadsRoot, fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    await certificateFile.CopyToAsync(stream);
                }
                bean.CertificateFilePath = $"/uploads/certificates/{fileName}";
            }

            bean.LastUpdated = DateTime.Now;

            await _context.SaveChangesAsync();

            return new JsonResult(new { id = bean.Id, success = true });
        }

        // ========== ROAST BATCH API HANDLERS ==========

        public async Task<IActionResult> OnGetRoastsAsync()
        {
            var roasts = await _context.RoastBatches
                .Include(r => r.BeanInventory)
                .OrderByDescending(r => r.RoastDate)
                .Select(r => new
                {
                    id = r.Id,
                    beanId = r.BeanInventoryId,
                    beanName = r.BeanInventory.Name,
                    date = r.RoastDate.ToString("yyyy-MM-dd"),
                    roastTime = r.RoastTimeSeconds,
                    data = r.TemperatureDataJson,
                    events = r.EventsJson
                })
                .ToListAsync();

            return new JsonResult(roasts);
        }

        public async Task<IActionResult> OnPostSaveRoastAsync([FromBody] RoastDto dto)
        {
            if (dto.BeanId <= 0)
                return BadRequest("Bean ID is required");

            var bean = await _context.BeanInventories.FindAsync(dto.BeanId);
            if (bean == null)
                return NotFound("Bean not found");

            var roast = new RoastBatch
            {
                BeanInventoryId = dto.BeanId,
                RoastDate = DateTime.Now,
                RoastTimeSeconds = dto.RoastTimeSeconds,
                GreenWeightGrams = dto.GreenWeight,
                RoastedWeightGrams = dto.RoastedWeight,
                WeightLossPercent = dto.WeightLossPercent,
                DTRPercent = dto.DTRPercent,
                FirstCrackTime = dto.FirstCrackTime,
                DropTime = dto.DropTime,
                TurnPointTime = dto.TurnPointTime,
                DryEndTime = dto.DryEndTime,
                TemperatureDataJson = JsonSerializer.Serialize(dto.TemperatureData ?? new List<object>()),
                EventsJson = JsonSerializer.Serialize(dto.Events ?? new List<object>()),
                Notes = dto.Notes
            };

            _context.RoastBatches.Add(roast);
            await _context.SaveChangesAsync();

            return new JsonResult(new { id = roast.Id, success = true });
        }

        // ========== CUPPING SCORE API HANDLERS ==========

        public async Task<IActionResult> OnGetCuppingScoresAsync(int? roastBatchId)
        {
            var query = _context.CuppingScores.AsQueryable();
            
            if (roastBatchId.HasValue)
                query = query.Where(c => c.RoastBatchId == roastBatchId.Value);

            var scores = await query
                .Include(c => c.RoastBatch)
                .ThenInclude(r => r.BeanInventory)
                .OrderByDescending(c => c.CuppingDate)
                .Select(c => new
                {
                    id = c.Id,
                    roastBatchId = c.RoastBatchId,
                    cupperName = c.CupperName,
                    cuppingDate = c.CuppingDate.ToString("yyyy-MM-dd"),
                    lotNumber = c.LotNumber,
                    finalScore = c.FinalScore
                })
                .ToListAsync();

            return new JsonResult(scores);
        }

        public async Task<IActionResult> OnGetCheckCuppingAsync()
        {
            var count = await _context.CuppingScores.CountAsync();
            var recent = await _context.CuppingScores
                .OrderByDescending(c => c.CuppingDate)
                .Take(20)
                .Select(c => new
                {
                    id = c.Id,
                    date = c.CuppingDate.ToString("yyyy-MM-dd"),
                    cupper = c.CupperName ?? "N/A",
                    finalScore = c.FinalScore,
                    roastBatchId = c.RoastBatchId
                })
                .ToListAsync();
            
            return new JsonResult(new { total = count, recent = recent });
        }

        public async Task<IActionResult> OnPostSaveCuppingAsync([FromBody] CuppingDto dto)
        {
            if (dto.RoastBatchId <= 0)
                return BadRequest("Roast Batch ID is required");

            var roastBatch = await _context.RoastBatches.FindAsync(dto.RoastBatchId);
            if (roastBatch == null)
                return NotFound("Roast batch not found");

            var cupping = new CuppingScore
            {
                RoastBatchId = dto.RoastBatchId,
                CupperName = dto.CupperName,
                CuppingDate = DateTime.Now,
                LotNumber = dto.LotNumber,
                RoastLevel = dto.RoastLevel,
                FragranceDry = dto.FragranceDry,
                FragranceBreak = dto.FragranceBreak,
                FragranceScore = dto.FragranceScore,
                FlavorScore = dto.FlavorScore,
                AftertasteScore = dto.AftertasteScore,
                AcidityScore = dto.AcidityScore,
                AcidityIntensity = dto.AcidityIntensity,
                BodyScore = dto.BodyScore,
                BodyIntensity = dto.BodyIntensity,
                BalanceScore = dto.BalanceScore,
                UniformityChecks = dto.UniformityChecks,
                SweetnessChecks = dto.SweetnessChecks,
                CleanCupChecks = dto.CleanCupChecks,
                OverallScore = dto.OverallScore,
                DefectCups = dto.DefectCups,
                FinalScore = dto.FinalScore,
                Notes = dto.Notes
            };

            _context.CuppingScores.Add(cupping);
            await _context.SaveChangesAsync();

            return new JsonResult(new { id = cupping.Id, success = true });
        }
    }

    // DTOs for API requests
    public class InventoryDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("country")]
        public string? Country { get; set; }
        [JsonPropertyName("farmer")]
        public string? Farmer { get; set; }
        [JsonPropertyName("process")]
        public string? Process { get; set; }
        [JsonPropertyName("totalKg")]
        public decimal TotalKg { get; set; }
        [JsonPropertyName("costPerKg")]
        public decimal? CostPerKg { get; set; }
    }

    public class RoastDto
    {
        public int BeanId { get; set; }
        public int? RoastTimeSeconds { get; set; }
        public decimal? GreenWeight { get; set; }
        public decimal? RoastedWeight { get; set; }
        public decimal? WeightLossPercent { get; set; }
        public decimal? DTRPercent { get; set; }
        public int? FirstCrackTime { get; set; }
        public int? DropTime { get; set; }
        public int? TurnPointTime { get; set; }
        public int? DryEndTime { get; set; }
        public List<object>? TemperatureData { get; set; }
        public List<object>? Events { get; set; }
        public string? Notes { get; set; }
    }

    public class CuppingDto
    {
        public int RoastBatchId { get; set; }
        public string? CupperName { get; set; }
        public string? LotNumber { get; set; }
        public string? RoastLevel { get; set; }
        public decimal? FragranceDry { get; set; }
        public decimal? FragranceBreak { get; set; }
        public decimal? FragranceScore { get; set; }
        public decimal? FlavorScore { get; set; }
        public decimal? AftertasteScore { get; set; }
        public decimal? AcidityScore { get; set; }
        public decimal? AcidityIntensity { get; set; }
        public decimal? BodyScore { get; set; }
        public decimal? BodyIntensity { get; set; }
        public decimal? BalanceScore { get; set; }
        public int? UniformityChecks { get; set; }
        public int? SweetnessChecks { get; set; }
        public int? CleanCupChecks { get; set; }
        public decimal? OverallScore { get; set; }
        public int? DefectCups { get; set; }
        public decimal? FinalScore { get; set; }
        public string? Notes { get; set; }
    }
}

