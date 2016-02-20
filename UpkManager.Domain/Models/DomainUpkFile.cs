using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;

using STR.MvvmCommon;


namespace UpkManager.Domain.Models {

  [Export]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class DomainUpkFile : ObservableObject {

    #region Private Fields

    private bool containsTargetObject;

    private bool isChecked;
    private bool isErrored;
    private bool isSelected;

    private long fileSize;

    private string id;

    private string gameFilename;

    private string notes;

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
      get { return gameFilename; }
      set { SetField(ref gameFilename, value, () => GameFilename); }
    }

    public ObservableCollection<string> ExportTypes {
      get { return exportTypes; }
      set { SetField(ref exportTypes, value, () => ExportTypes); }
    }

    public string Notes {
      get { return notes; }
      set { SetField(ref notes, value, () => Notes, () => NotesColumn); }
    }

    #endregion Properties

    #region Domain Properties

    public bool IsSelected {
      get { return isSelected; }
      set { SetField(ref isSelected, value, () => IsSelected); }
    }

    public bool IsErrored {
      get { return isErrored; }
      set { SetField(ref isErrored, value, () => IsErrored); }
    }

    public bool IsChecked {
      get { return isChecked; }
      set { SetField(ref isChecked, value, () => IsChecked); }
    }

    public bool ContainsTargetObject {
      get { return containsTargetObject; }
      set { SetField(ref containsTargetObject, value, () => ContainsTargetObject); }
    }

    public string Filename => Path.GetFileName(gameFilename);

    public string NotesColumn => (notes?.Length > 50 ? notes.Substring(0, 50) : notes)?.Replace("\r\n", " ").Replace('\t', ' ');

    #endregion Domain Properties

  }

}
