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

using STR.MvvmCommon.Contracts;

using UpkManager.Domain.Contracts;
using UpkManager.Domain.Models;

using UpkManager.Wpf.Messages.Application;
using UpkManager.Wpf.Messages.FileListing;
using UpkManager.Wpf.Messages.Rebuild;
using UpkManager.Wpf.Messages.Settings;
using UpkManager.Wpf.Messages.Status;
using UpkManager.Wpf.ViewEntities;
using UpkManager.Wpf.ViewModels;


namespace UpkManager.Wpf.Controllers {

  [Export(typeof(IController))]
  public sealed class ModsController : IController {

    #region Private Fields

    private DomainSettings settings;

    private readonly List<DomainUpkFile> allMods;

    private readonly IMessenger messenger;

    private readonly IMapper mapper;

    private readonly IUpkFileRepository repository;

    private readonly ModsViewModel viewModel;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public ModsController(ModsViewModel ViewModel, IMessenger Messenger, IMapper Mapper, IUpkFileRepository Repository) {
      viewModel = ViewModel;

      viewModel.Mods = new ObservableCollection<FileViewEntity>();

      messenger = Messenger;

      mapper = Mapper;

      repository = Repository;

      allMods = new List<DomainUpkFile>();
    }

    #endregion Constructor

    #region IController Implementation

    public async Task InitializeAsync() {
      registerMessages();

      await Task.CompletedTask;
    }

    public int InitializePriority { get; } = 100;

    #endregion IController Implementation

    #region Messenger

    private void registerMessages() {
      messenger.Register<AppLoadedMessage>(this, onApplicationLoaded);

      messenger.Register<SettingsChangedMessage>(this, onSettingsChanged);

      messenger.Register<FileListingLoadedMessage>(this, onFileListingLoaded);

      messenger.Register<ModFileBuiltMessage>(this, onModFileBuilt);
    }

    private void onApplicationLoaded(AppLoadedMessage message) {
      settings = message.Settings;
    }

    private void onSettingsChanged(SettingsChangedMessage message) {
      settings = message.Settings;
    }

    private void onFileListingLoaded(FileListingLoadedMessage message) {
      allMods.Clear();

      allMods.AddRange(message.Allfiles.SelectMany(f => f.ModdedFiles));

      viewModel.Mods.ForEach(mf => mf.PropertyChanged -= onFileViewEntityChanged);

      viewModel.Mods.Clear();

      viewModel.Mods.AddRange(mapper.Map<IEnumerable<FileViewEntity>>(allMods));

      viewModel.Mods.ForEach(mf => mf.PropertyChanged += onFileViewEntityChanged);
    }

    private void onModFileBuilt(ModFileBuiltMessage message) {
      allMods.Where(m => m.GameFilename == message.UpkFile.GameFilename).ToList().ForEach(m => allMods.Remove(m));

      allMods.Add(message.UpkFile);

      allMods.Sort(domainUpkfileComparison);

      viewModel.Mods.ForEach(mf => mf.PropertyChanged -= onFileViewEntityChanged);

      viewModel.Mods.Clear();

      viewModel.Mods.AddRange(mapper.Map<IEnumerable<FileViewEntity>>(allMods));

      viewModel.Mods.ForEach(mf => mf.PropertyChanged += onFileViewEntityChanged);
    }

    #endregion Messenger

    #region Private Methods

    private static int domainUpkfileComparison(DomainUpkFile left, DomainUpkFile right) {
      return String.Compare(left.Filename, right.Filename, StringComparison.CurrentCultureIgnoreCase);
    }

    private async void onFileViewEntityChanged(object sender, PropertyChangedEventArgs args) {
      FileViewEntity file = sender as FileViewEntity;

      if (file == null) return;

      switch(args.PropertyName) {
        case "IsSelected": {
          if (file.IsSelected) {
            viewModel.Mods.Where(f => f != file).ForEach(f => f.IsSelected = false);

            messenger.Send(new FileLoadingMessage());

            DomainUpkFile upkFile = allMods.Single(f => f.GameFilename == file.GameFilename);

            if (upkFile.Header == null) {
              await loadUpkFile(file, upkFile);

              if (file.IsErrored) return;
            }

            upkFile.LastAccess = DateTime.Now;

            messenger.Send(new FileLoadedMessage { FileViewEntity = file, File = upkFile });
          }

          break;
        }
      }
    }

    private async Task loadUpkFile(FileViewEntity file, DomainUpkFile upkFile) {
      try {
        upkFile.Header = await repository.LoadUpkFile(Path.Combine(settings.PathToGame, upkFile.GameFilename));

        await Task.Run(() => upkFile.Header.ReadHeaderAsync(onLoadProgress));

        file.IsErrored = false;
      }
      catch(Exception ex) {
        file.IsErrored = true;

        messenger.Send(new ApplicationErrorMessage { HeaderText = $"Error Loading UPK File: {upkFile.GameFilename}", Exception = ex });
      }
    }

    private void onLoadProgress(DomainLoadProgress progress) {
      messenger.Send(mapper.Map<LoadProgressMessage>(progress));
    }

    #endregion Private Methods

  }

}
