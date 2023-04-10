using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCMStrategy.Data.Abstract.CustomValidation;
using BCMStrategy.Resources;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class ForgotPasswordModel
  {
    [IsEmailIdExistAttribute(ErrorMessageResourceName = "ValidateEmailNotExist", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null, CheckForForgotPassword = true)]
    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessageResourceName = "ValidateInputEmail", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblEmailId", ResourceType = typeof(Resource))]
    public string EmailAddress { get; set; }

    public string LinkPrefix
    {
      get
      {
        return Helper.GetPathForSetPassword;
      }
    }

    public string ResetOrForgotPasswordLink
    {
      get
      {
        return string.Format("{0}", LinkPrefix);
      }
    }
  }
}
