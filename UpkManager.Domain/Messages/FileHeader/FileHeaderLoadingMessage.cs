using STR.Common.Messages;


namespace UpkManager.Domain.Messages.FileHeader {

  public class FileHeaderLoadingMessage : MessageBase {

    public string Filename { get; set; }

  }

}
