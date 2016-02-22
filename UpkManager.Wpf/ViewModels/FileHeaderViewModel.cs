using System.ComponentModel.Composition;

using STR.MvvmCommon;

using UpkManager.Domain.Models;


namespace UpkManager.Wpf.ViewModels {

  [Export]
  [ViewModel("FileHeaderViewModel")]
  public class FileHeaderViewModel : ObservableObject {

    #region Private Fields

    private RelayCommandAsync saveNotes;

    private DomainUpkFile file;

    private DomainHeader header;

    #endregion Private Fields

    #region Properties

    public RelayCommandAsync SaveNotes {
      get { return saveNotes; }
      set { SetField(ref saveNotes, value, () => SaveNotes); }
    }

    public DomainUpkFile File {
      get { return file; }
      set { SetField(ref file, value, () => File, () => AreNotesEnabled); }
    }

    public DomainHeader Header {
      get { return header; }
      set { SetField(ref header, value, () => Header); }
    }

    public bool AreNotesEnabled => file != null;

    #endregion Properties

  }

}
