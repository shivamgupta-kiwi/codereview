using BCMStrategy.Data.Abstract.CustomValidation;
using BCMStrategy.Resources;
using System.ComponentModel.DataAnnotations;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class ChangePasswordModel
  {
    
    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [MaxLength(100, ErrorMessageResourceName = "ValidateMaxLenField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [ValidateOldPasswordAttribute(ErrorMessageResourceName = "ValidateOldPassword", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblOldPassword", ResourceType = typeof(Resource))]
    [DataType(DataType.Password)]
    public string OldPassword { get; set; }
    
    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [MaxLength(100, ErrorMessageResourceName = "ValidateMaxLenField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [RegularExpression(@"^.*(?=.{8,})(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[@@#$%^&+=~!_]).*$", ErrorMessageResourceName = "ValidationPassword", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [CompareOldNNewPasswordAttribute(ErrorMessageResourceName = "ValidateOldNNewPassword", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblNewPassword", ResourceType = typeof(Resource))]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [MaxLength(100, ErrorMessageResourceName = "ValidateMaxLenField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblConfirmPassword", ResourceType = typeof(Resource))]
    [Compare("NewPassword", ErrorMessageResourceName = "ValidateCompare", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }
    
  }
}
