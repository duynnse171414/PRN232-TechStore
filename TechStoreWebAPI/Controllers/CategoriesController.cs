using DAO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TechStoreWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly TechStoreDBContext _db;

    public CategoriesController(TechStoreDBContext db) => _db = db;

    /// <summary>F01, F06 – Danh sách danh mục</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _db.Categories
            .Select(c => new { c.Id, c.Name })
            .ToListAsync();
        return Ok(categories);
    }

    [HttpPost]
    [Authorize(Roles = "admin,staff")]
    public async Task<IActionResult> Create([FromBody] string name)
    {
        var category = new Category { Name = name };
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();
        return Ok(new { category.Id, category.Name });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin,staff")]
    public async Task<IActionResult> Update(long id, [FromBody] string name)
    {
        var category = await _db.Categories.FindAsync(id);
        if (category == null) return NotFound();
        category.Name = name;
        await _db.SaveChangesAsync();
        return Ok(new { category.Id, category.Name });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(long id)
    {
        var category = await _db.Categories.FindAsync(id);
        if (category == null) return NotFound();
        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
