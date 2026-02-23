#nullable disable
using System.Collections.Generic;

namespace DAO.Models;

public partial class PcComponentType
{
    public long Id { get; set; }

    public string Name { get; set; }

    public bool IsRequired { get; set; }

    public int SortOrder { get; set; }

    public virtual ICollection<PcBuildItem> PcBuildItems { get; set; } = new List<PcBuildItem>();
}
