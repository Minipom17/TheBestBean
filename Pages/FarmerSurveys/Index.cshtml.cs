using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TheBestBean.Data;
using TheBestBean.Models;

namespace TheBestBean.Pages.FarmerSurveys
{
    public class IndexModel : PageModel
    {
        private readonly TheBestBeanContext _context;

        public IndexModel(TheBestBeanContext context)
        {
            _context = context;
        }

        public IList<FarmerSurvey> FarmerSurvey { get; set; } = default!;
        
        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            if (_context.FarmerSurveys != null)
            {
                var surveys = _context.FarmerSurveys.AsQueryable();

                if (!string.IsNullOrEmpty(SearchString))
                {
                    // Search in farm name, farmer name, or region (case-insensitive)
                    surveys = surveys.Where(s => 
                        s.FarmName.ToLower().Contains(SearchString.ToLower()) ||
                        s.FarmerName.ToLower().Contains(SearchString.ToLower()) ||
                        s.Region.ToLower().Contains(SearchString.ToLower()));
                }

                FarmerSurvey = await surveys
                    .OrderByDescending(s => s.SurveyDate)
                    .ToListAsync();
            }
        }
    }
}
