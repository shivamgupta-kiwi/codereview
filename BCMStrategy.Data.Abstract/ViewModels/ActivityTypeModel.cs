using BCMStrategy.Data.Abstract.CustomValidation;
using BCMStrategy.Resources;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class ActivityTypeModel
  {
    private string _activityTypeMasterHashId;

    [JsonIgnore]
    public int ActivityTypeMasterId { get; set; }

    public string ActivityTypeMasterHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_activityTypeMasterHashId))
        {
          _activityTypeMasterHashId = this.ActivityTypeMasterId == 0 ? string.Empty : this.ActivityTypeMasterId.ToEncrypt();
        }
        return _activityTypeMasterHashId;
      }
      set
      {
        _activityTypeMasterHashId = value;
      }
    }

    private string _metadataTypeMasterHashId;

    [JsonIgnore]
    public int MetadataTypeMasterId { get; set; }

    [Required(ErrorMessageResourceName = "ValidateRequireDropDown", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblMetadataTypes", ResourceType = typeof(Resource))]
    public string MetadataTypeMasterHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_metadataTypeMasterHashId))
        {
          _metadataTypeMasterHashId = this.MetadataTypeMasterId == 0 ? string.Empty : this.MetadataTypeMasterId.ToEncrypt();
        }
        return _metadataTypeMasterHashId;
      }
      set
      {
        _metadataTypeMasterHashId = value;
      }
    }

    public string MetaData { get; set; }

    [IsActivityTypeExistAttribute(ErrorMessageResourceName = "ValidateActivityTypeExist", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblActivityType", ResourceType = typeof(Resource))]
    public string ActivityName { get; set; }

    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblActivityValue", ResourceType = typeof(Resource))]
    [Range(0.01, 100.00, ErrorMessageResourceName = "ValidationDecimalValue", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    public string ActivityValue { get; set; }

    public string WebsiteType { get; set; }

    [CheckDuplicateStringAttribute(ErrorMessageResourceName = "ValidateDuplicateValue", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    //[Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [StringLength(200, ErrorMessageResourceName = "ValidationLength_200", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblRelatedActvityType", ResourceType = typeof(Resource))]
    public string RelatedActvityType { get; set; }

    public List<string> RelatedActvityTypeList { get; set; }
    public string RelatedActvityTypeDisplay
    {
      get
      {
        if (RelatedActvityTypeList != null)
        {
          return string.Join(", ", RelatedActvityTypeList);
        }
        else
        {
          return "";
        }
      }
    }

		public string ColorCode { get; set; }

  }
}
