namespace BusinessLogicLayer.DTOs;

public class VoucherDto
{
    public long Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public int? DiscountPercent { get; set; }
    public decimal? MaxDiscount { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public class CreateVoucherDto
{
    public string Code { get; set; }
    public string Name { get; set; }
    public int? DiscountPercent { get; set; }
    public decimal? MaxDiscount { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;
}
