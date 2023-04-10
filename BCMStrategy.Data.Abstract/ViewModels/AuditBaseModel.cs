using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class AuditBaseModel
  {
    public string Created { get; set; }

    public string Modified { get; set; }

    public string ModifiedBy { get; set; }

    public string CreatedBy { get; set; }
  }
}
