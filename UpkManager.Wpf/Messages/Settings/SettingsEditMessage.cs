using System;

using STR.Common.Messages;

using UpkManager.Wpf.ViewEntities;


namespace UpkManager.Wpf.Messages.Settings {

  public class SettingsEditMessage : MessageBase {

    public bool IsCancel { get; set; }

    public SettingsDialogViewEntity Settings { get; set; }

    public Action<SettingsEditMessage> Callback { get; set; }

  }

}
