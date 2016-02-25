using STR.Common.Messages;

using UpkManager.Wpf.ViewEntities;


namespace UpkManager.Wpf.Messages.Application {

  public class AppLoadedMessage : ApplicationLoadedMessage {

    public SettingsViewEntity Settings { get; set; }

  }

}
