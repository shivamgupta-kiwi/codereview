using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class WebLink
  {
    /// <summary>
    /// Gets or Sets the Id field
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the WebSiteTypeId
    /// </summary>
    public int WebSiteTypeId { get; set; }

    /// <summary>
    /// Gets or sets the WebLinkURL
    /// </summary>
    public string WebLinkURL { get; set; }

    /// <summary>
    /// Gets or sets the CategoryId
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Gets or Sets the Category Name
    /// </summary>
    public string CategoryName { get; set; }

    /// <summary>
    /// Gets or sets the IsActive
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the IsDeleted
    /// </summary>
    public bool IsDeleted { get; set; }

    public List<WebLinkConfiguration> WebLinkConfiguration { get; set; }

    public DateTime Created { get; set; }

    public String CreatedBy { get; set; }

    /// <summary>
    /// Store the data for the respective process
    /// </summary>
    public int ProcessId { get; set; }
  }
}