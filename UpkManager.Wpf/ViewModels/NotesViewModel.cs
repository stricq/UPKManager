using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;

using STR.MvvmCommon;

using UpkManager.Wpf.ViewEntities;


namespace UpkManager.Wpf.ViewModels {

  [Export]
  [ViewModel("NotesViewModel")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  [SuppressMessage("ReSharper", "MemberCanBeInternal")]
  public sealed class NotesViewModel : ObservableObject {

    #region Private Fields

    private FileViewEntity selectedFile;

    private RelayCommandAsync saveNotes;

    #endregion Private Fields

    #region Properties

    public bool AreNotesEnabled => selectedFile != null;

    public FileViewEntity SelectedFile {
      get { return selectedFile; }
      set { SetField(ref selectedFile, value, () => SelectedFile, () => AreNotesEnabled); }
    }

    public RelayCommandAsync SaveNotes {
      get { return saveNotes; }
      set { SetField(ref saveNotes, value, () => SaveNotes); }
    }

    #endregion Properties

  }

}
