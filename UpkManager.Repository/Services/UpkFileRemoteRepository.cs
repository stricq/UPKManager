using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using RestSharp;

using STR.Common.Extensions;

using UpkManager.Domain.Contracts;
using UpkManager.Domain.Models;

using UpkManager.Entities;


namespace UpkManager.Repository.Services {

  [Export(typeof(IUpkFileRemoteRepository))]
  public sealed class UpkFileRemoteRepository : IUpkFileRemoteRepository {

    #region Private Fields

    private readonly IMapper mapper;

    private readonly RestClient client;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public UpkFileRemoteRepository(IMapper Mapper) {
      mapper = Mapper;

      client = new RestClient($"{ConfigurationManager.AppSettings["ApiRootUrl"]}{ConfigurationManager.AppSettings["ApiVersion"]}/");
    }

    #endregion Constructor

    #region IUpkFileRemoteRepository Implementation

    public async Task<List<DomainUpkFile>> LoadUpkFiles(CancellationToken token) {
      try {
        RestRequest request = new RestRequest("UpkFile", Method.GET) { RequestFormat = DataFormat.Json };

        IRestResponse<List<UpkFile>> response = await client.ExecuteGetTaskAsync<List<UpkFile>>(request, token);

        if (response.StatusCode != HttpStatusCode.OK) throw new Exception(response.StatusDescription);

        List<UpkFile> toKeep = response.Data.GroupBy(f => new { f.ContentsRoot, f.Package }).Select(fg => fg.Aggregate((f1, f2) => f1.Exports.Count > f2.Exports.Count ? f1 : f2)).ToList();

        response.Data.Where(f => !toKeep.Contains(f)).ForEachAsync(f => DeleteUpkFile(f.Id)).FireAndForget();

        return await Task.Run(() => mapper.Map<List<DomainUpkFile>>(toKeep), token);
      }
      catch(TaskCanceledException) { }
      catch(OperationCanceledException) { }

      return new List<DomainUpkFile>();
    }

    public async Task SaveUpkFile(DomainUpkFile File) {
      UpkFile file = await Task.Run(() => mapper.Map<UpkFile>(File));

      RestRequest request = new RestRequest("UpkFile", Method.PUT) { RequestFormat = DataFormat.Json };

      request.AddBody(file);

      IRestResponse<string> response = await client.ExecuteTaskAsync<string>(request);

      if (response.StatusCode != HttpStatusCode.OK) throw new Exception(response.StatusDescription);

      File.Id = response.Data;
    }

    public async Task SaveUpkFile(List<DomainUpkFile> Files) {
      await Files.ForEachAsync(SaveUpkFile);
    }

    public async Task DeleteUpkFile(string Id) {
      RestRequest request = new RestRequest("UpkFile/{upkFileId}", Method.DELETE) { RequestFormat = DataFormat.Json };

      request.AddUrlSegment("upkFileId", Id);

      IRestResponse response = await client.ExecuteTaskAsync(request);

      if (response.StatusCode != HttpStatusCode.OK) throw new Exception(response.StatusDescription);
    }

    #endregion IUpkFileRemoteRepository Implementation

  }

}
