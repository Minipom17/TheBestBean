using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TheBestBean.Data;
using TheBestBean.Services;

namespace TheBestBean.Pages
{
    public class PayPalCompleteModel : PageModel
    {
        private readonly TheBestBeanContext _db;
        private readonly PayPalService _paypal;
        private readonly CartService _cart;
        private readonly BookingOpsNotifyService _opsNotify;

        public PayPalCompleteModel(TheBestBeanContext db, PayPalService paypal, CartService cart, BookingOpsNotifyService opsNotify)
        {
            _db = db;
            _paypal = paypal;
            _cart = cart;
            _opsNotify = opsNotify;
        }

        public async Task<IActionResult> OnGetAsync(string? order, string? token)
        {
            if (string.IsNullOrWhiteSpace(order))
            {
                return RedirectToPage("/Checkout");
            }

            var saved = await _db.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.OrderNumber == order);
            if (saved == null)
            {
                TempData["MpError"] = "PayPal came back without a matching order.";
                return RedirectToPage("/Checkout");
            }

            var payPalId = !string.IsNullOrWhiteSpace(token) ? token : saved.PayPalOrderId;
            var captured = await _paypal.CaptureAsync(payPalId);
            if (captured)
            {
                saved.PaymentStatus = "Paid";
                if (!string.IsNullOrWhiteSpace(payPalId))
                {
                    saved.PayPalOrderId = payPalId;
                }
                await _db.SaveChangesAsync();
                await _opsNotify.NotifyOrderPlacedAsync(saved);
                _cart.ClearCart(HttpContext.Session);
                return RedirectToPage("/OrderConfirmation", new { order = saved.OrderNumber });
            }

            saved.PaymentStatus = "Failed";
            await _db.SaveChangesAsync();
            TempData["MpError"] = "PayPal did not complete the payment. Try again or use Yape.";
            return RedirectToPage("/Checkout");
        }
    }
}
