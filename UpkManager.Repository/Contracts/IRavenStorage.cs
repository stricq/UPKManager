using System.Reflection;

using Raven.Client.Documents;
using Raven.Client.Documents.Session;


namespace UpkManager.Repository.Contracts {

  public interface IRavenStorage {

    IDocumentStore Store { get; }

    IAsyncDocumentSession Session { get; }

    void Initialize(string DatabaseName, Assembly IndexAssembly = null);

    void Shutdown();

  }

}
