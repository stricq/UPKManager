using System.Threading.Tasks;

using UpkManager.Domain.Helpers;


namespace UpkManager.Domain.Models.UpkFile {

  public abstract class DomainUpkBuilderBase {

    protected int BuilderSize { get; set; }

    protected int BuilderOffset { get; set; }

    public abstract int GetBuilderSize();

    public abstract Task WriteBuffer(ByteArrayWriter Writer, int CurrentOffset);

  }

}
