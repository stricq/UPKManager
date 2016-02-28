using System.ComponentModel.Composition;
using System.Windows.Navigation;

using STR.DialogView.Domain.Contracts;

using STR.MvvmCommon;


namespace UpkManager.Wpf.ViewModels.Dialogs {

  [Export]
  [ViewModel("AboutDialogViewModel")]
  public class AboutDialogViewModel : ObservableObject, IDialogViewModel {

    #region Private Fields

    private RelayCommand<RequestNavigateEventArgs> navigate;

    private RelayCommand ok;

    #endregion Private Fields

    #region Properties

    public RelayCommand<RequestNavigateEventArgs> Navigate {
      get { return navigate; }
      set { SetField(ref navigate, value, () => Navigate); }
    }

    public RelayCommand Ok {
      get { return ok; }
      set { SetField(ref ok, value, () => Ok); }
    }

    #endregion Properties

  }

}
