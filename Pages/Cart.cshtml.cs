using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TheBestBean.Models;
using TheBestBean.Services;

namespace TheBestBean.Pages
{
    public class CartModel : PageModel
    {
        private readonly CartService _cartService;

        public CartModel(CartService cartService)
        {
            _cartService = cartService;
        }

        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal CartTotal { get; set; }
        public int ItemCount { get; set; }
        public bool IsWorkshopOnlyCart { get; set; }
        public bool HasCoffeeItems { get; set; }
        public string KeepShoppingPage { get; set; } = "/GreenBeans";
        public string KeepShoppingLabel { get; set; } = "Keep shopping coffee";

        public void OnGet()
        {
            LoadCart();
        }

        public IActionResult OnPostUpdateQuantity(int productId, string productType, int quantity, string? weight = null, string? roastLevel = null)
        {
            _cartService.UpdateQuantity(HttpContext.Session, productId, productType, quantity, weight, roastLevel);
            return RedirectToPage();
        }

        public IActionResult OnPostRemoveItem(int productId, string productType, string? weight = null, string? roastLevel = null)
        {
            _cartService.RemoveFromCart(HttpContext.Session, productId, productType, weight, roastLevel);
            return RedirectToPage();
        }

        public IActionResult OnPostClearCart()
        {
            _cartService.ClearCart(HttpContext.Session);
            return RedirectToPage();
        }

        public IActionResult OnPostAddToCart(int productId, string productName, string productType, decimal price, string imageUrl, string description, string weight = "", int quantity = 1, string? roastLevel = null)
        {
            var roast = RoastProfiles.IsCoffeeProduct(productType) ? RoastProfiles.Normalize(roastLevel) : "";
            if (RoastProfiles.IsCoffeeProduct(productType) && !productName.Contains("·", StringComparison.Ordinal))
            {
                productName = $"{productName} · {RoastProfiles.CartLine(roast)}";
            }

            var cartItem = new CartItem
            {
                ProductId = productId,
                ProductName = productName,
                ProductType = productType,
                Price = price,
                Quantity = quantity,
                ImageUrl = imageUrl,
                Description = description,
                Weight = weight,
                RoastLevel = roast
            };

            _cartService.AddToCart(HttpContext.Session, cartItem);
            Ga4Ecommerce.QueueAddToCart(TempData, cartItem);
            return RedirectToPage();
        }

        private void LoadCart()
        {
            _cartService.ClampExperienceGuests(HttpContext.Session);
            CartItems = _cartService.GetCart(HttpContext.Session);
            CartTotal = _cartService.GetCartTotal(HttpContext.Session);
            ItemCount = _cartService.GetCartItemCount(HttpContext.Session);
            IsWorkshopOnlyCart = CartService.IsWorkshopOnlyCart(CartItems);
            HasCoffeeItems = CartService.HasCoffeeItems(CartItems);
            if (CartService.IsPayPalTestCart(CartItems))
            {
                KeepShoppingPage = "/PayPalTest";
                KeepShoppingLabel = "Back to $1 test";
            }
            else
            {
                KeepShoppingPage = IsWorkshopOnlyCart ? "/Experiences" : "/GreenBeans";
                KeepShoppingLabel = IsWorkshopOnlyCart ? "Keep browsing experiences" : "Keep shopping coffee";
            }
        }
    }
}
