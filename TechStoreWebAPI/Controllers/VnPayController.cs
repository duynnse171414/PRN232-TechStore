using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TechStoreWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VnPayController : ControllerBase
{
    private readonly IVnPayService _vnPayService;
    private readonly IOrderService _orderService;

    public VnPayController(IVnPayService vnPayService, IOrderService orderService)
    {
        _vnPayService = vnPayService;
        _orderService = orderService;
    }

    /// <summary>Tạo URL thanh toán VNPay cho đơn hàng pending</summary>
    [HttpPost("create-payment-url")]
    [Authorize]
    public async Task<IActionResult> CreatePaymentUrl([FromBody] CreateVnPayUrlBody body)
    {
        try
        {
            var order = await _orderService.GetByIdAsync(body.OrderId);
            if (order == null)
                return NotFound(new { success = false, message = "Không tìm thấy đơn hàng." });

            if (order.Status != "pending")
                return BadRequest(new { success = false, message = "Chỉ có thể thanh toán đơn hàng đang ở trạng thái pending." });

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "127.0.0.1";
            var orderInfo = $"Thanh toan don hang #{order.Id}";
            var amount = order.TotalAmount ?? 0;

            var paymentUrl = _vnPayService.CreatePaymentUrl(order.Id, amount, orderInfo, ipAddress);

            return Ok(new { success = true, data = new { orderId = order.Id, paymentUrl } });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// VNPay redirect user về đây sau khi thanh toán (browser redirect).
    /// Chỉ hiển thị kết quả, KHÔNG cập nhật DB (IPN làm việc đó).
    /// </summary>
    [HttpGet("payment-return")]
    [AllowAnonymous]
    public IActionResult PaymentReturn()
    {
        var result = _vnPayService.ProcessCallback(Request.Query);

        if (!result.IsValid)
            return BadRequest(new { success = false, message = "Chữ ký không hợp lệ." });

        return Ok(new
        {
            success = result.IsSuccess,
            message = result.IsSuccess ? "Thanh toán thành công." : $"Thanh toán thất bại (mã: {result.ResponseCode}).",
            data = new
            {
                result.OrderId,
                result.Amount,
                result.BankCode,
                result.ResponseCode,
                result.TransactionStatus,
                result.VnPayTransactionNo,
                result.PayDate
            }
        });
    }

    /// <summary>
    /// IPN – VNPay server gọi endpoint này để thông báo kết quả (server-to-server).
    /// Phải trả JSON { RspCode, Message } theo spec VNPay.
    /// </summary>
    [HttpGet("ipn")]
    [AllowAnonymous]
    public async Task<IActionResult> IpnCallback()
    {
        var result = _vnPayService.ProcessCallback(Request.Query);

        // Hash không hợp lệ
        if (!result.IsValid)
            return Ok(new { RspCode = "97", Message = "Invalid Checksum" });

        try
        {
            var order = await _orderService.GetByIdAsync(result.OrderId);

            // Đơn hàng không tồn tại
            if (order == null)
                return Ok(new { RspCode = "01", Message = "Order not found" });

            // Đơn hàng đã được xử lý rồi (paid/shipping/completed)
            if (order.Status is "paid" or "shipping" or "completed")
                return Ok(new { RspCode = "02", Message = "Order already confirmed" });

            // Kiểm tra số tiền khớp
            if (order.TotalAmount != result.Amount)
                return Ok(new { RspCode = "04", Message = "Invalid Amount" });

            // Thanh toán thành công → confirm payment
            if (result.IsSuccess)
            {
                await _orderService.ConfirmPaymentAsync(result.OrderId);
                return Ok(new { RspCode = "00", Message = "Confirm Success" });
            }

            // Thanh toán thất bại
            await _orderService.FailPaymentAsync(result.OrderId, $"VNPay error: {result.ResponseCode}");
            return Ok(new { RspCode = "00", Message = "Confirm Success" });
        }
        catch (Exception)
        {
            return Ok(new { RspCode = "99", Message = "Unknown error" });
        }
    }
}

/// <summary>Body cho request tạo URL VNPay</summary>
public class CreateVnPayUrlBody
{
    public long OrderId { get; set; }
}
