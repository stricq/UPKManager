using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

using STR.MvvmCommon;

using UpkManager.Wpf.ViewEntities.Tables;


namespace UpkManager.Wpf.ViewModels.Tables {

  [Export]
  [ViewModel("GenerationsTableViewModel")]
  public class GenerationsTableViewModel : ObservableObject {

    #region Private Fields

    private ObservableCollection<GenerationsTableEntryViewEntity> generationsTableEntries;

    #endregion Private Fields

    #region Properties

    public ObservableCollection<GenerationsTableEntryViewEntity> GenerationsTableEntries {
      get { return generationsTableEntries; }
      set { SetField(ref generationsTableEntries, value, () => GenerationsTableEntries); }
    }

    #endregion Properties

  }

}
