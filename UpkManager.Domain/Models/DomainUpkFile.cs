using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;

using STR.MvvmCommon;


namespace UpkManager.Domain.Models {

  [Export]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class DomainUpkFile : ObservableObject {

    #region Private Fields

    private bool isChecked;
    private bool isSelected;

    private long fileSize;

    private string id;

    private string fullFilename;

    private ObservableCollection<string> exportTypes;

    #endregion Private Fields

    #region Constructor

    public DomainUpkFile() {
      exportTypes = new ObservableCollection<string>();
    }

    #endregion Constructor

    #region Properties

    public long FileSize {
      get { return fileSize; }
      set { SetField(ref fileSize, value, () => FileSize); }
    }

    public string Id {
      get { return id; }
      set { SetField(ref id, value, () => Id); }
    }

    public string GameFilename {
      get { return fullFilename; }
      set { SetField(ref fullFilename, value, () => GameFilename); }
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

    public string SelectedType => ExportTypes.Any(et => et.Equals("Texture2D", StringComparison.InvariantCultureIgnoreCase)) ? "\u2713" : String.Empty;

    public string Filename => fullFilename.Substring(fullFilename.LastIndexOf(@"\") + 1);

    #endregion Domain Properties

  }

}
