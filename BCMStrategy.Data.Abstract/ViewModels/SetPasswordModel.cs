using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCMStrategy.Resources;
using Newtonsoft.Json;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class SetPasswordModel
  {
    private string _userMasterHashId;

    [JsonIgnore]
    public int UserMasterId { get; set; }

    public string UserMasterHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_userMasterHashId))
        {
          _userMasterHashId = this.UserMasterId == 0 ? string.Empty : this.UserMasterId.ToEncrypt();
        }
        return _userMasterHashId;
      }
      set
      {
        _userMasterHashId = value;
      }
    }

    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [RegularExpression(@"^.*(?=.{8,})(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[@@#$%^&+=~!_]).*$", ErrorMessageResourceName = "ValidationPassword", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblNewPassword", ResourceType = typeof(Resource))]
    public string Password { get; set; }

    [Display(Name = "LblConfirmPassword", ResourceType = typeof(Resource))]
    [Compare("Password", ErrorMessageResourceName = "ValidateCompare", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    public string ReEnterPassword { get; set; }

    public string HashKey { get; set; }
  }
}
