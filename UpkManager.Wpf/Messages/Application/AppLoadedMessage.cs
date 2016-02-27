using STR.Common.Messages;

using UpkManager.Domain.Models;


namespace UpkManager.Wpf.Messages.Application {

  public class AppLoadedMessage : ApplicationLoadedMessage {

    public DomainSettings Settings { get; set; }

  }

}
