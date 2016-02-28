using System.ComponentModel.Composition;
using System.Threading.Tasks;

using AutoMapper;

using STR.MvvmCommon;
using STR.MvvmCommon.Contracts;

using UpkManager.Domain.Contracts;

using UpkManager.Wpf.Messages.FileListing;
using UpkManager.Wpf.ViewEntities;
using UpkManager.Wpf.ViewModels;


namespace UpkManager.Wpf.Controllers {

  [Export(typeof(IController))]
  public class HeaderController : IController {

    #region Private Fields

    private string oldNotes;

    private readonly HeaderViewModel viewModel;

    private readonly IMessenger messenger;
    private readonly IMapper    mapper;

    private readonly IUpkFileRemoteRepository remoteRepository;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public HeaderController(HeaderViewModel ViewModel, IMessenger Messenger, IMapper Mapper, IUpkFileRemoteRepository RemoteRepository) {
      viewModel = ViewModel;

      messenger = Messenger;
         mapper = Mapper;

      remoteRepository = RemoteRepository;

      registerMessages();
      registerCommands();
    }

    #endregion Constructor

    #region Messages

    private void registerMessages() {
      messenger.Register<FileLoadingMessage>(this, onHeaderLoading);

      messenger.Register<FileLoadedMessage>(this, onFileLoaded);
    }

    private void onHeaderLoading(FileLoadingMessage message) {
      viewModel.File   = null;
      viewModel.Header = null;
    }

    private void onFileLoaded(FileLoadedMessage message) {
      viewModel.File   = message.File;
      viewModel.Header = mapper.Map<HeaderViewEntity>(message.File.Header);

      oldNotes = viewModel.File.Notes;
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

  }

}
