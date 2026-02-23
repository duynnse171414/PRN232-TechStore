namespace BusinessLogicLayer.DTOs;

public class PromotionDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public int? DiscountPercent { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
}

public class CreatePromotionDto
{
    public string Name { get; set; }
    public int DiscountPercent { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
