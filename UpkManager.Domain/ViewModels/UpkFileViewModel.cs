using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

using STR.Common.Contracts;
using STR.MvvmCommon;


namespace UpkManager.Domain.ViewModels {

  [Export]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class UpkFileViewModel : ObservableObject, ITraversable<UpkFileViewModel> {

    #region Private Fields

    private bool isChecked;
    private bool isSelected;

    private long fileSize;

    private string fullFilename;
    private string hasTextures;

    private ObservableCollection<UpkFileViewModel> children;

    #endregion Private Fields

    #region Constructor

    public UpkFileViewModel() {
      Children = new ObservableCollection<UpkFileViewModel>();
    }

    #endregion Constructor

    #region Properties

    public bool IsChecked {
      get { return isChecked; }
      set { SetField(ref isChecked, value, () => IsChecked); }
    }

    public bool IsSelected {
      get { return isSelected; }
      set { SetField(ref isSelected, value, () => IsSelected); }
    }

    public long FileSize {
      get { return fileSize; }
      set { SetField(ref fileSize, value, () => FileSize); }
    }

    public string FullFilename {
      get { return fullFilename; }
      set { SetField(ref fullFilename, value, () => FullFilename); }
    }

    public string HasTextures {
      get { return hasTextures; }
      set { SetField(ref hasTextures, value, () => HasTextures); }
    }

    public ObservableCollection<UpkFileViewModel> Children {
      get { return children; }
      set { SetField(ref children, value, () => Children); }
    }

    #endregion Properties

    #region Domain Properties

    public string Filename => fullFilename.Substring(fullFilename.LastIndexOf(@"\") + 1);

    #endregion Domain Properties

  }

}
