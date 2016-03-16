

namespace UpkManager.Domain.Models {

  public abstract class DomainUpkBuilderBase {

    public int BuilderSize { get; set; }

    public int BuilderOffset { get; set; }

    public abstract int GetBuilderSize();

  }

}
