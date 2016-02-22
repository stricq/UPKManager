using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Windows.Threading;

using STR.MvvmCommon.Contracts;

using UpkManager.Domain.Messages.FileHeader;
using UpkManager.Domain.Messages.Status;

using UpkManager.Wpf.ViewModels;


namespace UpkManager.Wpf.Controllers {

  [Export(typeof(IController))]
  public class StatusBarController : IController {

    #region Private Fields

    private readonly TimeSpan oneSecond = TimeSpan.FromSeconds(1);

    private readonly StatusBarViewModel viewModel;

    private readonly IMessenger messenger;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public StatusBarController(StatusBarViewModel ViewModel, IMessenger Messenger) {
      messenger = Messenger;

      viewModel = ViewModel;

      DispatcherTimer timer = new DispatcherTimer();

      timer.Tick    += onTimerTick;
      timer.Interval = oneSecond;

      timer.Start();

      viewModel.JobProgressText = "Idle";

      registerMessages();
    }

    #endregion Constructor

    #region Messages

    private void registerMessages() {
      messenger.Register<FileHeaderLoadingMessage>(this, onFileHeaderLoading);

      messenger.Register<LoadProgressMessage>(this, onLoadProgress);
    }

    private void onFileHeaderLoading(FileHeaderLoadingMessage message) {
      viewModel.StatusText = message.Filename;
    }

    private void onLoadProgress(LoadProgressMessage message) {
      if (message.IsComplete) {
        viewModel.JobProgressVisibility = false;
        viewModel.JobProgress           = 0.0;
        viewModel.JobProgressText       = "Idle";

        return;
      }

      viewModel.JobProgressVisibility = message.Total > 0;
      viewModel.JobProgressText       = message.Total > 0 ? $"{message.Text} [{message.Current} / {message.Total}]" : message.Text;

      if (message.Current > 0 && message.Total > 0) viewModel.JobProgress = message.Current / message.Total * 100.0;

      if (message.StatusText != null) viewModel.StatusText = message.StatusText;
    }

    #endregion Messages

    #region Private Methods

    private async void onTimerTick(object sender, EventArgs e) {
      await messenger.SendAsync(new StatusTickMessage());

      using(Process process = Process.GetCurrentProcess()) {
        viewModel.Memory = process.WorkingSet64 / 1024.0 / 1024.0;
      }
    }

    #endregion Private Methods

  }

}
