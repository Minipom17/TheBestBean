using TheBestBean.Models;
using System.Text.Json;

namespace TheBestBean.Services
{
    public class CartService
    {
        private const string CartSessionKey = "ShoppingCart";

        public List<CartItem> GetCart(ISession session)
        {
            var cartJson = session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItem>();
            }
            return JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
        }

        public void AddToCart(ISession session, CartItem item)
        {
            var cart = GetCart(session);
            var existingItem = cart.FirstOrDefault(i => i.ProductId == item.ProductId && i.ProductType == item.ProductType);

            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                cart.Add(item);
            }

            SaveCart(session, cart);
        }

        public void UpdateQuantity(ISession session, int productId, string productType, int quantity)
        {
            var cart = GetCart(session);
            var item = cart.FirstOrDefault(i => i.ProductId == productId && i.ProductType == productType);

            if (item != null)
            {
                if (quantity <= 0)
                {
                    cart.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                }
                SaveCart(session, cart);
            }
        }

        public void RemoveFromCart(ISession session, int productId, string productType)
        {
            var cart = GetCart(session);
            var item = cart.FirstOrDefault(i => i.ProductId == productId && i.ProductType == productType);

            if (item != null)
            {
                cart.Remove(item);
                SaveCart(session, cart);
            }
        }

        public void ClearCart(ISession session)
        {
            session.Remove(CartSessionKey);
        }

        public int GetCartItemCount(ISession session)
        {
            var cart = GetCart(session);
            return cart.Sum(i => i.Quantity);
        }

        public decimal GetCartTotal(ISession session)
        {
            var cart = GetCart(session);
            return cart.Sum(i => i.Subtotal);
        }

        private void SaveCart(ISession session, List<CartItem> cart)
        {
            var cartJson = JsonSerializer.Serialize(cart);
            session.SetString(CartSessionKey, cartJson);
        }
    }
}
