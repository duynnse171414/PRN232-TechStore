using System.Security.Claims;
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TechStoreWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartsController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartsController(ICartService cartService) => _cartService = cartService;

    private long GetUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>F07, F08 – Xem giỏ hàng của user hiện tại</summary>
    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var cart = await _cartService.GetCartAsync(GetUserId());
        return Ok(cart);
    }

    /// <summary>F07 – Thêm sản phẩm vào giỏ</summary>
    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] AddToCartDto dto)
    {
        var cart = await _cartService.AddItemAsync(GetUserId(), dto);
        return Ok(cart);
    }

    /// <summary>F08 – Cập nhật số lượng sản phẩm trong giỏ</summary>
    [HttpPut("items/{cartItemId}")]
    public async Task<IActionResult> UpdateItem(long cartItemId, [FromBody] UpdateCartItemDto dto)
    {
        try
        {
            var cart = await _cartService.UpdateItemAsync(cartItemId, dto);
            return Ok(cart);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>F08 – Xóa sản phẩm khỏi giỏ</summary>
    [HttpDelete("items/{cartItemId}")]
    public async Task<IActionResult> RemoveItem(long cartItemId)
    {
        try
        {
            var cart = await _cartService.RemoveItemAsync(cartItemId, GetUserId());
            return Ok(cart);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>Xóa toàn bộ giỏ hàng</summary>
    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        await _cartService.ClearCartAsync(GetUserId());
        return NoContent();
    }
}
