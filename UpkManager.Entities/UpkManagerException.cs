using System;

using STR.Common.Contracts;


namespace UpkManager.Entities {

  public class UpkManagerException : ModelBase {

    public string Message { get; set; }

    public string StackTrace { get; set; }

    public string MachineName { get; set; }

    public DateTime HappenedAt { get; set; }

  }

}
