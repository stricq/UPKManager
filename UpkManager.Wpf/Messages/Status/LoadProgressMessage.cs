using STR.Common.Messages;


namespace UpkManager.Wpf.Messages.Status {

  internal sealed class LoadProgressMessage : MessageBase {

    #region Properties

    public string Text { get; set; }

    public int Current { get; set; }

    public double Total { get; set; }

    public string StatusText { get; set; }

    public bool IsComplete { get; set; }

    public bool IsLocalMode { get; set; }

    #endregion Properties

  }

}
