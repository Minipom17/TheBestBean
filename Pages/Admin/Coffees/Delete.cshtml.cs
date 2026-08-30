using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TheBestBean.Data;
using TheBestBean.Models;

namespace TheBestBean.Pages_Admin_Coffees
{
    public class DeleteModel : PageModel
    {
        private readonly TheBestBean.Data.TheBestBeanContext _context;

        public DeleteModel(TheBestBean.Data.TheBestBeanContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CoffeeBean CoffeeBean { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coffeebean = await _context.CoffeeBean.FirstOrDefaultAsync(m => m.Id == id);

            if (coffeebean is not null)
            {
                CoffeeBean = coffeebean;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coffeebean = await _context.CoffeeBean.FindAsync(id);
            if (coffeebean != null)
            {
                CoffeeBean = coffeebean;
                _context.CoffeeBean.Remove(CoffeeBean);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
