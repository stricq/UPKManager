using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

using STR.MvvmCommon;

using UpkManager.Wpf.ViewEntities;
using UpkManager.Wpf.ViewEntities.Tables;


namespace UpkManager.Wpf.ViewModels.Tables {

  [Export]
  [ViewModel("NameTableViewModel")]
  public class NameTableViewModel : ObservableObject {

    #region Private Fields

    private ObservableCollection<NameTableEntryViewEntity> nameTableEntries;

    #endregion Private Fields

    #region Properties

    public ObservableCollection<NameTableEntryViewEntity> NameTableEntries {
      get { return nameTableEntries; }
      set { SetField(ref nameTableEntries, value, () => NameTableEntries); }
    }

    #endregion Properties

  }

}
