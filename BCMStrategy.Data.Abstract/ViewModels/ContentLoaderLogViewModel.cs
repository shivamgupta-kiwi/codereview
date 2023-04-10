using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  /// <summary>
  /// Content Loader LogView Model
  /// </summary>
  public class ContentLoaderLogViewModel
  {
    /// <summary>
    /// process Id
    /// </summary>
    public int ProcessId { get; set; }

    /// <summary>
    /// ProcessInstanceId
    /// </summary>
    public int ProcessInstanceId { get; set; }

    /// <summary>
    /// WebSiteId
    /// </summary>
    public int WebSiteId { get; set; }

    /// <summary>
    /// SiteUrl
    /// </summary>
    public string SiteUrl { get; set; }

    /// <summary>
    /// Link Level
    /// </summary>
    public int? LinkLevel { get; set; }

    /// <summary>
    /// IsContentUpdated
    /// </summary>
    public bool IsContentUpdated { get; set; }

    /// <summary>
    /// IsContentInserted
    /// </summary>
    public bool IsContentInserted { get; set; }

    /// <summary>
    /// CreatedDate
    /// </summary>
    public DateTime CreatedDate { get; set; }

  }
}
