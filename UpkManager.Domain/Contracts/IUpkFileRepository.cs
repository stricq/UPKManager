using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

using UpkManager.Domain.Models;
using UpkManager.Domain.Models.UpkFile;


namespace UpkManager.Domain.Contracts {

  [ContractClass(typeof(IUpkFileRepositoryContract))]
  public interface IUpkFileRepository {

    Task LoadDirectoryRecursiveFlat(List<DomainUpkFile> ParentFile, int Version, string ParentPath, string Path, string Filter);

    Task LoadDirectoryRecursive(DomainExportedObject Parent, string Path);

    Task<DomainHeader> LoadUpkFile(string Filename);

    Task SaveUpkFile(DomainHeader Header, string Filename);

    Task<int> GetGameVersion(string GamePath);

  }

  [ContractClassFor(typeof(IUpkFileRepository))]
  internal abstract class IUpkFileRepositoryContract : IUpkFileRepository {

    public Task LoadDirectoryRecursiveFlat(List<DomainUpkFile> ParentFile, int Version, string ParentPath, string Path, string Filter) {
      Contract.Requires(ParentFile != null);

      Contract.Requires(!String.IsNullOrEmpty(ParentPath));

      Contract.Requires(!String.IsNullOrEmpty(Path));

      Contract.Requires(!String.IsNullOrEmpty(Filter));

      return default(Task);
    }

    public Task LoadDirectoryRecursive(DomainExportedObject Parent, string Path) {
      Contract.Requires(Parent != null);

      Contract.Requires(!String.IsNullOrEmpty(Path));

      return default(Task);
    }

    public Task<DomainHeader> LoadUpkFile(string Filename) {
      Contract.Requires(!String.IsNullOrEmpty(Filename));

      Contract.Ensures(Contract.Result<DomainHeader>() != null);

      return default(Task<DomainHeader>);
    }

    public Task SaveUpkFile(DomainHeader Header, string Filename) {
      Contract.Requires(Header != null);
      Contract.Requires(Header.ExportTable != null);

      Contract.Requires(!String.IsNullOrEmpty(Filename));

      return default(Task);
    }

    public Task<int> GetGameVersion(string GamePath) {
      Contract.Requires(!String.IsNullOrEmpty(GamePath));

      return default(Task<int>);
    }

  }

}
