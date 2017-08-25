using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

using AutoMapper;

using STR.Common.Extensions;

using STR.MvvmCommon.Contracts;

using UpkManager.Wpf.Messages.FileListing;
using UpkManager.Wpf.ViewEntities.Tables;
using UpkManager.Wpf.ViewModels.Tables;


namespace UpkManager.Wpf.Controllers.Tables {

  [Export(typeof(IController))]
  public sealed class CompressionTablesController : IController {

    #region Private Fields

    private readonly CompressionTablesViewModel viewModel;

    private readonly IMessenger messenger;
    private readonly IMapper       mapper;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public CompressionTablesController(CompressionTablesViewModel ViewModel, IMessenger Messenger, IMapper Mapper) {
      viewModel = ViewModel;

      viewModel.CompressionTableEntries = new ObservableCollection<CompressionTableEntryViewEntity>();
      viewModel.CompressionBlocks       = new ObservableCollection<CompressionBlockViewEntity>();

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
      viewModel.CompressionTableEntries.ForEach(ct => ct.PropertyChanged -= onCompressionTableEntryChanged);

      viewModel.CompressionTableEntries.Clear();
      viewModel.CompressionBlocks.Clear();
    }

    private void onFileLoaded(FileLoadedMessage message) {
      viewModel.CompressionTableEntries.AddRange(mapper.Map<IEnumerable<CompressionTableEntryViewEntity>>(message.File.Header.CompressedChunks));

      viewModel.CompressionTableEntries.ForEach(ct => ct.PropertyChanged += onCompressionTableEntryChanged);
    }

    #endregion Messages

    #region Private Methods

    private void onCompressionTableEntryChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
      CompressionTableEntryViewEntity entry = sender as CompressionTableEntryViewEntity;

      if (entry == null) return;

      switch(e.PropertyName) {
        case "IsSelected": {
          if (entry.IsSelected) {
            viewModel.CompressionBlocks.Clear();

            viewModel.CompressionBlocks.AddRange(entry.CompressionBlocks);
          }

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
