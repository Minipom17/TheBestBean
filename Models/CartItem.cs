namespace TheBestBean.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductType { get; set; } = string.Empty; // "Bean" or "Equipment"
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Weight { get; set; } = string.Empty;

        public decimal Subtotal => Price * Quantity;
    }
}
