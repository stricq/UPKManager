using System.ComponentModel.Composition;

using STR.MvvmCommon;


namespace UpkManager.Domain.Models.Tables {

  [Export]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class DomainObjectTableEntry : ObservableObject {

    #region Private Fields

    private int tableIndex;

    private int ownerReference;

    private DomainNameTableIndex nameIndex;

    #endregion Private Fields

    #region Properties

    public int TableIndex {
      get { return tableIndex; }
      set { SetField(ref tableIndex, value, () => TableIndex); }
    }

    public int OwnerReference {
      get { return ownerReference; }
      set { SetField(ref ownerReference, value, () => OwnerReference); }
    }

    public DomainNameTableIndex NameIndex {
      get { return nameIndex; }
      set { SetField(ref nameIndex, value, () => NameIndex); }
    }

    #endregion Properties

    #region Domain Properties

    public string Name => nameIndex.Name;

    #endregion Domain Properties

  }

}
