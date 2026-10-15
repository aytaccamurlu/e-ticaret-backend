using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerce.Api.Data;
using ECommerce.Api.Dtos;
using ECommerce.Api.Models;
using ECommerce.Api.DTOs;

namespace ECommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // Herkes kategorileri listeleyebilir
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _context.Categories
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();

            return Ok(categories);
        }

        // Sadece Admin kategori ekleyebilir
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDto dto)
        {
            var category = new Category
            {
                Name = dto.Name
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Kategori başarıyla oluşturuldu.", categoryId = category.Id });
        }

        // Sadece Admin kategori silebilir
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound(new { message = "Kategori bulunamadı." });
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Kategori başarıyla silindi." });
        }
    }
}