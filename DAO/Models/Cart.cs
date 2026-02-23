#nullable disable
using System;
using System.Collections.Generic;

namespace DAO.Models;

/// <summary>Giỏ hàng – chỉ dành cho user đã đăng nhập.</summary>
public partial class Cart
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User User { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
