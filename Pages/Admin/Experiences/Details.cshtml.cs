using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TheBestBean.Data;
using TheBestBean.Models;

namespace TheBestBean.Pages_Admin_Experiences
{
    public class DetailsModel : PageModel
    {
        private readonly TheBestBean.Data.TheBestBeanContext _context;

        public DetailsModel(TheBestBean.Data.TheBestBeanContext context)
        {
            _context = context;
        }

        public Experience Experience { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var experience = await _context.Experiences.FirstOrDefaultAsync(m => m.Id == id);

            if (experience is not null)
            {
                Experience = experience;

                return Page();
            }

            return NotFound();
        }
    }
}
