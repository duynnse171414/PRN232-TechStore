namespace BusinessLogicLayer.DTOs;

public class ComponentTypeDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public bool IsRequired { get; set; }
    public int SortOrder { get; set; }
    public List<ProductDto> Products { get; set; } = new();
}

public class CreateBuildDto
{
    /// <summary>Nullable – null nếu chưa đăng nhập (build không lưu server)</summary>
    public long? UserId { get; set; }
    public string Name { get; set; }
    public List<BuildItemDto> Items { get; set; } = new();
}

public class BuildItemDto
{
    public long ComponentTypeId { get; set; }
    public long ProductId { get; set; }
    public int Quantity { get; set; } = 1;
}

public class BuildResponseDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public decimal? TotalPrice { get; set; }
    public DateTime? CreatedAt { get; set; }
    public List<BuildItemResponseDto> Items { get; set; } = new();
}

public class BuildItemResponseDto
{
    public long ComponentTypeId { get; set; }
    public string ComponentTypeName { get; set; }
    public long ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal? ProductPrice { get; set; }
    public int Quantity { get; set; }
}
