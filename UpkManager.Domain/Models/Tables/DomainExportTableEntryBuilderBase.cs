

using System.Threading.Tasks;

using UpkManager.Domain.Helpers;


namespace UpkManager.Domain.Models.Tables {

  public abstract class DomainExportTableEntryBuilderBase : DomainObjectTableEntryBase {

    protected int BuilderSerialDataSize { get; set; }

    protected int BuilderSerialDataOffset { get; set; }

    public abstract int GetObjectSize(int CurrentOffset);

    public abstract Task<ByteArrayWriter> WriteObjectBuffer();

  }

}
