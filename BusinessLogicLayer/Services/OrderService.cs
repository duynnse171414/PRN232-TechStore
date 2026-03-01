using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Interfaces;
using DAO.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogicLayer.Services;

public class OrderService : IOrderService
{
    private static readonly HashSet<string> ValidOrderStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "pending", "paid", "shipping", "completed", "cancelled"
    };

    private readonly TechStoreDBContext _db;

    public OrderService(TechStoreDBContext db) => _db = db;

    public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto dto)
    {
        var productIds = dto.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _db.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        if (products.Count != productIds.Count)
            throw new InvalidOperationException("Một hoặc nhiều sản phẩm không tồn tại.");

        foreach (var item in dto.Items)
        {
            if (item.Quantity <= 0)
                throw new InvalidOperationException("Số lượng sản phẩm phải lớn hơn 0.");

            var p = products[item.ProductId];
            if ((p.Stock ?? 0) < item.Quantity)
                throw new InvalidOperationException($"Sản phẩm '{p.Name}' không đủ tồn kho.");
        }

        var order = new Order
        {
            CustomerId = dto.CustomerId,
            AddressId = dto.AddressId,
            Notes = dto.Notes,
            Status = "pending",
            ShippingFee = 0,
            CreatedAt = DateTime.Now,
            OrderItems = dto.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = products[i.ProductId].Price
            }).ToList()
        };

        order.TotalAmount = order.OrderItems.Sum(oi => (oi.Price ?? 0) * (oi.Quantity ?? 1));

        _db.Orders.Add(order);

        if (!string.IsNullOrWhiteSpace(dto.PaymentMethod))
        {
            _db.Payments.Add(new Payment
            {
                Order = order,
                Method = dto.PaymentMethod,
                Status = "pending"
            });
        }

        await _db.SaveChangesAsync();
        return await GetByIdAsync(order.Id);
    }

    public async Task<OrderResponseDto> CreateOrderFromCartAsync(long userId, CheckoutFromCartRequest request)
    {
        var customer = await _db.Customers.FirstOrDefaultAsync(c => c.UserId == userId)
            ?? throw new InvalidOperationException("Không tìm thấy hồ sơ khách hàng.");

        var cart = await _db.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null || !cart.CartItems.Any())
            throw new InvalidOperationException("Giỏ hàng đang trống.");

        var items = cart.CartItems.Select(ci => new OrderItemDto
        {
            ProductId = ci.ProductId,
            Quantity = ci.Quantity ?? 1
        }).ToList();

        var order = await CreateOrderAsync(new CreateOrderDto
        {
            CustomerId = customer.Id,
            AddressId = request.AddressId,
            Notes = request.Notes,
            PaymentMethod = "checkout",
            Items = items
        });

        _db.CartItems.RemoveRange(cart.CartItems);
        await _db.SaveChangesAsync();

        return order;
    }

    public async Task<OrderResponseDto> ConfirmPaymentAsync(long orderId)
    {
        var order = await _db.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId)
            ?? throw new KeyNotFoundException("Order not found.");

        if (order.Status is "paid" or "shipping" or "completed")
            return await GetByIdAsync(orderId);

        if (order.Status == "cancelled")
            throw new InvalidOperationException("Đơn hàng đã bị huỷ, không thể thanh toán.");

        foreach (var item in order.OrderItems)
        {
            var product = item.Product;
            var quantity = item.Quantity ?? 1;

            if ((product.Stock ?? 0) < quantity)
                throw new InvalidOperationException($"Sản phẩm '{product.Name}' không đủ tồn kho để thanh toán.");

            product.Stock = (product.Stock ?? 0) - quantity;
        }

        order.Status = "paid";

        var payment = await _db.Payments
            .Where(p => p.OrderId == orderId)
            .OrderByDescending(p => p.Id)
            .FirstOrDefaultAsync();

        if (payment == null)
        {
            payment = new Payment { OrderId = order.Id, Method = "checkout", Status = "paid", PaidAt = DateTime.Now };
            _db.Payments.Add(payment);
        }
        else
        {
            payment.Status = "paid";
            payment.PaidAt = DateTime.Now;
        }

        await _db.SaveChangesAsync();
        return await GetByIdAsync(orderId);
    }

    public async Task<OrderResponseDto> FailPaymentAsync(long orderId, string? reason = null)
    {
        var order = await _db.Orders.FindAsync(orderId)
            ?? throw new KeyNotFoundException("Order not found.");

        if (order.Status == "paid")
            throw new InvalidOperationException("Đơn hàng đã thanh toán, không thể đánh dấu thất bại.");

        if (order.Status == "shipping" || order.Status == "completed")
            throw new InvalidOperationException("Đơn hàng đang giao/hoàn tất, không thể đánh dấu thanh toán thất bại.");

        order.Status = "cancelled";

        var payment = await _db.Payments
            .Where(p => p.OrderId == orderId)
            .OrderByDescending(p => p.Id)
            .FirstOrDefaultAsync();

        if (payment == null)
        {
            payment = new Payment { OrderId = order.Id, Method = "checkout", Status = "failed" };
            _db.Payments.Add(payment);
        }
        else
        {
            payment.Status = "failed";
        }

        if (!string.IsNullOrWhiteSpace(reason))
        {
            order.Notes = string.IsNullOrWhiteSpace(order.Notes)
                ? $"Payment failed: {reason}"
                : $"{order.Notes} | Payment failed: {reason}";
        }

        await _db.SaveChangesAsync();
        return await GetByIdAsync(orderId);
    }

    public async Task<PaymentStatusDto> GetPaymentStatusAsync(long orderId)
    {
        var orderExists = await _db.Orders.AnyAsync(o => o.Id == orderId);
        if (!orderExists)
            throw new KeyNotFoundException("Order not found.");

        var payment = await _db.Payments
            .Where(p => p.OrderId == orderId)
            .OrderByDescending(p => p.Id)
            .FirstOrDefaultAsync();

        if (payment == null)
        {
            return new PaymentStatusDto
            {
                OrderId = orderId,
                PaymentStatus = "pending",
                PaymentMethod = "checkout",
                PaidAt = null
            };
        }

        return new PaymentStatusDto
        {
            OrderId = orderId,
            PaymentStatus = payment.Status,
            PaymentMethod = payment.Method,
            PaidAt = payment.PaidAt
        };
    }

    public async Task<OrderResponseDto> GetByIdAsync(long id)
    {
        var order = await _db.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        return order == null ? null : MapToDto(order);
    }

    public async Task<List<OrderResponseDto>> GetByCustomerAsync(long customerId)
    {
        var orders = await _db.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return orders.Select(MapToDto).ToList();
    }

    public async Task<List<OrderResponseDto>> GetAllAsync(string status = null)
    {
        var query = _db.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(o => o.Status == status);

        var orders = await query.OrderByDescending(o => o.CreatedAt).ToListAsync();
        return orders.Select(MapToDto).ToList();
    }

    public async Task<OrderResponseDto> UpdateStatusAsync(long id, UpdateOrderStatusDto dto)
    {
        var order = await _db.Orders.FindAsync(id)
            ?? throw new KeyNotFoundException("Order not found.");

        var nextStatus = (dto.Status ?? string.Empty).Trim().ToLowerInvariant();
        if (!ValidOrderStatuses.Contains(nextStatus))
            throw new InvalidOperationException("Trạng thái không hợp lệ.");

        var currentStatus = (order.Status ?? string.Empty).Trim().ToLowerInvariant();
        if (!IsValidTransition(currentStatus, nextStatus))
            throw new InvalidOperationException($"Không thể chuyển trạng thái từ '{currentStatus}' sang '{nextStatus}'.");

        order.Status = nextStatus;
        if (!string.IsNullOrWhiteSpace(dto.TrackingNumber))
            order.TrackingNumber = dto.TrackingNumber;

        await _db.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    private static bool IsValidTransition(string from, string to)
    {
        if (from == to) return true;

        return from switch
        {
            "pending" => to is "paid" or "cancelled",
            "paid" => to is "shipping" or "cancelled",
            "shipping" => to is "completed",
            "completed" => false,
            "cancelled" => false,
            _ => false
        };
    }

    private static OrderResponseDto MapToDto(Order o) => new()
    {
        Id = o.Id,
        CustomerId = o.CustomerId,
        CustomerName = o.Customer?.Name,
        Status = o.Status,
        TotalAmount = o.TotalAmount,
        ShippingFee = o.ShippingFee,
        Notes = o.Notes,
        TrackingNumber = o.TrackingNumber,
        CreatedAt = o.CreatedAt,
        Items = o.OrderItems.Select(oi => new OrderItemResponseDto
        {
            ProductId = oi.ProductId,
            ProductName = oi.Product?.Name,
            Quantity = oi.Quantity,
            Price = oi.Price
        }).ToList()
    };
}
