using Microsoft.AspNetCore.Mvc;
using TheBestBean.Services;

namespace TheBestBean.ViewComponents
{
    public class CartIconViewComponent : ViewComponent
    {
        private readonly CartService _cartService;

        public CartIconViewComponent(CartService cartService)
        {
            _cartService = cartService;
        }

        public IViewComponentResult Invoke()
        {
            var itemCount = _cartService.GetCartItemCount(HttpContext.Session);
            return View(itemCount);
        }
    }
}
