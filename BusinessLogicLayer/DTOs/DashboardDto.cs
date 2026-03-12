namespace BusinessLogicLayer.DTOs;

public class DashboardSummaryDto
{
    public int TotalOrders { get; set; }
    public int PendingOrders { get; set; }
    public int CompletedOrders { get; set; }
    public int CancelledOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalCustomers { get; set; }
    public int TotalProducts { get; set; }
    public List<TopProductDto> TopSellingProducts { get; set; } = new();
}

public class TopProductDto
{
    public long ProductId { get; set; }
    public string ProductName { get; set; }
    public int TotalQuantitySold { get; set; }
    public decimal TotalRevenue { get; set; }
}
