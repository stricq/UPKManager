using System;
using System.Collections.Concurrent;
using System.ComponentModel.Composition;

using Raven.Client.Documents;

using UpkManager.Repository.Contracts;


namespace UpkManager.Repository.Services {

  [Export(typeof(IDocumentStoreManager))]
  public sealed class DocumentStoreManager : IDocumentStoreManager {

    #region Private Fields

    private readonly ConcurrentDictionary<string, Lazy<IDocumentStore>> stores = new ConcurrentDictionary<string, Lazy<IDocumentStore>>();

    #endregion Private Fields

    #region IDocumentStoreManager Implementation

    public IDocumentStore GetDocumentStoreFor(string DatabaseName) {
      Lazy<IDocumentStore> store = createDocumentStore(DatabaseName);

      return stores.GetOrAdd(DatabaseName, store).Value;
    }

    #endregion IDocumentStoreManager Implementation

    #region Private Methods

    private static Lazy<IDocumentStore> createDocumentStore(string databaseName) {
      return new Lazy<IDocumentStore>(() => {
        IDocumentStore documentStore = new DocumentStore {
          Database    = databaseName,
          Urls        = new [] {
            "https://raven.stricq.com"
          }
        };

        documentStore.Initialize();

        return documentStore;
      });
    }

    #endregion Private Methods

  }

}
