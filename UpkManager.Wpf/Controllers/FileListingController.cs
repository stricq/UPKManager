using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using STR.Common.Extensions;
using STR.Common.Messages;

using STR.DialogView.Domain.Messages;

using STR.MvvmCommon;
using STR.MvvmCommon.Contracts;

using UpkManager.Dds;
using UpkManager.Dds.Constants;

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
  public sealed class FileListingController : IController {

    #region Private Fields

    private bool isLoadInProgress;
    private bool isScanInProgress;

    private string oldPathToGame;

    private string locale;

    private CancellationTokenSource filterTokenSource;
    private CancellationTokenSource remoteTokenSource;

    private DomainSettings settings;

    private DomainVersion version;

    private DomainUpkFile selectedFile;

    private List<FileViewEntity> filesWithType;

    private readonly List<DomainUpkFile>  allFiles;
    private readonly List<FileViewEntity> allFileEntities;

    private readonly FileListingViewModel  viewModel;
    private readonly NotesViewModel   notesViewModel;
    private readonly MainMenuViewModel menuViewModel;

    private readonly IMessenger messenger;
    private readonly IMapper    mapper;

    private readonly IUpkFileRepository repository;

    private readonly IUpkFileRemoteRepository remoteRepository;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public FileListingController(FileListingViewModel ViewModel, NotesViewModel NotesViewModel, MainMenuViewModel MenuViewModel, IMessenger Messenger, IMapper Mapper, IUpkFileRepository Repository, IUpkFileRemoteRepository RemoteRepository) {
           viewModel = ViewModel;
      notesViewModel = NotesViewModel;
       menuViewModel = MenuViewModel;

      messenger = Messenger;
         mapper = Mapper;

      repository = Repository;

      remoteRepository = RemoteRepository;

      viewModel.Files = new ObservableCollection<FileViewEntity>();

      viewModel.FilterText = String.Empty;

          viewModel.PropertyChanged += onViewModelPropertyChanged;
      menuViewModel.PropertyChanged += onMenuViewModelPropertyChanged;

      allFiles = new List<DomainUpkFile>();

      allFileEntities = new List<FileViewEntity>();
    }

    #endregion Constructor

    #region IController Implementation

    public async Task InitializeAsync() {
      registerMessages();
      registerCommands();

      await Task.CompletedTask;
    }

    public int InitializePriority { get; } = 100;

    #endregion IController Implementation

    #region Messages

    private void registerMessages() {
      messenger.RegisterAsync<AppLoadedMessage>(this, onApplicationLoaded);

      messenger.Register<ApplicationClosingMessage>(this, onApplicationClosing);

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

    private void onApplicationClosing(ApplicationClosingMessage message) {
      resetToken(ref remoteTokenSource);

      remoteRepository.Shutdown();
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
      notesViewModel.SaveNotes = new RelayCommandAsync(onSaveNotesExecute, canSaveNotesExecute);

      menuViewModel.ReloadFiles = new RelayCommandAsync(onReloadFilesExecute, canReloadFilesExecute);

      menuViewModel.ExportFiles = new RelayCommandAsync(onExportFilesExecute, canExportFilesExecute);

      menuViewModel.SelectAllFiles   = new RelayCommand(onSelectAllFilesExecute,   canSelectAllFilesExecute);
      menuViewModel.DeselectAllFiles = new RelayCommand(onDeselectAllFilesExecute, canDeselectAllFilesExecute);

      menuViewModel.ScanUpkFiles = new RelayCommand(onScanUpkFilesExecute, canScanUpkFilesExecute);
    }

    #region SaveNotes Command

    private bool canSaveNotesExecute() {
      return String.Compare(notesViewModel.SelectedFile?.Notes, selectedFile?.Notes, StringComparison.CurrentCultureIgnoreCase) != 0 && !menuViewModel.IsOfflineMode;
    }

    private async Task onSaveNotesExecute() {
      selectedFile.Notes = notesViewModel.SelectedFile.Notes;

      await remoteRepository.SaveUpkFile(selectedFile);
    }

    #endregion SaveNotes Command

    #region ReloadFiles Command

    private bool canReloadFilesExecute() {
      return !isLoadInProgress && !isScanInProgress;
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
      return allFiles.Any() && !isLoadInProgress && !isScanInProgress;
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
      isLoadInProgress = true;

      selectedFile = null;

      notesViewModel.SelectedFile = null;

      messenger.SendUi(new FileLoadingMessage());

      allFileEntities.ForEach(fe => fe.PropertyChanged -= onFileEntityViewModelChanged);

      allFileEntities.Clear();

      allFiles.Clear();

      viewModel.Files.Clear();

      if (String.IsNullOrEmpty(settings.PathToGame)) return;

      LoadProgressMessage progress = new LoadProgressMessage { Text = "Loading Game Files..." };

      messenger.Send(progress);

      try {
        version = await repository.GetGameVersion(settings.PathToGame);
      }
      catch(FileNotFoundException) {
        messenger.Send(new MessageBoxDialogMessage { Header = "Game Version File Not Found", Message = "The ..\\bin\\Version.ini file could not be found.\n\nPlease ensure your settings are pointed to the 'contents' directory.", HasCancel = false });

        progress.IsComplete = true;

        messenger.Send(progress);

        isLoadInProgress = false;

        return;
      }

      try {
        locale  = await repository.GetGameLocale(settings.PathToGame);
      }
      catch(FileNotFoundException) {
        locale = CultureInfo.CurrentCulture.EnglishName.ToLowerInvariant();
      }

      List<DomainUpkFile> localFiles = await loadGameFiles();

      if (!localFiles.Any()) {
        progress.IsComplete = true;

        messenger.Send(progress);

        isLoadInProgress = false;

        return;
      }

      localFiles.ForEach(f => {
        f.CurrentVersion = version;
        f.CurrentLocale  = locale;
      });

      List<DomainUpkFile> mods = (from row in localFiles
                                   let path = Path.GetDirectoryName(row.GameFilename)
                                 where path != null
                                    && !path.ToLowerInvariant().EndsWith("cookedpc", StringComparison.CurrentCultureIgnoreCase)
                                select row).ToList();

      localFiles.RemoveAll(f => mods.Contains(f));

      progress.Text = "Loading Remote Database...";

      messenger.Send(progress);

      List<DomainUpkFile> remoteFiles = new List<DomainUpkFile>();

      string message = "No files returned from repository.";

      CancellationToken token = resetToken(ref remoteTokenSource);

      bool loadError = false;

      try {
        if (!menuViewModel.IsOfflineMode) remoteFiles = await remoteRepository.LoadUpkFiles(token);
      }
      catch(Exception ex) {
        message = ex.Message;

        remoteFiles = new List<DomainUpkFile>();

        loadError = true;
      }

      if ((loadError || token.IsCancellationRequested) && !remoteFiles.Any()) {
        if (loadError) {
          messenger.Send(new MessageBoxDialogMessage { Header = "Error Received from Remote Database", Message = $"The remote database returned an error.  Please try again in a few minutes.\n\n{message}\n\nThe program will continue using local files only.  Saving of file notes will be disabled.", HasCancel = false });
        }

        progress.IsLocalMode = true;

        menuViewModel.IsOfflineMode = true;

        viewModel.IsShowFilesWithType = false;
      }

      remoteFiles.ForEach(f => {
        f.CurrentVersion = version;
        f.CurrentLocale  = locale;
      });

      List<DomainUpkFile> matches = (from row1 in localFiles
                                     join row2 in remoteFiles on new { row1.ContentsRoot, row1.Package } equals new { row2.ContentsRoot, row2.Package }
                                    where row2.Exports.Any(f => f.Locale == locale && f.Filehash == row1.Filehash)
                                      let a = row2.GameFilename = row1.GameFilename
                                   select row2).ToList();

      if (matches.Any()) allFiles.AddRange(matches.OrderBy(f => f.Filename));

      List<DomainUpkFile> changes = (from row1 in localFiles
                                     join row2 in remoteFiles on new { row1.ContentsRoot, row1.Package } equals new { row2.ContentsRoot, row2.Package }
                                    where row2.Exports.All(f => f.Locale != locale || f.Filehash != row1.Filehash)
                                      let a = row2.GameFilename = row1.GameFilename
                                      let b = row2.NewFilehash  = row1.Filehash
                                      let c = row2.NewLocale    = row1.CurrentLocale
                                   select row2).ToList();

      if (changes.Any()) {
        allFiles.AddRange(changes.OrderBy(f => f.Filename));

        allFiles.Sort(domainUpkfileComparison);

        await scanUpkFiles(changes);
      }

      List<DomainUpkFile> adds = (from row1 in localFiles
                                  join row2 in remoteFiles on new { row1.ContentsRoot, row1.Package } equals new { row2.ContentsRoot, row2.Package } into fileGroup
                                  from sub  in fileGroup.DefaultIfEmpty()
                                 where sub == null
                                select row1).ToList();

      if (adds.Any()) {
        allFiles.AddRange(adds.OrderBy(f => f.Filename));

        allFiles.Sort(domainUpkfileComparison);

        if (!menuViewModel.IsOfflineMode) await scanUpkFiles(adds);
        else adds.ForEach(f => f.Id = Guid.NewGuid().ToString());
      }

      viewModel.AllTypes = menuViewModel.IsOfflineMode ? new ObservableCollection<string>() : new ObservableCollection<string>(allFiles.SelectMany(f => f.GetCurrentExports().Types.Select(e => e.Name)).Distinct().OrderBy(s => s));

      allFiles.ForEach(f => { f.ModdedFiles.AddRange(mods.Where(mf =>  Path.GetFileName(mf.GameFilename) == Path.GetFileName(f.GameFilename)
                                                                   && (Path.GetDirectoryName(mf.GameFilename) ?? String.Empty).StartsWith(Path.GetDirectoryName(f.GameFilename) ?? String.Empty))); });

      allFileEntities.AddRange(mapper.Map<List<FileViewEntity>>(allFiles));

      allFileEntities.ForEach(fe => fe.PropertyChanged += onFileEntityViewModelChanged);

      showFileTypes();

      progress.IsComplete = true;

      messenger.Send(progress);

      messenger.SendUi(new FileListingLoadedMessage { Allfiles = allFiles });

      isLoadInProgress = false;
    }

    private static int domainUpkfileComparison(DomainUpkFile left, DomainUpkFile right) {
      return String.Compare(left.Filename, right.Filename, StringComparison.CurrentCultureIgnoreCase);
    }

    private async Task<List<DomainUpkFile>> loadGameFiles() {
      List<DomainUpkFile> files = new List<DomainUpkFile>();

      if (String.IsNullOrEmpty(settings.PathToGame)) return files;

      try {
        await repository.LoadDirectoryRecursiveFlat(files, settings.PathToGame, settings.PathToGame, "*.upk");
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
            notesViewModel.SelectedFile = file;

            viewModel.Files.Where(f => f != file).ForEach(f => f.IsSelected = false);

            messenger.Send(new FileLoadingMessage());

            DomainUpkFile upkFile = allFiles.First(f => f.Filename == file.Filename);

            selectedFile = upkFile;

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

    private void onViewModelPropertyChanged(object sender, PropertyChangedEventArgs e) {
      switch(e.PropertyName) {
        case "IsShowFilesWithType":
        case "SelectedType": {
          showFileTypes();

          break;
        }
        case "IsFilterFiles":
        case "FilterText": {
          Task.Run(() => filterFiles(viewModel.FilterText, resetToken(ref filterTokenSource))).FireAndForget();

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
        case "IsOfflineMode": {
          if (menuViewModel.IsOfflineMode) resetToken(ref remoteTokenSource);

          break;
        }
      }
    }

    private void showFileTypes() {
      filesWithType = null;

      if (viewModel.IsShowFilesWithType) {
        allFileEntities.ForEach(f => { f.ContainsTargetObject = f.ExportTypes.Any(t => t.Equals(viewModel.SelectedType, StringComparison.CurrentCultureIgnoreCase)); });

        List<FileViewEntity> selectedFiles = allFileEntities.Where(f => f.ContainsTargetObject).ToList();

        filesWithType = selectedFiles;
      }

      Task.Run(() => filterFiles(viewModel.FilterText, resetToken(ref filterTokenSource))).FireAndForget();
    }

    private void filterFiles(string filterText, CancellationToken token) {
      try {
        List<FileViewEntity> selectedFiles = filesWithType ?? allFileEntities;

        if (viewModel.IsFilterFiles && !String.IsNullOrEmpty(filterText)) {
          try {
            Regex regex = new Regex(filterText, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

            if (token.IsCancellationRequested) return;

            selectedFiles = selectedFiles.Where(f => regex.IsMatch(f.Filename) || regex.IsMatch(f.RootDirectory) || regex.IsMatch(f.GameLocale) || regex.IsMatch(f.GameVersion.ToString()) || regex.IsMatch(f.Notes ?? String.Empty)).ToList();
          }
          catch(ArgumentException) { }
        }

        if (token.IsCancellationRequested) return;

        viewModel.Files = new ObservableCollection<FileViewEntity>(selectedFiles);
      }
      catch(TaskCanceledException) { }
      catch(OperationCanceledException) { }
    }

    private static CancellationToken resetToken(ref CancellationTokenSource tokenSource) {
      tokenSource?.Cancel();

      tokenSource = new CancellationTokenSource();

      return tokenSource.Token;
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
      isScanInProgress = true;

      LoadProgressMessage message = new LoadProgressMessage { Text = "Scanning UPK Files", Current = 0, Total = upkFiles.Count };

      List<DomainUpkFile> saveCache = new List<DomainUpkFile>();

      foreach(DomainUpkFile file in upkFiles) {
        FileViewEntity fileEntity = viewModel.Files.SingleOrDefault(fe => fe.Filename == file.Filename) ?? mapper.Map<FileViewEntity>(file);

        message.Current++;
        message.StatusText = Path.Combine(settings.PathToGame, file.GameFilename);

        messenger.Send(message);

        DomainExportVersion bestVersion = file.GetCurrentExports() ?? new DomainExportVersion { Versions = new List<DomainVersion>() };

        if (bestVersion.Versions.Contains(version)) {
          //
          // This should never happen.  Toss an error to the ui to track it.
          //
          messenger.Send(new ApplicationErrorMessage { HeaderText = "Odd Situation", ErrorMessage = $"Tried to scan a file, but the current version was already accounted for. {file.GameFilename} {version}/{locale}", OpenErrorWindow = true });

          continue;
        }

        if (bestVersion.Versions.Any() && bestVersion.Filehash == file.NewFilehash) {
          bestVersion.Versions.Add(version);

          if (fileEntity.ExportTypes == null || !fileEntity.ExportTypes.Any()) fileEntity.ExportTypes = new ObservableCollection<string>(bestVersion.Types.Select(t => t.Name));
        }
        else {
          await scanUpkFile(fileEntity, file);

          List<DomainExportType> exports = new List<DomainExportType>();

          foreach(string type in file.Header.ExportTable.Select(e => e.TypeReferenceNameIndex.Name).Distinct().OrderBy(s => s)) {
            exports.Add(new DomainExportType {
              Name        = type,
              ExportNames = file.Header.ExportTable.Where(e => e.TypeReferenceNameIndex.Name == type).Select(e => e.NameTableIndex.Name).Distinct().OrderBy(s => s).ToList()
            });
          }

          file.Exports.Add(new DomainExportVersion { Versions = new List<DomainVersion> { version }, Locale = locale, Filesize = file.Header.FileSize, Filehash = file.NewFilehash ?? file.Filehash, Types = exports });

          fileEntity.FileSize    = file.Header.FileSize;
          fileEntity.ExportTypes = new ObservableCollection<string>(exports.Select(e => e.Name));

          file.Header = null;
        }

        string path = Path.GetDirectoryName(file.GameFilename);

        if (path != null && path.ToLowerInvariant().EndsWith("cookedpc")) {
          if (!menuViewModel.IsOfflineMode) saveCache.Add(file);

          if (saveCache.Count == 50) {
            remoteRepository.SaveUpkFile(saveCache.ToList()).FireAndForget();

            saveCache.Clear();
          }
        }
      }

      if (saveCache.Any()) remoteRepository.SaveUpkFile(saveCache.ToList()).FireAndForget();

      message.IsComplete = true;

      messenger.Send(message);

      isScanInProgress = false;
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

      int compressor = menuViewModel.IsCompressorClusterFit ? 0 : menuViewModel.IsCompressorRangeFit ? 1 : 2;

      int errorMetric = menuViewModel.IsErrorMetricPerceptual ? 0 : 1;

      DdsSaveConfig config = new DdsSaveConfig(FileFormat.Unknown, compressor, errorMetric, menuViewModel.IsWeightColorByAlpha, false);

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

          await export.DomainObject.SaveObject(filename, config);
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
