using STR.MvvmCommon;


namespace UpkManager.Wpf.ViewEntities.Tables {

  public class ImportTableEntryViewEntity : ObservableObject {

    #region Private Fields

    private bool isSelected;

    private int ownerReference;

    private int tableIndex;

    private string        name;
    private string packageName;
    private string    typeName;

    #endregion Private Fields

    #region Properties

    public bool IsSelected {
      get { return isSelected; }
      set { SetField(ref isSelected, value, () => IsSelected); }
    }

    public int OwnerReference {
      get { return ownerReference; }
      set { SetField(ref ownerReference, value, () => OwnerReference); }
    }

    public int TableIndex {
      get { return tableIndex; }
      set { SetField(ref tableIndex, value, () => TableIndex); }
    }

    public string Name {
      get { return name; }
      set { SetField(ref name, value, () => Name); }
    }

    public string PackageName {
      get { return packageName; }
      set { SetField(ref packageName, value, () => PackageName); }
    }

    public string TypeName {
      get { return typeName; }
      set { SetField(ref typeName, value, () => TypeName); }
    }

    #endregion Properties

  }

}
