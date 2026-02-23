#nullable disable
using System.Collections.Generic;

namespace DAO.Models;

public partial class Role
{
    public int Id { get; set; }

    /// <summary>customer | staff | admin</summary>
    public string Name { get; set; }

    public string Description { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
