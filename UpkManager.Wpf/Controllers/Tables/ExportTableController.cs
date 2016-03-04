using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;

using AutoMapper;

using STR.Common.Extensions;

using STR.MvvmCommon.Contracts;

using UpkManager.Domain.Models;
using UpkManager.Domain.Models.Tables;

using UpkManager.Wpf.Messages.FileListing;
using UpkManager.Wpf.Messages.Tables;
using UpkManager.Wpf.ViewEntities.Tables;
using UpkManager.Wpf.ViewModels.Tables;


namespace UpkManager.Wpf.Controllers.Tables {

  [Export(typeof(IController))]
  public class ExportTableController : IController {

    #region Private Fields

    private DomainHeader header;

    private readonly List<DomainExportTableEntry> exportTableEntries;

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
      header = message.File.Header;

      viewModel.ExportTableEntries.AddRange(mapper.Map<IEnumerable<ExportTableEntryViewEntity>>(message.File.Header.ExportTable));

      exportTableEntries.AddRange(message.File.Header.ExportTable);

      viewModel.ExportTableEntries.ForEach(export => export.PropertyChanged += onExportTableEntryPropertyChanged);
    }

    #endregion Messages

    #region Private Methods

    private async void onExportTableEntryPropertyChanged(object sender, PropertyChangedEventArgs args) {
      ExportTableEntryViewEntity exportEntity = sender as ExportTableEntryViewEntity;

      if (exportEntity == null) return;

      switch(args.PropertyName) {
        case "IsSelected": {
          if (exportEntity.IsSelected) {
            viewModel.ExportTableEntries.Where(ex => ex != exportEntity).ForEach(ex => ex.IsSelected = false);

            DomainExportTableEntry export = exportTableEntries.Single(et => et.TableIndex == exportEntity.TableIndex);

            if (export.DomainObject == null) await export.ParseDomainObject(header, false, true);

            await messenger.SendAsync(new ExportTableEntrySelectedMessage { ExportTableEntry = export });
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
