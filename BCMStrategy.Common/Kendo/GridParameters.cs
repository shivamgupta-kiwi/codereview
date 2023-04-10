using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Common.Kendo
{
  public class GridParameters
  {
    /// <summary>
    /// Gets or sets the take.
    /// </summary>
    /// <value>
    /// The take.
    /// </value>
    public int Take { get; set; }

    /// <summary>
    /// Gets or sets the skip.
    /// </summary>
    /// <value>
    /// The skip.
    /// </value>
    public int Skip { get; set; }

    /// <summary>
    /// Gets or sets the page.
    /// </summary>
    /// <value>
    /// The page.
    /// </value>
    public int Page { get; set; }

    /// <summary>
    /// Gets or sets the size of the page.
    /// </summary>
    /// <value>
    /// The size of the page.
    /// </value>
    public int PageSize { get; set; }

    /// <summary>
    /// Gets or sets the filter.
    /// </summary>
    /// <value>
    /// The filter.
    /// </value>
    public GridFilter Filter { get; set; }

    /// <summary>
    /// Gets or sets the sort.
    /// </summary>
    /// <value>
    /// The sort.
    /// </value>
    public List<GridSort> Sort { get; set; }
  }
}
