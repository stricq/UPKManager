using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

using Ookii.Dialogs.Wpf;

using STR.Common.Extensions;

using STR.MvvmCommon;
using STR.MvvmCommon.Contracts;

using UpkManager.Domain.Contracts;
using UpkManager.Domain.Messages.FileHeader;
using UpkManager.Domain.Models;
using UpkManager.Domain.Models.Tables;
using UpkManager.Domain.ViewModels;


namespace UpkManager.Domain.Controllers {

  [Export(typeof(IController))]
  public class FileHeaderController : IController {

    #region Private Fields

    private readonly FileHeaderViewModel   viewModel;
    private readonly MainMenuViewModel menuViewModel;

    private readonly IMessenger messenger;

    private readonly IUpkFileRepository fileRepository;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public FileHeaderController(FileHeaderViewModel ViewModel, MainMenuViewModel MenuViewModel, IMessenger Messenger, IUpkFileRepository FileRepository) {
          viewModel =     ViewModel;
      menuViewModel = MenuViewModel;

      menuViewModel.PropertyChanged += onMenuViewModelPropertyChanged;

      messenger = Messenger;

      fileRepository = FileRepository;

      registerCommands();
    }

    #endregion Constructor

    #region Commands

    private void registerCommands() {
      menuViewModel.OpenFile = new RelayCommandAsync(onOpenFileExecuteAsync);
    }

    private async Task onOpenFileExecuteAsync() {
      VistaOpenFileDialog ofd = new VistaOpenFileDialog {
        Multiselect      = false,
        Filter           = "Unreal Package Files|*.upk",
        Title            = "Select Package Files"
      };

      bool? result = ofd.ShowDialog();

      if (!result.HasValue || !result.Value) return;

      if (viewModel.Header != null && viewModel.Header.ExportTable.Any()) {
        viewModel.Header.ExportTable.ForEach(et => et.PropertyChanged -= onExportTablePropertyChanged);
      }

      messenger.Send(new FileHeaderLoadingMessage { Filename = ofd.FileName });

      viewModel.Header = null;

      viewModel.Header = new DomainHeader { FullFilename = ofd.FileName };

      viewModel.Header = await fileRepository.LoadAndParseUpk(viewModel.Header, menuViewModel.IsSkipProperties, menuViewModel.IsSkipParsing, onLoadProgress);

      if (viewModel.Header != null && viewModel.Header.ExportTable.Any()) {
        viewModel.Header.ExportTable.ForEach(et => et.PropertyChanged += onExportTablePropertyChanged);
      }

      messenger.Send(new FileHeaderLoadedMessage { FileHeader = viewModel.Header });
    }

    #endregion Commands

    #region Private Methods

    private void onMenuViewModelPropertyChanged(object sender, PropertyChangedEventArgs e) {
      switch(e.PropertyName) {
        case "IsSkipProperties": {
          if (menuViewModel.IsSkipProperties) menuViewModel.IsSkipParsing = true;

          break;
        }
        default: {
          break;
        }
      }
    }

    private void onExportTablePropertyChanged(object sender, PropertyChangedEventArgs e) {
      DomainExportTableEntry export = sender as DomainExportTableEntry;

      if (export == null) return;

      switch(e.PropertyName) {
        case "IsSelected": {
          if (export.IsSelected) messenger.SendAsync(new ExportObjectSelectedMessage { ExportObject = export });

          break;
        }
        default: {
          break;
        }
      }
    }

    private void onLoadProgress(LoadProgressMessage message) {
      messenger.Send(message);
    }

    #endregion Private Methods

  }

}
