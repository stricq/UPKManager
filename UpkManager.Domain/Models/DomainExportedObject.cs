using System.Collections.Generic;
using System.IO;


namespace UpkManager.Domain.Models {

  public class DomainExportedObject {

    #region Properties

    public string Filename { get; set; }

    public DomainExportedObject Parent { get; set; }

    public List<DomainExportedObject> Children { get; set; }

    #endregion Properties

    #region Domain Properties

    public string Name => Path.GetFileName(Filename);

    #endregion Domain Properties

  }

}
