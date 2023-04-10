using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class LoaderLinkQueue
  {
    /// <summary>
    /// Gets or Sets the Id field
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or Sets the Process Id
    /// </summary>
    public int ProcessId { get; set; }

    /// <summary>
    /// Gets or Sets the Process Instance Id
    /// </summary>
    public int ProcessInstanceId { get; set; }

    /// <summary>
    /// Gets or Sets the WebSite id
    /// </summary>
    public int WebSiteId { get; set; }

    /// <summary>
    /// Gets or Sets the SiteURL
    /// </summary>
    public string SiteURL { get; set; }

    /// <summary>
    /// Gets or Sets the Instance Name
    /// </summary>
    public string InstanceName { get; set; }

    /// <summary>
    /// Gets or Sets the LoaderLinkQueue
    /// </summary>
    public string GUID { get; set; }

    /// <summary>
    /// Read Taken for the Given Link Queue URL
    /// </summary>
    public bool IsReadTaken { get; set; }

    /// <summary>
    /// Gets or Sets the Page Content
    /// </summary>
    public string pageContent { get; set; }

    /// <summary>
    /// Gets or Sets the Link Level
    /// </summary>
    public int LinkLevel { get; set; }

    /// <summary>
    /// Gets or Sets the IsSuccessful
    /// </summary>
    public bool IsSuccessful { get; set; }

    /// <summary>
    /// Gets or Sets the WebLinkBytes
    /// </summary>
    public decimal WebLinkBytes { get; set; }

    /// <summary>
    /// Get or set the IsHardCoded
    /// </summary>
    public bool IsHardCoded { get; set; }

    /// <summary>
    /// Get or set the IsHardCoded
    /// </summary>
    public int WebLinkTypeId { get; set; }

    /// <summary>
    /// Gets or Sets the Newer Process id
    /// </summary>
    public int NewerProcessId { get; set; }

    /// <summary>
    /// Page Type to Identify the types
    /// </summary>
    public int PageType { get; set; }

    /// <summary>
    /// Html Links Fetch boolean field
    /// </summary>
    public bool AllHtmlLinksFetch { get; set; }

    /// <summary>
    /// Regular expression defined for web links
    /// </summary>
    public string RegEx { get; set; }

    public DateTime? PublishDate { get; set; }

    public int LexiconCount { get; set; }

		public string ParentURL { get; set; }
  }

  public class ProcessLinkDetails
  {
    /// <summary>
    /// Gets or Sets the Engine Category
    /// </summary>
    public int EngineCategory { get; set; }

    /// <summary>
    /// Gets or Sets the Process Id
    /// </summary>
    public int ProcessId { get; set; }

    /// <summary>
    /// Gets or Sets the Process Instance Id
    /// </summary>
    public int ProcessInstanceId { get; set; }

    /// <summary>
    /// Gets or Sets No. of records
    /// </summary>
    public int NoOfRecords { get; set; }

    /// <summary>
    /// Gets or Sets Page Number
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Gets or Sets Website Type
    /// </summary>
    public int Websitetypes { get; set; }

    /// <summary>
    /// Gets or Sets Guid Number
    /// </summary>
    public string Guid { get; set; }

    public decimal WebLinkBytes { get; set; }
  }

  public class DateMonth
  {
    public int Day { get; set; }

    public int Year { get; set; }

    public string Month { get; set; }
  }
}