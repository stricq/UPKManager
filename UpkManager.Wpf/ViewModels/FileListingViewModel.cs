using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;

using STR.MvvmCommon;

using UpkManager.Wpf.ViewEntities;


namespace UpkManager.Wpf.ViewModels {

  [Export]
  [ViewModel("FileTreeViewModel")]
  [SuppressMessage("ReSharper", "MemberCanBeInternal")]
  public sealed class FileListingViewModel : ObservableObject {

    #region Private Fields

    private bool isShowFilesWithType;
    private bool isFilterFiles;

    private string selectedType;
    private string filterText;

    private ObservableCollection<FileViewEntity> files;

    private ObservableCollection<string> allTypes;

    #endregion Private Fields

    #region Properties

    public bool IsShowFilesWithType {
      get { return isShowFilesWithType; }
      set { SetField(ref isShowFilesWithType, value, () => IsShowFilesWithType); }
    }

    public bool IsFilterFiles {
      get { return isFilterFiles; }
      set { SetField(ref isFilterFiles, value, () => IsFilterFiles); }
    }

    public string SelectedType {
      get { return selectedType; }
      set { SetField(ref selectedType, value, () => SelectedType); }
    }

    public string FilterText {
      get { return filterText; }
      set { SetField(ref filterText, value, () => FilterText); }
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
