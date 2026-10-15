using ECommerce.Api.Models;

namespace ECommerce.Api.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Processing, Shipped, Completed, Cancelled
        public string ShippingAddress { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<OrderItem> OrderItems { get; set; } = new();
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; } // Satın alım anındaki fiyat
    }
}