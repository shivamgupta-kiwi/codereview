using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Common.Email
{
  public class EmailAttachmentViewModel
  {
    /// <summary>
    /// Gets or sets the name of the attachment.
    /// </summary>
    /// <value>
    /// The name of the attachment.
    /// </value>
    public string AttachmentName { get; set; }

    /// <summary>
    /// Gets or sets the attachment string.
    /// </summary>
    /// <value>
    /// The attachment string.
    /// </value>
    public string AttachmentString { get; set; }
  }
}