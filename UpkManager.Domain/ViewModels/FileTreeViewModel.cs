using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

using STR.MvvmCommon;


namespace UpkManager.Domain.ViewModels {

  [Export]
  [ViewModel("FileTreeViewModel")]
  public class FileTreeViewModel : ObservableObject {

    #region Private Fields

    private ObservableCollection<UpkFileViewModel> files;

    #endregion Private Fields

    #region Constructor

    public FileTreeViewModel() {
      Files = new ObservableCollection<UpkFileViewModel>();
    }

    #endregion Constructor

    #region Properties

    public ObservableCollection<UpkFileViewModel> Files {
      get { return files; }
      set { SetField(ref files, value, () => Files); }
    }

    #endregion Properties

  }

}
