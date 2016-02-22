using System.ComponentModel.Composition;
using System.Threading.Tasks;

using Ookii.Dialogs.Wpf;

using STR.MvvmCommon;
using STR.MvvmCommon.Contracts;

using UpkManager.Domain.Contracts;
using UpkManager.Domain.Messages.FileHeader;

using UpkManager.Wpf.ViewModels;


namespace UpkManager.Wpf.Controllers {

  [Export(typeof(IController))]
  public class PropertyController : IController {

    #region Private Fields

    private readonly PropertyViewModel     viewModel;
    private readonly MainMenuViewModel menuViewModel;

    private readonly IMessenger messenger;

    private readonly IUpkFileRepository fileRepository;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public PropertyController(PropertyViewModel ViewModel, MainMenuViewModel MenuViewModel, IMessenger Messenger, IUpkFileRepository FileRepository) {
          viewModel = ViewModel;
      menuViewModel = MenuViewModel;

      messenger = Messenger;

      fileRepository = FileRepository;

      registerMessages();
      registerCommands();
    }

    #endregion Constructor

    #region Messages

    private void registerMessages() {
      messenger.Register<ExportObjectSelectedMessage>(this, onExportObjectSelected);

      messenger.Register<FileHeaderLoadingMessage>(this, onFileHeaderLoading);
    }

    private void onExportObjectSelected(ExportObjectSelectedMessage message) {
      viewModel.Export = message.ExportObject;
    }

    private void onFileHeaderLoading(FileHeaderLoadingMessage message) {
      viewModel.Export = null;
    }

    #endregion Messages

    #region Commands

    private void registerCommands() {
      menuViewModel.SaveObjectAs = new RelayCommandAsync(onSaveObjectAsExecute, canSaveObjectAsExecute);
    }

    private bool canSaveObjectAsExecute() {
      return viewModel.Export != null && viewModel.Export.DomainObject.IsExportable;
    }

    private async Task onSaveObjectAsExecute() {
      VistaSaveFileDialog sfd = new VistaSaveFileDialog {
        DefaultExt = ".dds",
        Filter = "DirectDraw Surface|*.dds",
        Title = "Save Object As..."
      };

      bool? result = sfd.ShowDialog();

      if (!result.HasValue || !result.Value) return;

      await fileRepository.SaveObject(viewModel.Export, sfd.FileName);
    }

    #endregion Commands

  }

}
