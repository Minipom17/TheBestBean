using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TheBestBean.Models;

namespace TheBestBean.Pages
{
    public class OrderConfirmationModel : PageModel
    {
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public decimal OrderTotal { get; set; }
        public List<CartItem> OrderItems { get; set; } = new List<CartItem>();

        public IActionResult OnGet()
        {
            // Retrieve order information from TempData
            if (TempData["OrderNumber"] == null)
            {
                return RedirectToPage("/GreenBeans");
            }

            OrderNumber = TempData["OrderNumber"]?.ToString() ?? string.Empty;
            CustomerName = TempData["CustomerName"]?.ToString() ?? string.Empty;
            CustomerEmail = TempData["CustomerEmail"]?.ToString() ?? string.Empty;
            OrderTotal = TempData["OrderTotal"] != null ? Convert.ToDecimal(TempData["OrderTotal"]) : 0;
            
            var orderItemsJson = TempData["OrderItems"]?.ToString();
            if (!string.IsNullOrEmpty(orderItemsJson))
            {
                OrderItems = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(orderItemsJson) ?? new List<CartItem>();
            }

            return Page();
        }
    }
}
