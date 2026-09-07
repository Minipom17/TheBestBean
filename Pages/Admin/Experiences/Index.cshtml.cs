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
    public class IndexModel : PageModel
    {
        private readonly TheBestBean.Data.TheBestBeanContext _context;

        public IndexModel(TheBestBean.Data.TheBestBeanContext context)
        {
            _context = context;
        }

        public IList<Experience> Experience { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Experience = await _context.Experiences
                .OrderBy(e => e.SortOrder)
                .ThenBy(e => e.Id)
                .ToListAsync();
        }
    }
}
