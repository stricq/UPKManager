using System.ComponentModel.Composition;
using System.Configuration;
using System.Threading.Tasks;

using AutoMapper;

using RestSharp;

using UpkManager.Domain.Contracts;
using UpkManager.Domain.Models;

using UpkManager.Entities;


namespace UpkManager.Repository.Services {

  [Export(typeof(IExceptionRepository))]
  public class ExceptionRepository : IExceptionRepository {

    #region Private Fields

    private readonly IMapper mapper;

    private readonly RestClient client;

    #endregion Private Fields

    #region Constructor

    [ImportingConstructor]
    public ExceptionRepository(IMapper Mapper) {
      mapper = Mapper;

      client = new RestClient($"{ConfigurationManager.AppSettings["ApiRootUrl"]}{ConfigurationManager.AppSettings["ApiVersion"]}/");
    }

    #endregion Constructor

    #region IExceptionRepository Implementation

    public async Task SaveExceptionAsync(DomainUpkManagerException Exception) {
      UpkManagerException exception = await Task.Run(() => mapper.Map<UpkManagerException>(Exception));

      RestRequest request = new RestRequest("UpkException", Method.POST) { RequestFormat = DataFormat.Json };

      request.AddBody(exception);

      await client.ExecuteTaskAsync<string>(request);
    }

    #endregion IExceptionRepository Implementation

  }

}
