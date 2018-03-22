using System.Linq;

using Raven.Client.Documents.Indexes;

using UpkManager.Entities;


namespace UpkManager.Repository.Indexes {

  public sealed class UpkFileCommonIndex : AbstractIndexCreationTask<UpkFile> {

    public UpkFileCommonIndex() {
      Map = files => from file in files
                   select new { file.Package };
    }

  }

}
