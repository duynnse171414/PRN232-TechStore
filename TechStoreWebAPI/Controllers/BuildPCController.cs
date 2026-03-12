using System.Security.Claims;
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TechStoreWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BuildPCController : ControllerBase
{
    private readonly IBuildPCService _buildPCService;

    public BuildPCController(IBuildPCService buildPCService) => _buildPCService = buildPCService;

    private long? TryGetUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return raw == null ? null : long.Parse(raw);
    }

    /// <summary>F22 – Danh sách loại linh kiện kèm sản phẩm để chọn (public)</summary>
    [HttpGet("components")]
    public async Task<IActionResult> GetComponents()
    {
        var types = await _buildPCService.GetComponentTypesWithProductsAsync();
        return Ok(types);
    }

    /// <summary>
    /// F22, F23 – Lưu cấu hình Build PC.
    /// Nếu đã đăng nhập, tự động gán UserId; nếu không thì lưu ẩn danh.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateBuild([FromBody] CreateBuildDto dto)
    {
        try
        {
            dto.UserId ??= TryGetUserId();
            var build = await _buildPCService.CreateBuildAsync(dto);
            return Ok(build);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>F23 – Xem cấu hình Build PC theo ID</summary>
    [HttpGet("{buildId}")]
    public async Task<IActionResult> GetBuild(long buildId)
    {
        var build = await _buildPCService.GetBuildAsync(buildId);
        return build == null ? NotFound() : Ok(build);
    }

    /// <summary>F23 – Danh sách cấu hình đã lưu của user đang đăng nhập</summary>
    [HttpGet("my")]
    [Authorize]
    public async Task<IActionResult> GetMyBuilds()
    {
        var userId = TryGetUserId()!.Value;
        var builds = await _buildPCService.GetUserBuildsAsync(userId);
        return Ok(builds);
    }

    /// <summary>F23 – Cập nhật cấu hình Build PC</summary>
    [HttpPut("{buildId}")]
    [Authorize]
    public async Task<IActionResult> UpdateBuild(long buildId, [FromBody] UpdateBuildDto dto)
    {
        try
        {
            var build = await _buildPCService.UpdateBuildAsync(buildId, TryGetUserId()!.Value, dto);
            return Ok(build);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Build không tồn tại hoặc không thuộc về bạn." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>F23 – Xóa cấu hình Build PC</summary>
    [HttpDelete("{buildId}")]
    [Authorize]
    public async Task<IActionResult> DeleteBuild(long buildId)
    {
        var deleted = await _buildPCService.DeleteBuildAsync(buildId, TryGetUserId()!.Value);
        return deleted ? NoContent() : NotFound();
    }

    /// <summary>Thêm tất cả linh kiện trong build vào giỏ hàng</summary>
    [HttpPost("{buildId}/add-to-cart")]
    [Authorize]
    public async Task<IActionResult> AddBuildToCart(long buildId)
    {
        try
        {
            await _buildPCService.AddBuildToCartAsync(buildId, TryGetUserId()!.Value);
            return Ok(new { message = "Đã thêm linh kiện từ build vào giỏ hàng." });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Build không tồn tại." });
        }
    }
}
