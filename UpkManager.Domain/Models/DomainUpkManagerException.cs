using System;


namespace UpkManager.Domain.Models {

  public class DomainUpkManagerException  {

    public Exception Exception { get; set; }

    public string MachineName { get; set; }

    public DateTime HappenedAt { get; set; }

  }

}
