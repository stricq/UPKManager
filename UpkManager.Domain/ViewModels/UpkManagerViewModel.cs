using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows;

using STR.MvvmCommon;


namespace UpkManager.Domain.ViewModels {

  [Export]
  [ViewModel("UpkManagerViewModel")]
  public class UpkManagerViewModel : ObservableObject {

    #region Private Fields

    private RelayCommandAsync<RoutedEventArgs> loaded;

    private RelayCommand<CancelEventArgs> closing;

    #endregion Private Fields

    #region Properties

    public RelayCommandAsync<RoutedEventArgs> Loaded {
      get { return loaded; }
      set { SetField(ref loaded, value, () => Loaded); }
    }

    public RelayCommand<CancelEventArgs> Closing {
      get { return closing; }
      set { SetField(ref closing, value, () => Closing); }
    }

    #endregion Properties

  }

}
