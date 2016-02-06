using System.ComponentModel.Composition;

using STR.MvvmCommon;


namespace UpkManager.Domain.Models.Tables {

  [Export]
  [PartCreationPolicy(CreationPolicy.Shared)]
  public class DomainGenerationTableEntry : ObservableObject {

    #region Private Fields
    //
    // Repository Fields
    //
    private int exportTableCount;

    private int nameTableCount;

    private int netObjectCount;
    //
    // Domain Fields
    //
    private bool isSelected;

    #endregion Private Fields

    #region Properties

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

    #region Domain Properties

    public bool IsSelected {
      get { return isSelected; }
      set { SetField(ref isSelected, value, () => IsSelected); }
    }

    #endregion Domain Properties

  }

}
