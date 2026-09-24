using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TheBestBean.Data;
using TheBestBean.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace TheBestBean.Pages.Admin
{
    public class DispatchModel : PageModel
    {
        private readonly TheBestBeanContext _context;

        public DispatchModel(TheBestBeanContext context)
        {
            _context = context;
        }

        public IList<Order> Orders { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Orders = await _context.Orders
                .Include(o => o.Items)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(int orderId, string newStatus, string trackingNumber)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.OrderStatus = newStatus;
                order.TrackingNumber = trackingNumber;
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}
