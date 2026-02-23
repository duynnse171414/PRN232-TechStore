#nullable disable
using System;
using System.Collections.Generic;

namespace DAO.Models;

public partial class PcBuild
{
    public long Id { get; set; }

    /// <summary>NULL nếu cấu hình được tạo mà chưa đăng nhập (lưu tạm client-side).</summary>
    public long? UserId { get; set; }

    public string Name { get; set; }

    public decimal? TotalPrice { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User User { get; set; }

    public virtual ICollection<PcBuildItem> PcBuildItems { get; set; } = new List<PcBuildItem>();
}
