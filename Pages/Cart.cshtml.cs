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

        public void OnGet()
        {
            CartItems = _cartService.GetCart(HttpContext.Session);
            CartTotal = _cartService.GetCartTotal(HttpContext.Session);
            ItemCount = _cartService.GetCartItemCount(HttpContext.Session);
        }

        public IActionResult OnPostUpdateQuantity(int productId, string productType, int quantity)
        {
            _cartService.UpdateQuantity(HttpContext.Session, productId, productType, quantity);
            return RedirectToPage();
        }

        public IActionResult OnPostRemoveItem(int productId, string productType)
        {
            _cartService.RemoveFromCart(HttpContext.Session, productId, productType);
            return RedirectToPage();
        }

        public IActionResult OnPostClearCart()
        {
            _cartService.ClearCart(HttpContext.Session);
            return RedirectToPage();
        }

        public IActionResult OnPostAddToCart(int productId, string productName, string productType, decimal price, string imageUrl, string description, string weight = "", int quantity = 1)
        {
            var cartItem = new CartItem
            {
                ProductId = productId,
                ProductName = productName,
                ProductType = productType,
                Price = price,
                Quantity = quantity,
                ImageUrl = imageUrl,
                Description = description,
                Weight = weight
            };

            _cartService.AddToCart(HttpContext.Session, cartItem);
            return RedirectToPage();
        }
    }
}
