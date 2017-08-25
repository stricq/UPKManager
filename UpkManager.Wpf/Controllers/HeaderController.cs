using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

using AutoMapper;

using STR.MvvmCommon;
using STR.MvvmCommon.Contracts;

using UpkManager.Domain.Contracts;
using UpkManager.Domain.Models;

using UpkManager.Wpf.Messages.FileListing;
using UpkManager.Wpf.Messages.Status;
using UpkManager.Wpf.ViewEntities;
using UpkManager.Wpf.ViewModels;


namespace UpkManager.Wpf.Controllers {

  [Export(typeof(IController))]
  public sealed class HeaderController : IController {

    #region Private Fields

    private bool isLocalMode;

    private DomainUpkFile upkfile;

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
    }

    #endregion Constructor

    #region IController Implementation

    public async Task InitializeAsync() {
      registerMessages();
      registerCommands();

      await Task.CompletedTask;
    }

    public int InitializePriority { get; } = 100;

    #endregion IController Implementation

    #region Messages

    private void registerMessages() {
      messenger.Register<FileLoadingMessage>(this, onHeaderLoading);

      messenger.Register<FileLoadedMessage>(this, onFileLoaded);

      messenger.Register<LoadProgressMessage>(this, onLoadProgress);
    }

    private void onHeaderLoading(FileLoadingMessage message) {
      viewModel.File   = null;
      viewModel.Header = null;

      upkfile = null;
    }

    private void onFileLoaded(FileLoadedMessage message) {
      viewModel.File   = message.FileViewEntity;
      viewModel.Header = mapper.Map<HeaderViewEntity>(message.File.Header);

      upkfile = message.File;
    }

    private void onLoadProgress(LoadProgressMessage message) {
      if (!message.IsComplete) return;

      isLocalMode = message.IsLocalMode;
    }

    #endregion Messages

    #region Commands

    private void registerCommands() {
      viewModel.SaveNotes = new RelayCommandAsync(onSaveNotesExecute, canSaveNotesExecute);
    }

    #region SaveNotes Command

    private bool canSaveNotesExecute() {
      return String.Compare(viewModel.File?.Notes, upkfile?.Notes, StringComparison.CurrentCultureIgnoreCase) != 0 && !isLocalMode;
    }

    private async Task onSaveNotesExecute() {
      upkfile.Notes = viewModel.File.Notes;

      await remoteRepository.SaveUpkFile(upkfile);
    }

    #endregion SaveNotes Command

    #endregion Commands

  }

}
