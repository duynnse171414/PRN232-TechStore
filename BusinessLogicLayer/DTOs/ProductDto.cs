namespace BusinessLogicLayer.DTOs;

public class ProductDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Sku { get; set; }
    public string Description { get; set; }
    public string Warranty { get; set; }
    public decimal? Price { get; set; }
    public int? Stock { get; set; }
    public long? BrandId { get; set; }
    public string BrandName { get; set; }
    public long? CategoryId { get; set; }
    public string CategoryName { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public List<ProductSpecDto> Specs { get; set; } = new();
    public DateTime? CreatedAt { get; set; }
}

public class ProductSpecDto
{
    public string Key { get; set; }
    public string Value { get; set; }
}

public class CreateProductDto
{
    public string Name { get; set; }
    public string Sku { get; set; }
    public string Description { get; set; }
    public string Warranty { get; set; }
    public decimal? Price { get; set; }
    public int? Stock { get; set; }
    public long? BrandId { get; set; }
    public long? CategoryId { get; set; }
}

public class UpdateProductDto : CreateProductDto { }

public class ProductFilterDto
{
    public string Search { get; set; }
    public long? CategoryId { get; set; }
    public long? BrandId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}
