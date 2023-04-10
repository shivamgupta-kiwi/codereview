using BCMStrategy.Data.Abstract.CustomValidation;
using BCMStrategy.Resources;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class HeadStateImportModel
  {
    private string _headStateMasterHashId;

    [JsonIgnore]
    public int HeadStateMasterId { get; set; }


    public string HeadStateMasterHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_headStateMasterHashId))
        {
          _headStateMasterHashId = this.HeadStateMasterId == 0 ? string.Empty : this.HeadStateMasterId.ToEncrypt();
        }
        return _headStateMasterHashId;
      }
      set
      {
        _headStateMasterHashId = value;
      }
    }

    private string _countryMasterHashId;

    [JsonIgnore]
    public int CountryMasterId{ get; set; }


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
    [IsCountryExistAttribute(ErrorMessageResourceName = "ValidateCountryExist", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblCountry", ResourceType = typeof(Resource))]
    public string CountryName { get; set; }

    public string HeadStateName { get; set; }
    [IsDesignationExistAttribute(ErrorMessageResourceName = "ValidDesignationExist", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblDesignation", ResourceType = typeof(Resource))]
    public string DesignationName { get; set; }

    //[Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblFirstName", ResourceType = typeof(Resource))]
    [RegularExpression(@"^[a-zA-Z\s0-9.-]{2,400}$", ErrorMessageResourceName = "ValidationLength_2_400_String", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    public string FirstName { get; set; }

    [IsHeadStateForImportExistAttribute(ErrorMessageResourceName = "ValidateLastNameAlreadExist", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblLastName", ResourceType = typeof(Resource))]
    [RegularExpression(@"^[a-zA-Z\s0-9.-]{2,400}$", ErrorMessageResourceName = "ValidationLength_2_400_String", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    public string LastName { get; set; }

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