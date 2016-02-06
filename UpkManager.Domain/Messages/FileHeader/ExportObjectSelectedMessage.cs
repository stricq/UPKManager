using STR.Common.Messages;

using UpkManager.Domain.Models.Tables;


namespace UpkManager.Domain.Messages.FileHeader {

  public class ExportObjectSelectedMessage : MessageBase {

    public ExportObjectSelectedMessage() :  base(true) { }

    public DomainExportTableEntry ExportObject { get; set; }

  }

}
