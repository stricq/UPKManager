using System.Collections.Generic;
using System.Threading.Tasks;

using UpkManager.Domain.Models;


namespace UpkManager.Domain.Contracts {

  public interface IUpkFileRemoteRepository {

    Task<List<DomainUpkFile>> LoadUpkFiles(int GameVersion);

    Task SaveUpkFile(DomainUpkFile File);

    Task SaveUpkFile(List<DomainUpkFile> File);

  }

}
