using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using STR.Common.Extensions;
using STR.Common.Messages;

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

    private readonly IMessenger messenger;

    private readonly IUpkFileRepository repository;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public FileTreeController(FileTreeViewModel ViewModel, IMessenger Messenger, IUpkFileRepository Repository) {
      viewModel = ViewModel;

      messenger = Messenger;

      repository = Repository;

      registerMessages();
    }

    #endregion Constructor

    #region Messages

    private void registerMessages() {
      messenger.RegisterAsync<ApplicationLoadedMessage>(this, onApplicationLoaded);
    }

    private async Task onApplicationLoaded(ApplicationLoadedMessage message) {
      await loadDirectoryAsync(null, @"V:\Games\BnS\contents");

//    await scanUpkFiles();
    }

    #endregion Messages

    #region Private Methods

    private async Task loadDirectoryAsync(UpkFileViewModel parent, string path) {
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
          await loadDirectoryAsync(upkFile, upkFile.FullFilename);

          if (upkFile.Children.Count == 0) dirs.Remove(upkFile);
        }

        if (dirs.Count > 0) {
          dirs.ForEach(d => d.PropertyChanged += onUpkFileViewModelChanged);

          if (parent !=  null) parent.Children.AddRange(dirs.OrderBy(d => d.Filename));
          else viewModel.Files.AddRange(dirs.OrderBy(d => d.Filename));
        }
      }

      try {
        FileInfo[] files = await Task.Run(() => dirInfo.GetFiles("*.upk"));

        if (files.Length > 0) {
          List<UpkFileViewModel> upkFiles = files.Select(f => new UpkFileViewModel { FullFilename = f.FullName, FileSize = f.Length }).ToList();

          upkFiles.ForEach(d => d.PropertyChanged += onUpkFileViewModelChanged);

          if (parent != null) parent.Children.AddRange(upkFiles.OrderBy(d => d.Filename));
          else viewModel.Files.AddRange(upkFiles.OrderBy(d => d.Filename));
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
      List<UpkFileViewModel> upkFiles = viewModel.Files.Traverse(f => f.FileSize > 0).ToList();

      LoadProgressMessage message = new LoadProgressMessage { Text = "Scanning UPK Files", Current = 0, Total = upkFiles.Count };

      foreach(UpkFileViewModel upkFile in upkFiles) {
        DomainHeader header = new DomainHeader { FullFilename = upkFile.FullFilename };

        await repository.LoadAndParseUpk(header, true, true, null);

        if (header.ExportTable.Any(e => e.TypeName == ObjectType.Texture2D.ToString())) upkFile.HasTextures = "\u2713";

        message.Current += 1;

        messenger.Send(message);
      }

      message.IsComplete = true;

      messenger.Send(message);
    }

    #endregion Private Methods

  }

}
