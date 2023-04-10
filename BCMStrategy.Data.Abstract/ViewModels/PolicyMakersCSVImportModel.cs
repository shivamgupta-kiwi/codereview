using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCMStrategy.Data.Abstract.CustomValidation;
using BCMStrategy.Resources;
using Newtonsoft.Json;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class PolicyMakersCsvImportModel
  {
    private string _policyMakerMasterHashId;

    [JsonIgnore]
    public int PolicyMakerMasterId { get; set; }


    public string PolicyMakerMasterHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_policyMakerMasterHashId))
        {
          _policyMakerMasterHashId = this.PolicyMakerMasterId == 0 ? string.Empty : this.PolicyMakerMasterId.ToEncrypt();
        }
        return _policyMakerMasterHashId;
      }
      set
      {
        _policyMakerMasterHashId = value;
      }
    }

    private string _institutionTypeMasterHashId;

    [JsonIgnore]
    public int InstitutionTypeMasterId { get; set; }

    public string InstitutionTypeMasterHashId
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

   
    private string _countryMasterHashId;

    [JsonIgnore]
    public int CountryMasterId { get; set; }


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

    private string _designationMasterHashId;

    [JsonIgnore]
    public int DesignationMasterId { get; set; }


    public string DesignationMasterHashId
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
    
    public string PolicyName { get; set; }

    [IsCountryExistAttribute(ErrorMessageResourceName = "ValidateCountryExist", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    ////[Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    ////[Display(Name = "LblCountry", ResourceType = typeof(Resource))]
    public string CountryName { get; set; }

    [IsDesignationExistAttribute(ErrorMessageResourceName = "ValidDesignationExist", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblDesignation", ResourceType = typeof(Resource))]
    public string DesignationName { get; set; }

    [RegularExpression(@"^[a-zA-Z\s0-9.-]{2,500}$", ErrorMessageResourceName = "ValidationLength_2_500_String", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblFirstName", ResourceType = typeof(Resource))]
    public string PolicyFirstName { get; set; }

    [IsPolicyLastNameImportExistAttribute(ErrorMessageResourceName = "ValidatePolicyLastNameExist", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [RegularExpression(@"^[a-zA-Z\s0-9.-]{2,500}$", ErrorMessageResourceName = "ValidationLength_2_500_String", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblLastName", ResourceType = typeof(Resource))]
    public string PolicyLastName { get; set; }

    public List<KeyValuePair<string, string>> ErrorModel { get; set; }

    public bool IsValidRecord
    {
      get
      {
        if (this.ErrorModel != null && this.ErrorModel.Count > 0)
          return false;
        else
          return true;
      }
    }
  }
}
