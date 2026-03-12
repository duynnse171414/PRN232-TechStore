using BusinessLogicLayer.DTOs;
using DAO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TechStoreWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin")]
public class UsersController : ControllerBase
{
    private readonly TechStoreDBContext _db;

    public UsersController(TechStoreDBContext db) => _db = db;

    /// <summary>F19 – Danh sách tất cả users (Admin)</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _db.Users
            .Include(u => u.Role)
            .Include(u => u.Customer)
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                RoleName = u.Role.Name,
                CreatedAt = u.CreatedAt,
                CustomerName = u.Customer != null ? u.Customer.Name : null
            })
            .ToListAsync();

        return Ok(users);
    }

    /// <summary>Chi tiết user (Admin)</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var user = await _db.Users
            .Include(u => u.Role)
            .Include(u => u.Customer)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) return NotFound();

        return Ok(new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            RoleName = user.Role?.Name,
            CreatedAt = user.CreatedAt,
            CustomerName = user.Customer?.Name
        });
    }

    /// <summary>F19 – Cập nhật role cho user (Admin)</summary>
    [HttpPut("{id}/role")]
    public async Task<IActionResult> UpdateRole(long id, [FromBody] UpdateUserRoleDto dto)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();

        var roleExists = await _db.Roles.AnyAsync(r => r.Id == dto.RoleId);
        if (!roleExists) return BadRequest(new { message = "Role không tồn tại." });

        user.RoleId = dto.RoleId;
        await _db.SaveChangesAsync();

        await _db.Entry(user).Reference(u => u.Role).LoadAsync();
        return Ok(new { user.Id, user.Email, RoleName = user.Role?.Name });
    }
}
