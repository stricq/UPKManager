using System;
using System.Collections.Concurrent;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

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
      X509Certificate2 certificate;

      using(Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"UpkManager.Repository.Certificates.{databaseName}Client.pfx")) {
        if (stream == null) return null;

        byte[] data = new byte[stream.Length];

        stream.Read(data, 0, data.Length);

        certificate = new X509Certificate2(data);
      }

      return new Lazy<IDocumentStore>(() => {
        IDocumentStore documentStore = new DocumentStore {
//        Certificate = certificate,
          Database    = databaseName,
          Urls        = new [] {
            "http://raven.stricq.com:8282"
          }
        };

        documentStore.Initialize();

        return documentStore;
      });
    }

    #endregion Private Methods

  }

}
