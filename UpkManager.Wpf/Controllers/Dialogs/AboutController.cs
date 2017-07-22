using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Navigation;

using STR.DialogView.Domain.Messages;

using STR.MvvmCommon;
using STR.MvvmCommon.Contracts;

using UpkManager.Wpf.Messages.Application;
using UpkManager.Wpf.ViewModels.Dialogs;

using DialogNames = UpkManager.Wpf.Constants.DialogNames;


namespace UpkManager.Wpf.Controllers.Dialogs {

  [Export(typeof(IController))]
  public sealed class AboutController : IController {

    #region Private Fields

    private readonly AboutDialogViewModel viewModel;

    private readonly IMessenger messenger;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public AboutController(AboutDialogViewModel ViewModel, IMessenger Messenger) {
      viewModel = ViewModel;

      messenger = Messenger;
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
      messenger.Register<AboutMessage>(this, onAboutMessage);
    }

    private void onAboutMessage(AboutMessage message) {
      messenger.Send(new OpenDialogMessage { Name = DialogNames.About });
    }

    #endregion Messages

    #region Commands

    private void registerCommands() {
      viewModel.Navigate = new RelayCommand<RequestNavigateEventArgs>(onNavigateExecute);

      viewModel.Ok = new RelayCommand(onOkExecute);
    }

    private static void onNavigateExecute(RequestNavigateEventArgs args) {
      try {
        Process.Start(new ProcessStartInfo(args.Uri.AbsoluteUri));
      }
      catch(Exception) {
        // ignored
      }
    }

    private void onOkExecute() {
      messenger.Send(new CloseDialogMessage());
    }

    #endregion Commands

  }

}
