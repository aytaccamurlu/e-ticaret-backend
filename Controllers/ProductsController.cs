using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerce.Api.Data;
using ECommerce.Api.Dtos;
using ECommerce.Api.Models;

namespace ECommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // Herkes ürünleri listeleyebilir (Kategori adına göre de filtreleme eklenebilir)
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? categoryId)
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            var products = await query
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Stock = p.Stock,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category != null ? p.Category.Name : string.Empty,
                    ImageUrl = p.ImageUrl
                })
                .ToListAsync();

            return Ok(products);
        }

        // Tek bir ürünü ID ile getirme
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var p = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(x => x.Id == id);
            if (p == null)
            {
                return NotFound(new { message = "Ürün bulunamadı." });
            }

            var productDto = new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                CategoryId = p.CategoryId,
                CategoryName = p.Category != null ? p.Category.Name : string.Empty,
                ImageUrl = p.ImageUrl
            };

            return Ok(productDto);
        }

        // Sadece Admin ürün ekleyebilir
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
        {
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
            if (!categoryExists)
            {
                return BadRequest(new { message = "Belirtilen kategori bulunamadı." });
            }

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                CategoryId = dto.CategoryId,
                ImageUrl = dto.ImageUrl
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Ürün başarıyla oluşturuldu.", productId = product.Id });
        }

        // Sadece Admin ürün silebilir
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Ürün bulunamadı." });
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Ürün başarıyla silindi." });
        }
    }
}