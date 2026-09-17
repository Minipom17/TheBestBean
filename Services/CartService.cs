using TheBestBean.Models;
using System.Text.Json;

namespace TheBestBean.Services
{
    public class CartService
    {
        public const int MaxExperienceGuests = 6;
        public const string PayPalTestProductType = "PayPalTest";
        public const int PayPalTestProductId = -17;
        public const decimal PayPalTestPriceUsd = 1.00m;
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
            var existingItem = cart.FirstOrDefault(i => SameLine(i, item));

            if (existingItem != null)
            {
                var next = existingItem.Quantity + item.Quantity;
                if (IsExperienceBooking(item.ProductType))
                {
                    next = Math.Min(next, MaxExperienceGuests);
                    existingItem.Quantity = next;
                    existingItem.Description = WithGuestCount(existingItem.Description, next);
                }
                else
                {
                    existingItem.Quantity = next;
                }
            }
            else
            {
                if (IsExperienceBooking(item.ProductType))
                {
                    item.Quantity = Math.Clamp(item.Quantity, 1, MaxExperienceGuests);
                    item.Description = WithGuestCount(item.Description, item.Quantity);
                }
                cart.Add(item);
            }

            SaveCart(session, cart);
        }

        public void UpdateQuantity(ISession session, int productId, string productType, int quantity, string? weight = null, string? roastLevel = null)
        {
            var cart = GetCart(session);
            var item = FindLine(cart, productId, productType, weight, roastLevel);

            if (item != null)
            {
                if (quantity <= 0)
                {
                    cart.Remove(item);
                }
                else
                {
                    if (IsExperienceBooking(item.ProductType))
                    {
                        quantity = Math.Clamp(quantity, 1, MaxExperienceGuests);
                        item.Description = WithGuestCount(item.Description, quantity);
                    }
                    item.Quantity = quantity;
                }
                SaveCart(session, cart);
            }
        }

        public void RemoveFromCart(ISession session, int productId, string productType, string? weight = null, string? roastLevel = null)
        {
            var cart = GetCart(session);
            var item = FindLine(cart, productId, productType, weight, roastLevel);

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

        public static bool IsExperienceBooking(string? productType) =>
            string.Equals(productType, "Experience", StringComparison.OrdinalIgnoreCase);

        public static bool IsExperienceType(string? productType) =>
            IsExperienceBooking(productType)
            || string.Equals(productType, ExperienceAddOns.ProductType, StringComparison.OrdinalIgnoreCase);

        public void ClampExperienceGuests(ISession session)
        {
            var cart = GetCart(session);
            var changed = false;
            foreach (var item in cart)
            {
                if (!IsExperienceBooking(item.ProductType)) continue;
                var quantity = Math.Clamp(item.Quantity, 1, MaxExperienceGuests);
                if (quantity == item.Quantity) continue;
                item.Quantity = quantity;
                item.Description = WithGuestCount(item.Description, quantity);
                changed = true;
            }
            if (changed)
            {
                SaveCart(session, cart);
            }
        }

        public static bool HasExperienceItems(IEnumerable<CartItem> cart) =>
            cart.Any(i => string.Equals(i.ProductType, "Experience", StringComparison.OrdinalIgnoreCase));

        public static bool IsWorkshopOnlyCart(IEnumerable<CartItem> cart) =>
            cart.Any() && cart.All(i => IsExperienceType(i.ProductType));

        public static bool IsPayPalTestType(string? productType) =>
            string.Equals(productType, PayPalTestProductType, StringComparison.OrdinalIgnoreCase);

        public static bool IsPayPalTestCart(IEnumerable<CartItem> cart) =>
            cart.Any() && cart.All(i => IsPayPalTestType(i.ProductType));

        public static bool SkipsShipping(IEnumerable<CartItem> cart) =>
            IsWorkshopOnlyCart(cart) || IsPayPalTestCart(cart);

        public static bool IsCoffeeProduct(string? productType) => RoastProfiles.IsCoffeeProduct(productType);

        public static bool HasCoffeeItems(IEnumerable<CartItem> cart) =>
            cart.Any(i => IsCoffeeProduct(i.ProductType));

        private static CartItem? FindLine(List<CartItem> cart, int productId, string productType, string? weight, string? roastLevel)
        {
            return cart.FirstOrDefault(i =>
                i.ProductId == productId
                && i.ProductType == productType
                && (weight == null || string.Equals(i.Weight ?? "", weight, StringComparison.OrdinalIgnoreCase))
                && (roastLevel == null || string.Equals(i.RoastLevel ?? "", roastLevel, StringComparison.OrdinalIgnoreCase)));
        }

        private static bool SameLine(CartItem a, CartItem b) =>
            a.ProductId == b.ProductId
            && string.Equals(a.ProductType, b.ProductType, StringComparison.OrdinalIgnoreCase)
            && a.SlotId == b.SlotId
            && string.Equals(a.Weight ?? "", b.Weight ?? "", StringComparison.OrdinalIgnoreCase)
            && string.Equals(a.RoastLevel ?? "", b.RoastLevel ?? "", StringComparison.OrdinalIgnoreCase);

        private static string WithGuestCount(string? description, int guests)
        {
            var head = guests == 1 ? "1 guest" : $"{guests} guests";
            if (string.IsNullOrWhiteSpace(description)) return head;
            var sep = " · ";
            var i = description.IndexOf(sep, StringComparison.Ordinal);
            return i >= 0 ? head + description[i..] : head;
        }

        private void SaveCart(ISession session, List<CartItem> cart)
        {
            var cartJson = JsonSerializer.Serialize(cart);
            session.SetString(CartSessionKey, cartJson);
        }
    }
}
