using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using UpkManager.Domain.Models.UpkFile;


namespace UpkManager.Domain.Models {

  [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  public sealed class DomainUpkFile {

    #region Constructor

    public DomainUpkFile() {
      Exports = new List<DomainExportVersion>();

      ModdedFiles = new List<DomainUpkFile>();
    }

    #endregion Constructor

    #region Properties

    public string Id { get; set; }

    public long FileSize { get; set; }

    public string ContentsRoot { get; set; }

    public string Package { get; set; }

    public List<DomainExportVersion> Exports { get; set; } // Version => Type => Names

    public string Notes { get; set; }

    #endregion Properties

    #region Domain Properties

    public DomainHeader Header { get; set; }

    public DomainVersion CurrentVersion { get; set; }

    public string GameFilename { get; set; }

    public string Filename => $"{Package}.upk";

    public List<DomainUpkFile> ModdedFiles { get; set; }

    public bool IsModded => ModdedFiles.Any();

    public DateTime? LastAccess { get; set; }

    #endregion Domain Properties

    #region Domain Methods

    public List<DomainExportType> GetBestExports() {
      return GetBestExports(GetMaxVersion());
    }

    public List<DomainExportType> GetBestExports(DomainVersion version) {
      //
      // Check for exact version match
      //
      DomainExportVersion found = Exports.SingleOrDefault(v => v.Version == version);

      if (found != null) return found.Types;
      //
      // Else look for the max of all versions less than the specified version
      //
      DomainVersion max = Exports.Where(v => v.Version < version).Max(v => v.Version);

      if (max != null) {
        found = Exports.Single(v => v.Version == max);

        return found.Types;
      }
      //
      // Otherwise just return for the max version
      //
      max = Exports.Max(v => v.Version);

      if (max != null) {
        found = Exports.Single(v => v.Version == max);

        return found.Types;
      }
      //
      // Otherwise there is nothing to return
      //
      return new List<DomainExportType>();
    }

    public DomainVersion GetMaxVersion() {
      return !Exports.Any() ? CurrentVersion : Exports.Max(e => e.Version);
    }

    #endregion Domain Methods

  }

  public sealed class DomainExportVersion {

    public DomainExportVersion() {
      Types = new List<DomainExportType>();
    }

    public DomainVersion Version { get; set; }

    public List<DomainExportType> Types { get; set; }

  }

  public sealed class DomainExportType {

    public DomainExportType() {
      ExportNames = new List<string>();
    }

    public string Name { get; set; }

    public List<string> ExportNames { get; set; }

  }

}
