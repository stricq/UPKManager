using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using STR.Common.Contracts;


namespace UpkManager.Entities {

  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  public class UpkFile : ModelBase {

    public long FileSize { get; set; }

    public string ContentsRoot { get; set; }

    public string Package { get; set; }

    public List<ExportVersion> Exports { get; set; }

    public string Notes { get; set; }

  }

  public class ExportVersion {

    public string Version { get; set; }

    public List<ExportType> Types { get; set; }

  }

  public class ExportType {

    public string Name { get; set; }

    public List<string> ExportNames { get; set; }

  }

}
