using System.Threading.Tasks;

using UpkManager.Domain.Models;


namespace UpkManager.Domain.Contracts {

  public interface IUpkFileRepository {

    Task<DomainHeader> LoadUpkFile(string Filename);

    Task SaveUpkFile(DomainHeader Header, string Filename);

    Task<int> GetGameVersion(string GamePath);

  }

}
