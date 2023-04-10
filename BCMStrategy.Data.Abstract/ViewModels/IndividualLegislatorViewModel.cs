using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class IndividualLegislatorViewModel : AuditBaseModel
  {
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Designation { get; set; }
    public string Committee { get; set; }
    public string Entity { get; set; }
    public string Sector { get; set; }
    public string Country { get; set; }
  }
}
