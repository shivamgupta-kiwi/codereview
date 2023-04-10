using System.ComponentModel.DataAnnotations;
using BCMStrategy.Resources;
using Newtonsoft.Json;
using BCMStrategy.Data.Abstract.CustomValidation;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class UserModel
  {
    private string _userMasterHashId;

    [JsonIgnore]
    public int UserId { get; set; }

    public string UserMasterHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_userMasterHashId))
        {
          _userMasterHashId = this.UserId == 0 ? string.Empty : this.UserId.ToEncrypt();
        }
        return _userMasterHashId;
      }
      set
      {
        _userMasterHashId = value;
      }
    }

    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [RegularExpression(@"^[ A-Za-z0-9_@.,/#&+-]{2,100}$", ErrorMessageResourceName = "ValidationLength_2_100_String", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblFirstName", ResourceType = typeof(Resource))]
    
    public string FirstName { get; set; }

    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [RegularExpression(@"^[ A-Za-z0-9_@.,/#&+-]{2,100}$", ErrorMessageResourceName = "ValidationLength_2_100_String", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblLastName", ResourceType = typeof(Resource))]
    public string LastName { get; set; }

    [RegularExpression(@"^[ A-Za-z0-9_@.,/#&+-]{1,100}$", ErrorMessageResourceName = "ValidationLength_1_100_String", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblMiddleName", ResourceType = typeof(Resource))]
    public string MiddleName { get; set; }

    [IsEmailIdExistAttribute(ErrorMessageResourceName = "ValidateAlreadyExist", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    //[StringLength(100, ErrorMessageResourceName = "ValidateMinMaxLength", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null, MinimumLength = 3)]
    [RegularExpression("^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$", ErrorMessageResourceName = "ValidateInputEmail", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblEmailId", ResourceType = typeof(Resource))]
    public string EmailAddress { get; set; }

    private string _countryMasterHashId;

    public string CompanyName { get; set; }

    public string Designation { get; set; }

    [JsonIgnore]
    public int CountryMasterId { get; set; }

    [Required(ErrorMessageResourceName = "ValidateRequireDropDown", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblCountry", ResourceType = typeof(Resource))]
    public string CountryMasterHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_countryMasterHashId))
        {
          _countryMasterHashId = this.CountryMasterId == 0 ? string.Empty : this.CountryMasterId.ToEncrypt();
        }
        return _countryMasterHashId;
      }
      set
      {
        _countryMasterHashId = value;
      }
    }


    [RegularExpression(@"^[ A-Za-z0-9_@.,/#&+-]{2,500}$", ErrorMessageResourceName = "ValidationLength_2_500_String", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    public string Address { get; set; }

    public string UserType { get; set; }
    [RegularExpression(@"^[ A-Za-z0-9_@.,/#&+-]{2,100}$", ErrorMessageResourceName = "ValidationLength_2_100_String", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    public string State { get; set; }

    [RegularExpression(@"^[ A-Za-z0-9_@.,/#&+-]{2,100}$", ErrorMessageResourceName = "ValidationLength_2_100_String", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    public string City { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\s]{4,50}$", ErrorMessageResourceName = "ValidationLength_5_50_String", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    public string ZipCode { get; set; }

    [JsonIgnore]
    public bool Active { get; set; }

    private string _status;

    public string Status
    {
      get
      {
        if (string.IsNullOrEmpty(_status))
        {
          _status = this.Active ? Enums.Status.Active.ToString() : Enums.Status.Inactive.ToString();
        }
        return _status;
      }
      set
      {
        _status = value;
      }
    }
    public string OldStatus { get; set; }
    
    [JsonIgnore]
    public string LinkPrefix
    {
      get
      {
        return Helper.GetPathForSetPassword;
      }
    }

    [JsonIgnore]
    public string ResetOrForgotPasswordLink
    {
      get
      {
        return string.Format("{0}", LinkPrefix);
      }
    }

    public int? DefaultLexicon { get; set; }
  }
}