using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class WebLinkConfiguration
  {
    /// <summary>
    /// Gets or Sets WebLink Id
    /// </summary>
    public int WebLinkId { get; set; }

    /// <summary>
    /// Gets or Sets Category Navigated
    /// </summary>
    public bool? IsCategoryNavigated { get; set; }

    /// <summary>
    /// Gets or Sets Category RegEx
    /// </summary>
    public string CategoryRegEx { get; set; }

    /// <summary>
    /// Gets or Sets Linked Clicked
    /// </summary>
    public bool? LinkedClicked { get; set; }

    /// <summary>
    /// Gets or Sets Linked Regular Expression
    /// </summary>
    public string LinkedRegEx { get; set; }

    /// <summary>
    /// Gets or Sets Is Search Functionality
    /// </summary>
    public bool? IsSearchFunctionality { get; set; }

    /// <summary>
    /// Gets or Sets Search Keyword
    /// </summary>
    public string SearchKeyword { get; set; }

    /// <summary>
    /// Gets or Sets Contains Document
    /// </summary>
    public bool? IsContainsDocument { get; set; }

    /// <summary>
    /// Gets or Sets Is RSS Feed Available
    /// </summary>
    public bool? IsRSSFeedAvailable { get; set; }

    /// <summary>
    /// Gets or Sets RSS Feed Click Regular Expression
    /// </summary>
    public string RSSFeedClickRegEx { get; set; }

    /// <summary>
    /// Gets or Sets RSS Feed URL
    /// </summary>
    public string RSSFeedURL { get; set; }

    /// <summary>
    /// Gets or Sets Dropdown Search RegEx
    /// </summary>
    public string DropDownSearchRegEx { get; set; }

    /// <summary>
    /// Gets or Sets DateWise RegEx
    /// </summary>
    public string DateWiseRegEx { get; set; }

    /// <summary>
    /// Gets or Sets HyperLink Click RegEx
    /// </summary>
    public string HyperLinkClickRegEx { get; set; }

    /// <summary>
    /// Gets or Sets HyperLink Document RegEx
    /// </summary>
    public string HyperLinkDocumentRegEx { get; set; }
  }
}