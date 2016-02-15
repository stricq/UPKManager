using System.Collections.Generic;
using System.Threading.Tasks;

using UpkManager.Domain.Models;


namespace UpkManager.Domain.Contracts {

  public interface IUpkFileRemoteRepository {

    Task<List<DomainUpkFile>> LoadUpkFiles();

    Task SaveUpkFile(DomainUpkFile File);

  }

}
