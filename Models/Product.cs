public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }          // Standart Müşteri Fiyatı
    public decimal DealerPrice { get; set; }     // Bayiye Özel Fiyat
    public int Stock { get; set; }
    public int CategoryId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public Category? Category { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}