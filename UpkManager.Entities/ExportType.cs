using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;


namespace UpkManager.Entities {

  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  public class ExportType {

    public string Name { get; set; }

    public List<string> ExportNames { get; set; }

  }

}