using STR.MvvmCommon;


namespace UpkManager.Wpf.ViewEntities.Tables {

  public class GenerationsTableEntryViewEntity : ObservableObject {

    #region Private Fields

    private bool isErrored;
    private bool isSelected;

    private int exportTableCount;

    private int nameTableCount;

    private int netObjectCount;

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

    public int ExportTableCount {
      get { return exportTableCount; }
      set { SetField(ref exportTableCount, value, () => ExportTableCount); }
    }

    public int NameTableCount {
      get { return nameTableCount; }
      set { SetField(ref nameTableCount, value, () => NameTableCount); }
    }

    public int NetObjectCount {
      get { return netObjectCount; }
      set { SetField(ref netObjectCount, value, () => NetObjectCount); }
    }

    #endregion Properties

  }

}
