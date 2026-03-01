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

    private static IActionResult ApiOk(object data, string message = "Success")
        => new OkObjectResult(new { success = true, message, data });

    private static IActionResult ApiCreated(object data, string message = "Created")
        => new ObjectResult(new { success = true, message, data }) { StatusCode = StatusCodes.Status201Created };

    private static IActionResult ApiBadRequest(string message)
        => new BadRequestObjectResult(new { success = false, message });

    private static IActionResult ApiNotFound(string message = "Not found")
        => new NotFoundObjectResult(new { success = false, message });

    /// <summary>
    /// F09–F11 – Đặt hàng.
    /// - Đã đăng nhập: tự động liên kết với hồ sơ của user.
    /// - Khách vãng lai: cần cung cấp GuestName + GuestPhone.
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest request)
    {
        try
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
                    return ApiBadRequest("GuestName là bắt buộc khi đặt hàng không đăng nhập.");

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

            return ApiCreated(order, "Tạo đơn hàng thành công.");
        }
        catch (InvalidOperationException ex)
        {
            return ApiBadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Checkout từ giỏ hàng (member) – FE có thể dùng như nút “Thanh toán”.
    /// Chưa cần chọn cổng thanh toán, chỉ cần tạo đơn ở trạng thái pending.
    /// </summary>
    [HttpPost("checkout")]
    [Authorize]
    public async Task<IActionResult> CheckoutFromCart([FromBody] CheckoutFromCartRequest request)
    {
        try
        {
            var order = await _orderService.CreateOrderFromCartAsync(TryGetUserId()!.Value, request);
            return ApiCreated(order, "Checkout thành công, đơn hàng đang chờ thanh toán.");
        }
        catch (InvalidOperationException ex)
        {
            return ApiBadRequest(ex.Message);
        }
    }

    /// <summary>Demo thanh toán thành công (member/admin/staff) – chuyển đơn sang paid.</summary>
    [HttpPost("{id}/pay")]
    [Authorize]
    public async Task<IActionResult> MarkPaid(long id)
    {
        try
        {
            var order = await _orderService.ConfirmPaymentAsync(id);
            return ApiOk(order, "Thanh toán thành công.");
        }
        catch (KeyNotFoundException)
        {
            return ApiNotFound("Không tìm thấy đơn hàng.");
        }
        catch (InvalidOperationException ex)
        {
            return ApiBadRequest(ex.Message);
        }
    }

    /// <summary>Demo thanh toán thất bại – hủy đơn (compensation mức đơn giản).</summary>
    [HttpPost("{id}/payment-failed")]
    [Authorize]
    public async Task<IActionResult> MarkPaymentFailed(long id, [FromQuery] string? reason = null)
    {
        try
        {
            var order = await _orderService.FailPaymentAsync(id, reason);
            return ApiOk(order, "Đã cập nhật trạng thái thanh toán thất bại.");
        }
        catch (KeyNotFoundException)
        {
            return ApiNotFound("Không tìm thấy đơn hàng.");
        }
        catch (InvalidOperationException ex)
        {
            return ApiBadRequest(ex.Message);
        }
    }

    /// <summary>Trạng thái thanh toán của đơn hàng</summary>
    [HttpGet("{id}/payment-status")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPaymentStatus(long id)
    {
        try
        {
            var payment = await _orderService.GetPaymentStatusAsync(id);
            return ApiOk(payment);
        }
        catch (KeyNotFoundException)
        {
            return ApiNotFound("Không tìm thấy đơn hàng.");
        }
    }

    /// <summary>F12 – Tra cứu đơn hàng theo ID</summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(long id)
    {
        var order = await _orderService.GetByIdAsync(id);
        return order == null
            ? ApiNotFound("Không tìm thấy đơn hàng.")
            : ApiOk(order);
    }

    /// <summary>F17 – Lịch sử đơn hàng của user đang đăng nhập</summary>
    [HttpGet("my")]
    [Authorize]
    public async Task<IActionResult> GetMyOrders()
    {
        var userId = TryGetUserId()!.Value;
        var profile = await _customerService.GetProfileByUserIdAsync(userId);
        if (profile == null) return ApiOk(new List<OrderResponseDto>());

        var orders = await _orderService.GetByCustomerAsync(profile.Id);
        return ApiOk(orders);
    }

    /// <summary>F13 – Danh sách đơn hàng (Admin/Staff)</summary>
    [HttpGet]
    [Authorize(Roles = "admin,staff")]
    public async Task<IActionResult> GetAll([FromQuery] string status = null)
    {
        var orders = await _orderService.GetAllAsync(status);
        return ApiOk(orders);
    }

    /// <summary>Admin/Staff – Xem đơn theo customerId</summary>
    [HttpGet("customer/{customerId}")]
    [Authorize(Roles = "admin,staff")]
    public async Task<IActionResult> GetByCustomer(long customerId)
    {
        var orders = await _orderService.GetByCustomerAsync(customerId);
        return ApiOk(orders);
    }

    /// <summary>F13 – Cập nhật trạng thái đơn (Admin/Staff)</summary>
    [HttpPut("{id}/status")]
    [Authorize(Roles = "admin,staff")]
    public async Task<IActionResult> UpdateStatus(long id, [FromBody] UpdateOrderStatusDto dto)
    {
        try
        {
            var order = await _orderService.UpdateStatusAsync(id, dto);
            return ApiOk(order, "Cập nhật trạng thái đơn hàng thành công.");
        }
        catch (KeyNotFoundException)
        {
            return ApiNotFound("Không tìm thấy đơn hàng.");
        }
        catch (InvalidOperationException ex)
        {
            return ApiBadRequest(ex.Message);
        }
    }
}
