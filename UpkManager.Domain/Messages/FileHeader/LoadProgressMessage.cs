using STR.Common.Messages;


namespace UpkManager.Domain.Messages.FileHeader {

  public class LoadProgressMessage : MessageBase {

    public string Text { get; set; }

    public double Current { get; set; }

    public double Total { get; set; }

    public bool IsComplete { get; set; }

  }

}
