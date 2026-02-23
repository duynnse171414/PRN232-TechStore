using System.Security.Claims;
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TechStoreWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService) => _customerService = customerService;

    private long GetUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>F16 – Xem hồ sơ mua hàng của user đang đăng nhập</summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var profile = await _customerService.GetProfileByUserIdAsync(GetUserId());
        return profile == null ? NotFound() : Ok(profile);
    }

    /// <summary>F16 – Cập nhật hồ sơ (tên, email liên lạc, phone)</summary>
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileDto dto)
    {
        var profile = await _customerService.GetOrCreateCustomerByUserIdAsync(GetUserId());
        try
        {
            var updated = await _customerService.UpdateProfileAsync(profile.Id, dto);
            return Ok(updated);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>F09 – Danh sách địa chỉ giao hàng của user đang đăng nhập</summary>
    [HttpGet("me/addresses")]
    public async Task<IActionResult> GetMyAddresses()
    {
        var profile = await _customerService.GetOrCreateCustomerByUserIdAsync(GetUserId());
        var addresses = await _customerService.GetAddressesAsync(profile.Id);
        return Ok(addresses);
    }

    /// <summary>F09 – Thêm địa chỉ mới</summary>
    [HttpPost("me/addresses")]
    public async Task<IActionResult> AddAddress([FromBody] CreateAddressDto dto)
    {
        var profile = await _customerService.GetOrCreateCustomerByUserIdAsync(GetUserId());
        var address = await _customerService.AddAddressAsync(profile.Id, dto);
        return Ok(address);
    }

    /// <summary>Xóa địa chỉ</summary>
    [HttpDelete("me/addresses/{addressId}")]
    public async Task<IActionResult> DeleteAddress(long addressId)
    {
        var profile = await _customerService.GetOrCreateCustomerByUserIdAsync(GetUserId());
        var deleted = await _customerService.DeleteAddressAsync(profile.Id, addressId);
        return deleted ? NoContent() : NotFound();
    }

    /// <summary>F19 – Danh sách tất cả khách hàng (Admin/Staff)</summary>
    [HttpGet]
    [Authorize(Roles = "admin,staff")]
    public async Task<IActionResult> GetAll()
    {
        var customers = await _customerService.GetAllCustomersAsync();
        return Ok(customers);
    }

    /// <summary>Admin – Xem hồ sơ theo customer.id</summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "admin,staff")]
    public async Task<IActionResult> GetById(long id)
    {
        var profile = await _customerService.GetProfileAsync(id);
        return profile == null ? NotFound() : Ok(profile);
    }
}
