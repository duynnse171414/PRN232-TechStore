using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Interfaces;
using DAO.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogicLayer.Services;

public class CustomerService : ICustomerService
{
    private readonly TechStoreDBContext _db;

    public CustomerService(TechStoreDBContext db) => _db = db;

    public async Task<CustomerProfileDto> GetProfileAsync(long customerId)
    {
        var c = await _db.Customers.FindAsync(customerId);
        return c == null ? null : MapToDto(c);
    }

    public async Task<CustomerProfileDto> GetProfileByUserIdAsync(long userId)
    {
        var c = await _db.Customers.FirstOrDefaultAsync(x => x.UserId == userId);
        return c == null ? null : MapToDto(c);
    }

    public async Task<CustomerProfileDto> GetOrCreateCustomerByUserIdAsync(
        long userId, string name = null, string email = null, string phone = null)
    {
        var existing = await _db.Customers.FirstOrDefaultAsync(c => c.UserId == userId);
        if (existing != null) return MapToDto(existing);

        var user = await _db.Users.FindAsync(userId);
        var customer = new Customer
        {
            UserId = userId,
            Name = name ?? user?.Email ?? "Khách hàng",
            Email = email ?? user?.Email,
            Phone = phone,
            CreatedAt = DateTime.Now
        };

        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();
        return MapToDto(customer);
    }

    public async Task<CustomerProfileDto> CreateGuestCustomerAsync(CreateGuestCustomerDto dto)
    {
        var customer = new Customer
        {
            UserId = null,
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            CreatedAt = DateTime.Now
        };

        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();
        return MapToDto(customer);
    }

    public async Task<CustomerProfileDto> UpdateProfileAsync(long customerId, UpdateProfileDto dto)
    {
        var c = await _db.Customers.FindAsync(customerId)
            ?? throw new KeyNotFoundException("Customer not found.");

        c.Name = dto.Name;
        c.Email = dto.Email;
        c.Phone = dto.Phone;
        await _db.SaveChangesAsync();
        return MapToDto(c);
    }

    public async Task<List<AddressDto>> GetAddressesAsync(long customerId)
    {
        var addresses = await _db.Addresses
            .Where(a => a.CustomerId == customerId)
            .ToListAsync();

        return addresses.Select(MapAddressToDto).ToList();
    }

    public async Task<AddressDto> AddAddressAsync(long customerId, CreateAddressDto dto)
    {
        if (dto.IsDefault)
        {
            var existing = await _db.Addresses
                .Where(a => a.CustomerId == customerId && a.IsDefault == true)
                .ToListAsync();
            existing.ForEach(a => a.IsDefault = false);
        }

        var address = new Address
        {
            CustomerId = customerId,
            AddressLine = dto.AddressLine,
            City = dto.City,
            District = dto.District,
            Ward = dto.Ward,
            IsDefault = dto.IsDefault
        };

        _db.Addresses.Add(address);
        await _db.SaveChangesAsync();
        return MapAddressToDto(address);
    }

    public async Task<bool> DeleteAddressAsync(long customerId, long addressId)
    {
        var address = await _db.Addresses
            .FirstOrDefaultAsync(a => a.Id == addressId && a.CustomerId == customerId);

        if (address == null) return false;

        _db.Addresses.Remove(address);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<CustomerProfileDto>> GetAllCustomersAsync()
    {
        var customers = await _db.Customers
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return customers.Select(MapToDto).ToList();
    }

    private static CustomerProfileDto MapToDto(Customer c) => new()
    {
        Id = c.Id,
        UserId = c.UserId,
        Name = c.Name,
        Email = c.Email,
        Phone = c.Phone,
        CreatedAt = c.CreatedAt
    };

    private static AddressDto MapAddressToDto(Address a) => new()
    {
        Id = a.Id,
        AddressLine = a.AddressLine,
        City = a.City,
        District = a.District,
        Ward = a.Ward,
        IsDefault = a.IsDefault
    };
}
