using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class InstitutionViewModel : AuditBaseModel
  {
    public string Country { get; set; }
    public string InstitutionType { get; set; }
    public string Institution { get; set; }
    public bool EuropeanUnion { get; set; }
    public string EntityName { get; set; }
  }
}
