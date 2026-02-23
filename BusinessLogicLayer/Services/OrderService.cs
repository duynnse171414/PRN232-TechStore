using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Interfaces;
using DAO.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogicLayer.Services;

public class OrderService : IOrderService
{
    private readonly TechStoreDBContext _db;

    public OrderService(TechStoreDBContext db) => _db = db;

    public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto dto)
    {
        var productIds = dto.Items.Select(i => i.ProductId).ToList();
        var products = await _db.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

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
                Price = products.TryGetValue(i.ProductId, out var p) ? p.Price : 0
            }).ToList()
        };

        order.TotalAmount = order.OrderItems.Sum(oi => (oi.Price ?? 0) * (oi.Quantity ?? 1));

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        if (!string.IsNullOrWhiteSpace(dto.PaymentMethod))
        {
            _db.Payments.Add(new Payment
            {
                OrderId = order.Id,
                Method = dto.PaymentMethod,
                Status = "pending"
            });
            await _db.SaveChangesAsync();
        }

        return await GetByIdAsync(order.Id);
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

        order.Status = dto.Status;
        if (!string.IsNullOrWhiteSpace(dto.TrackingNumber))
            order.TrackingNumber = dto.TrackingNumber;

        await _db.SaveChangesAsync();
        return await GetByIdAsync(id);
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
