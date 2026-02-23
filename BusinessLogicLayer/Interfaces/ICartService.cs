using BusinessLogicLayer.DTOs;

namespace BusinessLogicLayer.Interfaces;

public interface ICartService
{
    Task<CartDto> GetCartAsync(long userId);
    Task<CartDto> AddItemAsync(long userId, AddToCartDto dto);
    Task<CartDto> UpdateItemAsync(long cartItemId, UpdateCartItemDto dto);
    Task<CartDto> RemoveItemAsync(long cartItemId, long userId);
    Task ClearCartAsync(long userId);
}
