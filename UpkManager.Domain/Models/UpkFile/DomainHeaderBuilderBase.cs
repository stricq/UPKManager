

namespace UpkManager.Domain.Models.UpkFile {

  public abstract class DomainHeaderBuilderBase : DomainUpkBuilderBase {

    protected int BuilderNameTableOffset { get; set; }

    protected int BuilderExportTableOffset { get; set; }

    protected int BuilderImportTableOffset { get; set; }

    protected int BuilderDependsTableOffset { get; set; }

  }

}
