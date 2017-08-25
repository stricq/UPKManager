using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

using STR.MvvmCommon;


namespace UpkManager.Wpf.ViewEntities {

  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  [SuppressMessage("ReSharper", "MemberCanBeInternal")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  public class FileViewEntity : ObservableObject {

    #region Private Fields

    private bool containsTargetObject;

    private bool isChecked;
    private bool isSelected;
    private bool isErrored;

    private string gameVersion;

    private long fileSize;

    private string id;

    private string gameFilename;
    private string     filename;

    private string notes;

    private ObservableCollection<string> exportTypes;

    #endregion Private Fields

    #region Properties

    public bool ContainsTargetObject {
      get { return containsTargetObject; }
      set { SetField(ref containsTargetObject, value, () => ContainsTargetObject); }
    }

    public bool IsChecked {
      get { return isChecked; }
      set { SetField(ref isChecked, value, () => IsChecked); }
    }

    public bool IsSelected {
      get { return isSelected; }
      set { SetField(ref isSelected, value, () => IsSelected); }
    }

    public bool IsErrored {
      get { return isErrored; }
      set { SetField(ref isErrored, value, () => IsErrored); }
    }

    public long FileSize {
      get { return fileSize; }
      set { SetField(ref fileSize, value, () => FileSize); }
    }

    public string Id {
      get { return id; }
      set { SetField(ref id, value, () => Id); }
    }

    public string GameVersion {
      get { return gameVersion; }
      set { SetField(ref gameVersion, value, () => GameVersion); }
    }

    public string GameFilename {
      get { return gameFilename; }
      set { SetField(ref gameFilename, value, () => GameFilename, () => Filename); }
    }

    public string Filename {
      get { return filename; }
      set { SetField(ref filename, value, () => Filename); }
    }

    public string Notes {
      get { return notes; }
      set { SetField(ref notes, value, () => Notes, () => NotesColumn); }
    }

    public string NotesColumn => (Notes?.Length > 50 ? Notes.Substring(0, 50) : Notes)?.Replace("\r\n", " ").Replace('\t', ' ');

    public ObservableCollection<string> ExportTypes {
      get { return exportTypes; }
      set { SetField(ref exportTypes, value, () => ExportTypes); }
    }

    #endregion Properties

  }

}
