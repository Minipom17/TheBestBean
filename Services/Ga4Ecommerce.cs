using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using TheBestBean.Models;

namespace TheBestBean.Services
{
    public static class Ga4Ecommerce
    {
        public const string TempKey = "Ga4Events";
        public const string ViewKey = "Ga4PageEvents";

        private static readonly JsonSerializerOptions JsonOpts = new();

        public static Dictionary<string, object> Item(CartItem item)
        {
            return new Dictionary<string, object>
            {
                ["item_id"] = item.ProductId.ToString(),
                ["item_name"] = item.ProductName,
                ["item_category"] = string.IsNullOrWhiteSpace(item.ProductType) ? "Coffee" : item.ProductType,
                ["price"] = decimal.Round(item.Price, 2),
                ["quantity"] = item.Quantity
            };
        }

        public static Dictionary<string, object> Item(int id, string name, string category, decimal price, int quantity = 1)
        {
            return Item(new CartItem
            {
                ProductId = id,
                ProductName = name,
                ProductType = category,
                Price = price,
                Quantity = quantity
            });
        }

        public static Dictionary<string, object> Payload(IEnumerable<CartItem> items, decimal? value = null, string? transactionId = null)
        {
            var list = items.ToList();
            var payload = new Dictionary<string, object>
            {
                ["currency"] = "USD",
                ["value"] = decimal.Round(value ?? list.Sum(i => i.Subtotal), 2),
                ["items"] = list.Select(Item).ToList()
            };
            if (!string.IsNullOrEmpty(transactionId))
            {
                payload["transaction_id"] = transactionId;
            }
            return payload;
        }

        public static void Queue(ITempDataDictionary temp, string eventName, Dictionary<string, object> parameters)
        {
            var list = Read(temp);
            list.Add(new Dictionary<string, object>
            {
                ["event"] = eventName,
                ["params"] = parameters
            });
            temp[TempKey] = JsonSerializer.Serialize(list, JsonOpts);
        }

        public static void QueueAddToCart(ITempDataDictionary temp, CartItem item)
        {
            var payload = Payload(new[] { item });
            Queue(temp, "add_to_cart", payload);
            if (string.Equals(item.ProductType, "Experience", StringComparison.OrdinalIgnoreCase))
            {
                Queue(temp, "book_experience", payload);
            }
        }

        public static void SetPageEvent(ViewDataDictionary view, string eventName, Dictionary<string, object> parameters)
        {
            view[ViewKey] = JsonSerializer.Serialize(
                new[]
                {
                    new Dictionary<string, object>
                    {
                        ["event"] = eventName,
                        ["params"] = parameters
                    }
                },
                JsonOpts);
        }

        private static List<Dictionary<string, object>> Read(ITempDataDictionary temp)
        {
            if (temp[TempKey] is string json && !string.IsNullOrWhiteSpace(json))
            {
                return JsonSerializer.Deserialize<List<Dictionary<string, object>>>(json, JsonOpts)
                    ?? new List<Dictionary<string, object>>();
            }
            return new List<Dictionary<string, object>>();
        }
    }
}
