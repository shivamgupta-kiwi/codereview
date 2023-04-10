using Newtonsoft.Json;
using System;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class ProcessEventLog
  {
    /// <summary>
    /// Gets or Sets Id of the Process Event
    /// </summary>
    [JsonIgnore]
    public int Id { get; set; }

    /// <summary>
    /// Gets or Sets Process Event Id
    /// </summary>
    public int ProcessEventId { get; set; }

    /// <summary>
    /// Gets or Sets Process Instance Id
    /// </summary>
    public int ProcessInstanceId { get; set; }

    /// <summary>
    /// Gets or Sets Site URL
    /// </summary>
    public string SiteUrl { get; set; }

    /// <summary>
    /// Gets or Sets Created Date Time
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Gets or Sets Created By
    /// </summary>
    public string CreatedBy { get; set; }

  }
}
