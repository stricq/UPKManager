using STR.Common.Messages;

using UpkManager.Domain.Models;


namespace UpkManager.Wpf.Messages.FileListing {

  public class FileLoadedMessage : MessageBase {

    public DomainUpkFile File { get; set; }

  }

}
