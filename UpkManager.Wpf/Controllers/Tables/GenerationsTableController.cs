using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

using AutoMapper;

using STR.MvvmCommon.Contracts;

using UpkManager.Wpf.Messages.FileListing;
using UpkManager.Wpf.ViewEntities.Tables;
using UpkManager.Wpf.ViewModels.Tables;


namespace UpkManager.Wpf.Controllers.Tables {

  [Export(typeof(IController))]
  public sealed class GenerationsTableController : IController {

    #region Private Fields

    private readonly GenerationsTableViewModel viewModel;

    private readonly IMessenger messenger;
    private readonly IMapper       mapper;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public GenerationsTableController(GenerationsTableViewModel ViewModel, IMessenger Messenger, IMapper Mapper) {
      viewModel = ViewModel;

      viewModel.GenerationsTableEntries = new ObservableCollection<GenerationsTableEntryViewEntity>();

      messenger = Messenger;
         mapper = Mapper;
    }

    #endregion Constructor

    #region IController Implementation

    public async Task InitializeAsync() {
      registerMessages();

      await Task.CompletedTask;
    }

    public int InitializePriority { get; } = 100;

    #endregion IController Implementation

    #region Messages

    private void registerMessages() {
      messenger.Register<FileLoadingMessage>(this, onFileLoading);
      messenger.Register<FileLoadedMessage>(this, onFileLoaded);
    }

    private void onFileLoading(FileLoadingMessage message) {
      viewModel.GenerationsTableEntries.Clear();
    }

    private void onFileLoaded(FileLoadedMessage message) {
      viewModel.GenerationsTableEntries = new ObservableCollection<GenerationsTableEntryViewEntity>(mapper.Map<IEnumerable<GenerationsTableEntryViewEntity>>(message.File.Header.GenerationTable));
    }

    #endregion Messages

  }

}
