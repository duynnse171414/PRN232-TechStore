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

            if (product.Stock < item.Quantity)
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

        build.TotalPrice = dto.Items.Sum(i => products[i.ProductId].Price * i.Quantity);

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

    public async Task<BuildResponseDto> UpdateBuildAsync(long buildId, long userId, UpdateBuildDto dto)
    {
        var build = await _db.PcBuilds
            .Include(b => b.PcBuildItems)
            .FirstOrDefaultAsync(b => b.Id == buildId && b.UserId == userId)
            ?? throw new KeyNotFoundException("Build không tồn tại hoặc không thuộc về bạn.");

        if (dto.Items == null || dto.Items.Count == 0)
            throw new InvalidOperationException("Build phải có ít nhất 1 linh kiện.");

        if (dto.Items.Any(i => i.Quantity <= 0))
            throw new InvalidOperationException("Số lượng linh kiện phải lớn hơn 0.");

        var duplicatedTypes = dto.Items.GroupBy(i => i.ComponentTypeId).Where(g => g.Count() > 1).ToList();
        if (duplicatedTypes.Any())
            throw new InvalidOperationException("Mỗi loại linh kiện chỉ được chọn 1 sản phẩm trong 1 build.");

        var componentTypeIds = dto.Items.Select(i => i.ComponentTypeId).Distinct().ToList();
        var requiredTypeIds = await _db.PcComponentTypes.Where(t => t.IsRequired).Select(t => t.Id).ToListAsync();
        if (requiredTypeIds.Except(componentTypeIds).Any())
            throw new InvalidOperationException("Build chưa chọn đủ các linh kiện bắt buộc.");

        var productIds = dto.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _db.Products.Where(p => productIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id);
        if (products.Count != productIds.Count)
            throw new InvalidOperationException("Một hoặc nhiều sản phẩm không tồn tại.");

        foreach (var item in dto.Items)
        {
            var product = products[item.ProductId];
            if (product.ComponentTypeId != item.ComponentTypeId)
                throw new InvalidOperationException($"Sản phẩm '{product.Name}' không thuộc loại linh kiện đã chọn.");
            if (product.Stock < item.Quantity)
                throw new InvalidOperationException($"Sản phẩm '{product.Name}' không đủ tồn kho.");
        }

        // Remove old items, add new ones
        _db.PcBuildItems.RemoveRange(build.PcBuildItems);
        build.Name = string.IsNullOrWhiteSpace(dto.Name) ? build.Name : dto.Name;
        build.PcBuildItems = dto.Items.Select(i => new PcBuildItem
        {
            ComponentTypeId = i.ComponentTypeId,
            ProductId = i.ProductId,
            Quantity = i.Quantity
        }).ToList();
        build.TotalPrice = dto.Items.Sum(i => products[i.ProductId].Price * i.Quantity);

        await _db.SaveChangesAsync();
        return await GetBuildAsync(build.Id);
    }

    public async Task<bool> DeleteBuildAsync(long buildId, long userId)
    {
        var build = await _db.PcBuilds
            .Include(b => b.PcBuildItems)
            .FirstOrDefaultAsync(b => b.Id == buildId && b.UserId == userId);

        if (build == null) return false;

        _db.PcBuildItems.RemoveRange(build.PcBuildItems);
        _db.PcBuilds.Remove(build);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task AddBuildToCartAsync(long buildId, long userId)
    {
        var build = await _db.PcBuilds
            .Include(b => b.PcBuildItems)
            .FirstOrDefaultAsync(b => b.Id == buildId)
            ?? throw new KeyNotFoundException("Build không tồn tại.");

        // Get or create cart for user
        var cart = await _db.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
        {
            cart = new Cart { UserId = userId, CreatedAt = DateTime.Now };
            _db.Carts.Add(cart);
            await _db.SaveChangesAsync();
        }

        // Add each build item to cart, merge if product already exists
        foreach (var buildItem in build.PcBuildItems)
        {
            var existing = cart.CartItems.FirstOrDefault(ci => ci.ProductId == buildItem.ProductId);
            if (existing != null)
                existing.Quantity += buildItem.Quantity;
            else
                cart.CartItems.Add(new CartItem
                {
                    CartId = cart.Id,
                    ProductId = buildItem.ProductId,
                    Quantity = buildItem.Quantity
                });
        }

        await _db.SaveChangesAsync();
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
