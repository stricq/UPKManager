using System.Threading.Tasks;

using UpkManager.Domain.Models;


namespace UpkManager.Domain.Contracts {

  public interface IUpkFileRepository {

    Task<DomainHeader> LoadUpkFile(string filename);

  }

}
