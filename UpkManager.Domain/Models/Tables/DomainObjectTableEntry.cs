

namespace UpkManager.Domain.Models.Tables {

  public class DomainObjectTableEntry {

    #region Properties

    public int TableIndex { get; set; }

    public int OwnerReference { get; set; }

    public DomainNameTableIndex NameIndex { get; set; }

    #endregion Properties

  }

}
