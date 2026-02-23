#nullable disable
using System;
using System.Collections.Generic;

namespace DAO.Models;

/// <summary>
/// Hồ sơ mua hàng. UserId = NULL nếu là khách vãng lai (guest).
/// </summary>
public partial class Customer
{
    public long Id { get; set; }

    /// <summary>NULL = guest, non-null = tài khoản đã đăng ký</summary>
    public long? UserId { get; set; }

    public string Name { get; set; }

    /// <summary>Email liên lạc (không dùng để đăng nhập)</summary>
    public string Email { get; set; }

    public string Phone { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User User { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
