using Microsoft.EntityFrameworkCore;
using TheBestBean.Data;
using TheBestBean.Models;

namespace TheBestBean.Services
{
    public class FarmProfileService
    {
        private readonly TheBestBeanContext _context;

        public FarmProfileService(TheBestBeanContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateFarmProfileFromSurvey(int surveyId)
        {
            var survey = await _context.FarmerSurveys.FindAsync(surveyId);
            if (survey == null) return false;

            // Check if farm profile already exists for this farm
            var existingProfile = await _context.FarmProfiles
                .FirstOrDefaultAsync(fp => fp.FarmName.ToLower() == survey.FarmName.ToLower());

            if (existingProfile == null)
            {
                // Create new farm profile
                var farmProfile = new FarmProfile
                {
                    FarmName = survey.FarmName,
                    FarmerName = survey.FarmerName,
                    Region = survey.Region,
                    GPSLatitude = ExtractLatitude(survey.GpsCoordinates),
                    GPSLongitude = ExtractLongitude(survey.GpsCoordinates),
                    RelatedSurveyId = survey.Id,
                    FarmPhotos = "[]", // Empty photo array initially
                    FarmStory = CreateFarmStoryFromSurvey(survey),
                    CoffeeVarieties = survey.VarietiesGrown ?? "",
                    ProcessingMethod = survey.ProcessingMethod ?? "",
                    FarmingPhilosophy = CreatePhilosophyFromSurvey(survey),
                    EnvironmentalFeatures = CreateEnvironmentalDescription(survey),
                    ClimateNotes = CreateClimateNotesFromSurvey(survey),
                    QualityHighlights = CreateQualityHighlightsFromSurvey(survey),
                    QualityRating = survey.QualityRating,
                    SustainabilityPractices = CreateSustainabilityFromSurvey(survey),
                    CommunityImpact = CreateCommunityImpactFromSurvey(survey),
                    CreatedDate = DateTime.Now,
                    LastUpdated = DateTime.Now,
                    IsActive = true
                };

                _context.FarmProfiles.Add(farmProfile);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                // Update existing farm profile with new survey data
                existingProfile.RelatedSurveyId = survey.Id;
                existingProfile.FarmerName = survey.FarmerName;
                existingProfile.QualityRating = survey.QualityRating;
                existingProfile.LastUpdated = DateTime.Now;
                
                // Update other fields if they're empty
                if (string.IsNullOrEmpty(existingProfile.CoffeeVarieties))
                    existingProfile.CoffeeVarieties = survey.VarietiesGrown ?? "";
                if (string.IsNullOrEmpty(existingProfile.ProcessingMethod))
                    existingProfile.ProcessingMethod = survey.ProcessingMethod ?? "";

                await _context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> UpdateFarmProfileFromSurvey(int surveyId)
        {
            var survey = await _context.FarmerSurveys.FindAsync(surveyId);
            if (survey == null) return false;

            // Find the farm profile related to this survey
            var farmProfile = await _context.FarmProfiles
                .FirstOrDefaultAsync(fp => fp.RelatedSurveyId == surveyId);

            if (farmProfile != null)
            {
                // Update the farm profile with new survey data
                farmProfile.FarmName = survey.FarmName;
                farmProfile.FarmerName = survey.FarmerName;
                farmProfile.Region = survey.Region;
                farmProfile.GPSLatitude = ExtractLatitude(survey.GpsCoordinates);
                farmProfile.GPSLongitude = ExtractLongitude(survey.GpsCoordinates);
                farmProfile.CoffeeVarieties = survey.VarietiesGrown ?? "";
                farmProfile.ProcessingMethod = survey.ProcessingMethod ?? "";
                farmProfile.QualityRating = survey.QualityRating;
                farmProfile.LastUpdated = DateTime.Now;

                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                // If no farm profile exists, create one
                return await CreateFarmProfileFromSurvey(surveyId);
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

        private string CreateFarmStoryFromSurvey(FarmerSurvey survey)
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

        private string CreatePhilosophyFromSurvey(FarmerSurvey survey)
        {
            var philosophy = "";
            
            if (survey.FertilizationType != null && survey.FertilizationType.Contains("organic"))
                philosophy += "Organic farming practices, ";
            
            if (survey.HasIntercropping)
                philosophy += "intercropping and agroforestry systems, ";
            
            if (!string.IsNullOrEmpty(survey.Certifications))
                philosophy += $"certified {survey.Certifications.ToLower()}, ";
            
            philosophy += "focusing on sustainable coffee production.";
            
            return philosophy;
        }

        private string CreateEnvironmentalDescription(FarmerSurvey survey)
        {
            var description = "";
            
            if (!string.IsNullOrEmpty(survey.SlopeType))
                description += $"{survey.SlopeType.ToLower()} terrain with ";
            if (!string.IsNullOrEmpty(survey.Aspect))
                description += $"{survey.Aspect.ToLower()} exposure. ";
            if (!string.IsNullOrEmpty(survey.SoilType))
                description += $"The soil consists primarily of {survey.SoilType.ToLower()}. ";
            if (!string.IsNullOrEmpty(survey.ShadeTrees))
                description += $"Shade is provided by {survey.ShadeTrees}. ";
            if (!string.IsNullOrEmpty(survey.WaterSource))
                description += $"Water is sourced from {survey.WaterSource.ToLower()}. ";
            
            return description.Trim();
        }

        private string CreateClimateNotesFromSurvey(FarmerSurvey survey)
        {
            var notes = "";
            
            if (survey.ElevationMeters > 0)
                notes += $"{survey.ElevationMeters}m elevation, ";
            if (!string.IsNullOrEmpty(survey.TemperatureRange))
                notes += $"temperature {survey.TemperatureRange}, ";
            if (!string.IsNullOrEmpty(survey.RainfallPatterns))
                notes += $"rainfall patterns: {survey.RainfallPatterns}, ";
            
            return notes.TrimEnd(',', ' ');
        }

        private string CreateQualityHighlightsFromSurvey(FarmerSurvey survey)
        {
            var highlights = "";
            
            if (survey.ElevationMeters > 1500)
                highlights += "High altitude cultivation creates concentrated flavors. ";
            
            if (!string.IsNullOrEmpty(survey.ProcessingMethod))
                highlights += $"{survey.ProcessingMethod} processing enhances cup characteristics. ";
            
            if (!string.IsNullOrEmpty(survey.VarietiesGrown))
                highlights += $"Premium varieties: {survey.VarietiesGrown}. ";
            
            return highlights.Trim();
        }

        private string CreateSustainabilityFromSurvey(FarmerSurvey survey)
        {
            var sustainability = "";
            
            if (survey.FertilizationType != null && survey.FertilizationType.Contains("organic"))
                sustainability += "Organic fertilization methods, ";
            
            if (survey.HasIntercropping)
                sustainability += "agroforestry systems for biodiversity, ";
            
            if (!string.IsNullOrEmpty(survey.SoilFertilityManagement))
                sustainability += $"soil management: {survey.SoilFertilityManagement.ToLower()}, ";
            
            sustainability += "environmental stewardship practices.";
            
            return sustainability;
        }

        private string CreateCommunityImpactFromSurvey(FarmerSurvey survey)
        {
            var impact = "";
            
            if (!string.IsNullOrEmpty(survey.LaborSystem))
                impact += $"Employment practices: {survey.LaborSystem.ToLower()}, ";
            
            if (!string.IsNullOrEmpty(survey.CooperativeMembership) && survey.CooperativeMembership != "None")
                impact += $"cooperative participation: {survey.CooperativeMembership}, ";
            
            if (!string.IsNullOrEmpty(survey.Certifications))
                impact += $"certifications support community standards: {survey.Certifications.ToLower()}, ";
            
            impact += "contributing to local economic development.";
            
            return impact;
        }
    }
}
