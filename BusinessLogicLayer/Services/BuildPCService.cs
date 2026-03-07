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

        var products = await _db.Products
            .Include(p => p.ProductImages)
            .Where(p => p.ComponentTypeId != null)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        var productsByType = products
            .GroupBy(p => p.ComponentTypeId!.Value)
            .ToDictionary(
                g => g.Key,
                g => g.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Sku = p.Sku,
                    Price = p.Price,
                    Stock = p.Stock,
                    ImageUrls = p.ProductImages.Select(i => i.ImageUrl).ToList()
                }).ToList());

        return types.Select(t => new ComponentTypeDto
        {
            Id = t.Id,
            Name = t.Name,
            IsRequired = t.IsRequired,
            SortOrder = t.SortOrder,
            Products = productsByType.TryGetValue(t.Id, out var list) ? list : new List<ProductDto>()
        }).ToList();
    }

    public async Task<BuildResponseDto> CreateBuildAsync(CreateBuildDto dto)
    {
        if (dto == null)
            throw new InvalidOperationException("Dữ liệu build không hợp lệ.");

        if (dto.Items == null || dto.Items.Count == 0)
            throw new InvalidOperationException("Build phải có ít nhất 1 linh kiện.");

        if (dto.Items.Any(i => i.Quantity <= 0))
            throw new InvalidOperationException("Số lượng linh kiện phải lớn hơn 0.");

        var duplicatedTypes = dto.Items
            .GroupBy(i => i.ComponentTypeId)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();
        if (duplicatedTypes.Any())
            throw new InvalidOperationException("Mỗi loại linh kiện chỉ được chọn 1 sản phẩm trong 1 build.");

        var componentTypeIds = dto.Items.Select(i => i.ComponentTypeId).Distinct().ToList();
        var requiredTypeIds = await _db.PcComponentTypes
            .Where(t => t.IsRequired)
            .Select(t => t.Id)
            .ToListAsync();

        var missingRequired = requiredTypeIds.Except(componentTypeIds).ToList();
        if (missingRequired.Any())
            throw new InvalidOperationException("Build chưa chọn đủ các linh kiện bắt buộc.");

        var productIds = dto.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _db.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        if (products.Count != productIds.Count)
            throw new InvalidOperationException("Một hoặc nhiều sản phẩm không tồn tại.");

        foreach (var item in dto.Items)
        {
            var product = products[item.ProductId];

            if (product.ComponentTypeId == null)
                throw new InvalidOperationException($"Sản phẩm '{product.Name}' chưa được gán loại linh kiện Build PC.");

            if (product.ComponentTypeId != item.ComponentTypeId)
                throw new InvalidOperationException($"Sản phẩm '{product.Name}' không thuộc loại linh kiện đã chọn.");

            if ((product.Stock ?? 0) < item.Quantity)
                throw new InvalidOperationException($"Sản phẩm '{product.Name}' không đủ tồn kho.");
        }

        var build = new PcBuild
        {
            UserId = dto.UserId,
            Name = string.IsNullOrWhiteSpace(dto.Name) ? "My Build" : dto.Name,
            CreatedAt = DateTime.Now,
            PcBuildItems = dto.Items.Select(i => new PcBuildItem
            {
                ComponentTypeId = i.ComponentTypeId,
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList()
        };

        build.TotalPrice = dto.Items.Sum(i => (products[i.ProductId].Price ?? 0) * i.Quantity);

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
