using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Interfaces;
using DAO.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BusinessLogicLayer.Services;

public class AuthService : IAuthService
{
    private readonly TechStoreDBContext _db;
    private readonly IConfiguration _config;

    public AuthService(TechStoreDBContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<AuthResultDto> RegisterAsync(RegisterDto dto)
    {
        if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
            throw new InvalidOperationException("Email đã được sử dụng.");

        var user = new User
        {
            Email = dto.Email,
            PasswordHash = HashPassword(dto.Password),
            RoleId = 1,   // 1 = customer (seed data)
            CreatedAt = DateTime.Now
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        // Tạo hồ sơ mua hàng liên kết với tài khoản
        var customer = new Customer
        {
            UserId = user.Id,
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            CreatedAt = DateTime.Now
        };
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        // Load role navigation for JWT generation
        await _db.Entry(user).Reference(u => u.Role).LoadAsync();

        return BuildResult(user, customer.Id);
    }

    public async Task<AuthResultDto> LoginAsync(LoginDto dto)
    {
        var user = await _db.Users
            .Include(u => u.Role)
            .Include(u => u.Customer)
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Email hoặc mật khẩu không đúng.");

        return BuildResult(user, user.Customer?.Id);
    }

    private AuthResultDto BuildResult(User user, long? customerId) => new()
    {
        Token = GenerateJwt(user),
        UserId = user.Id,
        CustomerId = customerId,
        Name = user.Customer?.Name ?? user.Email,
        Email = user.Email,
        Role = user.Role?.Name ?? "customer"
    };

    private string GenerateJwt(User user)
    {
        var roleName = user.Role?.Name ?? "customer";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(ClaimTypes.Name, user.Customer?.Name ?? user.Email ?? ""),
            new Claim(ClaimTypes.Role, roleName)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    private static bool VerifyPassword(string password, string hash)
        => HashPassword(password) == hash;
}
