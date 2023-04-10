using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class LegislatorAuditViewModel : AuditBaseModel
  {
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EntityName { get; set; }
    public string Sector { get; set; }
    public string Country { get; set; }
  }
}
