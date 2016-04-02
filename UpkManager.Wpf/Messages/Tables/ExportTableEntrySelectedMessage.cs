using STR.Common.Messages;

using UpkManager.Domain.Models.UpkFile.Tables;


namespace UpkManager.Wpf.Messages.Tables {

  public class ExportTableEntrySelectedMessage : MessageBase {

    public ExportTableEntrySelectedMessage() : base(true) { }

    public DomainExportTableEntry ExportTableEntry { get; set; }

  }

}
