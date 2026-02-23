using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Interfaces;
using DAO.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogicLayer.Services;

public class BuildPCService : IBuildPCService
{
    private readonly TechStoreDBContext _db;

    public BuildPCService(TechStoreDBContext db) => _db = db;

    public async Task<List<ComponentTypeDto>> GetComponentTypesWithProductsAsync()
    {
        var types = await _db.PcComponentTypes
            .OrderBy(t => t.SortOrder)
            .ToListAsync();

        var result = new List<ComponentTypeDto>();

        foreach (var t in types)
        {
            var products = await _db.Products
                .Include(p => p.ProductImages)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Sku = p.Sku,
                    Price = p.Price,
                    Stock = p.Stock,
                    ImageUrls = p.ProductImages.Select(i => i.ImageUrl).ToList()
                })
                .ToListAsync();

            result.Add(new ComponentTypeDto
            {
                Id = t.Id,
                Name = t.Name,
                IsRequired = t.IsRequired,
                SortOrder = t.SortOrder,
                Products = products
            });
        }

        return result;
    }

    public async Task<BuildResponseDto> CreateBuildAsync(CreateBuildDto dto)
    {
        var productIds = dto.Items.Select(i => i.ProductId).ToList();
        var products = await _db.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        var build = new PcBuild
        {
            UserId = dto.UserId,
            Name = dto.Name ?? "My Build",
            CreatedAt = DateTime.Now,
            PcBuildItems = dto.Items.Select(i => new PcBuildItem
            {
                ComponentTypeId = i.ComponentTypeId,
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList()
        };

        build.TotalPrice = dto.Items.Sum(i =>
            products.TryGetValue(i.ProductId, out var p) ? (p.Price ?? 0) * i.Quantity : 0);

        _db.PcBuilds.Add(build);
        await _db.SaveChangesAsync();
        return await GetBuildAsync(build.Id);
    }

    public async Task<BuildResponseDto> GetBuildAsync(long buildId)
    {
        var build = await _db.PcBuilds
            .Include(b => b.PcBuildItems).ThenInclude(i => i.Product)
            .Include(b => b.PcBuildItems).ThenInclude(i => i.ComponentType)
            .FirstOrDefaultAsync(b => b.Id == buildId);

        return build == null ? null : MapToDto(build);
    }

    public async Task<List<BuildResponseDto>> GetUserBuildsAsync(long userId)
    {
        var builds = await _db.PcBuilds
            .Include(b => b.PcBuildItems).ThenInclude(i => i.Product)
            .Include(b => b.PcBuildItems).ThenInclude(i => i.ComponentType)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        return builds.Select(MapToDto).ToList();
    }

    private static BuildResponseDto MapToDto(PcBuild b) => new()
    {
        Id = b.Id,
        Name = b.Name,
        TotalPrice = b.TotalPrice,
        CreatedAt = b.CreatedAt,
        Items = b.PcBuildItems.Select(i => new BuildItemResponseDto
        {
            ComponentTypeId = i.ComponentTypeId,
            ComponentTypeName = i.ComponentType?.Name,
            ProductId = i.ProductId,
            ProductName = i.Product?.Name,
            ProductPrice = i.Product?.Price,
            Quantity = i.Quantity
        }).ToList()
    };
}
