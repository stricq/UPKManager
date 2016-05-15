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

using STR.DialogView.Domain.Messages;

using STR.MvvmCommon;
using STR.MvvmCommon.Contracts;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Contracts;
using UpkManager.Domain.Models;
using UpkManager.Domain.Models.UpkFile;
using UpkManager.Domain.Models.UpkFile.Tables;

using UpkManager.Wpf.Messages.Application;
using UpkManager.Wpf.Messages.FileListing;
using UpkManager.Wpf.Messages.Settings;
using UpkManager.Wpf.Messages.Status;
using UpkManager.Wpf.ViewEntities;
using UpkManager.Wpf.ViewModels;


namespace UpkManager.Wpf.Controllers {

  [Export(typeof(IController))]
  public class FileListingController : IController {

    #region Private Fields

    private string oldPathToGame;

    private DomainSettings settings;

    private readonly List<DomainUpkFile> allFiles;

    private readonly FileListingViewModel viewModel;
    private readonly MainMenuViewModel menuViewModel;

    private readonly IMessenger messenger;
    private readonly IMapper    mapper;

    private readonly IUpkFileRepository repository;

    private readonly IUpkFileRemoteRepository remoteRepository;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public FileListingController(FileListingViewModel ViewModel, MainMenuViewModel MenuViewModel, IMessenger Messenger, IMapper Mapper, IUpkFileRepository Repository, IUpkFileRemoteRepository RemoteRepository) {
          viewModel = ViewModel;
      menuViewModel = MenuViewModel;

      messenger = Messenger;
         mapper = Mapper;

      repository = Repository;

      remoteRepository = RemoteRepository;

          viewModel.PropertyChanged += onViewModelPropertyChanged;
      menuViewModel.PropertyChanged += onMenuViewModelPropertyChanged;

      allFiles = new List<DomainUpkFile>();

      registerMessages();
      registerCommands();
    }

    #endregion Constructor

    #region Messages

    private void registerMessages() {
      messenger.RegisterAsync<AppLoadedMessage>(this, onApplicationLoaded);

      messenger.Register<SettingsChangedMessage>(this, onSettingsChanged);

      messenger.RegisterAsync<StatusTickMessage>(this, onStatusTick);
    }

    private async Task onApplicationLoaded(AppLoadedMessage message) {
      settings = message.Settings;

      oldPathToGame = settings.PathToGame;

      viewModel.SelectedType        = ObjectTypes.Texture2D.ToString();
      viewModel.IsShowFilesWithType = true;

      await loadAllFiles();
    }

    private async void onSettingsChanged(SettingsChangedMessage message) {
      if (settings.PathToGame != oldPathToGame) {
        viewModel.Files.ForEach(f => f.PropertyChanged -= onFileEntityViewModelChanged);

        messenger.Send(new FileLoadingMessage());

        await loadAllFiles();
      }

      oldPathToGame = settings.PathToGame;
    }

    private async Task onStatusTick(StatusTickMessage message) {
      await Task.Run(() => {
        allFiles.ForEach(f => {
          if (f.LastAccess.HasValue && (DateTime.Now - f.LastAccess.Value).Minutes > 2) {
            f.Header     = null;
            f.LastAccess = null;
          }
        });
      });
    }

    #endregion Messages

    #region Commands

    private void registerCommands() {
      menuViewModel.ReloadFiles = new RelayCommandAsync(onReloadFilesExecute, canReloadFilesExecute);

      menuViewModel.ExportFiles = new RelayCommandAsync(onExportFilesExecute, canExportFilesExecute);

      menuViewModel.SelectAllFiles   = new RelayCommand(onSelectAllFilesExecute,   canSelectAllFilesExecute);
      menuViewModel.DeselectAllFiles = new RelayCommand(onDeselectAllFilesExecute, canDeselectAllFilesExecute);

      menuViewModel.ScanUpkFiles = new RelayCommand(onScanUpkFilesExecute, canScanUpkFilesExecute);
    }

    #region ReloadFiles Command

    private static bool canReloadFilesExecute() {
      return true;
    }

    private async Task onReloadFilesExecute() {
      await loadAllFiles();
    }

    #endregion ReloadFiles Command

    #region ExportFiles Command

    private bool canExportFilesExecute() {
      return viewModel.Files.Any(f => f.IsChecked);
    }

    private async Task onExportFilesExecute() {
      List<string> ids = viewModel.Files.Where(fe => fe.IsChecked).Select(fe => fe.Id).ToList();

      List<DomainUpkFile> upkFiles = allFiles.Where(f => ids.Contains(f.Id)).ToList();

      await exportFileObjects(upkFiles);
    }

    #endregion ExportFiles Command

    #region SelectAllFiles Command

    private bool canSelectAllFilesExecute() {
      return viewModel.Files.Any();
    }

    private void onSelectAllFilesExecute() {
      viewModel.Files.ForEach(f => f.IsChecked = true);
    }

    #endregion SelectAllFiles Command

    #region DeselectAllFiles Command

    private bool canDeselectAllFilesExecute() {
      return viewModel.Files.Any();
    }

    private void onDeselectAllFilesExecute() {
      viewModel.Files.ForEach(f => f.IsChecked = false);
    }

    #endregion DeselectAllFiles Command

    #region ScanUpkFiles Command

    private bool canScanUpkFilesExecute() {
      return allFiles.Any();
    }

    private void onScanUpkFilesExecute() {
      messenger.Send(new MessageBoxDialogMessage { Message = "Scanning all the UPK files can take a very long time.\n\nAre you sure?", Callback = onScanUpkFilesResponse });
    }

    private async void onScanUpkFilesResponse(MessageBoxDialogMessage message) {
      if (message.IsCancel) return;

      await scanUpkFiles(allFiles);
    }

    #endregion ScanUpkFiles Command

    #endregion Commands

    #region Private Methods

    private async Task loadAllFiles() {
      messenger.Send(new FileLoadingMessage());

      allFiles.Clear();

      viewModel.Files.Clear();

      if (String.IsNullOrEmpty(settings.PathToGame)) return;

      LoadProgressMessage progress = new LoadProgressMessage { Text = "Loading Game Files..." };

      messenger.Send(progress);

      int version;

      try {
        version = await repository.GetGameVersion(settings.PathToGame);
      }
      catch(FileNotFoundException) {
        messenger.Send(new MessageBoxDialogMessage { Header = "Game Version File Not Found", Message = "The VersionInfo_BnS.ini file could not be found.\n\nPlease ensure your settings are pointed to the 'contents' directory.", HasCancel = false });

        progress.IsComplete = true;

        messenger.Send(progress);

        return;
      }

      List<DomainUpkFile> localFiles = await loadGameFiles(version);

      if (!localFiles.Any()) {
        progress.IsComplete = true;

        messenger.Send(progress);

        return;
      }

      List<DomainUpkFile> mods = (from row in localFiles
                                   let path = Path.GetDirectoryName(row.GameFilename)
                                 where path != null
                                    && !path.ToLowerInvariant().EndsWith("cookedpc")
                                select row).ToList();

      localFiles.RemoveAll(f => mods.Contains(f));

      progress.Text = "Loading Remote Database...";

      messenger.Send(progress);

      List<DomainUpkFile> remoteFiles;

      try {
        remoteFiles = await remoteRepository.LoadUpkFiles(version);
      }
      catch(Exception ex) {
        messenger.Send(new MessageBoxDialogMessage { Header = "Error Received from Remote Database", Message = $"The remote database returned an error.  Please try again in a few minutes.\n\n{ex.Message}", HasCancel = false });

        progress.IsComplete = true;

        messenger.Send(progress);

        return;
      }

      List<DomainUpkFile> matches = (from row1 in localFiles
                                     join row2 in remoteFiles on row1.GameFilename.ToLowerInvariant() equals row2.GameFilename.ToLowerInvariant()
                                    where row1.FileSize == row2.FileSize
                                   select row2).ToList();

      if (matches.Any()) allFiles.AddRange(matches.OrderBy(f => f.Filename));

      List<DomainUpkFile> changes = (from row1 in localFiles
                                     join row2 in remoteFiles on row1.GameFilename.ToLowerInvariant() equals row2.GameFilename.ToLowerInvariant()
                                    where row1.FileSize != row2.FileSize
                                   select row2).ToList();

      if (changes.Any()) {
        changes.ForEach(f => {
          if (f.GameVersion != version) {
            f.GameVersion = version;
            f.Id          = null;
          }
        });

        allFiles.AddRange(changes.OrderBy(f => f.Filename));

        allFiles.Sort(domainUpkfileComparison);

        await scanUpkFiles(changes);
      }

      List<DomainUpkFile> adds = (from row1 in localFiles
                                  join row2 in remoteFiles on row1.GameFilename.ToLowerInvariant() equals row2.GameFilename.ToLowerInvariant() into fileGroup
                                  from sub  in fileGroup.DefaultIfEmpty()
                                 where sub == null
                                select row1).ToList();

      if (adds.Any()) {
        allFiles.AddRange(adds.OrderBy(f => f.Filename));

        allFiles.Sort(domainUpkfileComparison);

        await scanUpkFiles(adds);
      }

      viewModel.AllTypes = new ObservableCollection<string>(allFiles.SelectMany(f => f.ExportTypes).Distinct().OrderBy(s => s));

      // ReSharper disable once PossibleNullReferenceException
      allFiles.ForEach(f => { f.ModdedFiles.AddRange(mods.Where(mf => Path.GetFileName(mf.GameFilename) == Path.GetFileName(f.GameFilename)
                                                                   && Path.GetDirectoryName(mf.GameFilename).StartsWith(Path.GetDirectoryName(f.GameFilename)))); });

      filterFiles();

      progress.IsComplete = true;

      messenger.Send(progress);

      messenger.Send(new FileListingLoadedMessage { Allfiles = allFiles });
    }

    private static int domainUpkfileComparison(DomainUpkFile left, DomainUpkFile right) {
      return String.Compare(left.Filename, right.Filename, StringComparison.CurrentCultureIgnoreCase);
    }

    private async Task<List<DomainUpkFile>> loadGameFiles(int version) {
      List<DomainUpkFile> files = new List<DomainUpkFile>();

      if (String.IsNullOrEmpty(settings.PathToGame)) return files;

      try {
        await repository.LoadDirectoryRecursiveFlat(files, version, settings.PathToGame, settings.PathToGame, "*.upk");
      }
      catch(Exception ex) {
        messenger.Send(new ApplicationErrorMessage { ErrorMessage = ex.Message, Exception = ex });
      }

      return files;
    }

    private async void onFileEntityViewModelChanged(object sender, PropertyChangedEventArgs e) {
      FileViewEntity file = sender as FileViewEntity;

      if (file == null) return;

      switch(e.PropertyName) {
        case "IsSelected": {
          if (file.IsSelected) {
            viewModel.Files.Where(f => f != file).ForEach(f => f.IsSelected = false);

            messenger.Send(new FileLoadingMessage());

            DomainUpkFile upkFile = allFiles.Single(f => f.Id == file.Id);

            if (upkFile.Header == null) {
              await loadUpkFile(file, upkFile);

              if (file.IsErrored) return;

//            await repository.SaveUpkFile(upkFile.Header, $@"V:\{upkFile.Filename}");
            }

            upkFile.LastAccess = DateTime.Now;

            messenger.Send(new FileLoadedMessage { FileViewEntity = file, File = upkFile });
          }

          break;
        }
        default: {
          break;
        }
      }
    }

    private void onViewModelPropertyChanged(object sender, PropertyChangedEventArgs e) {
      switch(e.PropertyName) {
        case "IsShowFilesWithType":
        case "SelectedType": {
          filterFiles();

          break;
        }
        default: {
          break;
        }
      }
    }

    private void onMenuViewModelPropertyChanged(object sender, PropertyChangedEventArgs e) {
      switch(e.PropertyName) {
        case "IsSkipProperties": {
          if (menuViewModel.IsSkipProperties) menuViewModel.IsSkipParsing = true;

          break;
        }
        case "IsSkipParsing": {
          if (!menuViewModel.IsSkipParsing) menuViewModel.IsSkipProperties = false;

          break;
        }
        default: {
          break;
        }
      }
    }

    private void filterFiles() {
      List<DomainUpkFile> selectedFiles;

      if (viewModel.IsShowFilesWithType) {
        allFiles.ForEach(f => { f.ContainsTargetObject = f.ExportTypes.Any(t => t.Equals(viewModel.SelectedType, StringComparison.CurrentCultureIgnoreCase)); });

        selectedFiles = allFiles.Where(f => f.ContainsTargetObject).ToList();
      }
      else selectedFiles = allFiles;

      List<FileViewEntity> fileEntities = mapper.Map<List<FileViewEntity>>(selectedFiles);

      fileEntities.ForEach(fe => fe.PropertyChanged += onFileEntityViewModelChanged);

      viewModel.Files.ForEach(fe => fe.PropertyChanged -= onFileEntityViewModelChanged);

      viewModel.Files.Clear();

      viewModel.Files.AddRange(fileEntities);
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

    private async Task scanUpkFiles(List<DomainUpkFile> upkFiles) {
      LoadProgressMessage message = new LoadProgressMessage { Text = "Scanning UPK Files", Current = 0, Total = upkFiles.Count };

      List<DomainUpkFile> saveCache = new List<DomainUpkFile>();

      foreach(DomainUpkFile file in upkFiles) {
        FileViewEntity fileEntity = viewModel.Files.SingleOrDefault(fe => fe.Id == file.Id) ?? mapper.Map<FileViewEntity>(file);

        message.Current++;
        message.StatusText = Path.Combine(settings.PathToGame, file.GameFilename);

        messenger.Send(message);

        await scanUpkFile(fileEntity, file);

        file.FileSize    = file.Header.FileSize;
        file.ExportTypes = new List<string>(file.Header.ExportTable.Select(e => e.TypeReferenceNameIndex.Name).Distinct().OrderBy(s => s));

        fileEntity.FileSize    = file.Header.FileSize;
        fileEntity.ExportTypes = new ObservableCollection<string>(file.ExportTypes);

        file.Header = null;

        string path = Path.GetDirectoryName(file.GameFilename);

        if (path != null && path.ToLowerInvariant().EndsWith("cookedpc")) {
          saveCache.Add(file);

          if (saveCache.Count == 50) {
            remoteRepository.SaveUpkFile(saveCache).FireAndForget();

            saveCache.Clear();
          }
        }
      }

      if (saveCache.Any()) remoteRepository.SaveUpkFile(saveCache).FireAndForget();

      message.IsComplete = true;

      messenger.Send(message);
    }

    private async Task scanUpkFile(FileViewEntity file, DomainUpkFile upkFile) {
      try {
        upkFile.Header = await repository.LoadUpkFile(Path.Combine(settings.PathToGame, upkFile.GameFilename));

        await Task.Run(() => upkFile.Header.ReadHeaderAsync(null));

        file.IsErrored = false;
      }
      catch(Exception ex) {
        file.IsErrored = true;

        messenger.Send(new ApplicationErrorMessage { HeaderText = "Error Scanning UPK File.", ErrorMessage = $"Scanning {upkFile.GameFilename}", Exception = ex });
      }
    }

    private async Task exportFileObjects(List<DomainUpkFile> files) {
      LoadProgressMessage message = new LoadProgressMessage { Text = "Exporting...", Total = files.Count };

      foreach(DomainUpkFile file in files) {
        FileViewEntity fileEntity = viewModel.Files.Single(fe => fe.Id == file.Id);

        string directory = Path.Combine(settings.ExportPath, Path.GetDirectoryName(file.GameFilename), Path.GetFileNameWithoutExtension(file.GameFilename));

        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

        DomainHeader header = file.Header;

        if (header == null) {
          try {
            header = await repository.LoadUpkFile(Path.Combine(settings.PathToGame, file.GameFilename));

            await Task.Run(() => header.ReadHeaderAsync(null));
          }
          catch(Exception ex) {
            messenger.Send(new ApplicationErrorMessage { HeaderText = "Error Loading UPK File", ErrorMessage = $"Filename: {file.GameFilename}", Exception = ex });

            fileEntity.IsErrored = true;

            continue;
          }
        }

        message.Current++;

        foreach(DomainExportTableEntry export in header.ExportTable) {
          if (export.DomainObject == null) {
            try {
              await export.ParseDomainObject(header, false, false);
            }
            catch(Exception ex) {
              messenger.Send(new ApplicationErrorMessage { HeaderText = "Error Parsing Object", ErrorMessage = $"Filename: {header.Filename}\nExport Name: {export.NameTableIndex.Name}\nType: {export.TypeReferenceNameIndex.Name}", Exception = ex });

              fileEntity.IsErrored = true;

              continue;
            }
          }

          if (!export.DomainObject.IsExportable) continue;

          string filename = Path.Combine(directory, $"{export.NameTableIndex.Name}{export.DomainObject.FileExtension}");

          message.StatusText = filename;

          messenger.Send(message);

          await export.DomainObject.SaveObject(filename);
        }

        file.Header = null;
      }

      message.IsComplete = true;
      message.StatusText = null;

      messenger.Send(message);
    }

    #endregion Private Methods

  }

}
