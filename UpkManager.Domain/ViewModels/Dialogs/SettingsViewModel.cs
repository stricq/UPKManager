using System.ComponentModel.Composition;

using STR.DialogView.Domain.Contracts;
using STR.MvvmCommon;

using UpkManager.Domain.Messages.Application;


namespace UpkManager.Domain.ViewModels.Dialogs {

  [Export]
  [ViewModel("SettingsViewModel")]
  public class SettingsViewModel : ObservableObject, IDialogViewModel {

    #region Private Fields

    private SettingsEditMessage message;

    private RelayCommand selectGameDir;

    private RelayCommand ok;
    private RelayCommand cancel;

    #endregion Private Fields

    #region Properties

    public SettingsEditMessage Message {
      get { return message; }
      set { SetField(ref message, value, () => Message); }
    }

    public RelayCommand SelectGameDir {
      get { return selectGameDir; }
      set { SetField(ref selectGameDir, value, () => SelectGameDir); }
    }

    public RelayCommand Ok {
      get { return ok; }
      set { SetField(ref ok, value, () => Ok); }
    }

    public RelayCommand Cancel {
      get { return cancel; }
      set { SetField(ref cancel, value, () => Cancel); }
    }

    #endregion Properties

  }

}
