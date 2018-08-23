using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using UpkManager.Domain.Models.UpkFile;


namespace UpkManager.Domain.Models {

  [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  public sealed class DomainUpkFile {

    #region Private Fields

    private long filesize;

    private string filehash;

    #endregion Private Fields

    #region Constructor

    public DomainUpkFile() {
      Exports = new List<DomainExportVersion>();

      ModdedFiles = new List<DomainUpkFile>();
    }

    #endregion Constructor

    #region Properties

    public string Id { get; set; }

    public string ContentsRoot { get; set; }

    public string Package { get; set; }

    public List<DomainExportVersion> Exports { get; set; } // Version => Type => Names

    public string Notes { get; set; }

    #endregion Properties

    #region Domain Properties

    public DomainHeader Header { get; set; }

    public DomainVersion CurrentVersion { get; set; }

    public string CurrentLocale { get; set; }

    public long Filesize {
      get => GetCurrentExports()?.Filesize ?? filesize;
      set => filesize = value;
    }

    public string Filehash {
      get => GetCurrentExports()?.Filehash ?? filehash;
      set => filehash = value;
    }

    public string GameFilename { get; set; }

    public string Filename => $"{Package}.upk";

    public List<DomainUpkFile> ModdedFiles { get; set; }

    public bool IsModded => ModdedFiles.Any();

    public DateTime? LastAccess { get; set; }

    public string NewFilehash { get; set; }

    public string NewLocale { get; set; }

    #endregion Domain Properties

    #region Domain Methods

    public DomainExportVersion GetCurrentExports() {
      return GetExports(CurrentVersion, CurrentLocale);
    }

    public DomainExportVersion GetExports(DomainVersion version, string locale) {
      //
      // Check for exact version match
      //
      DomainExportVersion found = Exports.SingleOrDefault(v => v.Versions.Contains(version) && v.Locale == locale);

      if (found != null) return found;
      //
      // Otherwise just return for the max version
      //
      DomainVersion max = Exports.Where(v => v.Locale == locale).SelectMany(v => v.Versions).Max();

      if (max != null) {
        found = Exports.Single(v => v.Versions.Contains(max) && v.Locale == locale);

        return found;
      }
      //
      // Otherwise there is nothing to return
      //
      return null;
    }

    public DomainVersion GetLeastVersion() {
      DomainExportVersion version = GetCurrentExports();

      return version.Versions.Max();
    }

    #endregion Domain Methods

  }

  public sealed class DomainExportVersion {

    public DomainExportVersion() {
      Types = new List<DomainExportType>();
    }

    public List<DomainVersion> Versions { get; set; }

    public string Locale { get; set; }

    public long Filesize { get; set; }

    public string Filehash { get; set; }

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
