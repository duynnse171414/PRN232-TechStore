namespace BusinessLogicLayer.DTOs;

public class RegisterDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Password { get; set; }
}

public class LoginDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class ChangePasswordDto
{
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
}

public class UserDto
{
    public long Id { get; set; }
    public string Email { get; set; }
    public string RoleName { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string CustomerName { get; set; }
}

public class UpdateUserRoleDto
{
    public int RoleId { get; set; }
}

public class AuthResultDto
{
    public string Token { get; set; }
    public long UserId { get; set; }
    public long? CustomerId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
}
