namespace BusinessLogicLayer.DTOs;

public class DashboardResponseDto
{
    public DashboardSummaryDto Summary { get; set; } = new();
    public List<RevenueDataDto> RevenueData { get; set; } = new();
    public List<CategoryDataDto> CategoryData { get; set; } = new();
    public List<TopProductDto> TopSellingProducts { get; set; } = new();
    public List<RecentOrderDto> RecentOrders { get; set; } = new();
}

public class DashboardSummaryDto
{
    public int TotalOrders { get; set; }
    public int PendingOrders { get; set; }
    public int CompletedOrders { get; set; }
    public int CancelledOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalCustomers { get; set; }
    public int TotalProducts { get; set; }
}

public class RevenueDataDto
{
    public string Month { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public int Orders { get; set; }
}

public class CategoryDataDto
{
    public string Name { get; set; } = string.Empty;
    public int Value { get; set; }
}

public class TopProductDto
{
    public long ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int TotalQuantitySold { get; set; }
    public decimal TotalRevenue { get; set; }
}

public class RecentOrderDto
{
    public long Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? CreatedAt { get; set; }
}
