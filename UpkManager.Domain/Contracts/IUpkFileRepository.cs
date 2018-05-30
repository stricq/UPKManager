using System.Collections.Generic;
using System.Threading.Tasks;

using UpkManager.Domain.Models;
using UpkManager.Domain.Models.UpkFile;


namespace UpkManager.Domain.Contracts {

  public interface IUpkFileRepository {

    Task LoadDirectoryRecursiveFlat(List<DomainUpkFile> ParentFile, string ParentPath, string Path, string Filter, bool isRoot = true);

    Task LoadDirectoryRecursive(DomainExportedObject Parent, string Path);

    Task<DomainHeader> LoadUpkFile(string Filename);

    Task SaveUpkFile(DomainHeader Header, string Filename);

    Task<DomainVersion> GetGameVersion(string GamePath);

    Task<string> GetGameLocale(string GamePath);

  }

}
