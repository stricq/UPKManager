using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using STR.Common.Extensions;
using STR.Common.Messages;

using STR.MvvmCommon;
using STR.MvvmCommon.Contracts;

using UpkManager.Domain.Contracts;
using UpkManager.Domain.Models;

using UpkManager.Wpf.Messages.Application;
using UpkManager.Wpf.Messages.Rebuild;
using UpkManager.Wpf.Messages.Settings;
using UpkManager.Wpf.ViewEntities;
using UpkManager.Wpf.ViewModels;


namespace UpkManager.Wpf.Controllers {

  [Export(typeof(IController))]
  public class RebuildController : IController {

    #region Private Fields

    private bool isSelf;

    private FileSystemWatcher fileWatcher;

    private DomainSettings settings;

    private readonly IMessenger messenger;

    private readonly IMapper mapper;

    private readonly IUpkFileRepository repository;

    private readonly  RebuildViewModel     viewModel;
    private readonly MainMenuViewModel menuViewModel;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public RebuildController(RebuildViewModel ViewModel, MainMenuViewModel MenuViewModel, IMessenger Messenger, IMapper Mapper, IUpkFileRepository Repository) {
          viewModel =     ViewModel;
      menuViewModel = MenuViewModel;

      messenger = Messenger;

      mapper = Mapper;

      repository = Repository;

      registerMessages();
      registerCommands();
    }

    #endregion Constructor

    #region Messages

    private void registerMessages() {
      messenger.Register<AppLoadedMessage>(this, onApplicationLoaded);

      messenger.Register<SettingsChangedMessage>(this, onSettingsChanged);
    }

    private void onApplicationLoaded(AppLoadedMessage message) {
      settings = message.Settings;

      loadExportFiles();

      setupWatchers();
    }

    private void onSettingsChanged(SettingsChangedMessage message) {
      settings = message.Settings;

      setupWatchers();
    }

    #endregion Messages

    #region Commands

    private void registerCommands() {
      menuViewModel.RebuildExported = new RelayCommandAsync(onRebuildExportedExecute, canRebuildExportedExecute);
    }

    #region RebuildExported Command

    private bool canRebuildExportedExecute() {
      return viewModel.ExportsTree?.Traverse(e => e.IsChecked).Any() ?? false;
    }

    private async Task onRebuildExportedExecute() {
      await Task.FromResult(1);
    }

    #endregion RebuildExported Command

    #endregion Commands

    #region Private Methods

    private void setupWatchers() {
      if (String.IsNullOrEmpty(settings.ExportPath)) return;

      fileWatcher = new FileSystemWatcher {
        Path                  = settings.ExportPath,
        NotifyFilter          = NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Size,
        Filter                = String.Empty,
        IncludeSubdirectories = true
      };

      fileWatcher.Changed += onWatcherChanged;
      fileWatcher.Created += onWatcherChanged;
      fileWatcher.Deleted += onWatcherChanged;
      fileWatcher.Renamed += onWatcherRenamed;

      fileWatcher.EnableRaisingEvents = true;
    }

    private void onWatcherChanged(object sender, FileSystemEventArgs e) {
      loadExportFiles();
    }

    private void onWatcherRenamed(object sender, RenamedEventArgs e) {
      loadExportFiles();
    }

    private async void loadExportFiles() {
      if (String.IsNullOrEmpty(settings.ExportPath)) return;

      DomainExportedObject root = new DomainExportedObject();

      try {
        await repository.LoadDirectoryRecursive(root, settings.ExportPath);
      }
      catch(Exception ex) {
        messenger.Send(new ApplicationErrorMessage { ErrorMessage = ex.Message, Exception = ex });
      }

      if (root.Children == null || !root.Children.Any()) return;

      viewModel.ExportsTree?.Traverse(e => true).ToList().ForEach(e => e.PropertyChanged -= onExportedObjectViewEntityChanged);

      viewModel.ExportsTree = new ObservableCollection<ExportedObjectViewEntity>(mapper.Map<IEnumerable<ExportedObjectViewEntity>>(root.Children));

      viewModel.ExportsTree.Traverse(e => true).ToList().ForEach(e => e.PropertyChanged += onExportedObjectViewEntityChanged);
    }

    private void onExportedObjectViewEntityChanged(object sender, PropertyChangedEventArgs args) {
      if (isSelf) return;

      ExportedObjectViewEntity entity = sender as ExportedObjectViewEntity;

      if (entity == null) return;

      switch(args.PropertyName) {
        case "IsChecked": {
          entity.Children?.ForEach(e => e.IsChecked = entity.IsChecked);

          if (Path.HasExtension(entity.Name)) {
            isSelf = true;

            if (entity.IsChecked) entity.Parent.IsChecked = true;
            else {
              if (!entity.Parent.Children.Any(e => e.IsChecked)) entity.Parent.IsChecked = false;
            }

            isSelf = false;
          }

          break;
        }
        case "IsSelected": {
          if (entity.IsSelected && Path.HasExtension(entity.Filename)) messenger.Send(new ExportedObjectSelectedMessage { Filename = entity.Filename });

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
