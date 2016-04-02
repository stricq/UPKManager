using STR.Common.Messages;

using UpkManager.Domain.Models.UpkFile.Properties;


namespace UpkManager.Wpf.Messages.Tables {

  public class PropertySelectedMessage : MessageBase {

    public PropertySelectedMessage() : base(true) { }

    public DomainProperty Property { get; set; }

  }

}
