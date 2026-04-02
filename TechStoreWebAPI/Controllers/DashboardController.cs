using BusinessLogicLayer.DTOs;
using DAO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace TechStoreWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize(Roles = "admin,staff")]
public class DashboardController : ControllerBase
{
    private readonly TechStoreDBContext _db;

    public DashboardController(TechStoreDBContext db) => _db = db;

    /// <summary>Dashboard tổng quan cho Admin/Staff</summary>
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var paidStatuses = new[] { "paid", "shipping", "completed" };
        var currentYear = DateTime.UtcNow.Year;

        var orderCounts = await _db.Orders
            .GroupBy(o => (o.Status ?? string.Empty).ToLower())
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();

        var totalRevenue = await _db.Orders
            .Where(o => paidStatuses.Contains((o.Status ?? string.Empty).ToLower()))
            .SumAsync(o => o.TotalAmount ?? 0);

        var revenueData = await _db.Orders
            .Where(o => o.CreatedAt.HasValue
                && o.CreatedAt.Value.Year == currentYear
                && paidStatuses.Contains((o.Status ?? string.Empty).ToLower()))
            .GroupBy(o => o.CreatedAt!.Value.Month)
            .Select(g => new RevenueDataDto
            {
                Month = "T" + g.Key,
                Revenue = g.Sum(o => o.TotalAmount ?? 0),
                Orders = g.Count()
            })
            .OrderBy(x => x.Month)
            .ToListAsync();

        var categoryData = await _db.OrderItems
            .Where(oi => oi.Order != null
                && oi.Product != null
                && oi.Product.Category != null
                && paidStatuses.Contains((oi.Order.Status ?? string.Empty).ToLower()))
            .GroupBy(oi => oi.Product.Category.Name)
            .Select(g => new CategoryDataDto
            {
                Name = g.Key,
                Value = g.Sum(oi => oi.Quantity)
            })
            .OrderByDescending(x => x.Value)
            .ToListAsync();

        var topProducts = await _db.OrderItems
            .Include(oi => oi.Product)
            .Include(oi => oi.Order)
            .Where(oi => paidStatuses.Contains((oi.Order.Status ?? string.Empty).ToLower()))
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

        var recentOrders = await _db.Orders
            .Include(o => o.Customer)
            .OrderByDescending(o => o.CreatedAt)
            .Take(5)
            .Select(o => new
            {
                Id = o.Id,
                CustomerName = o.Customer != null ? o.Customer.Name : string.Empty,
                TotalAmount = o.TotalAmount ?? 0,
                Status = o.Status,
                CreatedAt = o.CreatedAt
            })
            .ToListAsync();

        var response = new DashboardResponseDto
        {
            Summary = new DashboardSummaryDto
            {
                TotalOrders = orderCounts.Sum(x => x.Count),
                PendingOrders = orderCounts.FirstOrDefault(x => x.Status == "pending")?.Count ?? 0,
                CompletedOrders = orderCounts.FirstOrDefault(x => x.Status == "completed")?.Count ?? 0,
                CancelledOrders = orderCounts.FirstOrDefault(x => x.Status == "cancelled")?.Count ?? 0,
                TotalRevenue = totalRevenue,
                TotalCustomers = await _db.Customers.CountAsync(),
                TotalProducts = await _db.Products.CountAsync()
            },
            RevenueData = revenueData.OrderBy(x => int.Parse(x.Month[1..])).ToList(),
            CategoryData = categoryData,
            TopSellingProducts = topProducts,
            RecentOrders = recentOrders
                .Select(o => new RecentOrderDto
                {
                    Id = o.Id,
                    CustomerName = o.CustomerName,
                    TotalAmount = o.TotalAmount,
                    Status = string.IsNullOrWhiteSpace(o.Status)
                        ? string.Empty
                        : CultureInfo.InvariantCulture.TextInfo.ToTitleCase(o.Status.ToLower()),
                    CreatedAt = o.CreatedAt
                })
                .ToList()
        };

        return Ok(response);
    }
}
