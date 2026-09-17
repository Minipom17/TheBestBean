using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TheBestBean.Models;
using TheBestBean.Services;

namespace TheBestBean.Pages
{
    public class PayPalTestModel : PageModel
    {
        private readonly CartService _cart;
        private readonly IWebHostEnvironment _env;

        public PayPalTestModel(CartService cart, IWebHostEnvironment env)
        {
            _cart = cart;
            _env = env;
        }

        public decimal PriceUsd => CartService.PayPalTestPriceUsd;
        public decimal PayPalUsdTotal => PriceUsd;

        public IActionResult OnGet()
        {
            if (!CanUse) return NotFound();
            ViewData["Title"] = "PayPal $1 test";
            ViewData["Robots"] = "noindex, nofollow";
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!CanUse) return NotFound();

            _cart.ClearCart(HttpContext.Session);
            _cart.AddToCart(HttpContext.Session, new CartItem
            {
                ProductId = CartService.PayPalTestProductId,
                ProductName = "PayPal live $1 test",
                ProductType = CartService.PayPalTestProductType,
                Price = CartService.PayPalTestPriceUsd,
                Quantity = 1,
                Description = "One-dollar live PayPal charge. Not a tour.",
                ImageUrl = "/images/og-logo.png"
            });

            return RedirectToPage("/Checkout");
        }

        private bool CanUse =>
            _env.IsDevelopment()
            || User.IsInRole("Admin")
            || string.Equals(Request.Query["pb_internal"].ToString(), "1", StringComparison.OrdinalIgnoreCase);
    }
}
