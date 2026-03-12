using BusinessLogicLayer.DTOs;
using DAO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TechStoreWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin,staff")]
public class DashboardController : ControllerBase
{
    private readonly TechStoreDBContext _db;

    public DashboardController(TechStoreDBContext db) => _db = db;

    /// <summary>Dashboard tổng quan cho Admin/Staff</summary>
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var paidStatuses = new[] { "paid", "shipping", "completed" };

        var orderCounts = await _db.Orders
            .GroupBy(o => o.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();

        var totalRevenue = await _db.Orders
            .Where(o => paidStatuses.Contains(o.Status))
            .SumAsync(o => o.TotalAmount ?? 0);

        var topProducts = await _db.OrderItems
            .Include(oi => oi.Product)
            .Include(oi => oi.Order)
            .Where(oi => paidStatuses.Contains(oi.Order.Status))
            .GroupBy(oi => new { oi.ProductId, oi.Product.Name })
            .Select(g => new TopProductDto
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.Name,
                TotalQuantitySold = g.Sum(oi => oi.Quantity),
                TotalRevenue = g.Sum(oi => oi.Price * oi.Quantity)
            })
            .OrderByDescending(t => t.TotalQuantitySold)
            .Take(5)
            .ToListAsync();

        var summary = new DashboardSummaryDto
        {
            TotalOrders = orderCounts.Sum(x => x.Count),
            PendingOrders = orderCounts.FirstOrDefault(x => x.Status == "pending")?.Count ?? 0,
            CompletedOrders = orderCounts.FirstOrDefault(x => x.Status == "completed")?.Count ?? 0,
            CancelledOrders = orderCounts.FirstOrDefault(x => x.Status == "cancelled")?.Count ?? 0,
            TotalRevenue = totalRevenue,
            TotalCustomers = await _db.Customers.CountAsync(),
            TotalProducts = await _db.Products.CountAsync(),
            TopSellingProducts = topProducts
        };

        return Ok(summary);
    }
}
