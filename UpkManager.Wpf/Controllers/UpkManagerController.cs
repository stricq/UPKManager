using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using AutoMapper;

using STR.Common.Messages;

using STR.MvvmCommon;
using STR.MvvmCommon.Contracts;

using UpkManager.Domain.Contracts;
using UpkManager.Domain.Models;

using UpkManager.Wpf.Messages.Application;
using UpkManager.Wpf.Messages.Settings;
using UpkManager.Wpf.Messages.Status;
using UpkManager.Wpf.ViewEntities;
using UpkManager.Wpf.ViewModels;


namespace UpkManager.Wpf.Controllers {

  [Export(typeof(IController))]
  public sealed class UpkManagerController : IController {

    #region Private Fields

    private bool isStartupComplete;

    private DateTime lastSave = DateTime.Now;

    private DomainSettings settings;

    private readonly UpkManagerViewModel   viewModel;
    private readonly MainMenuViewModel menuViewModel;

    private readonly IMessenger messenger;

    private readonly ISettingsRepository  settingsRepository;

    private readonly IMapper mapper;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public UpkManagerController(UpkManagerViewModel ViewModel, MainMenuViewModel MenuViewModel, IMessenger Messenger, ISettingsRepository SettingsRepository, IMapper Mapper) {
      if (Application.Current != null) Application.Current.DispatcherUnhandledException += onCurrentDispatcherUnhandledException;

      AppDomain.CurrentDomain.UnhandledException += onDomainUnhandledException;

      Dispatcher.CurrentDispatcher.UnhandledException += onCurrentDispatcherUnhandledException;

      TaskScheduler.UnobservedTaskException += onUnobservedTaskException;

      System.Windows.Forms.Application.ThreadException += onThreadException;

          viewModel =     ViewModel;
      menuViewModel = MenuViewModel;

      messenger = Messenger;

      settingsRepository  = SettingsRepository;

      mapper = Mapper;
    }

    #endregion Constructor

    #region IController Implementation

    public async Task InitializeAsync() {
      settings = await settingsRepository.LoadSettingsAsync();

      viewModel.Settings = mapper.Map<SettingsWindowViewEntity>(settings);

      registerMessages();
      registerCommands();
    }

    public int InitializePriority { get; } = 1000;

    #endregion IController Implementation

    #region Messages

    private void registerMessages() {
      messenger.RegisterAsync<StatusTickMessage>(this, onStatusTick);
    }

    private async Task onStatusTick(StatusTickMessage message) {
      if ((DateTime.Now - lastSave).Seconds > 3 && viewModel.Settings.AreSettingsChanged) {
        mapper.Map(viewModel.Settings, settings);

        await settingsRepository.SaveSettings(settings);

        viewModel.Settings.AreSettingsChanged = false;

        lastSave = DateTime.Now;
      }
    }

    #endregion Messages

    #region Commands

    private void registerCommands() {
      viewModel.Loaded = new RelayCommandAsync<RoutedEventArgs>(onLoadedExecuteAsync);

      viewModel.Closing = new RelayCommand<CancelEventArgs>(onClosingExecute);

      viewModel.SizeChanged = new RelayCommand<SizeChangedEventArgs>(onSizeChanged);

      menuViewModel.Settings = new RelayCommand(onSettingsExecute);
      menuViewModel.About    = new RelayCommand(onAboutExecute);
      menuViewModel.Exit     = new RelayCommand(onExitExecute);
    }

    private async Task onLoadedExecuteAsync(RoutedEventArgs args) {
      isStartupComplete = true;

      await messenger.SendAsync(new AppLoadedMessage { Settings = settings });
    }

    private void onClosingExecute(CancelEventArgs args) {
      ApplicationClosingMessage message = new ApplicationClosingMessage();

      messenger.Send(message);

      args.Cancel = message.Cancel;
    }

    private void onSizeChanged(SizeChangedEventArgs args) {
      viewModel.Settings.SplitterDistance = args.NewSize.Width + 7;
    }

    private void onSettingsExecute() {
      messenger.Send(new SettingsEditMessage { Settings = mapper.Map<SettingsDialogViewEntity>(settings), Callback = onSettingsEditResponse });
    }

    private void onAboutExecute() {
      messenger.Send(new AboutMessage());
    }

    private void onExitExecute() {
      ApplicationClosingMessage message = new ApplicationClosingMessage();

      messenger.Send(message);

      if (!message.Cancel) Application.Current.Shutdown();
    }

    #endregion Commands

    #region Private Methods

    private void onSettingsEditResponse(SettingsEditMessage message) {
      if (message.IsCancel) return;

      mapper.Map(message.Settings, settings);

      messenger.Send(new SettingsChangedMessage { Settings = settings });

      settingsRepository.SaveSettings(mapper.Map<DomainSettings>(settings));
    }

    private void onDomainUnhandledException(object sender, UnhandledExceptionEventArgs e) {
      Exception ex = e.ExceptionObject as Exception;

      if (ex != null) {
        if (e.IsTerminating) MessageBox.Show(ex.Message, "Fatal Domain Unhandled Exception");
        else messenger.SendUi(new ApplicationErrorMessage { ErrorMessage = "Domain Unhandled Exception", Exception = ex });
      }
    }

    private void onCurrentDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
      if (e.Exception != null) {
        if (isStartupComplete) {
          messenger.SendUi(new ApplicationErrorMessage { ErrorMessage = "Dispatcher Unhandled Exception", Exception = e.Exception });

          e.Handled = true;
        }
        else {
          e.Handled = true;

          MessageBox.Show(e.Exception.Message, "Fatal Dispatcher Exception");

          Application.Current.Shutdown();
        }
      }
    }

    private void onUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e) {
      if (e.Exception == null || e.Exception.InnerExceptions.Count == 0) return;

      foreach(Exception ex in e.Exception.InnerExceptions) {
        if (isStartupComplete) {
          messenger.SendUi(new ApplicationErrorMessage { ErrorMessage = "Unobserved Task Exception", Exception = ex });
        }
        else {
          MessageBox.Show(ex.Message, "Fatal Unobserved Task Exception");
        }
      }

      if (!isStartupComplete) Application.Current.Shutdown();

      e.SetObserved();
    }

    private void onThreadException(object sender, ThreadExceptionEventArgs e) {
      if (e.Exception == null) return;

      messenger.SendUi(new ApplicationErrorMessage { ErrorMessage = "Thread Exception", Exception = e.Exception });
    }

    #endregion Private Methods

  }

}
