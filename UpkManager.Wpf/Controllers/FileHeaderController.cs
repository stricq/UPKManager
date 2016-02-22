using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using STR.Common.Extensions;

using STR.MvvmCommon;
using STR.MvvmCommon.Contracts;

using UpkManager.Domain.Contracts;
using UpkManager.Domain.Messages.Application;
using UpkManager.Domain.Messages.FileHeader;
using UpkManager.Domain.Models;
using UpkManager.Domain.Models.Tables;

using UpkManager.Wpf.ViewModels;


namespace UpkManager.Wpf.Controllers {

  [Export(typeof(IController))]
  public class FileHeaderController : IController {

    #region Private Fields

    private string oldNotes;

    private DomainUpkManagerSettings settings;

    private readonly FileHeaderViewModel   viewModel;
    private readonly MainMenuViewModel menuViewModel;

    private readonly IMessenger messenger;

    private readonly IUpkFileRepository         fileRepository;
    private readonly IUpkFileRemoteRepository remoteRepository;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public FileHeaderController(FileHeaderViewModel ViewModel, MainMenuViewModel MenuViewModel, IMessenger Messenger, IUpkFileRepository FileRepository, IUpkFileRemoteRepository RemoteRepository) {
          viewModel =     ViewModel;
      menuViewModel = MenuViewModel;

      menuViewModel.PropertyChanged += onMenuViewModelPropertyChanged;

      messenger = Messenger;

        fileRepository =   FileRepository;
      remoteRepository = RemoteRepository;

      registerMessages();
      registerCommands();
    }

    #endregion Constructor

    #region Messages

    private void registerMessages() {
      messenger.Register<AppLoadedMessage>(this, onAppLoaded);

      messenger.Register<FileHeaderLoadingMessage>(this, onFileHeaderLoading);

      messenger.RegisterAsync<FileHeaderSelectedMessage>(this, onFileHeaderSelected);

      messenger.Register<SettingsChangedMessage>(this, onSettingsChanged);
    }

    private void onAppLoaded(AppLoadedMessage message) {
      settings = message.Settings;
    }

    private void onFileHeaderLoading(FileHeaderLoadingMessage message) {
      viewModel.File   = null;
      viewModel.Header = null;
    }

    private async Task onFileHeaderSelected(FileHeaderSelectedMessage message) {
      await loadUpkFile(Path.Combine(settings.PathToGame, message.File.GameFilename));

      viewModel.File = message.File;

      oldNotes = viewModel.File.Notes;

      message.File.IsErrored = viewModel.Header.IsErrored;
    }

    private void onSettingsChanged(SettingsChangedMessage message) {
      settings = message.Settings;
    }

    #endregion Messages

    #region Commands

    private void registerCommands() {
      viewModel.SaveNotes = new RelayCommandAsync(onSaveNotesExecute, canSaveNotesExecute);
    }

    #region SaveNotes Command

    private bool canSaveNotesExecute() {
      return viewModel.File?.Notes != oldNotes;
    }

    private async Task onSaveNotesExecute() {
      oldNotes = viewModel.File.Notes;

      await remoteRepository.SaveUpkFile(viewModel.File);
    }

    #endregion SaveNotes Command

    #endregion Commands

    #region Private Methods

    private void onMenuViewModelPropertyChanged(object sender, PropertyChangedEventArgs e) {
      switch(e.PropertyName) {
        case "IsSkipProperties": {
          if (menuViewModel.IsSkipProperties) menuViewModel.IsSkipParsing = true;

          break;
        }
        case "IsSkipParsing": {
          if (!menuViewModel.IsSkipParsing) menuViewModel.IsSkipProperties = false;

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

    private async Task loadUpkFile(string fullFilename) {
      messenger.Send(new FileHeaderLoadingMessage { Filename = fullFilename });

      viewModel.Header = null;

      viewModel.Header = new DomainHeader { FullFilename = fullFilename };

      viewModel.Header = await fileRepository.LoadAndParseUpk(viewModel.Header, menuViewModel.IsSkipProperties, menuViewModel.IsSkipParsing, onLoadProgress);

      if (viewModel.Header != null && viewModel.Header.ExportTable.Any()) {
        viewModel.Header.ExportTable.ForEach(et => et.PropertyChanged += onExportTablePropertyChanged);
      }

      messenger.Send(new FileHeaderLoadedMessage { FileHeader = viewModel.Header });
    }

    #endregion Private Methods

  }

}
