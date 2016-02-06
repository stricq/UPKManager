using System.ComponentModel.Composition;

using STR.Common.Messages;
using STR.MvvmCommon.Contracts;


namespace UpkManager.Domain.Controllers {

  [Export(typeof(IController))]
  public class FileTreeController : IController {

    #region Private Fields

    private readonly IMessenger messenger;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public FileTreeController(IMessenger Messenger) {
      messenger = Messenger;

      registerMessages();
    }

    #endregion Constructor

    #region Messages

    private void registerMessages() {
      messenger.Register<ApplicationLoadedMessage>(this, onApplicationLoaded);
    }

    private void onApplicationLoaded(ApplicationLoadedMessage message) {

    }

    #endregion Messages

  }

}
