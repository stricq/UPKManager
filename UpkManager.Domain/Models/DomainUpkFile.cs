using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace UpkManager.Domain.Models {

  public class DomainUpkFile {

    #region Constructor

    public DomainUpkFile() {
      ExportTypes = new List<string>();

      ModdedFiles = new List<DomainUpkFile>();
    }

    #endregion Constructor

    #region Properties

    public long FileSize { get; set; }

    public string Id { get; set; }

    public string GameFilename { get; set; }

    public List<string> ExportTypes { get; set; }

    public string Notes { get; set; }

    #endregion Properties

    #region Domain Properties

    public DomainHeader Header { get; set; }

    public string Filename => Path.GetFileName(GameFilename);

    public bool ContainsTargetObject { get; set; }

    public bool IsErrored { get; set; }

    public List<DomainUpkFile> ModdedFiles { get; set; }

    public bool IsModded => ModdedFiles.Any();

    #endregion Domain Properties

  }

}
