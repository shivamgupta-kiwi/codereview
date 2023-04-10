using BCMStrategy.Data.Abstract.CustomValidation;
using BCMStrategy.Resources;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class InstitutionModel
  {
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

    [IsCountryEuropeanUnionAttribute(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
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

    [IsInstitutionsExistAttribute(ErrorMessageResourceName = "ValidateInstitutionExist", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [RegularExpression(@"^[a-zA-Z\s0-9_./&+-]{2,500}$", ErrorMessageResourceName = "ValidationLength_2_500_String", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblInstitutions", ResourceType = typeof(Resource))]
    public string InstitutionsName { get; set; }

    public bool IsEuropeanUnion { get; set; }

    public string IsEuropeanUnionString { get; set; }
    /// <summary>
    /// Entity Name
    /// </summary>
    public string EntityName { get; set; }

  }
}
