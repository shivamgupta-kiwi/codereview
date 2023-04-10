using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class RssFeedDetails
  {
    public int Id { get; set; }

    public int WebSiteId { get; set; }

    public string RegEx { get; set; }

    public string RSSFeedURL { get; set; }
  }

  public class RssFeedWebSiteUrlDetails
  {
    public int Id { get; set; }

    public string SiteURL { get; set; }

    public string PublishDate { get; set; }
  }
}