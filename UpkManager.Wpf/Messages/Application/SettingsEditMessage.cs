using System;

using STR.Common.Messages;

using UpkManager.Wpf.ViewEntities;


namespace UpkManager.Wpf.Messages.Application {

  public class SettingsEditMessage : MessageBase {

    public bool IsCancel { get; set; }

    public SettingsViewEntity Settings { get; set; }

    public Action<SettingsEditMessage> Callback { get; set; }

  }

}
