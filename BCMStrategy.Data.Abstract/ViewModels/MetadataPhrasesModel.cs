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
  public class MetadataPhrasesModel
  {
    private string _metadataPhrasesMasterHashId;

    [JsonIgnore]
    public int MetadataPhrasesMasterId { get; set; }

    public string MetadataPhrasesMasterHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_metadataPhrasesMasterHashId))
        {
          _metadataPhrasesMasterHashId = this.MetadataPhrasesMasterId == 0 ? string.Empty : this.MetadataPhrasesMasterId.ToEncrypt();
        }
        return _metadataPhrasesMasterHashId;
      }
      set
      {
        _metadataPhrasesMasterHashId = value;
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

    [IsMetadataPhrasesExistAttribute(ErrorMessageResourceName = "ValidateMetadataPhrasesExist", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblMetadataPhrases", ResourceType = typeof(Resource))]
    public string MetadataPhrases { get; set; }

    public string WebsiteType { get; set; }

		private string _activityTypeMasterHashId;

		[JsonIgnore]
		public int ActivityTypeMasterId { get; set; }
    [IsPhraseActivityTypeAttribute(ErrorMessageResourceName = "ValidateMetadataPhrasesExist", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
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
