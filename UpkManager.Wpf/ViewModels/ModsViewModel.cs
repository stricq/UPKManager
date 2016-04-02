using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

using STR.MvvmCommon;

using UpkManager.Wpf.ViewEntities;


namespace UpkManager.Wpf.ViewModels {

  [Export]
  [ViewModel("ModsViewModel")]
  public class ModsViewModel : ObservableObject {

    #region Private Fields

    private ObservableCollection<FileViewEntity> mods;

    #endregion Private Fields

    #region Constructor

    public ModsViewModel() {
      mods = new ObservableCollection<FileViewEntity>();
    }

    #endregion Constructor

    #region Properties

    public ObservableCollection<FileViewEntity> Mods {
      get { return mods; }
      set { SetField(ref mods, value, () => Mods); }
    }

    #endregion Properties

  }

}
