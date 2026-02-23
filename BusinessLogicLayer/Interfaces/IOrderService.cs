using BusinessLogicLayer.DTOs;

namespace BusinessLogicLayer.Interfaces;

public interface IOrderService
{
    Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto dto);
    Task<OrderResponseDto> GetByIdAsync(long id);
    Task<List<OrderResponseDto>> GetByCustomerAsync(long customerId);
    Task<List<OrderResponseDto>> GetAllAsync(string status = null);
    Task<OrderResponseDto> UpdateStatusAsync(long id, UpdateOrderStatusDto dto);
}
