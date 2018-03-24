using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Raven.Client.Documents.Commands;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using Raven.Client.Util;

using STR.Common.Extensions;

using UpkManager.Domain.Contracts;
using UpkManager.Domain.Models;

using UpkManager.Entities;

using UpkManager.Repository.Contracts;
using UpkManager.Repository.Indexes;


namespace UpkManager.Repository.Services {

  [Export(typeof(IUpkFileRemoteRepository))]
  public sealed class UpkFileRavenRepository : IUpkFileRemoteRepository {

    #region Private Fields

    private readonly IMapper mapper;

    private readonly IRavenStorage store;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public UpkFileRavenRepository(IMapper Mapper, IRavenStorage Store) {
      mapper = Mapper;

      store = Store;

      store.Initialize("UpkManager", typeof(UpkFileCommonIndex).Assembly);
    }

    #endregion Constructor

    #region IUpkFileRemoteRepository Implementation

    public async Task<List<DomainUpkFile>> LoadUpkFiles(CancellationToken token) {
      try {
        List<UpkFile> files = new List<UpkFile>();

        using(IAsyncDocumentSession session = store.Session) {
          IRavenQueryable<UpkFile> query = session.Query<UpkFile, UpkFileCommonIndex>();

          using(IAsyncEnumerator<StreamResult<UpkFile>> enumerator = await session.Advanced.StreamAsync(query, token)) {
            while(await enumerator.MoveNextAsync()) {
              files.Add(enumerator.Current.Document);
            }
          }
        }

        return mapper.Map<List<DomainUpkFile>>(files);
      }
      catch(TaskCanceledException) { }
      catch(OperationCanceledException) { }

      return new List<DomainUpkFile>();
    }

    public async Task SaveUpkFile(DomainUpkFile File) {
      if (File == null) return;

      UpkFile file = mapper.Map<UpkFile>(File);

      using(IAsyncDocumentSession session = store.Session) {
        if (String.IsNullOrEmpty(file.Id)) await session.StoreAsync(file);
        else {
          UpkFile dbFile = await session.LoadAsync<UpkFile>(file.Id);

          mapper.Map(file, dbFile);
        }

        await session.SaveChangesAsync();

        File.Id = file.Id;
      }
    }

    public async Task SaveUpkFile(List<DomainUpkFile> Files) {
      if (Files == null || !Files.Any()) return;

      List<UpkFile> files = mapper.Map<List<UpkFile>>(Files);

      using(IAsyncDocumentSession session = store.Session) {
        for(int index = 0; index < files.Count(f => !String.IsNullOrEmpty(f.Id)); index += 1024) {
          List<UpkFile> dbFiles = (await session.LoadAsync<UpkFile>(files.Where(f => !String.IsNullOrEmpty(f.Id))
                                                                         .Skip(index).Take(1024)
                                                                         .Select(f => f.Id))).Where(result => result.Value != null)
                                                                                             .Select(result => result.Value)
                                                                                             .ToList();

          var items = (from row1 in dbFiles
                       join row2 in files on row1.Id equals row2.Id
                     select new { DbFile = row1, File = row2 }).ToList();

          items.ForEach(item => mapper.Map(item.File, item.DbFile));
        }

        foreach(UpkFile file in files.Where(f => String.IsNullOrEmpty(f.Id))) await session.StoreAsync(file);

        await session.SaveChangesAsync();

        Files.Zip(files, Tuple.Create).ForEach(t => t.Item1.Id = t.Item2.Id);
      }
    }

    public void Shutdown() {
      store.Shutdown();
    }

    #endregion IUpkFileRemoteRepository Implementation

  }

}
