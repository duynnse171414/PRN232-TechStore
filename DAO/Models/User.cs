#nullable disable
using System;
using System.Collections.Generic;

namespace DAO.Models;

public partial class User
{
    public long Id { get; set; }

    public string Email { get; set; }

    public string PasswordHash { get; set; }

    public int RoleId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Role Role { get; set; }

    public virtual Customer Customer { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<PcBuild> PcBuilds { get; set; } = new List<PcBuild>();
}
