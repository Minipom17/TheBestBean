using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TheBestBean.Data;
using TheBestBean.Models;
using TheBestBean.Services;

namespace TheBestBean.Pages
{
    public class OrderConfirmationModel : PageModel
    {
        private readonly TheBestBeanContext _db;

        public OrderConfirmationModel(TheBestBeanContext db)
        {
            _db = db;
        }

        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public decimal OrderTotal { get; set; }
        public decimal CardSurcharge { get; set; }
        public string PaymentMethod { get; set; } = "Yape";
        public string PaymentStatus { get; set; } = "Pending";
        public List<CartItem> OrderItems { get; set; } = new List<CartItem>();
        public bool IsPayPalUsd => PayPalService.IsUsd(PaymentMethod);
        public bool IsPayPal => PayPalService.IsPayPalMethod(PaymentMethod);
        public bool IsCulqi => CulqiService.IsCulqiMethod(PaymentMethod);
        public decimal PayPalUsdCharged => OrderTotal + CardSurcharge;
        public decimal PayPalCadCharged => LocalPricing.Cad(OrderTotal) + CardSurcharge;

        public async Task<IActionResult> OnGetAsync(string? order)
        {
            var orderNumber = order ?? TempData["OrderNumber"]?.ToString();
            if (string.IsNullOrWhiteSpace(orderNumber))
            {
                return RedirectToPage("/GreenBeans");
            }

            var saved = await _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
            if (saved != null)
            {
                OrderNumber = saved.OrderNumber;
                CustomerName = saved.FullName;
                CustomerEmail = saved.Email;
                OrderTotal = saved.TotalAmount;
                CardSurcharge = saved.CardSurcharge;
                PaymentMethod = saved.PaymentMethod;
                PaymentStatus = saved.PaymentStatus;
                OrderItems = saved.Items.Select(i => new CartItem
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    ProductType = i.ProductType,
                    Price = i.UnitPrice,
                    Quantity = i.Quantity
                }).ToList();
            }
            else
            {
                OrderNumber = orderNumber;
                CustomerName = TempData["CustomerName"]?.ToString() ?? string.Empty;
                CustomerEmail = TempData["CustomerEmail"]?.ToString() ?? string.Empty;
                OrderTotal = TempData["OrderTotal"] != null ? Convert.ToDecimal(TempData["OrderTotal"]) : 0;
                PaymentMethod = TempData["PaymentMethod"]?.ToString() ?? "Yape";
                var orderItemsJson = TempData["OrderItems"]?.ToString();
                if (!string.IsNullOrEmpty(orderItemsJson))
                {
                    OrderItems = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(orderItemsJson) ?? new List<CartItem>();
                }
            }

            var mpStatus = Request.Query["status"].ToString();
            var collectionStatus = Request.Query["collection_status"].ToString();
            if (string.Equals(mpStatus, "approved", StringComparison.OrdinalIgnoreCase)
                || string.Equals(collectionStatus, "approved", StringComparison.OrdinalIgnoreCase))
            {
                PaymentStatus = "Paid";
            }

            if (OrderItems.Count > 0 && string.Equals(PaymentStatus, "Paid", StringComparison.OrdinalIgnoreCase))
            {
                Ga4Ecommerce.SetPageEvent(ViewData, "purchase", Ga4Ecommerce.Payload(OrderItems, OrderTotal, OrderNumber));
            }

            return Page();
        }
    }
}
