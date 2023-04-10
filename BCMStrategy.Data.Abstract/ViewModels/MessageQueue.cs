using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  /// <summary>
  /// Message Queue information
  /// </summary>
  public class MessageQueue
  {
    /// <summary>
    /// Gets or Sets Message Body
    /// </summary>
    public string MessageBody { get; set; }

    /// <summary>
    /// Gets or Sets the Queue Type
    /// </summary>
    public int QueueType { get; set; }
  }
}