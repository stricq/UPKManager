using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

using STR.MvvmCommon;

using UpkManager.Wpf.ViewEntities;
using UpkManager.Wpf.ViewEntities.Tables;


namespace UpkManager.Wpf.ViewModels.Tables {

  [Export]
  [ViewModel("ImportTableViewModel")]
  public class ImportTableViewModel : ObservableObject {

    #region Private Fields

    private ObservableCollection<ImportTableEntryViewEntity> importTableEntries;

    #endregion Private Fields

    #region Properties

    public ObservableCollection<ImportTableEntryViewEntity> ImportTableEntries {
      get { return importTableEntries; }
      set { SetField(ref importTableEntries, value, () => ImportTableEntries); }
    }

    #endregion Properties

  }

}
