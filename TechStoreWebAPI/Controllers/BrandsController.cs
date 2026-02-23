using DAO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TechStoreWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrandsController : ControllerBase
{
    private readonly TechStoreDBContext _db;

    public BrandsController(TechStoreDBContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var brands = await _db.Brands.Select(b => new { b.Id, b.Name }).ToListAsync();
        return Ok(brands);
    }

    [HttpPost]
    [Authorize(Roles = "admin,staff")]
    public async Task<IActionResult> Create([FromBody] string name)
    {
        var brand = new Brand { Name = name };
        _db.Brands.Add(brand);
        await _db.SaveChangesAsync();
        return Ok(new { brand.Id, brand.Name });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin,staff")]
    public async Task<IActionResult> Update(long id, [FromBody] string name)
    {
        var brand = await _db.Brands.FindAsync(id);
        if (brand == null) return NotFound();
        brand.Name = name;
        await _db.SaveChangesAsync();
        return Ok(new { brand.Id, brand.Name });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(long id)
    {
        var brand = await _db.Brands.FindAsync(id);
        if (brand == null) return NotFound();
        _db.Brands.Remove(brand);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
