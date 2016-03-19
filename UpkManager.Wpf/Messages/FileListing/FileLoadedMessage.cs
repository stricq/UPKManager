using STR.Common.Messages;

using UpkManager.Domain.Models;

using UpkManager.Wpf.ViewEntities;


namespace UpkManager.Wpf.Messages.FileListing {

  public class FileLoadedMessage : MessageBase {

    public FileLoadedMessage() : base(true) { }

    public FileViewEntity FileViewEntity { get; set; }

    public DomainUpkFile File { get; set; }

  }

}
