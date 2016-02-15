using STR.Common.Messages;

using UpkManager.Domain.Models;


namespace UpkManager.Domain.Messages.FileHeader {

  public class FileHeaderSelectedMessage : MessageBase {

    public FileHeaderSelectedMessage() : base(true) { }

    public DomainUpkFile File { get; set; }

  }

}
