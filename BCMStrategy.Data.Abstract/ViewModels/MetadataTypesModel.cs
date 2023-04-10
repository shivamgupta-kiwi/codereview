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
  public class MetadataTypesModel
  {
    private string _metadataTypesMasterHashId;

    [JsonIgnore]
    public int MetadataTypesMasterId { get; set; }

    public string MetadataTypesMasterHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_metadataTypesMasterHashId))
        {
          _metadataTypesMasterHashId = this.MetadataTypesMasterId == 0 ? string.Empty : this.MetadataTypesMasterId.ToEncrypt();
        }
        return _metadataTypesMasterHashId;
      }
      set
      {
        _metadataTypesMasterHashId = value;
      }
    }

    private string _websiteTypeMasterHashId;

    [JsonIgnore]
    public int WebsiteTypeMasterId { get; set; }

    [Required(ErrorMessageResourceName = "ValidateRequireDropDown", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblWebsiteTypes", ResourceType = typeof(Resource))]
    public string WebsiteTypeHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_websiteTypeMasterHashId))
        {
          _websiteTypeMasterHashId = this.WebsiteTypeMasterId == 0 ? string.Empty : this.WebsiteTypeMasterId.ToEncrypt();
        }
        return _websiteTypeMasterHashId;
      }
      set
      {
        _websiteTypeMasterHashId = value;
      }
    }

    public string WebsiteType { get; set; }

    [IsMetadataTypesExistAttribute(ErrorMessageResourceName = "ValidateInstitutionExist", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblMetadata", ResourceType = typeof(Resource))]
    public string MetaData { get; set; }
  

    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Range(0.01, 100.00, ErrorMessageResourceName = "ValidationDecimalValue", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblMetadataValue", ResourceType = typeof(Resource))]
    public string MetaDataValue { get; set; }
   
    public Nullable<bool> IsActivityTypeExist { get; set; }

    private string _status;
    public string Status
    {
      get
      {
        if (string.IsNullOrEmpty(_status))
        {
          _status = this.IsActivityTypeExist == true ? Enums.Status.Yes.ToString() : Enums.Status.No.ToString();
        }
        return _status;
      }
      set
      {
        _status = value;
      }
    }

  }

	public class ChartColors
	{
		public string ProprietaryName { get; set; }

		public string ProprietaryColor { get; set; }
	}
}
