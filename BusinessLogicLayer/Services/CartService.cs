using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Interfaces;
using DAO.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogicLayer.Services;

public class CartService : ICartService
{
    private readonly TechStoreDBContext _db;

    public CartService(TechStoreDBContext db) => _db = db;

    public async Task<CartDto> GetCartAsync(long userId)
    {
        var cart = await GetOrCreateCartAsync(userId);
        return MapCart(cart);
    }

    public async Task<CartDto> AddItemAsync(long userId, AddToCartDto dto)
    {
        var cart = await GetOrCreateCartAsync(userId);

        var existing = cart.CartItems.FirstOrDefault(ci => ci.ProductId == dto.ProductId);
        if (existing != null)
            existing.Quantity = (existing.Quantity ?? 1) + dto.Quantity;
        else
            cart.CartItems.Add(new CartItem
            {
                CartId = cart.Id,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            });

        await _db.SaveChangesAsync();
        return MapCart(cart);
    }

    public async Task<CartDto> UpdateItemAsync(long cartItemId, UpdateCartItemDto dto)
    {
        var item = await _db.CartItems
            .Include(ci => ci.Cart)
            .FirstOrDefaultAsync(ci => ci.Id == cartItemId)
            ?? throw new KeyNotFoundException("Cart item not found.");

        if (dto.Quantity <= 0)
            _db.CartItems.Remove(item);
        else
            item.Quantity = dto.Quantity;

        await _db.SaveChangesAsync();

        var cart = await _db.Carts
            .Include(c => c.CartItems).ThenInclude(ci => ci.Product).ThenInclude(p => p.ProductImages)
            .FirstAsync(c => c.Id == item.CartId);

        return MapCart(cart);
    }

    public async Task<CartDto> RemoveItemAsync(long cartItemId, long userId)
    {
        var item = await _db.CartItems.FindAsync(cartItemId)
            ?? throw new KeyNotFoundException("Cart item not found.");

        _db.CartItems.Remove(item);
        await _db.SaveChangesAsync();

        var cart = await GetOrCreateCartAsync(userId);
        return MapCart(cart);
    }

    public async Task ClearCartAsync(long userId)
    {
        var cart = await _db.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart != null)
        {
            _db.CartItems.RemoveRange(cart.CartItems);
            await _db.SaveChangesAsync();
        }
    }

    private async Task<Cart> GetOrCreateCartAsync(long userId)
    {
        var cart = await _db.Carts
            .Include(c => c.CartItems).ThenInclude(ci => ci.Product).ThenInclude(p => p.ProductImages)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart != null) return cart;

        cart = new Cart { UserId = userId, CreatedAt = DateTime.Now };
        _db.Carts.Add(cart);
        await _db.SaveChangesAsync();
        return cart;
    }

    private static CartDto MapCart(Cart cart) => new()
    {
        CartId = cart.Id,
        Items = cart.CartItems.Select(ci => new CartItemDto
        {
            CartItemId = ci.Id,
            ProductId = ci.ProductId,
            ProductName = ci.Product?.Name,
            ProductPrice = ci.Product?.Price,
            ImageUrl = ci.Product?.ProductImages?.FirstOrDefault()?.ImageUrl,
            Quantity = ci.Quantity,
            Subtotal = (ci.Product?.Price ?? 0) * (ci.Quantity ?? 1)
        }).ToList(),
        Total = cart.CartItems.Sum(ci => (ci.Product?.Price ?? 0) * (ci.Quantity ?? 1))
    };
}
