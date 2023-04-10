using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class ActionNounPlusVerbAuditViewModel : AuditBaseModel
  {
    public string ActionType { get; set; }
    public bool HardCoded { get; set; }
    public string Noun { get; set; }
    public string Verb { get; set; }
  }
}
