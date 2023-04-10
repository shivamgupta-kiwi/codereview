using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Common.Kendo
{
  /// <summary>
  /// Grid Filter Class
  /// </summary>
  public class GridFilter
  {
    /// <summary>
    /// Gets or sets the field.
    /// </summary>
    /// <value>
    /// The field.
    /// </value>
    public string Field { get; set; }

    /// <summary>
    /// Gets or sets the operator.
    /// </summary>
    /// <value>
    /// The operator.
    /// </value>
    public string Operator { get; set; }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public object Value { get; set; }

    /// <summary>
    /// Gets or sets the logic.
    /// </summary>
    /// <value>
    /// The logic.
    /// </value>
    public string Logic { get; set; }

    /// <summary>
    /// Gets or sets the filters.
    /// </summary>
    /// <value>
    /// The filters.
    /// </value>
    public IEnumerable<GridFilter> Filters { get; set; }
  }
}
