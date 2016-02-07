using STR.Common.Messages;


namespace UpkManager.Domain.Messages.FileHeader {

  public class FileHeaderSelectedMessage : MessageBase {

    public FileHeaderSelectedMessage() : base(true) { }

    public string FullFilename { get; set; }

  }

}
