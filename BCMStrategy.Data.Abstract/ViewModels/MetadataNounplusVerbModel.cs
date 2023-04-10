using BCMStrategy.Data.Abstract.CustomValidation;
using BCMStrategy.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class MetadataNounplusVerbModel
  {
    private string _metadataNounplusVerbMasterHashId;

    [JsonIgnore]
    public int MetadataNounplusVerbMasterId { get; set; }

    public string MetadataNounplusVerbMasterHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_metadataNounplusVerbMasterHashId))
        {
          _metadataNounplusVerbMasterHashId = this.MetadataNounplusVerbMasterId == 0 ? string.Empty : this.MetadataNounplusVerbMasterId.ToEncrypt();
        }
        return _metadataNounplusVerbMasterHashId;
      }
      set
      {
        _metadataNounplusVerbMasterHashId = value;
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

    public string WebsiteType { get; set; }


    [Required(ErrorMessageResourceName = "ValidateRequireDropDown", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblMetadataNoun", ResourceType = typeof(Resource))]
    public string MetadataDynamicNounplusVerb { get; set; }


    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblMetadataNoun", ResourceType = typeof(Resource))]
    public string Noun { get; set; }

    [IsMetadataVerbExistAttribute(ErrorMessageResourceName = "ValidateActivityTypeExist", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [IsMetadataNounVerbDuplicateAttribute(ErrorMessageResourceName = "ValidateActivityTypeExist", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblMetadataVerb", ResourceType = typeof(Resource))]
    public string Verb { get; set; }

    public bool IsHardCoded { get; set; }

    private string _status;
    public string Status
    {
      get
      {
        if (string.IsNullOrEmpty(_status))
        {
          _status = this.IsHardCoded ? Enums.Status.Yes.ToString() : Enums.Status.No.ToString();
        }
        return _status;
      }
      set
      {
        _status = value;
      }
    }

    private string _activityTypeMasterHashId;

    [JsonIgnore]
    public  int ActivityTypeMasterId { get; set; }
    [IsNounPlusVerbActivityTypeAttribute(ErrorMessageResourceName = "ValidateMetadataPhrasesExist", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
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

    public string ActivityType { get; set; }

    public string ActivityValue { get; set; }
  }
}
