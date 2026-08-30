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
    public class DetailsModel : PageModel
    {
        private readonly TheBestBeanContext _context;

        public DetailsModel(TheBestBeanContext context)
        {
            _context = context;
        }

      public FarmerSurvey FarmerSurvey { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.FarmerSurveys == null)
            {
                return NotFound();
            }

            var farmersurvey = await _context.FarmerSurveys.FirstOrDefaultAsync(m => m.Id == id);
            if (farmersurvey == null)
            {
                return NotFound();
            }
            else 
            {
                FarmerSurvey = farmersurvey;
            }
            return Page();
        }
    }
}




