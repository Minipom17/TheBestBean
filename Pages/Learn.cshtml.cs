using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TheBestBean.Pages
{
    public class LearnModel : PageModel
    {
        private readonly ILogger<LearnModel> _logger;

        public LearnModel(ILogger<LearnModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            // No additional logic needed for static content
        }
    }
}
