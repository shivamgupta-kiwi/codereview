using BCMStrategy.Data.Abstract.CustomValidation;
using BCMStrategy.Resources;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class PolicyMakerModel : DesignationProperty
  {
    private string _policyMakerHashId;

    [JsonIgnore]
    public int PolicyMakerId { get; set; }

    public string PolicyMakerHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_policyMakerHashId))
        {
          _policyMakerHashId = this.PolicyMakerId == 0 ? string.Empty : this.PolicyMakerId.ToEncrypt();
        }
        return _policyMakerHashId;
      }
      set
      {
        _policyMakerHashId = value;
      }
    }

    private string _countryMasterHashId;

    [JsonIgnore]
    public int? CountryMasterId { get; set; }

    ////[Required(ErrorMessageResourceName = "ValidateRequireDropDown", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    ////[Display(Name = "LblCountry", ResourceType = typeof(Resource))]
    public string CountryMasterHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_countryMasterHashId))
        {
          _countryMasterHashId = (this.CountryMasterId == 0 || this.CountryMasterId == null) ? string.Empty : this.CountryMasterId.ToEncrypt();
        }
        return _countryMasterHashId;
      }
      set
      {
        _countryMasterHashId = value;
      }
    }

    public string CountryName { get; set; }

    private string _institutionTypeMasterHashId;

    [JsonIgnore]
    public int InstitutionTypeMasterId { get; set; }

    [Required(ErrorMessageResourceName = "ValidateRequireDropDown", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblInstitutionTypes", ResourceType = typeof(Resource))]
    public string InstitutionTypeHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_institutionTypeMasterHashId))
        {
          _institutionTypeMasterHashId = this.InstitutionTypeMasterId == 0 ? string.Empty : this.InstitutionTypeMasterId.ToEncrypt();
        }
        return _institutionTypeMasterHashId;
      }
      set
      {
        _institutionTypeMasterHashId = value;
      }
    }

    public string InstitutionType { get; set; }

    ////private string _designationMasterHashId;

    ////[JsonIgnore]
    ////public int DesignationMasterId { get; set; }

    ////[Required(ErrorMessageResourceName = "ValidateRequireDropDown", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    ////[Display(Name = "LblDesignationType", ResourceType = typeof(Resource))]
    ////public string DesignationHashId
    ////{
    ////  get
    ////  {
    ////    if (string.IsNullOrEmpty(_designationMasterHashId))
    ////    {
    ////      _designationMasterHashId = this.DesignationMasterId == 0 ? string.Empty : this.DesignationMasterId.ToEncrypt();
    ////    }
    ////    return _designationMasterHashId;
    ////  }
    ////  set
    ////  {
    ////    _designationMasterHashId = value;
    ////  }
    ////}

    ////public string DesignationName { get; set; }

    public string PolicyMakerName { get; set; }

    [RegularExpression(@"^[a-zA-Z\s0-9.-]{2,500}$", ErrorMessageResourceName = "ValidationLength_2_500_String", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblFirstName", ResourceType = typeof(Resource))]
    public string PolicyFirstName { get; set; }

    [IsPolicyLastNameExistAttribute(ErrorMessageResourceName = "ValidatePolicyLastNameExist", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [RegularExpression(@"^[a-zA-Z\s0-9.-]{2,500}$", ErrorMessageResourceName = "ValidationLength_2_500_String", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblLastName", ResourceType = typeof(Resource))]
    public string PolicyLastName { get; set; }

    [JsonIgnore]
    public string PolicyMakerFirstNLastName
    {
      get
      {
        return string.Format("{0} {1}", PolicyFirstName, PolicyLastName).Trim();
      }
    }

    [JsonIgnore]
    public string PolicyMakerDesignationNLName
    {
      get
      {
        if (!string.IsNullOrEmpty(DesignationName) && !string.IsNullOrEmpty(PolicyLastName))
        {
          return string.Format("{0} {1}", DesignationName.Trim(), PolicyLastName).Trim();
        }
        else
        {
          return string.Empty;
        }
      }
    }
  }
}