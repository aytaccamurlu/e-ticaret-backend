using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ECommerce.Api.Data;
using ECommerce.Api.Dtos;
using ECommerce.Api.Models;

namespace ECommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto dto)
        {
            if (dto.Items == null || !dto.Items.Any())
            {
                return BadRequest(new { message = "Sipariş en az bir ürün içermelidir." });
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Kullanıcı kimliği doğrulanamadı." });
            }
            int userId = int.Parse(userIdClaim);

            decimal totalPrice = 0;
            var orderItems = new List<OrderItem>();

            foreach (var itemDto in dto.Items)
            {
                var product = await _context.Products.FindAsync(itemDto.ProductId);
                if (product == null)
                {
                    return BadRequest(new { message = $"ID'si {itemDto.ProductId} olan ürün bulunamadı." });
                }

                if (product.Stock < itemDto.Quantity)
                {
                    return BadRequest(new { message = $"'{product.Name}' ürünü için yeterli stok yok. (Stok: {product.Stock})" });
                }

                product.Stock -= itemDto.Quantity;

                decimal itemTotal = product.Price * itemDto.Quantity;
                totalPrice += itemTotal;

                orderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = itemDto.Quantity,
                    UnitPrice = product.Price // Burası UnitPrice olarak güncellendi
                });
            }

            var order = new Order
            {
                UserId = userId,
                TotalPrice = totalPrice,
                TotalAmount = totalPrice, // Modeldeki TotalAmount alanı da dolduruluyor
                CreatedAt = DateTime.UtcNow,
                OrderItems = orderItems
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Sipariş başarıyla oluşturuldu.", orderId = order.Id, totalPrice });
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim == null) return Unauthorized();

            IQueryable<Order> query = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product);

            if (roleClaim != "Admin")
            {
                int userId = int.Parse(userIdClaim);
                query = query.Where(o => o.UserId == userId);
            }

            var orders = await query
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    TotalPrice = o.TotalPrice,
                    CreatedAt = o.CreatedAt,
                    Items = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        ProductId = oi.ProductId,
                        ProductName = oi.Product != null ? oi.Product.Name : string.Empty,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice // Burası UnitPrice olarak güncellendi
                    }).ToList()
                })
                .ToListAsync();

            return Ok(orders);
        }
    }
}