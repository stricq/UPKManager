using System.ComponentModel.Composition;
using System.Reflection;

using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Session;

using UpkManager.Repository.Contracts;


namespace UpkManager.Repository.Services {

  [Export(typeof(IRavenStorage))]
  public sealed class RavenStorage : IRavenStorage {

    #region Private Fields

    private readonly IDocumentStoreManager manager;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public RavenStorage(IDocumentStoreManager Manager) {
      manager = Manager;
    }

    #endregion Constructor

    #region IRavenStorage Implementation

    public IDocumentStore Store { get; private set; }

    public IAsyncDocumentSession Session => Store?.OpenAsyncSession();

    public void Initialize(string DatabaseName, Assembly IndexAssembly = null) {
      Store = manager.GetDocumentStoreFor(DatabaseName);

#if DEBUG
      if (IndexAssembly != null) IndexCreation.CreateIndexes(IndexAssembly, Store);
#endif
    }

    public void Shutdown() {
      Store?.Dispose();
    }

    #endregion IRavenStorage Implementation

  }

}
