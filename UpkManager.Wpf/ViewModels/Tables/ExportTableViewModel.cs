using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

using STR.MvvmCommon;

using UpkManager.Wpf.ViewEntities;
using UpkManager.Wpf.ViewEntities.Tables;


namespace UpkManager.Wpf.ViewModels.Tables {

  [Export]
  [ViewModel("ExportTableViewModel")]
  public class ExportTableViewModel : ObservableObject {

    #region Private Fields

    private ObservableCollection<ExportTableEntryViewEntity> exportTableEntries;

    #endregion Private Fields

    #region Properties

    public ObservableCollection<ExportTableEntryViewEntity> ExportTableEntries {
      get { return exportTableEntries; }
      set { SetField(ref exportTableEntries, value, () => ExportTableEntries); }
    }

    #endregion Properties

  }

}
