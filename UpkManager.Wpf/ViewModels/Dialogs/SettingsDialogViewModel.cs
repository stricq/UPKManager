using System.ComponentModel.Composition;

using STR.DialogView.Domain.Contracts;

using STR.MvvmCommon;

using UpkManager.Wpf.Messages.Application;
using UpkManager.Wpf.Messages.Settings;


namespace UpkManager.Wpf.ViewModels.Dialogs {

  [Export]
  [ViewModel("SettingsViewModel")]
  public class SettingsDialogViewModel : ObservableObject, IDialogViewModel {

    #region Private Fields

    private SettingsEditMessage message;

    private RelayCommand selectGameDir;
    private RelayCommand selectExportPath;

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

    public RelayCommand SelectExportPath {
      get { return selectExportPath; }
      set { SetField(ref selectExportPath, value, () => SelectExportPath); }
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
