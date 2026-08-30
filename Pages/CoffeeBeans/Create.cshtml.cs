using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TheBestBean.Data;
using TheBestBean.Models;

namespace TheBestBean.Pages.CoffeeBeans
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
            ViewData["CoffeeFarmId"] = new SelectList(_context.CoffeeFarm, "Id", "Name");
            ViewData["CoffeeRegionId"] = new SelectList(_context.CoffeeRegion, "Id", "Name");
            ViewData["OriginCountryId"] = new SelectList(_context.OriginCountry, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public CoffeeBean CoffeeBean { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.CoffeeBean == null || CoffeeBean == null)
            {
                return Page();
            }

            _context.CoffeeBean.Add(CoffeeBean);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
