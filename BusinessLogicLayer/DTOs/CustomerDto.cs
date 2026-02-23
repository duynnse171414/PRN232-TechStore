namespace BusinessLogicLayer.DTOs;

/// <summary>Hồ sơ mua hàng (không chứa thông tin đăng nhập)</summary>
public class CustomerProfileDto
{
    public long Id { get; set; }
    public long? UserId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public class UpdateProfileDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
}

/// <summary>Tạo hồ sơ khách vãng lai khi guest checkout</summary>
public class CreateGuestCustomerDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
}

public class AddressDto
{
    public long Id { get; set; }
    public string AddressLine { get; set; }
    public string City { get; set; }
    public string District { get; set; }
    public string Ward { get; set; }
    public bool? IsDefault { get; set; }
}

public class CreateAddressDto
{
    public string AddressLine { get; set; }
    public string City { get; set; }
    public string District { get; set; }
    public string Ward { get; set; }
    public bool IsDefault { get; set; }
}
