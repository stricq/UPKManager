using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

using STR.MvvmCommon;


namespace UpkManager.Domain.ViewModels {

  [Export]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class UpkFileViewModel : ObservableObject {

    #region Private Fields

    private bool isChecked;
    private bool isSelected;

    private long fileSize;

    private string fullFilename;
    private string selectedType;

    private ObservableCollection<string> exportTypes;

    #endregion Private Fields

    #region Constructor

    public UpkFileViewModel() {
      exportTypes = new ObservableCollection<string>();
    }

    #endregion Constructor

    #region Properties

    public long FileSize {
      get { return fileSize; }
      set { SetField(ref fileSize, value, () => FileSize); }
    }

    public string FullFilename {
      get { return fullFilename; }
      set { SetField(ref fullFilename, value, () => FullFilename); }
    }

    public ObservableCollection<string> ExportTypes {
      get { return exportTypes; }
      set { SetField(ref exportTypes, value, () => ExportTypes); }
    }

    #endregion Properties

    #region Domain Properties

    public bool IsSelected {
      get { return isSelected; }
      set { SetField(ref isSelected, value, () => IsSelected); }
    }

    public bool IsChecked {
      get { return isChecked; }
      set { SetField(ref isChecked, value, () => IsChecked); }
    }

    public string SelectedType {
      get { return selectedType; }
      set { SetField(ref selectedType, value, () => SelectedType); }
    }

    public string Filename => fullFilename.Substring(fullFilename.LastIndexOf(@"\") + 1);

    #endregion Domain Properties

  }

}
