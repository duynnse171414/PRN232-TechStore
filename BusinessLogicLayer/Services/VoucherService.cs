using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Interfaces;
using DAO.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogicLayer.Services;

public class VoucherService : IVoucherService
{
    private readonly TechStoreDBContext _db;

    public VoucherService(TechStoreDBContext db) => _db = db;

    public async Task<List<VoucherDto>> GetAllAsync()
    {
        var vouchers = await _db.Vouchers
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync();

        return vouchers.Select(MapToDto).ToList();
    }

    public async Task<List<VoucherDto>> GetActiveAsync()
    {
        var now = DateTime.Now;
        var vouchers = await _db.Vouchers
            .Where(v => v.IsActive &&
                        (v.StartDate == null || v.StartDate <= now) &&
                        (v.EndDate == null || v.EndDate >= now))
            .OrderBy(v => v.EndDate)
            .ToListAsync();

        return vouchers.Select(MapToDto).ToList();
    }

    public async Task<VoucherDto> GetByIdAsync(long id)
    {
        var voucher = await _db.Vouchers.FindAsync(id);
        return voucher == null ? null : MapToDto(voucher);
    }

    public async Task<VoucherDto> GetByCodeAsync(string code)
    {
        var voucher = await _db.Vouchers
            .FirstOrDefaultAsync(v => v.Code == code);

        return voucher == null ? null : MapToDto(voucher);
    }

    public async Task<VoucherDto> CreateAsync(CreateVoucherDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Code))
            throw new InvalidOperationException("Voucher code là bắt buộc.");

        if (await _db.Vouchers.AnyAsync(v => v.Code == dto.Code))
            throw new InvalidOperationException("Voucher code đã tồn tại.");

        var voucher = new Voucher
        {
            Code = dto.Code.Trim(),
            Name = dto.Name,
            DiscountPercent = dto.DiscountPercent,
            MaxDiscount = dto.MaxDiscount,
            MinOrderAmount = dto.MinOrderAmount,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.Now
        };

        _db.Vouchers.Add(voucher);
        await _db.SaveChangesAsync();

        return MapToDto(voucher);
    }

    public async Task<VoucherDto> UpdateAsync(long id, CreateVoucherDto dto)
    {
        var voucher = await _db.Vouchers.FindAsync(id)
            ?? throw new KeyNotFoundException("Voucher không tồn tại.");

        if (string.IsNullOrWhiteSpace(dto.Code))
            throw new InvalidOperationException("Voucher code là bắt buộc.");

        var newCode = dto.Code.Trim();
        var duplicateCode = await _db.Vouchers
            .AnyAsync(v => v.Id != id && v.Code == newCode);
        if (duplicateCode)
            throw new InvalidOperationException("Voucher code đã tồn tại.");

        voucher.Code = newCode;
        voucher.Name = dto.Name;
        voucher.DiscountPercent = dto.DiscountPercent;
        voucher.MaxDiscount = dto.MaxDiscount;
        voucher.MinOrderAmount = dto.MinOrderAmount;
        voucher.StartDate = dto.StartDate;
        voucher.EndDate = dto.EndDate;
        voucher.IsActive = dto.IsActive;

        await _db.SaveChangesAsync();

        return MapToDto(voucher);
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var voucher = await _db.Vouchers.FindAsync(id);
        if (voucher == null) return false;

        _db.Vouchers.Remove(voucher);
        await _db.SaveChangesAsync();
        return true;
    }

    private static VoucherDto MapToDto(Voucher v) => new()
    {
        Id = v.Id,
        Code = v.Code,
        Name = v.Name,
        DiscountPercent = v.DiscountPercent,
        MaxDiscount = v.MaxDiscount,
        MinOrderAmount = v.MinOrderAmount,
        StartDate = v.StartDate,
        EndDate = v.EndDate,
        IsActive = v.IsActive,
        CreatedAt = v.CreatedAt
    };
}
