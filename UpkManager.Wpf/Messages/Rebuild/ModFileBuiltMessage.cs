using STR.Common.Messages;

using UpkManager.Domain.Models;


namespace UpkManager.Wpf.Messages.Rebuild {

  public class ModFileBuiltMessage : MessageBase {

    public DomainUpkFile UpkFile { get; set; }

  }

}
