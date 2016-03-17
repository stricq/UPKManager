

namespace UpkManager.Domain.Models.Tables {

  public abstract class DomainExportTableEntryBuilderBase : DomainObjectTableEntryBase {

    protected int BuilderSerialDataSize { get; set; }

    protected int BuilderSerialDataOffset { get; set; }

  }

}
