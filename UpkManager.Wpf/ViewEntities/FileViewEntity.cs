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
    private string gameLocale;
    private string rootDirectory;
    private string fileHash;

    private long fileSize;

    private string id;

    private string gameFilename;
    private string     filename;

    private string notes;

    private ObservableCollection<string> exportTypes;

    #endregion Private Fields

    #region Properties

    public bool ContainsTargetObject {
      get => containsTargetObject;
      set { SetField(ref containsTargetObject, value, () => ContainsTargetObject); }
    }

    public bool IsChecked {
      get => isChecked;
      set { SetField(ref isChecked, value, () => IsChecked); }
    }

    public bool IsSelected {
      get => isSelected;
      set { SetField(ref isSelected, value, () => IsSelected); }
    }

    public bool IsErrored {
      get => isErrored;
      set { SetField(ref isErrored, value, () => IsErrored); }
    }

    public long FileSize {
      get => fileSize;
      set { SetField(ref fileSize, value, () => FileSize); }
    }

    public string Id {
      get => id;
      set { SetField(ref id, value, () => Id); }
    }

    public string GameVersion {
      get => gameVersion;
      set { SetField(ref gameVersion, value, () => GameVersion); }
    }

    public string GameLocale {
      get => gameLocale;
      set { SetField(ref gameLocale, value, () => GameLocale); }
    }

    public string RootDirectory {
      get => rootDirectory;
      set { SetField(ref rootDirectory, value, () => RootDirectory); }
    }

    public string FileHash {
      get => fileHash;
      set { SetField(ref fileHash, value, () => FileHash); }
    }

    public string GameFilename {
      get => gameFilename;
      set { SetField(ref gameFilename, value, () => GameFilename, () => Filename); }
    }

    public string Filename {
      get => filename;
      set { SetField(ref filename, value, () => Filename); }
    }

    public string Notes {
      get => notes;
      set { SetField(ref notes, value, () => Notes, () => NotesColumn); }
    }

    public string NotesColumn => (Notes?.Length > 50 ? Notes.Substring(0, 50) : Notes)?.Replace("\r\n", " ").Replace('\t', ' ');

    public ObservableCollection<string> ExportTypes {
      get => exportTypes;
      set { SetField(ref exportTypes, value, () => ExportTypes); }
    }

    #endregion Properties

  }

}
