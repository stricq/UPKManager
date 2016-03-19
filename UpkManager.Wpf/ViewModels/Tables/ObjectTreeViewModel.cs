using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

using STR.MvvmCommon;

using UpkManager.Wpf.ViewEntities.Tables;


namespace UpkManager.Wpf.ViewModels.Tables {

  [Export]
  [ViewModel("ObjectTreeViewModel")]
  public class ObjectTreeViewModel : ObservableObject {

    #region Private Fields

    private ObservableCollection<ObjectTreeViewEntity> objectTree;

    #endregion Private Fields

    #region Properties

    public ObservableCollection<ObjectTreeViewEntity> ObjectTree {
      get { return objectTree; }
      set { SetField(ref objectTree, value, () => ObjectTree); }
    }

    #endregion Properties

  }

}
