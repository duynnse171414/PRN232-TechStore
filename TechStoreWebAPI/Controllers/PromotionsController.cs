using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TechStoreWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PromotionsController : ControllerBase
{
    private readonly IPromotionService _promotionService;

    public PromotionsController(IPromotionService promotionService) => _promotionService = promotionService;

    /// <summary>F20 – Tất cả khuyến mãi (public)</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _promotionService.GetAllAsync());

    /// <summary>F20 – Các khuyến mãi đang hoạt động</summary>
    [HttpGet("active")]
    public async Task<IActionResult> GetActive() => Ok(await _promotionService.GetActiveAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var promo = await _promotionService.GetByIdAsync(id);
        return promo == null ? NotFound() : Ok(promo);
    }

    /// <summary>F21 – Tạo khuyến mãi (Admin/Staff)</summary>
    [HttpPost]
    [Authorize(Roles = "admin,staff")]
    public async Task<IActionResult> Create([FromBody] CreatePromotionDto dto)
    {
        var promo = await _promotionService.CreateAsync(dto);
        return Ok(promo);
    }

    /// <summary>F21 – Cập nhật khuyến mãi</summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "admin,staff")]
    public async Task<IActionResult> Update(long id, [FromBody] CreatePromotionDto dto)
    {
        try
        {
            var promo = await _promotionService.UpdateAsync(id, dto);
            return Ok(promo);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>F21 – Xóa khuyến mãi</summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(long id)
    {
        var deleted = await _promotionService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
