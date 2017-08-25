using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

using STR.MvvmCommon.Contracts;

using UpkManager.Wpf.ViewModels;


namespace UpkManager.Wpf.Controllers {

  [Export(typeof(IController))]
  public sealed class HeaderTablesController : IController {

    #region Private Fields

    private readonly HeaderTablesViewModel viewModel;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public HeaderTablesController(HeaderTablesViewModel ViewModel, MainMenuViewModel MenuViewModel) {
      viewModel = ViewModel;

      MenuViewModel.PropertyChanged += onMenuViewModelChanged;
    }

    #endregion Constructor

    #region IController Implementation

    public async Task InitializeAsync() {
      await Task.CompletedTask;
    }

    public int InitializePriority { get; } = 100;

    #endregion IController Implementation

    #region Private Methods

    private void onMenuViewModelChanged(object sender, PropertyChangedEventArgs e) {
      switch(e.PropertyName) {
        case "IsViewRawData": {
          viewModel.IsViewCleanData = !viewModel.IsViewCleanData;
          viewModel.IsViewRawData   = !viewModel.IsViewRawData;

          break;
        }
        default: {
          break;
        }
      }
    }

    #endregion Private Methods

  }

}
