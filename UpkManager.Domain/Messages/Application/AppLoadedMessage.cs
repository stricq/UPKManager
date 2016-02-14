using STR.Common.Messages;

using UpkManager.Domain.Models;


namespace UpkManager.Domain.Messages.Application {

  public class AppLoadedMessage : ApplicationLoadedMessage {

    public DomainUpkManagerSettings Settings { get; set; }

  }

}
