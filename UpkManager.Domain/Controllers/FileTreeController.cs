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
using UpkManager.Domain.Messages.FileHeader;
using UpkManager.Domain.Models;
using UpkManager.Domain.ViewModels;


namespace UpkManager.Domain.Controllers {

  [Export(typeof(IController))]
  public class FileTreeController : IController {

    #region Private Fields

    private readonly FileTreeViewModel viewModel;
    private readonly MainMenuViewModel menuViewModel;

    private readonly IMessenger messenger;

    private readonly IUpkFileRepository repository;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public FileTreeController(FileTreeViewModel ViewModel, MainMenuViewModel MenuViewModel, IMessenger Messenger, IUpkFileRepository Repository) {
          viewModel = ViewModel;
      menuViewModel = MenuViewModel;

      messenger = Messenger;

      repository = Repository;

      registerMessages();
      registerCommands();
    }

    #endregion Constructor

    #region Messages

    private void registerMessages() {
      messenger.RegisterAsync<ApplicationLoadedMessage>(this, onApplicationLoaded);
    }

    private async Task onApplicationLoaded(ApplicationLoadedMessage message) {
      List<UpkFileViewModel> files = new List<UpkFileViewModel>();

      await loadDirectoryAsync(files, @"V:\Games\BnS\contents");

      viewModel.Files.Clear();

      viewModel.Files.AddRange(files.OrderBy(f => f.FullFilename));
    }

    #endregion Messages

    #region Commands

    private void registerCommands() {
      menuViewModel.ScanUpkFiles = new RelayCommandAsync(onScanUpkFilesExecute);
    }

    private async Task onScanUpkFilesExecute() {
      await scanUpkFiles();
    }

    #endregion Commands

    #region Private Methods

    private async Task loadDirectoryAsync(List<UpkFileViewModel> parent, string path) {
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
        List<UpkFileViewModel> dirs = dirInfos.Select(dir => new UpkFileViewModel { FullFilename = dir.FullName }).ToList();

        foreach(UpkFileViewModel upkFile in dirs.ToList()) {
          List<UpkFileViewModel> children = new List<UpkFileViewModel>();

          await loadDirectoryAsync(children, upkFile.FullFilename);

          if (children.Count == 0) dirs.Remove(upkFile);
          else parent.AddRange(children);
        }
      }

      try {
        FileInfo[] files = await Task.Run(() => dirInfo.GetFiles("*.upk"));

        if (files.Length > 0) {
          List<UpkFileViewModel> upkFiles = files.Select(f => new UpkFileViewModel { FullFilename = f.FullName, FileSize = f.Length }).ToList();

          upkFiles.ForEach(d => d.PropertyChanged += onUpkFileViewModelChanged);

          parent.AddRange(upkFiles);
        }
      }
      catch(Exception ex) {
        messenger.Send(new ApplicationErrorMessage { ErrorMessage = ex.Message, Exception = ex });
      }
    }

    private async void onUpkFileViewModelChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
      UpkFileViewModel upkFile = sender as UpkFileViewModel;

      if (upkFile == null) return;

      switch(e.PropertyName) {
        case "IsSelected": {
          if (upkFile.IsSelected && upkFile.FileSize > 0) await messenger.SendAsync(new FileHeaderSelectedMessage { FullFilename = upkFile.FullFilename });

          break;
        }
        default: {
          break;
        }
      }
    }

    private async Task scanUpkFiles() {
      List<UpkFileViewModel> upkFiles = viewModel.Files.ToList();

      LoadProgressMessage message = new LoadProgressMessage { Text = "Scanning UPK Files", Current = 0, Total = upkFiles.Count };

      foreach(UpkFileViewModel upkFile in upkFiles) {
        DomainHeader header = new DomainHeader { FullFilename = upkFile.FullFilename };

        message.Current   += 1;
        message.StatusText = upkFile.FullFilename;

        messenger.Send(message);

        try {
          await repository.LoadAndParseUpk(header, true, true, null);
        }
        catch(Exception ex) {
          messenger.Send(new ApplicationErrorMessage { ErrorMessage = "Error Scanning UPK File.", Exception = ex, HeaderText = "Scan Error" });
        }

        upkFile.ExportTypes.AddRange(header.ExportTable.Select(e => e.TypeName).Distinct().OrderBy(s => s));

        if (upkFile.ExportTypes.Any(t => t == ObjectType.Texture2D.ToString())) upkFile.SelectedType = ObjectType.Texture2D.ToString();
      }

      message.IsComplete = true;

      messenger.Send(message);
    }

    #endregion Private Methods

  }

}
