using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Threading.Tasks;

using AutoMapper;

using RestSharp;

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

    public async Task<List<DomainUpkFile>> LoadUpkFiles() {
      RestRequest request = new RestRequest("UpkFile", Method.GET) { RequestFormat = DataFormat.Json };

      IRestResponse<List<UpkFile>> response = await client.ExecuteGetTaskAsync<List<UpkFile>>(request);

      return await Task.Run(() => mapper.Map<List<DomainUpkFile>>(response.Data));
    }

    public async Task SaveUpkFile(DomainUpkFile File) {
      await Task.FromResult(1);
    }

    #endregion IUpkFileRemoteRepository Implementation

  }

}
