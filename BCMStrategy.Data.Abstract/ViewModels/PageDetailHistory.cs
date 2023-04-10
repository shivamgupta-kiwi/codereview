using SolrNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class PageDetailHistory
  {
    [SolrUniqueKey("id")]
    public string Id { get; set; }

    [SolrUniqueKey("processId")]
    public int ProcessId { get; set; }

    [SolrUniqueKey("processInstanceId")]
    public int ProcessInstanceId { get; set; }

    [SolrField("guidId")]
    public string GuidId { get; set; }

    [SolrField("url")]
    public string URL { get; set; }

    [SolrField("pageSource")]
    public string[] PageSource { get; set; }

    [SolrField("addedDateTime")]
    public DateTime AddedDateTime { get; set; }
  }
}