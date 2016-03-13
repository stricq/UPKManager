using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;

using AutoMapper;

using RestSharp;

using STR.Common.Extensions;

using UpkManager.Domain.Contracts;
using UpkManager.Domain.Models;

using UpkManager.Entities;


namespace UpkManager.Repository.Services {

  [Export(typeof(IUpkFileRemoteRepository))]
  public class UpkFileRemoteRepository : IUpkFileRemoteRepository {

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

    public async Task<List<DomainUpkFile>> LoadUpkFiles(int GameVersion) {
      RestRequest request = new RestRequest("UpkFile/{Version}", Method.GET) { RequestFormat = DataFormat.Json };

      request.AddParameter("Version", GameVersion, ParameterType.UrlSegment);

      IRestResponse<List<UpkFile>> response = await client.ExecuteGetTaskAsync<List<UpkFile>>(request);

      if (response.StatusCode != HttpStatusCode.OK) throw new Exception(response.StatusDescription);

      return await Task.Run(() => mapper.Map<List<DomainUpkFile>>(response.Data));
    }

    public async Task SaveUpkFile(DomainUpkFile File) {
      UpkFile file = await Task.Run(() => mapper.Map<UpkFile>(File));

      RestRequest request = new RestRequest("UpkFile", Method.PUT) { RequestFormat = DataFormat.Json };

      request.AddBody(file);

      IRestResponse<string> response = await client.ExecuteTaskAsync<string>(request);

      File.Id = response.Data;
    }

    public async Task SaveUpkFile(List<DomainUpkFile> Files) {
      await Files.ForEachAsync(SaveUpkFile);
    }

    #endregion IUpkFileRemoteRepository Implementation

  }

}
