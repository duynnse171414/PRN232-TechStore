using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Interfaces;
using DAO.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogicLayer.Services;

public class PromotionService : IPromotionService
{
    private readonly TechStoreDBContext _db;

    public PromotionService(TechStoreDBContext db) => _db = db;

    public async Task<List<PromotionDto>> GetAllAsync()
    {
        var promos = await _db.Promotions.OrderByDescending(p => p.StartDate).ToListAsync();
        return promos.Select(MapToDto).ToList();
    }

    public async Task<List<PromotionDto>> GetActiveAsync()
    {
        var now = DateTime.Now;
        var promos = await _db.Promotions
            .Where(p => p.StartDate <= now && p.EndDate >= now)
            .OrderBy(p => p.EndDate)
            .ToListAsync();
        return promos.Select(MapToDto).ToList();
    }

    public async Task<PromotionDto> GetByIdAsync(long id)
    {
        var p = await _db.Promotions.FindAsync(id);
        return p == null ? null : MapToDto(p);
    }

    public async Task<PromotionDto> CreateAsync(CreatePromotionDto dto)
    {
        var promo = new Promotion
        {
            Name = dto.Name,
            DiscountPercent = dto.DiscountPercent,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate
        };
        _db.Promotions.Add(promo);
        await _db.SaveChangesAsync();
        return MapToDto(promo);
    }

    public async Task<PromotionDto> UpdateAsync(long id, CreatePromotionDto dto)
    {
        var promo = await _db.Promotions.FindAsync(id)
            ?? throw new KeyNotFoundException("Promotion not found.");

        promo.Name = dto.Name;
        promo.DiscountPercent = dto.DiscountPercent;
        promo.StartDate = dto.StartDate;
        promo.EndDate = dto.EndDate;

        await _db.SaveChangesAsync();
        return MapToDto(promo);
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var promo = await _db.Promotions.FindAsync(id);
        if (promo == null) return false;
        _db.Promotions.Remove(promo);
        await _db.SaveChangesAsync();
        return true;
    }

    private static PromotionDto MapToDto(Promotion p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        DiscountPercent = p.DiscountPercent,
        StartDate = p.StartDate,
        EndDate = p.EndDate,
        IsActive = p.StartDate <= DateTime.Now && p.EndDate >= DateTime.Now
    };
}
