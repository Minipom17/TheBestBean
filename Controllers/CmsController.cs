using Microsoft.AspNetCore.Mvc;
using TheBestBean.Data;
using TheBestBean.Models;

namespace TheBestBean.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CmsController : ControllerBase
    {
        private readonly TheBestBeanContext _context;
        private readonly IWebHostEnvironment _env;

        public CmsController(TheBestBeanContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public class InlineUpdateDto
        {
            public string EntityType { get; set; } = string.Empty;
            public string EntityId { get; set; } = string.Empty;
            public string Field { get; set; } = string.Empty;
            public string Value { get; set; } = string.Empty;
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateContent([FromBody] List<InlineUpdateDto> updates)
        {
            if (User.Identity?.IsAuthenticated != true) return new UnauthorizedResult();
            if (updates == null || !updates.Any()) return BadRequest();

            foreach (var update in updates)
            {
                if (update.EntityType == "SiteContent")
                {
                    var content = await _context.SiteContent.FindAsync(update.EntityId);
                    if (content != null)
                    {
                        content.Value = update.Value;
                    }
                    else
                    {
                        // UPSERT logic: if it doesn't exist, create it automatically!
                        content = new SiteContent { Key = update.EntityId, Value = update.Value };
                        _context.SiteContent.Add(content);
                    }
                }
                else if (update.EntityType == "Experience")
                {
                    if (int.TryParse(update.EntityId, out int id))
                    {
                        var exp = await _context.Experiences.FindAsync(id);
                        if (exp != null)
                        {
                            switch (update.Field)
                            {
                                case "Title": exp.Title = update.Value; break;
                                case "Description": exp.Description = update.Value; break;
                                case "LongDescription": exp.LongDescription = update.Value; break;
                                case "Price": if (decimal.TryParse(update.Value, out decimal p)) exp.Price = p; break;
                                case "ImageUrl": exp.ImageUrl = update.Value; break;
                                case "Difficulty": exp.Difficulty = update.Value; break;
                                case "Location": exp.Location = update.Value; break;
                                case "Month": exp.Month = update.Value; break;
                                case "Duration": exp.Duration = update.Value; break;
                                case "Category": exp.Category = update.Value; break;
                            }
                        }
                    }
                }
                else if (update.EntityType == "CoffeeBean")
                {
                    if (int.TryParse(update.EntityId, out int id))
                    {
                        var bean = await _context.CoffeeBean.FindAsync(id);
                        if (bean != null)
                        {
                            var propertyInfo = bean.GetType().GetProperty(update.Field);
                            if (propertyInfo != null && propertyInfo.CanWrite)
                            {
                                object? convertedValue = update.Value;
                                if (propertyInfo.PropertyType == typeof(decimal) && decimal.TryParse(update.Value, out decimal d))
                                {
                                    convertedValue = d;
                                }
                                propertyInfo.SetValue(bean, convertedValue);
                            }
                        }
                    }
                }
                else if (update.EntityType == "FlavorZone")
                {
                    var zone = await _context.FlavorZones.FindAsync(update.EntityId);
                    if (zone != null)
                    {
                        var propertyInfo = zone.GetType().GetProperty(update.Field);
                        if (propertyInfo != null && propertyInfo.CanWrite)
                        {
                            propertyInfo.SetValue(zone, update.Value);
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            if (User.Identity?.IsAuthenticated != true) return new UnauthorizedResult();

            if (image != null && image.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "images", "uploads");
                Directory.CreateDirectory(uploadsFolder);
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);
                
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }
                
                return Ok(new { imageUrl = "/images/uploads/" + fileName });
            }
            return BadRequest();
        }
    }
}
