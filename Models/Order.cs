using System.ComponentModel.DataAnnotations;

namespace TheBestBean.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string DeliveryMethod { get; set; } = string.Empty;
        public string OrderNotes { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
