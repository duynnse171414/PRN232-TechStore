using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Interfaces;
using DAO.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogicLayer.Services;

public class ProductService : IProductService
{
    private readonly TechStoreDBContext _db;

    public ProductService(TechStoreDBContext db) => _db = db;

    public async Task<(List<ProductDto> Items, int Total)> GetProductsAsync(ProductFilterDto filter)
    {
        var query = _db.Products
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(p => p.Name.Contains(filter.Search) || (p.Sku != null && p.Sku.Contains(filter.Search)));

        if (filter.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == filter.CategoryId);

        if (filter.BrandId.HasValue)
            query = query.Where(p => p.BrandId == filter.BrandId);

        if (filter.MinPrice.HasValue)
            query = query.Where(p => p.Price >= filter.MinPrice);

        if (filter.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= filter.MaxPrice);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(p => MapToDto(p))
            .ToListAsync();

        return (items, total);
    }

    public async Task<ProductDto> GetByIdAsync(long id)
    {
        var p = await _db.Products
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .Include(p => p.ProductSpecs)
            .FirstOrDefaultAsync(p => p.Id == id);

        return p == null ? null : MapToDto(p);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Sku = dto.Sku,
            Description = dto.Description,
            Warranty = dto.Warranty,
            Price = dto.Price,
            Stock = dto.Stock ?? 0,
            BrandId = dto.BrandId,
            CategoryId = dto.CategoryId,
            CreatedAt = DateTime.Now
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return await GetByIdAsync(product.Id);
    }

    public async Task<ProductDto> UpdateAsync(long id, UpdateProductDto dto)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return null;

        product.Name = dto.Name;
        product.Sku = dto.Sku;
        product.Description = dto.Description;
        product.Warranty = dto.Warranty;
        product.Price = dto.Price;
        product.Stock = dto.Stock ?? product.Stock;
        product.BrandId = dto.BrandId;
        product.CategoryId = dto.CategoryId;

        await _db.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return false;

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
        return true;
    }

    private static ProductDto MapToDto(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Sku = p.Sku,
        Description = p.Description,
        Warranty = p.Warranty,
        Price = p.Price,
        Stock = p.Stock,
        BrandId = p.BrandId,
        BrandName = p.Brand?.Name,
        CategoryId = p.CategoryId,
        CategoryName = p.Category?.Name,
        CreatedAt = p.CreatedAt,
        ImageUrls = p.ProductImages?.Select(i => i.ImageUrl).ToList() ?? new(),
        Specs = p.ProductSpecs?.Select(s => new ProductSpecDto { Key = s.SpecKey, Value = s.SpecValue }).ToList() ?? new()
    };
}
