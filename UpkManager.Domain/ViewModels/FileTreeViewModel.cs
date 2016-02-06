using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

using STR.MvvmCommon;


namespace UpkManager.Domain.ViewModels {

  [Export]
  [ViewModel("FileTreeViewModel")]
  public class FileTreeViewModel : ObservableObject {

    #region Private Fields

    private ObservableCollection<UpkFileInfo> files;

    #endregion Private Fields

    #region Properties

    public ObservableCollection<UpkFileInfo> Files {
      get { return files; }
      set { SetField(ref files, value, () => Files); }
    }

    #endregion Properties

  }

}
