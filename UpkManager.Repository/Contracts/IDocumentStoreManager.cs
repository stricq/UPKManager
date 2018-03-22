using Raven.Client.Documents;


namespace UpkManager.Repository.Contracts {

  public interface IDocumentStoreManager {

    IDocumentStore GetDocumentStoreFor(string DatabaseName);

  }

}
