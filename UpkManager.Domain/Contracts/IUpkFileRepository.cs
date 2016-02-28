using System;
using System.IO;
using System.Threading.Tasks;

using UpkManager.Domain.Models;
using UpkManager.Domain.Models.Tables;


namespace UpkManager.Domain.Contracts {

  public interface IUpkFileRepository {

    Task<DomainHeader> LoadAndParseUpk(string Filename, bool SkipProperties, bool SkipParsing, Action<DomainLoadProgress> LoadProgress);

    Task SaveObject(DomainExportTableEntry Export, string Filename);

    Stream GetObjectStream(DomainExportTableEntry Export);

  }

}
