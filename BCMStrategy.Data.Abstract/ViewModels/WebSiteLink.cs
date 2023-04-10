using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class WebSiteLink
  {
    /// <summary>
    /// Id of the Site
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// URL of the Site
    /// </summary>
    public string SiteURL { get; set; }

    /// <summary>
    /// Instance of the Web Site
    /// </summary>
    public string InstanceName { get; set; }

    /// <summary>
    /// GUID of the WebSite
    /// </summary>
    public string GUID { get; set; }
  }
}
