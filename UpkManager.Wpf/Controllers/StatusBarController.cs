using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Threading;

using STR.MvvmCommon.Contracts;

using UpkManager.Wpf.Messages.FileListing;
using UpkManager.Wpf.Messages.Status;
using UpkManager.Wpf.ViewModels;


namespace UpkManager.Wpf.Controllers {

  [Export(typeof(IController))]
  public sealed class StatusBarController : IController {

    #region Private Fields

    private int lastCurrent;

    private readonly DispatcherTimer timer;

    private readonly TimeSpan oneSecond = TimeSpan.FromSeconds(1);

    private readonly StatusBarViewModel viewModel;

    private readonly IMessenger messenger;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public StatusBarController(StatusBarViewModel ViewModel, IMessenger Messenger) {
      messenger = Messenger;

      viewModel = ViewModel;

      timer = new DispatcherTimer();

      viewModel.JobProgressText = "Idle";
    }

    #endregion Constructor

    #region IController Implementation

    public async Task InitializeAsync() {
      timer.Tick    += onTimerTick;
      timer.Interval = oneSecond;

      timer.Start();

      registerMessages();

      await Task.CompletedTask;
    }

    public int InitializePriority { get; } = 500;

    #endregion IController Implementation

    #region Messages

    private void registerMessages() {
      messenger.Register<FileLoadingMessage>(this, onFileLoading);
      messenger.Register<FileLoadedMessage>(this, onFileLoaded);

      messenger.Register<LoadProgressMessage>(this, onLoadProgress);
    }

    private void onFileLoading(FileLoadingMessage message) {
      viewModel.StatusText = null;
    }

    private void onFileLoaded(FileLoadedMessage message) {
      viewModel.StatusText = message.File.Header.FullFilename;
    }

    private void onLoadProgress(LoadProgressMessage message) {
      if (message.IsComplete) {
        viewModel.JobProgressVisibility = false;
        viewModel.JobProgress           = 0.0;
        viewModel.JobProgressText       = "Idle";

        lastCurrent = 0;

        return;
      }

      viewModel.JobProgressVisibility = message.Total > 0;
      viewModel.JobProgressText       = message.Total > 0 ? $"{message.Text} [{message.Current:N0} / {message.Total:N0}]" : message.Text;

      if (message.Current == 0) lastCurrent = 0;

      if (message.Current > 0 && message.Total > 0 && message.Current > lastCurrent) {
        viewModel.JobProgress = message.Current / message.Total * 100.0;

        lastCurrent = message.Current;
      }

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
