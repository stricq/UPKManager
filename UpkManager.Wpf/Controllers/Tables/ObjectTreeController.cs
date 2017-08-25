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
using STR.Common.Messages;

using STR.MvvmCommon.Contracts;

using UpkManager.Domain.Models.UpkFile;
using UpkManager.Domain.Models.UpkFile.Tables;

using UpkManager.Wpf.Messages.FileListing;
using UpkManager.Wpf.Messages.Status;
using UpkManager.Wpf.Messages.Tables;
using UpkManager.Wpf.ViewEntities.Tables;
using UpkManager.Wpf.ViewModels;
using UpkManager.Wpf.ViewModels.Tables;


namespace UpkManager.Wpf.Controllers.Tables {

  [Export(typeof(IController))]
  public sealed class ObjectTreeController : IController {

    #region Private Fields

    private int importRecursion = 0;

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

      try {
        List<ObjectTreeViewEntity> packages = imports.Where(import => import.OwnerReferenceNameIndex ==  null)
                                                     .Select(import => import.PackageNameIndex.Name)
                                                     .Distinct()
                                                     .Select(packageName => new ObjectTreeViewEntity { Name = packageName, IsExpanded = packageName.Equals("Core", StringComparison.CurrentCultureIgnoreCase) })
                                                     .ToList();

        viewModel.ObjectTree = new ObservableCollection<ObjectTreeViewEntity>(packages.OrderBy(package => package.Name));

        foreach(ObjectTreeViewEntity package in packages) {
          List<ObjectTreeViewEntity> firstTypes = imports.Where(import => import.OwnerReferenceNameIndex == null
                                                                       && package.Name == import.PackageNameIndex.Name)
                                                         .Select(import => import.TypeNameIndex.Name)
                                                         .Distinct()
                                                         .Select(typeName1 => new ObjectTreeViewEntity { Name = typeName1, Parent = package, IsExpanded = typeName1.Equals("Package", StringComparison.CurrentCultureIgnoreCase) })
                                                         .ToList();

          package.Children = new ObservableCollection<ObjectTreeViewEntity>(firstTypes.OrderBy(type => type.Name));

          List<string> names = new List<string> { "Engine", "Core" };

          foreach(ObjectTreeViewEntity type1 in firstTypes) {
            List<ObjectTreeViewEntity> firstNames = imports.Where(import => import.OwnerReferenceNameIndex == null
                                                                         && package.Name == import.PackageNameIndex.Name
                                                                         &&   type1.Name == import.TypeNameIndex.Name)
                                                           .Select(import => import.NameTableIndex.Name)
                                                           .Distinct()
                                                           .Select(name1 => new ObjectTreeViewEntity { Name = name1, Parent = type1, IsExpanded = names.Contains(name1) })
                                                           .ToList();

            type1.Children = new ObservableCollection<ObjectTreeViewEntity>(firstNames.OrderBy(name => name.Name));

            recursiveImports(firstNames);
          }
        }
      }
      catch(Exception ex) {
        messenger.SendUi(new ApplicationErrorMessage { HeaderText = "Error Building Object Tree", Exception = ex });
      }

      message.IsComplete = true;

      messenger.SendUi(message);
    }

    private void recursiveImports(List<ObjectTreeViewEntity> parents) {
      foreach(ObjectTreeViewEntity parent in parents) {
        List<ObjectTreeViewEntity> secondTypes = imports.Where(import => import.OwnerReferenceNameIndex != null
                                                                      && parent.Name == import.OwnerReferenceNameIndex.Name)
                                                        .Select(import => import.TypeNameIndex.Name)
                                                        .Distinct()
                                                        .Select(typeName2 => new ObjectTreeViewEntity { Name = typeName2, Parent = parent, IsExpanded = typeName2.Equals("Class", StringComparison.CurrentCultureIgnoreCase) })
                                                        .ToList();

        if (secondTypes.Any()) parent.Children = new ObservableCollection<ObjectTreeViewEntity>(secondTypes.OrderBy(type => type.Name));

        foreach(ObjectTreeViewEntity type2 in secondTypes) {
          List<ObjectTreeViewEntity> secondNames = imports.Where(import => import.OwnerReferenceNameIndex != null
                                                                        && parent.Name == import.OwnerReferenceNameIndex.Name
                                                                        && type2.Name == import.TypeNameIndex.Name)
                                                          .Select(import => import.NameTableIndex.Name)
                                                          .Distinct()
                                                          .Select(name2 => new ObjectTreeViewEntity { Name = name2, Parent = type2, IsImport = true })
                                                          .ToList();

          if (secondNames.Any()) type2.Children = new ObservableCollection<ObjectTreeViewEntity>(secondNames.OrderBy(name => name.Name));

          if (secondNames.Any()) {
            importRecursion++;

            if (importRecursion < 11) recursiveImports(secondNames);

            importRecursion--;
          }

          foreach(ObjectTreeViewEntity name2 in secondNames) {
            List<ObjectTreeViewEntity> exportNames = exports.Where(export => export.ArchetypeReferenceNameIndex != null
                                                                          && name2.Name == export.ArchetypeReferenceNameIndex.Name)
                                                            .Select(export => mapper.Map<ObjectTreeViewEntity>(export))
                                                            .ToList();

            if (!exportNames.Any()) continue;

            exportNames.ForEach(exportName => {
              exportName.Parent           = name2;
              exportName.PropertyChanged += onObjectTreeViewEntityPropertyChanged;
            });

            name2.IsImport = !exportNames.Any();
            name2.Children = new ObservableCollection<ObjectTreeViewEntity>(exportNames.OrderBy(name => name.Name));
          }

          foreach(ObjectTreeViewEntity name2 in secondNames) {
            List<ObjectTreeViewEntity> exportNames = exports.Where(export => export.ArchetypeReferenceNameIndex == null
                                                                          && name2.Name == export.TypeReferenceNameIndex.Name)
                                                            .Select(export => mapper.Map<ObjectTreeViewEntity>(export))
                                                            .ToList();

            if (!exportNames.Any()) continue;

            exportNames.ForEach(exportName => {
              exportName.Parent           = name2;
              exportName.PropertyChanged += onObjectTreeViewEntityPropertyChanged;
            });

            name2.IsImport = !exportNames.Any();
            name2.Children = new ObservableCollection<ObjectTreeViewEntity>(exportNames.OrderBy(name => name.Name));
          }
        }
      }
    }

    private void buildObjectTypes(ObjectTreeViewEntity parentEntity, CancellationToken token) {
      List<string> types = imports.Where(import => import.PackageNameIndex.Name.Equals(parentEntity.Name, StringComparison.CurrentCultureIgnoreCase))
                                  .Select(import => import.TypeNameIndex.Name)
                                  .Distinct()
                                  .ToList();

      imports.RemoveAll(import => types.Contains(import.NameTableIndex.Name));

      List<ObjectTreeViewEntity> entities = types.Select(type => new ObjectTreeViewEntity { Name = type, IsExpanded = type.Equals("Class", StringComparison.CurrentCultureIgnoreCase) }).ToList();

      parentEntity.Children = new ObservableCollection<ObjectTreeViewEntity>(entities.OrderBy(entity => entity.Name));

      Parallel.ForEach(parentEntity.Children, childEntity => {
        if (token.IsCancellationRequested) return;

        buildObjectChildren(childEntity, token);
      });
    }

    private void buildObjectChildren(ObjectTreeViewEntity parentEntity, CancellationToken token) {
      List<ObjectTreeViewEntity> entities = new List<ObjectTreeViewEntity>();

      List<DomainImportTableEntry> importChildren = imports.Where(import => import.TypeNameIndex.Name.Equals(parentEntity.Name, StringComparison.CurrentCultureIgnoreCase)).ToList();

      entities.AddRange(mapper.Map<IEnumerable<ObjectTreeViewEntity>>(importChildren));

      if (token.IsCancellationRequested) return;

      List<DomainExportTableEntry> exportChildren = exports.Where(export => export.TypeReferenceNameIndex.Name.Equals(parentEntity.Name, StringComparison.CurrentCultureIgnoreCase)).ToList();

      entities.AddRange(mapper.Map<IEnumerable<ObjectTreeViewEntity>>(exportChildren));

      if (token.IsCancellationRequested) return;

      entities.Where(entity => entity.IsExport).ForEach(entity => entity.PropertyChanged += onObjectTreeViewEntityPropertyChanged);

      parentEntity.Children = new ObservableCollection<ObjectTreeViewEntity>(entities.OrderBy(entity => entity.IsImport).ThenBy(entity => entity.Name));

      if (token.IsCancellationRequested) return;

      Parallel.ForEach(parentEntity.Children, childEntity => {
        if (token.IsCancellationRequested) return;

        buildObjectChildren(childEntity, token);
      });
    }

    private async void onObjectTreeViewEntityPropertyChanged(object sender, PropertyChangedEventArgs args) {
      ObjectTreeViewEntity entity = sender as ObjectTreeViewEntity;

      if (entity == null || !entity.IsExport) return;

      switch(args.PropertyName) {
        case "IsSelected": {
          if (entity.IsSelected) {
            DomainExportTableEntry export = exports.Single(et => et.TableIndex == entity.TableIndex);

            if (export.DomainObject == null) await export.ParseDomainObject(header, menuViewModel.IsSkipProperties, menuViewModel.IsSkipParsing);

            messenger.Send(new ExportTableEntrySelectedMessage { ExportTableEntry = export });
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
