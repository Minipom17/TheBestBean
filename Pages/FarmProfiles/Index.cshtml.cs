using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TheBestBean.Data;
using TheBestBean.Models;

namespace TheBestBean.Pages.FarmProfiles
{
    public class IndexModel : PageModel
    {
        private readonly TheBestBeanContext _context;

        public IndexModel(TheBestBeanContext context)
        {
            _context = context;
        }

        public IList<FarmProfile> FarmProfile { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.FarmProfiles != null)
            {
                FarmProfile = await _context.FarmProfiles
                    .Include(fp => fp.RelatedSurvey)
                    .Where(fp => fp.IsActive)
                    .OrderByDescending(fp => fp.LastUpdated)
                    .ToListAsync();
            }
        }
    }
}




