using System.ComponentModel;
using System.ComponentModel.Composition;

using STR.MvvmCommon.Contracts;

using UpkManager.Wpf.ViewModels;


namespace UpkManager.Wpf.Controllers {

  [Export(typeof(IController))]
  public class HeaderTablesController : IController {

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
