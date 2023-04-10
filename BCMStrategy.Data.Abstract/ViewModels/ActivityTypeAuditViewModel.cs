using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class ActivityTypeAuditViewModel : AuditBaseModel
  {
    public string ActionType { get; set; }
    public string ActivityType { get; set; }
    public string ActivityValue { get; set; }
  }
}
