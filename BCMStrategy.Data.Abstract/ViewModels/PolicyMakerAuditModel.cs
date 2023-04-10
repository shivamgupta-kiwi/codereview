using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class PolicyMakerAuditModel : AuditBaseModel
  {
    public string InstitutionType { get; set; }

    public string CountryName { get; set; }

    public string DesignationName { get; set; }

    public string PolicyFirstName { get; set; }

    public string PolicyLastName { get; set; }
  }
}
