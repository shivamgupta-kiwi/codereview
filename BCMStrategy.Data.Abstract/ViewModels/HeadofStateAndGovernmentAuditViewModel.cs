﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class HeadofStateAndGovernmentAuditViewModel : AuditBaseModel
  {
    public string Country { get; set; }
    public string Designation { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
  }
}
