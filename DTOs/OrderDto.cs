namespace ECommerce.Api.Dtos
{
    public class OrderItemCreateDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderCreateDto
    {
        public List<OrderItemCreateDto> Items { get; set; } = new();
    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class OrderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}