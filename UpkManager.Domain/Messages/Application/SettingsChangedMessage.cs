using UpkManager.Domain.Models;


namespace UpkManager.Domain.Messages.Application {

  public class SettingsChangedMessage {

    public DomainUpkManagerSettings Settings { get; set; }

  }

}
