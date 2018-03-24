using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using UpkManager.Domain.Models;


namespace UpkManager.Domain.Contracts {

  public interface IUpkFileRemoteRepository {

    Task<List<DomainUpkFile>> LoadUpkFiles(CancellationToken token);

    Task SaveUpkFile(DomainUpkFile File);

    Task SaveUpkFile(List<DomainUpkFile> Files);

    void Shutdown();

  }

}
