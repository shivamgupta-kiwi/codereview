using SolrNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolrNet.Impl;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class PageDetails
  {
    [SolrUniqueKey("id")]
    public string Id { get; set; }

    [SolrUniqueKey("itemId")]
    public int ItemId { get; set; }

    [SolrField("guidId")]
    public string GuidId { get; set; }

    [SolrField("url")]
    public string URL { get; set; }

    [SolrField("pageSource")]
    public string[] PageSource { get; set; }

    [SolrField("addedDateTime")]
    public DateTime AddedDateTime { get; set; }

    [SolrField("status")]
    public string Status { get; set; }
  }

  public class LexiconDetails
  {
    [SolrUniqueKey("scrapperDetailId")]
    public int ScrapperDetailId { get; set; }

    [SolrUniqueKey("processId")]
    public int ProcessId { get; set; }

    [SolrUniqueKey("processInstanceId")]
    public int ProcessInstanceId { get; set; }

    [SolrField("guidId")]
    public string GuidId { get; set; }

    [SolrField("pageSource")]
    public string[] PageSource { get; set; }

    [SolrField("addedDateTime")]
    public DateTime AddedDateTime { get; set; }
  }

  public class PageDetailsView
  {
    public SolrSearchParameters Search { get; set; }
    public ICollection<PageDetails> Products { get; set; }
    public int TotalCount { get; set; }
    public IDictionary<string, ICollection<KeyValuePair<string, int>>> Facets { get; set; }
    public string DidYouMean { get; set; }
    public bool QueryError { get; set; }
    public IDictionary<string, HighlightedSnippets> Highlight { get; set; }

    public PageDetailsView()
    {
      Search = new SolrSearchParameters();
      Facets = new Dictionary<string, ICollection<KeyValuePair<string, int>>>();
      Products = new List<PageDetails>();
      Highlight = new Dictionary<string, HighlightedSnippets>();
    }
  }

  public class LexiconDetailsView
  {
    public SolrSearchParameters Search { get; set; }
    public ICollection<LexiconDetails> Products { get; set; }
    public int TotalCount { get; set; }
    public string DidYouMean { get; set; }
    public bool QueryError { get; set; }

    public LexiconDetailsView()
    {
      Search = new SolrSearchParameters();
      Products = new List<LexiconDetails>();
    }
  }
}
