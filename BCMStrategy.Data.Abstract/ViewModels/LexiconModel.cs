using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using BCMStrategy.Resources;
using BCMStrategy.Data.Abstract.CustomValidation;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class LexiconModel
  {
    private string _lexiconIssueMasterHashId;

    [JsonIgnore]
    public int LexiconeIssueMasterId { get; set; }


    public string LexiconeIssueMasterHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_lexiconIssueMasterHashId))
        {
          _lexiconIssueMasterHashId = this.LexiconeIssueMasterId == 0 ? string.Empty : this.LexiconeIssueMasterId.ToEncrypt();
        }
        return _lexiconIssueMasterHashId;
      }
      set
      {
        _lexiconIssueMasterHashId = value;
      }
    }

    private string _lexiconTypeMasterHashId;

    public int LexiconeTypeMasterId { get; set; }

    [Required(ErrorMessageResourceName = "ValidateRequireDropDown", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblLexiconType", ResourceType = typeof(Resource))]
    public string LexiconeTypeMasterHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_lexiconTypeMasterHashId))
        {
          _lexiconTypeMasterHashId = this.LexiconeTypeMasterId == 0 ? string.Empty : this.LexiconeTypeMasterId.ToEncrypt();
        }
        return _lexiconTypeMasterHashId;
      }
      set
      {
        _lexiconTypeMasterHashId = value;
      }
    }

    [IsLexiconExistAttribute(ErrorMessageResourceName = "ValidateLexiconExist", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblLexicon", ResourceType = typeof(Resource))]
    public string LexiconIssue { get; set; }

    [CheckDuplicateStringAttribute(ErrorMessageResourceName = "ValidateDuplicateValue", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [StringLength(200, ErrorMessageResourceName = "ValidationLength_200", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblCombinationValue", ResourceType = typeof(Resource))]
    public string CombinationValue { get; set; }
   
    public bool IsNested { get; set; }
    private string _status;
    public string Status
    {
      get
      {
        if (string.IsNullOrEmpty(_status))
        {
          _status = this.IsNested  ? Enums.Status.Yes.ToString() : Enums.Status.No.ToString();
        }
        return _status;
      }
      set
      {
        _status = value;
      }
    }
    public List<string> Linker { get; set; }
    public string Linkers
    {
      get
      {
        if (Linker != null)
        {
          return string.Join(" = ", Linker);
        }
        else
        {
          return "";
        }
      }
    }

    public int? TotalRecordCount { get; set; }

    [CheckDuplicateStringAttribute(ErrorMessageResourceName = "ValidateDuplicateValue", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [StringLength(200, ErrorMessageResourceName = "ValidationLength_200", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblLinker", ResourceType = typeof(Resource))]
    public string LexiconLinkers { get; set; }

    public string LexiconType { get; set; }

    public int LexiconTypeId { get; set; }


  }
}
