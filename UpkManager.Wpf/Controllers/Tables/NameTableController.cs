using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

using AutoMapper;

using STR.Common.Extensions;

using STR.MvvmCommon.Contracts;

using UpkManager.Wpf.Messages.FileListing;
using UpkManager.Wpf.ViewEntities.Tables;
using UpkManager.Wpf.ViewModels.Tables;


namespace UpkManager.Wpf.Controllers.Tables {

  [Export(typeof(IController))]
  public class NameTableController : IController {

    #region Private Fields

    private readonly NameTableViewModel viewModel;

    private readonly IMessenger messenger;
    private readonly IMapper       mapper;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public NameTableController(NameTableViewModel ViewModel, IMessenger Messenger, IMapper Mapper) {
      viewModel = ViewModel;

      viewModel.NameTableEntries = new ObservableCollection<NameTableEntryViewEntity>();

      messenger = Messenger;
         mapper = Mapper;

      registerMessages();
    }

    #endregion Constructor

    #region Messages

    private void registerMessages() {
      messenger.Register<FileLoadingMessage>(this, onFileLoading);
      messenger.Register<FileLoadedMessage>(this, onFileLoaded);
    }

    private void onFileLoading(FileLoadingMessage message) {
      viewModel.NameTableEntries.Clear();
    }

    private void onFileLoaded(FileLoadedMessage message) {
      viewModel.NameTableEntries.AddRange(mapper.Map<IEnumerable<NameTableEntryViewEntity>>(message.File.Header.NameTable));
    }

    #endregion Messages

  }

}
