using System.Security.Claims;
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TechStoreWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ICustomerService _customerService;

    public OrdersController(IOrderService orderService, ICustomerService customerService)
    {
        _orderService = orderService;
        _customerService = customerService;
    }

    private long? TryGetUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return raw == null ? null : long.Parse(raw);
    }

    /// <summary>
    /// F09–F11 – Đặt hàng.
    /// - Đã đăng nhập: tự động liên kết với hồ sơ của user.
    /// - Khách vãng lai: cần cung cấp GuestName + GuestPhone.
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest request)
    {
        long customerId;

        var userId = TryGetUserId();
        if (userId.HasValue)
        {
            var profile = await _customerService.GetOrCreateCustomerByUserIdAsync(userId.Value);
            customerId = profile.Id;
        }
        else
        {
            if (string.IsNullOrWhiteSpace(request.GuestName))
                return BadRequest("GuestName là bắt buộc khi đặt hàng không đăng nhập.");

            var guest = await _customerService.CreateGuestCustomerAsync(new CreateGuestCustomerDto
            {
                Name = request.GuestName,
                Email = request.GuestEmail,
                Phone = request.GuestPhone
            });
            customerId = guest.Id;
        }

        var order = await _orderService.CreateOrderAsync(new CreateOrderDto
        {
            CustomerId = customerId,
            AddressId = request.AddressId,
            Notes = request.Notes,
            PaymentMethod = request.PaymentMethod,
            Items = request.Items
        });

        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    /// <summary>F12 – Tra cứu đơn hàng theo ID</summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(long id)
    {
        var order = await _orderService.GetByIdAsync(id);
        return order == null ? NotFound() : Ok(order);
    }

    /// <summary>F17 – Lịch sử đơn hàng của user đang đăng nhập</summary>
    [HttpGet("my")]
    [Authorize]
    public async Task<IActionResult> GetMyOrders()
    {
        var userId = TryGetUserId()!.Value;
        var profile = await _customerService.GetProfileByUserIdAsync(userId);
        if (profile == null) return Ok(new List<OrderResponseDto>());

        var orders = await _orderService.GetByCustomerAsync(profile.Id);
        return Ok(orders);
    }

    /// <summary>F13 – Danh sách đơn hàng (Admin/Staff)</summary>
    [HttpGet]
    [Authorize(Roles = "admin,staff")]
    public async Task<IActionResult> GetAll([FromQuery] string status = null)
    {
        var orders = await _orderService.GetAllAsync(status);
        return Ok(orders);
    }

    /// <summary>Admin/Staff – Xem đơn theo customerId</summary>
    [HttpGet("customer/{customerId}")]
    [Authorize(Roles = "admin,staff")]
    public async Task<IActionResult> GetByCustomer(long customerId)
    {
        var orders = await _orderService.GetByCustomerAsync(customerId);
        return Ok(orders);
    }

    /// <summary>F13 – Cập nhật trạng thái đơn (Admin/Staff)</summary>
    [HttpPut("{id}/status")]
    [Authorize(Roles = "admin,staff")]
    public async Task<IActionResult> UpdateStatus(long id, [FromBody] UpdateOrderStatusDto dto)
    {
        try
        {
            var order = await _orderService.UpdateStatusAsync(id, dto);
            return Ok(order);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
