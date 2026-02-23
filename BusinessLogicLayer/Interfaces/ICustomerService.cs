using BusinessLogicLayer.DTOs;

namespace BusinessLogicLayer.Interfaces;

public interface ICustomerService
{
    /// <summary>Lấy hồ sơ theo customer.id</summary>
    Task<CustomerProfileDto> GetProfileAsync(long customerId);

    /// <summary>Lấy hồ sơ theo user.id (cho user đã đăng nhập)</summary>
    Task<CustomerProfileDto> GetProfileByUserIdAsync(long userId);

    /// <summary>
    /// Lấy hoặc tạo hồ sơ mua hàng cho user đã đăng nhập.
    /// Dùng khi checkout hoặc xem giỏ hàng lần đầu.
    /// </summary>
    Task<CustomerProfileDto> GetOrCreateCustomerByUserIdAsync(long userId, string name = null, string email = null, string phone = null);

    /// <summary>Tạo hồ sơ khách vãng lai (guest checkout)</summary>
    Task<CustomerProfileDto> CreateGuestCustomerAsync(CreateGuestCustomerDto dto);

    Task<CustomerProfileDto> UpdateProfileAsync(long customerId, UpdateProfileDto dto);

    Task<List<AddressDto>> GetAddressesAsync(long customerId);
    Task<AddressDto> AddAddressAsync(long customerId, CreateAddressDto dto);
    Task<bool> DeleteAddressAsync(long customerId, long addressId);

    Task<List<CustomerProfileDto>> GetAllCustomersAsync();
}
