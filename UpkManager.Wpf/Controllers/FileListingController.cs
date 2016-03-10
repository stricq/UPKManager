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
using UpkManager.Domain.Models.Tables;

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

    private List<DomainUpkFile> allFiles;

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
    }

    private async Task onApplicationLoaded(AppLoadedMessage message) {
      settings = message.Settings;

      oldPathToGame = settings.PathToGame;

      viewModel.SelectedType        = ObjectType.Texture2D.ToString();
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

    #endregion Messages

    #region Commands

    private void registerCommands() {
      menuViewModel.ExportFiles = new RelayCommandAsync(onExportFilesExecute, canExportFilesExecute);

      menuViewModel.SelectAllFiles   = new RelayCommand(onSelectAllFilesExecute,   canSelectAllFilesExecute);
      menuViewModel.DeselectAllFiles = new RelayCommand(onDeselectAllFilesExecute, canDeselectAllFilesExecute);

      menuViewModel.ScanUpkFiles = new RelayCommand(onScanUpkFilesExecute, canScanUpkFilesExecute);
    }

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
      allFiles.Clear();

      viewModel.Files.Clear();

      if (String.IsNullOrEmpty(settings.PathToGame)) return;

      LoadProgressMessage progress = new LoadProgressMessage { Text = "Loading Game Files..." };

      messenger.Send(progress);

      List<DomainUpkFile> localFiles = await loadGameFiles();

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

      List<DomainUpkFile> remoteFiles = await remoteRepository.LoadUpkFiles();

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

      allFiles.ForEach(f => { f.ModdedFiles.AddRange(mods.Where(mf => Path.GetFileName(mf.GameFilename) == Path.GetFileName(f.GameFilename))); });

      filterFiles();

      progress.IsComplete = true;

      messenger.Send(progress);
    }

    private static int domainUpkfileComparison(DomainUpkFile left, DomainUpkFile right) {
      return String.Compare(left.Filename, right.Filename, StringComparison.CurrentCultureIgnoreCase);
    }

    private async Task<List<DomainUpkFile>> loadGameFiles() {
      List<DomainUpkFile> files = new List<DomainUpkFile>();

      if (String.IsNullOrEmpty(settings.PathToGame)) return files;

      await loadDirectoryAsync(files, settings.PathToGame);

      return files;
    }

    private async Task loadDirectoryAsync(List<DomainUpkFile> parent, string path) {
      DirectoryInfo   dirInfo;
      DirectoryInfo[] dirInfos;

      try {
        dirInfo  = new DirectoryInfo(path);
        dirInfos = await Task.Run(() => dirInfo.GetDirectories());
      }
      catch(Exception ex) {
        messenger.Send(new ApplicationErrorMessage { ErrorMessage = ex.Message, Exception = ex });

        return;
      }

      if (dirInfos.Length > 0) {
        List<DomainUpkFile> dirs = dirInfos.Select(dir => new DomainUpkFile { GameFilename = dir.FullName.Replace(settings.PathToGame, null) }).ToList();

        foreach(DomainUpkFile upkFile in dirs.ToList()) {
          List<DomainUpkFile> children = new List<DomainUpkFile>();

          await loadDirectoryAsync(children, Path.Combine(settings.PathToGame, upkFile.GameFilename));

          if (children.Count == 0) dirs.Remove(upkFile);
          else parent.AddRange(children);
        }
      }

      try {
        FileInfo[] files = await Task.Run(() => dirInfo.GetFiles("*.upk"));

        if (files.Length > 0) {
          List<DomainUpkFile> upkFiles = files.Select(f => new DomainUpkFile { GameFilename = f.FullName.Replace(settings.PathToGame, null), FileSize = f.Length }).ToList();

          parent.AddRange(upkFiles);
        }
      }
      catch(Exception ex) {
        messenger.Send(new ApplicationErrorMessage { ErrorMessage = ex.Message, Exception = ex });
      }
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

            if (upkFile.Header == null) await loadUpkFile(upkFile);

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

    private async Task loadUpkFile(DomainUpkFile file) {
      file.Header = await repository.LoadUpkFile(Path.Combine(settings.PathToGame, file.GameFilename));

      await Task.Run(() => file.Header.ReadHeaderAsync());

      file.IsErrored = file.Header.IsErrored;
    }

    private void onLoadProgress(DomainLoadProgress progress) {
      messenger.Send(mapper.Map<LoadProgressMessage>(progress));
    }

    private async Task scanUpkFiles(List<DomainUpkFile> fileEntities) {
      LoadProgressMessage message = new LoadProgressMessage { Text = "Scanning UPK Files", Current = 0, Total = fileEntities.Count };

      foreach(DomainUpkFile file in fileEntities) {
        message.Current   += 1;
        message.StatusText = Path.Combine(settings.PathToGame, file.GameFilename);

        messenger.Send(message);

        await scanUpkFile(file);

        file.FileSize  = file.Header.FileSize;

        file.FileSize  = file.Header.FileSize;
        file.IsErrored = file.Header.IsErrored;

        file.ExportTypes = new List<string>(file.Header.ExportTable.Select(e => e.TypeReferenceNameIndex.Name).Distinct().OrderBy(s => s));

        file.Header = null;

        string path = Path.GetDirectoryName(file.GameFilename);

        if (path != null && path.ToLowerInvariant().EndsWith("cookedpc")) await remoteRepository.SaveUpkFile(file);
      }

      filterFiles();

      message.IsComplete = true;

      messenger.Send(message);
    }

    private async Task scanUpkFile(DomainUpkFile file) {
      try {
        file.Header = await repository.LoadUpkFile(Path.Combine(settings.PathToGame, file.GameFilename));
      }
      catch(Exception ex) {
        messenger.Send(new ApplicationErrorMessage { ErrorMessage = "Error Scanning UPK File.", Exception = ex, HeaderText = "Scan Error" });
      }
    }

    private async Task exportFileObjects(List<DomainUpkFile> files) {
      LoadProgressMessage message = new LoadProgressMessage { Text = "Exporting...", Total = files.Count };

      foreach(DomainUpkFile file in files) {
        string directory = Path.Combine(settings.ExportPath, Path.GetDirectoryName(file.GameFilename), Path.GetFileNameWithoutExtension(file.GameFilename));

        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

        DomainHeader header = file.Header;

        if (header == null) {
          header = await repository.LoadUpkFile(Path.Combine(settings.PathToGame, file.GameFilename));

          await Task.Run(() => header.ReadHeaderAsync());
        }

        if (header.IsErrored) file.IsErrored = true;

        message.Current += 1;

        foreach(DomainExportTableEntry export in header.ExportTable) {
          if (export.DomainObject == null) {
            try {
              await export.ParseDomainObject(header, false, false);
            }
            catch(Exception ex) {
              messenger.Send(new ApplicationErrorMessage { HeaderText = "Error Parsing Object", ErrorMessage = $"Filename: {header.Filename}\nExport Name: {export.NameIndex.Name}, Type: {export.TypeReferenceNameIndex.Name}", Exception = ex });

              return;
            }
          }

          if (export.IsErrored || !export.DomainObject.IsExportable) continue;

          string filename = Path.Combine(directory, $"{export.NameIndex.Name}.dds");

          message.StatusText = filename;

          messenger.Send(message);

          await export.DomainObject.SaveObject(filename);
        }
      }

      message.IsComplete = true;
      message.StatusText = null;

      messenger.Send(message);
    }

    #endregion Private Methods

  }

}
