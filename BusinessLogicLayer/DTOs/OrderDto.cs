namespace BusinessLogicLayer.DTOs;

/// <summary>
/// Dùng cho cả guest và logged-in checkout.
/// Controller truyền customerId đã được resolve trước khi gọi service.
/// </summary>
public class CreateOrderDto
{
    public long CustomerId { get; set; }
    public long? AddressId { get; set; }
    public string Notes { get; set; }
    public string PaymentMethod { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

/// <summary>Body request từ client khi đặt hàng (guest hoặc member)</summary>
public class PlaceOrderRequest
{
    // Dành cho guest (bỏ trống nếu đã đăng nhập)
    public string GuestName { get; set; }
    public string GuestEmail { get; set; }
    public string GuestPhone { get; set; }

    public long? AddressId { get; set; }
    public string Notes { get; set; }
    public string PaymentMethod { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    public long ProductId { get; set; }
    public int Quantity { get; set; }
}

public class OrderResponseDto
{
    public long Id { get; set; }
    public long CustomerId { get; set; }
    public string CustomerName { get; set; }
    public string Status { get; set; }
    public decimal? TotalAmount { get; set; }
    public decimal? ShippingFee { get; set; }
    public string Notes { get; set; }
    public string TrackingNumber { get; set; }
    public DateTime? CreatedAt { get; set; }
    public List<OrderItemResponseDto> Items { get; set; } = new();
}

public class OrderItemResponseDto
{
    public long ProductId { get; set; }
    public string ProductName { get; set; }
    public int? Quantity { get; set; }
    public decimal? Price { get; set; }
}

public class UpdateOrderStatusDto
{
    public string Status { get; set; }
    public string TrackingNumber { get; set; }
}
