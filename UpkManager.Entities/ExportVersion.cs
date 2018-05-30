using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;


namespace UpkManager.Entities {

  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  public class ExportVersion {

    public List<string> Versions { get; set; }

    public string Locale { get; set; }

    public long Filesize { get; set; }

    public string Filehash { get; set; }

    public List<ExportType> Types { get; set; }

  }

}
