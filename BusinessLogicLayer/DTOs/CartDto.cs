namespace BusinessLogicLayer.DTOs;

public class CartDto
{
    public long CartId { get; set; }
    public List<CartItemDto> Items { get; set; } = new();
    public decimal Total { get; set; }
}

public class CartItemDto
{
    public long CartItemId { get; set; }
    public long ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal? ProductPrice { get; set; }
    public string ImageUrl { get; set; }
    public int? Quantity { get; set; }
    public decimal? Subtotal { get; set; }
}

public class AddToCartDto
{
    public long ProductId { get; set; }
    public int Quantity { get; set; } = 1;
}

public class UpdateCartItemDto
{
    public int Quantity { get; set; }
}
