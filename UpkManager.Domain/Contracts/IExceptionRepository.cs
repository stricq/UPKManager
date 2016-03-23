using System.Threading.Tasks;

using UpkManager.Domain.Models;


namespace UpkManager.Domain.Contracts {

  public interface IExceptionRepository {

    Task SaveExceptionAsync(DomainUpkManagerException Exception);

  }

}
