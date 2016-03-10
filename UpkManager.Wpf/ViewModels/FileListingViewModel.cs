using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

using STR.MvvmCommon;

using UpkManager.Wpf.ViewEntities;


namespace UpkManager.Wpf.ViewModels {

  [Export]
  [ViewModel("FileTreeViewModel")]
  public class FileListingViewModel : ObservableObject {

    #region Private Fields

    private bool isShowFilesWithType;

    private string selectedType;

    private ObservableCollection<FileViewEntity> files;

    private ObservableCollection<string> allTypes;

    #endregion Private Fields

    #region Constructor

    public FileListingViewModel() {
      files = new ObservableCollection<FileViewEntity>();
    }

    #endregion Constructor

    #region Properties

    public bool IsShowFilesWithType {
      get { return isShowFilesWithType; }
      set { SetField(ref isShowFilesWithType, value, () => IsShowFilesWithType); }
    }

    public string SelectedType {
      get { return selectedType; }
      set { SetField(ref selectedType, value, () => SelectedType); }
    }

    public ObservableCollection<FileViewEntity> Files {
      get { return files; }
      set { SetField(ref files, value, () => Files); }
    }

    public ObservableCollection<string> AllTypes {
      get { return allTypes; }
      set { SetField(ref allTypes, value, () => AllTypes); }
    }

    #endregion Properties

  }

}
