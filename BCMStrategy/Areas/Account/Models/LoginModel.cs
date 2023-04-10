using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BCMStrategy.Resources;

namespace BCMStrategy.Areas.Account.Models
{
  public class LoginModel
  {
    /// <summary>
    /// Gets or sets the name of the Login Model
    /// </summary>
    /// <value>The name of the User</value>.
    [DataType(DataType.EmailAddress)]
    [EmailAddress(ErrorMessageResourceName = "ValidateEmailAddress", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource))]
    [MaxLength(100, ErrorMessageResourceName = "ValidateMaxLenField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblEmailId", ResourceType = typeof(Resource))]
    public string Username { get; set; }

    /// <summary>
    /// Gets or sets the Password of User
    /// </summary>
    /// <value>The Password of the User</value>.
    [Required]
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this RememberMe is selected.
    /// </summary>
    /// <value>
    /// RememberMe  <c>true</c> if user selected remember me; otherwise, <c>false</c>.
    /// </value>.
    public bool RememberMe { get; set; }

    public bool IsDirectLogin { get; set; }
  }

  public class LoginResult
  {
    public bool Success { get; set; }

    public string ResponseMessage { get; set; }

    public SignInResult Result { get; set; }
  }
}