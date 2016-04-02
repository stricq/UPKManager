

namespace UpkManager.Domain.Models.UpkFile.Tables {

  public abstract class DomainObjectTableEntryBase : DomainUpkBuilderBase {

    #region Properties

    public int OwnerReference { get; protected set; }

    public DomainNameTableIndex NameTableIndex { get; protected set; }

    #endregion Properties

    #region Domain Properties

    public int TableIndex { get; set; }

    #endregion Domain Properties

  }

}
