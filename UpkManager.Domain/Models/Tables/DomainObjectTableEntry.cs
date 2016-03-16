

namespace UpkManager.Domain.Models.Tables {

  public class DomainObjectTableEntry : DomainUpkBuilderBase {

    #region Properties

    public int OwnerReference { get; set; }

    public DomainNameTableIndex NameIndex { get; set; }

    #endregion Properties

    #region Domain Properties

    public int TableIndex { get; set; }

    #endregion Domain Properties

    #region DomainUpkBuilderBase Implementation

    public override int GetBuilderSize() {
      return sizeof(int)
           + NameIndex.GetBuilderSize();
    }

    #endregion DomainUpkBuilderBase Implementation

  }

}
