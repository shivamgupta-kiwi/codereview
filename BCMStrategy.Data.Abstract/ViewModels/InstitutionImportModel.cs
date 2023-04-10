using BCMStrategy.Data.Abstract.CustomValidation;
using BCMStrategy.Resources;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class InstitutionCsvImportModel
  {
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

    private string _institutionMasterHashId;

    [JsonIgnore]
    public int InstitutionMasterId { get; set; }

    public string InstitutionMasterHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_institutionMasterHashId))
        {
          _institutionMasterHashId = this.InstitutionMasterId == 0 ? string.Empty : this.InstitutionMasterId.ToEncrypt();
        }
        return _institutionMasterHashId;
      }
      set
      {
        _institutionMasterHashId = value;
      }
    }

    private string _countryMasterHashId;

    [JsonIgnore]
    public int CountryMasterId { get; set; }

    [RegularExpression(@"^[a-zA-Z\s0-9_./&+-]{2,500}$", ErrorMessageResourceName = "ValidationLength_2_500_String", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblEntityName", ResourceType = typeof(Resource))]
    public string EntityName { get; set; }

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

    [IsCountryExistAttribute(ErrorMessageResourceName = "ValidateCountryExist", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblCountry", ResourceType = typeof(Resource))]
    public string CountryName { get; set; }

    [IsInstitutionsImportExistAttribute(ErrorMessageResourceName = "ValidateInstitutionAlreadExist", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [RegularExpression(@"^[a-zA-Z\s0-9_./&+-]{2,500}$", ErrorMessageResourceName = "ValidationLength_2_500_String", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblInstitutions", ResourceType = typeof(Resource))]
    public string InstitutionName { get; set; }

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

    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    public bool IsEuropeanUnion { get; set; }

    public string IsEuropeanUnionStr
    {
      get
      {
        return this.IsEuropeanUnion ? Enums.Status.Yes.ToString() : Enums.Status.No.ToString();
      }

    }
  }
}
