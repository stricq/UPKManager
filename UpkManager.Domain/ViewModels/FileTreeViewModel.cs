using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

using STR.MvvmCommon;

using UpkManager.Domain.Models;


namespace UpkManager.Domain.ViewModels {

  [Export]
  [ViewModel("FileTreeViewModel")]
  public class FileTreeViewModel : ObservableObject {

    #region Private Fields

    private ObservableCollection<DomainUpkFile> files;

    #endregion Private Fields

    #region Constructor

    public FileTreeViewModel() {
      Files = new ObservableCollection<DomainUpkFile>();
    }

    #endregion Constructor

    #region Properties

    public ObservableCollection<DomainUpkFile> Files {
      get { return files; }
      set { SetField(ref files, value, () => Files); }
    }

    #endregion Properties

  }

}
