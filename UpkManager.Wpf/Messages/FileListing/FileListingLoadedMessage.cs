using System.Collections.Generic;

using STR.Common.Messages;

using UpkManager.Domain.Models;


namespace UpkManager.Wpf.Messages.FileListing {

  public class FileListingLoadedMessage : MessageBase {

    public List<DomainUpkFile> Allfiles { get; set; }

  }

}
