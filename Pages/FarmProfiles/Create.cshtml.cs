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
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.IO;

namespace TheBestBean.Pages.FarmProfiles
{
    public class CreateModel : PageModel
    {
        private readonly TheBestBeanContext _context;

        public CreateModel(TheBestBeanContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            PopulateSurveys();
            return Page();
        }

        [BindProperty]
        public FarmProfile FarmProfile { get; set; } = new FarmProfile();

        [BindProperty]
        public List<IFormFile> Uploads { get; set; } = new List<IFormFile>();

        public List<FarmerSurvey> AvailableSurveys { get; set; } = new List<FarmerSurvey>();
        public SelectList SurveySelectList { get; set; } = default!;

        private void PopulateSurveys()
        {
            if (_context.FarmerSurveys != null)
            {
                AvailableSurveys = _context.FarmerSurveys
                    .OrderByDescending(s => s.SurveyDate)
                    .ToList();

                SurveySelectList = new SelectList(AvailableSurveys, "Id", "FarmName");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                PopulateSurveys();
                return Page();
            }

            // Set creation and update dates
            FarmProfile.CreatedDate = DateTime.Now;
            FarmProfile.LastUpdated = DateTime.Now;
            FarmProfile.IsActive = true;

            // Handle uploads -> save to wwwroot/uploads/farms and store JSON array of paths
            var photoPaths = new List<string>();
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
                    photoPaths.Add($"/uploads/farms/{fileName}");
                }
            }

            FarmProfile.FarmPhotos = JsonSerializer.Serialize(photoPaths);

            // Auto-populate from related survey if selected
            if (FarmProfile.RelatedSurveyId.HasValue)
            {
                await PopulateFromSurvey(FarmProfile.RelatedSurveyId.Value);
            }

            _context.FarmProfiles.Add(FarmProfile);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Profile", new { id = FarmProfile.Id });
        }

        private async Task PopulateFromSurvey(int surveyId)
        {
            var survey = await _context.FarmerSurveys.FindAsync(surveyId);
            if (survey != null)
            {
                // Auto-populate basic fields
                FarmProfile.FarmName = survey.FarmName;
                FarmProfile.FarmerName = survey.FarmerName;
                FarmProfile.Region = survey.Region;
                FarmProfile.GPSLatitude = ExtractLatitude(survey.GpsCoordinates);
                FarmProfile.GPSLongitude = ExtractLongitude(survey.GpsCoordinates);
                FarmProfile.QualityRating = survey.QualityRating;

                // Coffee information
                if (string.IsNullOrEmpty(FarmProfile.CoffeeVarieties) && !string.IsNullOrEmpty(survey.VarietiesGrown))
                    FarmProfile.CoffeeVarieties = survey.VarietiesGrown;
                
                if (string.IsNullOrEmpty(FarmProfile.ProcessingMethod) && !string.IsNullOrEmpty(survey.ProcessingMethod))
                    FarmProfile.ProcessingMethod = survey.ProcessingMethod;

                // Create story from survey data
                if (string.IsNullOrEmpty(FarmProfile.FarmStory))
                {
                    FarmProfile.FarmStory = CreateStoryFromSurvey(survey);
                }

                // Environmental features
                if (string.IsNullOrEmpty(FarmProfile.EnvironmentalFeatures))
                {
                    FarmProfile.EnvironmentalFeatures = CreateEnvironmentalDescription(survey);
                }
            }
        }

        private string? ExtractLatitude(string? coordinates)
        {
            if (string.IsNullOrEmpty(coordinates)) return null;
            var parts = coordinates.Split(',');
            return parts.Length > 0 ? parts[0].Trim() : null;
        }

        private string? ExtractLongitude(string? coordinates)
        {
            if (string.IsNullOrEmpty(coordinates)) return null;
            var parts = coordinates.Split(',');
            return parts.Length > 1 ? parts[1].Trim() : null;
        }

        private string CreateStoryFromSurvey(FarmerSurvey survey)
        {
            var story = $"Located in {survey.Region}, {survey.FarmName} is managed by {survey.FarmerName}. ";
            
            if (survey.ElevationMeters > 0)
                story += $"The farm sits at {survey.ElevationMeters} meters above sea level, ";
            
            if (survey.TotalFarmSizeHa > 0)
                story += $"covering {survey.TotalFarmSizeHa} hectares total with {survey.CoffeePlantedAreaHa} hectares dedicated to coffee production. ";
            
            if (!string.IsNullOrEmpty(survey.UniqueQualities))
                story += $"Special characteristics of this farm include: {survey.UniqueQualities}. ";
            
            return story.Trim();
        }

        private string CreateEnvironmentalDescription(FarmerSurvey survey)
        {
            var description = "";
            
            if (!string.IsNullOrEmpty(survey.SlopeType))
                description += $"The farm features {survey.SlopeType.ToLower()} terrain. ";
            
            if (!string.IsNullOrEmpty(survey.Aspect))
                description += $"It benefits from {survey.Aspect.ToLower()} exposure. ";
            
            if (!string.IsNullOrEmpty(survey.SoilType))
                description += $"The soil consists primarily of {survey.SoilType.ToLower()}. ";
            
            if (!string.IsNullOrEmpty(survey.WaterSource))
                description += $"Water is sourced from {survey.WaterSource.ToLower()}. ";
            
            if (!string.IsNullOrEmpty(survey.ShadeTrees))
                description += $"Shade is provided by {survey.ShadeTrees}. ";
            
            return description.Trim();
        }
    }
}




