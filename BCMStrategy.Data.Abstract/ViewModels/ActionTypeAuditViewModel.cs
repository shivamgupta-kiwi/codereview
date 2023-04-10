using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class ActionTypeAuditViewModel : AuditBaseModel
  {
    public string WebsiteType { get; set; }
    public string ActionType { get; set; }
    public bool? GranularActivityTypeExists{ get; set; }

    public string ActionTypeValue { get; set; }
  }
}
