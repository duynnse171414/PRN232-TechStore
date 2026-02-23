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
        dto.UserId ??= TryGetUserId();
        var build = await _buildPCService.CreateBuildAsync(dto);
        return Ok(build);
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
}
