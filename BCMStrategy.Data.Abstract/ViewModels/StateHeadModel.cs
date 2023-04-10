using BCMStrategy.Data.Abstract.CustomValidation;
using BCMStrategy.Resources;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class StateHeadModel : DesignationProperty
  {
    private string _stateHeadMasterHashId;

    [JsonIgnore]
    public int StateHeadMasterId { get; set; }

    public string StateHeadMasterHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_stateHeadMasterHashId))
        {
          _stateHeadMasterHashId = this.StateHeadMasterId == 0 ? string.Empty : this.StateHeadMasterId.ToEncrypt();
        }
        return _stateHeadMasterHashId;
      }
      set
      {
        _stateHeadMasterHashId = value;
      }
    }

    private string _countryMasterHashId;

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

    public string CountryName { get; set; }

    public string StateHeadName { get; set; }

    [Display(Name = "LblFirstName", ResourceType = typeof(Resource))]
    [RegularExpression(@"^[a-zA-Z\s0-9.-]{2,400}$", ErrorMessageResourceName = "ValidationLength_2_400_String", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    public string FirstName { get; set; }

    [IsHeadStateExistAttribute(ErrorMessageResourceName = "HeadOfStateExist", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblLastName", ResourceType = typeof(Resource))]
    [RegularExpression(@"^[a-zA-Z\s0-9.-]{2,400}$", ErrorMessageResourceName = "ValidationLength_2_400_String", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    public string LastName { get; set; }
  }

  public class DesignationProperty
  {
    private string _designationMasterHashId;

    [JsonIgnore]
    public int DesignationMasterId { get; set; }

    [Required(ErrorMessageResourceName = "ValidateRequireDropDown", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblDesignationType", ResourceType = typeof(Resource))]
    public string DesignationHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_designationMasterHashId))
        {
          _designationMasterHashId = this.DesignationMasterId == 0 ? string.Empty : this.DesignationMasterId.ToEncrypt();
        }
        return _designationMasterHashId;
      }
      set
      {
        _designationMasterHashId = value;
      }
    }

    public string DesignationName { get; set; }
  }
}