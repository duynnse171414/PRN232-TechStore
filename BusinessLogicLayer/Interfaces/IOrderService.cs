using BusinessLogicLayer.DTOs;

namespace BusinessLogicLayer.Interfaces;

public interface IOrderService
{
    Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto dto);
    Task<OrderResponseDto> CreateOrderFromCartAsync(long userId, CheckoutFromCartRequest request);
    Task<OrderResponseDto> ConfirmPaymentAsync(long orderId);
    Task<OrderResponseDto> FailPaymentAsync(long orderId, string? reason = null);
    Task<PaymentStatusDto> GetPaymentStatusAsync(long orderId);
    Task<OrderResponseDto> GetByIdAsync(long id);
    Task<List<OrderResponseDto>> GetByCustomerAsync(long customerId);
    Task<List<OrderResponseDto>> GetAllAsync(string status = null);
    Task<OrderResponseDto> UpdateStatusAsync(long id, UpdateOrderStatusDto dto);
}
