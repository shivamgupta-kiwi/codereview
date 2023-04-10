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
  public class InternationalOrganizationModel
  {
    private string _internaltionalOrganizationMasterHashId;

    [JsonIgnore]
    public int InternaltionalOrganizationMasterId { get; set; }

    public string InternaltionalOrganizationMasterHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_internaltionalOrganizationMasterHashId))
        {
          _internaltionalOrganizationMasterHashId = this.InternaltionalOrganizationMasterId == 0 ? string.Empty : this.InternaltionalOrganizationMasterId.ToEncrypt();
        }
        return _internaltionalOrganizationMasterHashId;
      }
      set
      {
        _internaltionalOrganizationMasterHashId = value;
      }
    }

    private string _designationMasterHashId;

    [JsonIgnore]
    public int DesignationMasterId { get; set; }
    //[Required(ErrorMessageResourceName = "ValidateRequireDropDown", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblDesignationType", ResourceType = typeof(Resource))]
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

    public string DesignationName { get; set; }

    //[IsInternationOrganizationExist(ErrorMessageResourceName = "ValidateInternationOrgAlreadExist", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [RegularExpression(@"^[a-zA-Z\s0-9.-]{2,400}$", ErrorMessageResourceName = "ValidationLength_2_400_String", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblOrganizationName", ResourceType = typeof(Resource))]
    public string OrganizationName { get; set; }
    
    [Display(Name = "LblFirstName", ResourceType = typeof(Resource))]
    [RegularExpression(@"^[a-zA-Z\s0-9.-]{2,400}$", ErrorMessageResourceName = "ValidationLength_2_400_String", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    public string LeaderFirstName { get; set; }

    [IsInternationOrganizationLastNameExistAttribute(ErrorMessageResourceName = "ValidateOrgLastNameExist", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [RegularExpression(@"^[a-zA-Z\s0-9.-]{2,100}$", ErrorMessageResourceName = "ValidationLength_2_100_String", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblLastName", ResourceType = typeof(Resource))]
    public string LeaderLastName { get; set; }

    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblEntityName", ResourceType = typeof(Resource))]
    [RegularExpression(@"^[a-zA-Z\s0-9.-]{2,100}$", ErrorMessageResourceName = "ValidationLength_2_100_String", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    public string EntityName { get; set; }

    public string LeaderName { get; set; }

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
  }
}
