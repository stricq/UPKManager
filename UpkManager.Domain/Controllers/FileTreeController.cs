using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using STR.Common.Extensions;
using STR.Common.Messages;

using STR.MvvmCommon;
using STR.MvvmCommon.Contracts;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Contracts;
using UpkManager.Domain.Messages.Application;
using UpkManager.Domain.Messages.FileHeader;
using UpkManager.Domain.Models;
using UpkManager.Domain.ViewModels;


namespace UpkManager.Domain.Controllers {

  [Export(typeof(IController))]
  public class FileTreeController : IController {

    #region Private Fields

    private DomainUpkManagerSettings settings;

    private readonly FileTreeViewModel viewModel;
    private readonly MainMenuViewModel menuViewModel;

    private readonly IMessenger messenger;

    private readonly IUpkFileRepository repository;

    private readonly IUpkFileRemoteRepository remoteRepository;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public FileTreeController(FileTreeViewModel ViewModel, MainMenuViewModel MenuViewModel, IMessenger Messenger, IUpkFileRepository Repository, IUpkFileRemoteRepository RemoteRepository) {
          viewModel = ViewModel;
      menuViewModel = MenuViewModel;

      messenger = Messenger;

      repository = Repository;

      remoteRepository = RemoteRepository;

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

      if (String.IsNullOrEmpty(settings.PathToGame)) return;

      viewModel.Files.Clear();

      LoadProgressMessage progress = new LoadProgressMessage { Text = "Loading Game Files..." };

      messenger.Send(progress);

      List<DomainUpkFile> localFiles = await loadGameFiles();

      progress.Text = "Loading Remote Database...";

      messenger.Send(progress);

      List<DomainUpkFile> remoteFiles = await remoteRepository.LoadUpkFiles();

      List<DomainUpkFile> matches = (from row1 in localFiles
                                     join row2 in remoteFiles on row1.GameFilename.ToLowerInvariant() equals row2.GameFilename.ToLowerInvariant()
                                    where row1.FileSize == row2.FileSize
                                   select row2).ToList();

      if (matches.Any()) {
        matches.ForEach(d => d.PropertyChanged += onUpkFileViewModelChanged);

        viewModel.Files.AddRange(matches.OrderBy(f => f.Filename));
      }

      List<DomainUpkFile> mods = (from row1 in localFiles
                                  join row2 in remoteFiles on row1.GameFilename.ToLowerInvariant() equals row2.GameFilename.ToLowerInvariant()
                                 where row1.FileSize != row2.FileSize
                                select row2).ToList();

      if (mods.Any()) {
        mods.ForEach(d => d.PropertyChanged += onUpkFileViewModelChanged);

        viewModel.Files.AddRange(mods.OrderBy(f => f.Filename));

        viewModel.Files.Sort(f => f.Filename);

        await scanUpkFiles(mods);
      }

      List<DomainUpkFile> adds = (from row1 in localFiles
                                  join row2 in remoteFiles on row1.GameFilename.ToLowerInvariant() equals row2.GameFilename.ToLowerInvariant() into fileGroup
                                  from sub  in fileGroup.DefaultIfEmpty()
                                 where sub == null
                                select row1).ToList();

      if (adds.Any()) {
        adds.ForEach(d => d.PropertyChanged += onUpkFileViewModelChanged);

        viewModel.Files.AddRange(adds.OrderBy(f => f.Filename));

        viewModel.Files.Sort(f => f.Filename);

        await scanUpkFiles(adds);
      }

      progress.IsComplete = true;

      messenger.Send(progress);
    }

    private async void onSettingsChanged(SettingsChangedMessage message) {
      settings = message.Settings;

      await loadGameFiles();
    }

    #endregion Messages

    #region Commands

    private void registerCommands() {
      menuViewModel.ScanUpkFiles = new RelayCommandAsync(onScanUpkFilesExecute, canScanUpkFilesExecute);
    }

    private async Task onScanUpkFilesExecute() {
      if (viewModel.Files.Any()) await scanUpkFiles(viewModel.Files.ToList());
    }

    private bool canScanUpkFilesExecute() {
      return viewModel.Files.Any();
    }

    #endregion Commands

    #region Private Methods

    private async Task<List<DomainUpkFile>> loadGameFiles() {
      List<DomainUpkFile> files = new List<DomainUpkFile>();

      if (String.IsNullOrEmpty(settings.PathToGame)) return files;

      await loadDirectoryAsync(files, settings.PathToGame);

      return files;
    }

    private async Task loadDirectoryAsync(List<DomainUpkFile> parent, string path) {
      if (path.EndsWith("mod") || path.EndsWith("mods")) return;

      DirectoryInfo   dirInfo;
      DirectoryInfo[] dirInfos;

      try {
        dirInfo  = new DirectoryInfo(path);
        dirInfos = await Task.Run(() => dirInfo.GetDirectories());
      }
      catch(Exception) {
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

    private async void onUpkFileViewModelChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
      DomainUpkFile upkFile = sender as DomainUpkFile;

      if (upkFile == null) return;

      switch(e.PropertyName) {
        case "IsSelected": {
          if (upkFile.IsSelected && upkFile.FileSize > 0) await messenger.SendAsync(new FileHeaderSelectedMessage { FullFilename = Path.Combine(settings.PathToGame, upkFile.GameFilename) });

          break;
        }
        default: {
          break;
        }
      }
    }

    private async Task scanUpkFiles(List<DomainUpkFile> upkFiles) {
      LoadProgressMessage message = new LoadProgressMessage { Text = "Scanning UPK Files", Current = 0, Total = upkFiles.Count };

      foreach(DomainUpkFile upkFile in upkFiles) {
        DomainHeader header = new DomainHeader { FullFilename = Path.Combine(settings.PathToGame, upkFile.GameFilename) };

        message.Current   += 1;
        message.StatusText = Path.Combine(settings.PathToGame, upkFile.GameFilename);

        messenger.Send(message);

        await scanUpkFile(header);

        upkFile.ExportTypes.AddRange(header.ExportTable.Select(e => e.TypeName).Distinct().OrderBy(s => s));

        await remoteRepository.SaveUpkFile(upkFile);
      }

      message.IsComplete = true;

      messenger.Send(message);
    }

    private async Task scanUpkFile(DomainHeader header) {
      try {
        await repository.LoadAndParseUpk(header, true, true, null);
      }
      catch(Exception ex) {
        messenger.Send(new ApplicationErrorMessage { ErrorMessage = "Error Scanning UPK File.", Exception = ex, HeaderText = "Scan Error" });
      }
    }

    #endregion Private Methods

  }

}
