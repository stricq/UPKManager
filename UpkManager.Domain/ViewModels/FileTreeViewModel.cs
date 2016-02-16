using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

using STR.MvvmCommon;

using UpkManager.Domain.Models;


namespace UpkManager.Domain.ViewModels {

  [Export]
  [ViewModel("FileTreeViewModel")]
  public class FileTreeViewModel : ObservableObject {

    #region Private Fields

    private bool isShowFilesWithType;

    private string selectedType;

    private ObservableCollection<DomainUpkFile> allFiles;

    private ObservableCollection<DomainUpkFile> files;

    private ObservableCollection<string> allTypes;

    #endregion Private Fields

    #region Constructor

    public FileTreeViewModel() {
      AllFiles = new ObservableCollection<DomainUpkFile>();

      Files = new ObservableCollection<DomainUpkFile>();
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

    public ObservableCollection<DomainUpkFile> AllFiles {
      get { return allFiles; }
      set { SetField(ref allFiles, value, () => AllFiles); }
    }

    public ObservableCollection<DomainUpkFile> Files {
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
