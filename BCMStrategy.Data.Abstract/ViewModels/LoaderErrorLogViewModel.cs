using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class LoaderErrorLogViewModel
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
    /// Error Desc
    /// </summary>
    public string ErrorDesc { get; set; }

    /// <summary>
    /// CreatedDate
    /// </summary>
    public DateTime CreatedDate { get; set; }
  }
}
