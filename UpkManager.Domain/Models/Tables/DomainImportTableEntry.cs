using System.ComponentModel.Composition;


namespace UpkManager.Domain.Models.Tables {

  [Export]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class DomainImportTableEntry : DomainObjectTableEntry {

    #region Private Fields
    //
    // Repository Fields
    //
    private DomainNameTableIndex packageNameIndex;

    private DomainNameTableIndex typeNameIndex;
    //
    // Domain Fields
    //
    private bool isSelected;

    #endregion Private Fields

    #region Properties

    public DomainNameTableIndex PackageNameIndex {
      get { return packageNameIndex; }
      set { SetField(ref packageNameIndex, value, () => PackageNameIndex); }
    }

    public DomainNameTableIndex TypeNameIndex {
      get { return typeNameIndex; }
      set { SetField(ref typeNameIndex, value, () => TypeNameIndex); }
    }

    #endregion Properties

    #region Domain Properties

    public bool IsSelected {
      get { return isSelected; }
      set { SetField(ref isSelected, value, () => IsSelected); }
    }

    public string PackageName => packageNameIndex.Name;

    public string TypeName => typeNameIndex.Name;

    #endregion Domain Properties

  }

}
