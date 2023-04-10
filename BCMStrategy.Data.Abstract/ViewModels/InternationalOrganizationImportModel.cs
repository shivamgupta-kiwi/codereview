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
  public class InternationalOrganizationImportModel
  {
    private string _internationalOrganizationMasterHashId;

    [JsonIgnore]
    public int InternationalOrganizationMasterId { get; set; }


    public string InternationalOrganizationMasterHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_internationalOrganizationMasterHashId))
        {
          _internationalOrganizationMasterHashId = this.InternationalOrganizationMasterId == 0 ? string.Empty : this.InternationalOrganizationMasterId.ToEncrypt();
        }
        return _internationalOrganizationMasterHashId;
      }
      set
      {
        _internationalOrganizationMasterHashId = value;
      }
    }

    private string _designzationMasterHashId;

    [JsonIgnore]
    public int DesignationMasterId { get; set; }


    public string DesignationMasterHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_designzationMasterHashId))
        {
          _designzationMasterHashId = this.DesignationMasterId == 0 ? string.Empty : this.DesignationMasterId.ToEncrypt();
        }
        return _designzationMasterHashId;
      }
      set
      {
        _designzationMasterHashId = value;
      }
    }

    [IsDesignationExistAttribute(ErrorMessageResourceName = "ValidDesignationExist", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    public string DesignationName { get; set; }

    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [RegularExpression(@"^[a-zA-Z\s0-9.-]{2,400}$", ErrorMessageResourceName = "ValidationLength_2_400_String", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblOrganizationName", ResourceType = typeof(Resource))]
    public string OrganizationName { get; set; }
    
    public string LeaderName { get; set; }

    [RegularExpression(@"^[a-zA-Z\s0-9.-]{2,400}$", ErrorMessageResourceName = "ValidationLength_2_400_String", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblFirstName", ResourceType = typeof(Resource))]
    public string LeaderFirstName { get; set; }

    [IsInternationOrganizationLastNameExistImportAttribute(ErrorMessageResourceName = "ValidateOrgLastNameExist", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [RegularExpression(@"^[a-zA-Z\s0-9.-]{2,100}$", ErrorMessageResourceName = "ValidationLength_2_100_String", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblLastName", ResourceType = typeof(Resource))]
    public string LeaderLastName { get; set; }

    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [RegularExpression(@"^[a-zA-Z\s0-9.-]{2,100}$", ErrorMessageResourceName = "ValidationLength_2_100_String", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblEntityName", ResourceType = typeof(Resource))]
    public string EntityName { get; set; }

    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblIsMultiLateral", ResourceType = typeof(Resource))]
    public string IsMultiLateralStr { get; set; }

    public bool IsMultiLateral { get; set; }
    private string _status;
    public string Status
    {
      get
      {
        if (string.IsNullOrEmpty(_status))
        {
          _status = this.IsMultiLateral ? Enums.Status.Yes.ToString() : Enums.Status.No.ToString();
        }
        return _status;
      }
      set
      {
        _status = value;
      }
    }

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
