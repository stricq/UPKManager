using STR.MvvmCommon;


namespace UpkManager.Wpf.ViewEntities.Tables {

  public class NameTableEntryViewEntity : ObservableObject {

    #region Private Fields

    private bool isErrored;
    private bool isSelected;

    private int tableIndex;

    private string name;

    private ulong flags;

    #endregion Private Fields

    #region Properties

    public bool IsErrored {
      get { return isErrored; }
      set { SetField(ref isErrored, value, () => IsErrored); }
    }

    public bool IsSelected {
      get { return isSelected; }
      set { SetField(ref isSelected, value, () => IsSelected); }
    }

    public int TableIndex {
      get { return tableIndex; }
      set { SetField(ref tableIndex, value, () => TableIndex); }
    }

    public string Name {
      get { return name; }
      set { SetField(ref name, value, () => Name); }
    }

    public ulong Flags {
      get { return flags; }
      set { SetField(ref flags, value, () => Flags); }
    }

    #endregion Properties

  }

}
