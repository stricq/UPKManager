using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

using STR.MvvmCommon;

using UpkManager.Wpf.ViewEntities;


namespace UpkManager.Wpf.ViewModels {

  [Export]
  [ViewModel("RebuildViewModel")]
  public class RebuildViewModel : ObservableObject {

    #region Private Fields

    private ObservableCollection<ExportedObjectViewEntity> exportsTree;

    #endregion Private Fields

    #region Properties

    public ObservableCollection<ExportedObjectViewEntity> ExportsTree {
      get { return exportsTree; }
      set { SetField(ref exportsTree, value, () => ExportsTree); }
    }

    #endregion Properties

  }

}
