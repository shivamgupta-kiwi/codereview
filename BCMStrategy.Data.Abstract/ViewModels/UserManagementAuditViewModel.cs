using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class UserManagementAuditViewModel : AuditBaseModel
  {
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public string Address { get; set; }
    public string Country { get; set; }
    public string State{ get; set; }
    public string ZipCode { get; set; }
    public string City { get; set; }
  }

  public class UserManagementChangePasswordModel : AuditBaseModel
  {
    public string OldPassword { get; set; }

    public string NewPassword { get; set; }

    public string ConfirmPassword { get; set; }
  }
}