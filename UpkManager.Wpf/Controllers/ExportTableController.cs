using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;

using AutoMapper;

using STR.Common.Extensions;

using STR.MvvmCommon.Contracts;

using UpkManager.Domain.Models.Tables;

using UpkManager.Wpf.Messages.FileListing;
using UpkManager.Wpf.Messages.Tables;
using UpkManager.Wpf.ViewEntities;
using UpkManager.Wpf.ViewModels;


namespace UpkManager.Wpf.Controllers {

  [Export(typeof(IController))]
  public class ExportTableController : IController {

    #region Private Fields

    private List<DomainExportTableEntry> exportTableEntries;

    private readonly ExportTableViewModel viewModel;

    private readonly IMessenger messenger;
    private readonly IMapper       mapper;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public ExportTableController(ExportTableViewModel ViewModel, IMessenger Messenger, IMapper Mapper) {
      viewModel = ViewModel;

      viewModel.ExportTableEntries = new ObservableCollection<ExportTableEntryViewEntity>();

      exportTableEntries = new List<DomainExportTableEntry>();

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
      viewModel.ExportTableEntries.ForEach(export => export.PropertyChanged -= onExportTableEntryPropertyChanged);

      viewModel.ExportTableEntries.Clear();

      exportTableEntries.Clear();
    }

    private void onFileLoaded(FileLoadedMessage message) {
      viewModel.ExportTableEntries.AddRange(mapper.Map<IEnumerable<ExportTableEntryViewEntity>>(message.File.Header.ExportTable));

      exportTableEntries.AddRange(message.File.Header.ExportTable);

      viewModel.ExportTableEntries.ForEach(export => export.PropertyChanged += onExportTableEntryPropertyChanged);
    }

    private void onExportTableEntryPropertyChanged(object sender, PropertyChangedEventArgs args) {
      ExportTableEntryViewEntity export = sender as ExportTableEntryViewEntity;

      if (export == null) return;

      switch(args.PropertyName) {
        case "IsSelected": {
          if (export.IsSelected) messenger.Send(new ExportTableEntrySelectedMessage { ExportTableEntry = exportTableEntries.Single(et => et.TableIndex == export.TableIndex) });

          break;
        }
        default: {
          break;
        }
      }
    }

    #endregion Messages

  }

}
