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

    public List<DomainExportType> GetBestExports(string version) {
      DomainExportVersion current = Exports.SingleOrDefault(v => v.Version == version);

      if (current != null) return current.Types;

      int[] numeric = convertVersion(version);

      int lastSum = Int32.MinValue;

      foreach(DomainExportVersion domainVersion in Exports) {
        int[] n = convertVersion(domainVersion.Version);

        int sum = 0;

        for(int i = 0; i < 4; ++i) sum += numeric[i] - n[i] + (3 - i) * 100;

        if (lastSum < 0 && sum > 0) {
          lastSum = sum;

          current = domainVersion;
        }
        else {
          if (lastSum > 0 && sum > 0 && sum < lastSum) {
            lastSum = sum;

            current = domainVersion;
          }
          else {
            if (lastSum < 0 && sum < 0 && sum > lastSum) {
              lastSum = sum;

              current = domainVersion;
            }
          }
        }
      }

      return current?.Types;
    }

    public string GetMaxVersion() {
      long numeric = 0;

      string version = "0.0.0.0";

      foreach(string ver in Exports.Select(e => e.Version)) {
        long current = convertToLong(ver);

        if (current > numeric) {
          numeric = current;

          version = ver;
        }
      }

      return version;
    }

    #endregion Domain Methods

    #region Private Methods

    private static int[] convertVersion(string version) {
      int[] numeric = new int[4];

      string[] parts = version.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

      for(int i = 0; i < 4; ++i) numeric[i] = i >= parts.Length ? 0 : Int32.Parse(parts[i]);

      return numeric;
    }

    private static long convertToLong(string version) {
      string[] parts = version.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

      long numeric = 0;

      for(int i = 0; i < 4; ++i) numeric += Int32.Parse(parts[i]) + (3 - i) * 10000;

      return numeric;
    }

    #endregion Private Methods

  }

  public sealed class DomainExportVersion {

    public DomainExportVersion() {
      Types = new List<DomainExportType>();
    }

    public string Version { get; set; }

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
