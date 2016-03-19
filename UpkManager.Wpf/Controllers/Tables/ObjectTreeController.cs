using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using STR.Common.Extensions;

using STR.MvvmCommon.Contracts;

using UpkManager.Domain.Models;
using UpkManager.Domain.Models.Tables;

using UpkManager.Wpf.Messages.FileListing;
using UpkManager.Wpf.Messages.Status;
using UpkManager.Wpf.Messages.Tables;
using UpkManager.Wpf.ViewEntities.Tables;
using UpkManager.Wpf.ViewModels;
using UpkManager.Wpf.ViewModels.Tables;


namespace UpkManager.Wpf.Controllers.Tables {

  [Export(typeof(IController))]
  public class ObjectTreeController : IController {

    #region Private Fields

    private CancellationTokenSource tokenSource;

    private DomainHeader header;

    private List<DomainImportTableEntry> imports;
    private List<DomainExportTableEntry> exports;

    private readonly IMessenger messenger;
    private readonly IMapper    mapper;

    private readonly ObjectTreeViewModel   viewModel;
    private readonly MainMenuViewModel menuViewModel;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public ObjectTreeController(ObjectTreeViewModel ViewModel, MainMenuViewModel MenuViewModel, IMessenger Messenger, IMapper Mapper) {
          viewModel =     ViewModel;
      menuViewModel = MenuViewModel;

      viewModel.ObjectTree = new ObservableCollection<ObjectTreeViewEntity>();

      messenger = Messenger;
      mapper    = Mapper;

      registerMessages();
    }

    #endregion Constructor

    #region Messages

    private void registerMessages() {
      messenger.Register<FileLoadingMessage>(this, onFileLoading);

      messenger.Register<FileLoadedMessage>(this, onFileLoaded);
    }

    private void onFileLoading(FileLoadingMessage message) {
      tokenSource?.Cancel();

      viewModel.ObjectTree.Traverse(entity => entity.IsExport).ForEach(entity => entity.PropertyChanged -= onObjectTreeViewEntityPropertyChanged);

      viewModel.ObjectTree.Clear();

      header = null;

      imports = null;
      exports = null;
    }

    private void onFileLoaded(FileLoadedMessage message) {
      header = message.File.Header;

      imports = header.ImportTable.ToList();
      exports = header.ExportTable.ToList();

      Task.Run(() => buildObjectParents(resetToken())).FireAndForget();
    }

    #endregion Messages

    #region Private Methods

    private CancellationToken resetToken() {
      tokenSource?.Cancel();

      tokenSource = new CancellationTokenSource();

      return tokenSource.Token;
    }

    private void buildObjectParents(CancellationToken token) {
      LoadProgressMessage message = new LoadProgressMessage { Text = "Building Object Tree..." };

      messenger.SendUi(message);

      List<string> packages = imports.Select(import => import.PackageNameIndex.Name).Distinct().ToList();

      List<ObjectTreeViewEntity> entities = packages.Select(package => new ObjectTreeViewEntity { Name = package }).ToList();

      viewModel.ObjectTree = new ObservableCollection<ObjectTreeViewEntity>(entities.OrderBy(entity => entity.Name));

      foreach(ObjectTreeViewEntity childEntity in viewModel.ObjectTree) {
        if (token.IsCancellationRequested) return;

        buildObjectTypes(childEntity, token);
      }

      message.IsComplete = true;

      messenger.SendUi(message);
    }

    private void buildObjectTypes(ObjectTreeViewEntity parentEntity, CancellationToken token) {
      List<string> types = imports.Where(import => import.PackageNameIndex.Name.Equals(parentEntity.Name, StringComparison.CurrentCultureIgnoreCase))
                                  .Select(import => import.TypeNameIndex.Name)
                                  .Distinct()
                                  .ToList();

      List<ObjectTreeViewEntity> entities = types.Select(type => new ObjectTreeViewEntity { Name = type }).ToList();

      parentEntity.Children = new ObservableCollection<ObjectTreeViewEntity>(entities.OrderBy(entity => entity.Name));

      foreach(ObjectTreeViewEntity childEntity in parentEntity.Children) {
        if (token.IsCancellationRequested) return;

        buildObjectChildren(childEntity, token);
      }
    }

    private void buildObjectChildren(ObjectTreeViewEntity parentEntity, CancellationToken token) {
      List<ObjectTreeViewEntity> entities = new List<ObjectTreeViewEntity>();

      List<DomainImportTableEntry> importChildren = imports.Where(import => import.TypeNameIndex.Name.Equals(parentEntity.Name, StringComparison.CurrentCultureIgnoreCase)).ToList();

      entities.AddRange(mapper.Map<IEnumerable<ObjectTreeViewEntity>>(importChildren));

      List<DomainExportTableEntry> exportChildren = exports.Where(export => export.TypeReferenceNameIndex.Name.Equals(parentEntity.Name, StringComparison.CurrentCultureIgnoreCase)).ToList();

      entities.AddRange(mapper.Map<IEnumerable<ObjectTreeViewEntity>>(exportChildren));

      entities.Where(entity => entity.IsExport).ForEach(entity => entity.PropertyChanged += onObjectTreeViewEntityPropertyChanged);

      parentEntity.Children = new ObservableCollection<ObjectTreeViewEntity>(entities.OrderBy(entity => entity.IsImport).ThenBy(entity => entity.Name));

      foreach(ObjectTreeViewEntity childEntity in parentEntity.Children) {
        if (token.IsCancellationRequested) return;

        buildObjectChildren(childEntity, token);
      }
    }

    private async void onObjectTreeViewEntityPropertyChanged(object sender, PropertyChangedEventArgs args) {
      ObjectTreeViewEntity entity = sender as ObjectTreeViewEntity;

      if (entity == null || !entity.IsExport) return;

      switch(args.PropertyName) {
        case "IsSelected": {
          if (entity.IsSelected) {
            DomainExportTableEntry export = exports.Single(et => et.TableIndex == entity.TableIndex);

            if (export.DomainObject == null) await export.ParseDomainObject(header, menuViewModel.IsSkipProperties, menuViewModel.IsSkipParsing);

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
