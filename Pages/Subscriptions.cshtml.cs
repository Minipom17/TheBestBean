using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TheBestBean.Models;
using TheBestBean.Services;

namespace TheBestBean.Pages
{
    public class SubscriptionsModel : PageModel
    {
        private readonly ILogger<SubscriptionsModel> _logger;
        private readonly Services.CartService _cartService;

        public SubscriptionsModel(ILogger<SubscriptionsModel> logger, Services.CartService cartService)
        {
            _logger = logger;
            _cartService = cartService;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPostAddToCart(int productId, string productName, string productType, decimal price, string description, int quantity = 1)
        {
            var cartItem = new Models.CartItem
            {
                ProductId = productId,
                ProductName = productName,
                ProductType = productType,
                Price = price,
                Quantity = quantity,
                ImageUrl = "/Media/Shop/Green_bean.svg", // Default image for subs
                Description = description
            };

            _cartService.AddToCart(HttpContext.Session, cartItem);
            Ga4Ecommerce.QueueAddToCart(TempData, cartItem);

            return RedirectToPage("/Cart");
        }
    }
}
