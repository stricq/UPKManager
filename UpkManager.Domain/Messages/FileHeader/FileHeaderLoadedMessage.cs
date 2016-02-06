using STR.Common.Messages;

using UpkManager.Domain.Models;


namespace UpkManager.Domain.Messages.FileHeader {

  public class FileHeaderLoadedMessage : MessageBase {

    public DomainHeader FileHeader { get; set; }

  }

}
