using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using STR.Common.Extensions;
using STR.Common.Messages;

using STR.MvvmCommon.Contracts;

using UpkManager.Domain.Messages.FileHeader;
using UpkManager.Domain.ViewModels;


namespace UpkManager.Domain.Controllers {

  [Export(typeof(IController))]
  public class FileTreeController : IController {

    #region Private Fields

    private readonly FileTreeViewModel viewModel;

    private readonly IMessenger messenger;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public FileTreeController(FileTreeViewModel ViewModel, IMessenger Messenger) {
      viewModel = ViewModel;

      messenger = Messenger;

      registerMessages();
    }

    #endregion Constructor

    #region Messages

    private void registerMessages() {
      messenger.RegisterAsync<ApplicationLoadedMessage>(this, onApplicationLoaded);
    }

    private async Task onApplicationLoaded(ApplicationLoadedMessage message) {
      await loadDirectoryAsync(null, @"V:\Games\BnS\contents");
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
          if (upkFile.IsSelected) await messenger.SendAsync(new FileHeaderSelectedMessage { FullFilename = upkFile.FullFilename });

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
