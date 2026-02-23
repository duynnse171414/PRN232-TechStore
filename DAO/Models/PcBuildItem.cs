#nullable disable

namespace DAO.Models;

public partial class PcBuildItem
{
    public long Id { get; set; }

    public long BuildId { get; set; }

    public long ComponentTypeId { get; set; }

    public long ProductId { get; set; }

    public int Quantity { get; set; }

    public virtual PcBuild Build { get; set; }

    public virtual PcComponentType ComponentType { get; set; }

    public virtual Product Product { get; set; }
}
