#nullable disable
using System;
using System.Collections.Generic;

namespace DAO.Models;

public partial class Voucher
{
    public long Id { get; set; }

    public string Code { get; set; }

    public string Name { get; set; }

    public int? DiscountPercent { get; set; }

    public decimal? MaxDiscount { get; set; }

    public decimal? MinOrderAmount { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
