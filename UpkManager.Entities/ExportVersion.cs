using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;


namespace UpkManager.Entities {

  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  public class ExportVersion {

    public string Version { get; set; }

    public List<ExportType> Types { get; set; }

  }

}