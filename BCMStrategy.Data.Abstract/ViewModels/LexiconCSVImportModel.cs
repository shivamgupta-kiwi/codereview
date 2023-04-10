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
  public class LexiconCsvImportModel
  {
    private string _lexiconIssueMasterHashId;

    [JsonIgnore]
    public int LexiconIssueMasterId { get; set; }


    public string LexiconIssueMasterHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_lexiconIssueMasterHashId))
        {
          _lexiconIssueMasterHashId = this.LexiconIssueMasterId == 0 ? string.Empty : this.LexiconIssueMasterId.ToEncrypt();
        }
        return _lexiconIssueMasterHashId;
      }
      set
      {
        _lexiconIssueMasterHashId = value;
      }
    }

    private string _lexiconTypeMasterHashId;

    public int LexiconTypeMasterId { get; set; }

    [Required(ErrorMessageResourceName = "ValidateRequireDropDown", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblLexiconType", ResourceType = typeof(Resource))]
    public string LexiconTypeMasterHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_lexiconTypeMasterHashId))
        {
          _lexiconTypeMasterHashId = this.LexiconTypeMasterId == 0 ? string.Empty : this.LexiconTypeMasterId.ToEncrypt();
        }
        return _lexiconTypeMasterHashId;
      }
      set
      {
        _lexiconTypeMasterHashId = value;
      }
    }

    [IsLexiconImportExistAttribute(ErrorMessageResourceName = "ValidateLexiconExist", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblLexicon", ResourceType = typeof(Resource))]
    public string LexiconIssue { get; set; }

    [CheckDuplicateStringAttribute(ErrorMessageResourceName = "ValidateDuplicateValue", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
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

    [CheckDuplicateStringAttribute(ErrorMessageResourceName = "ValidateDuplicateValue", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblLinker", ResourceType = typeof(Resource))]
    public string LexiconLinkers { get; set; }

    public string LexiconType { get; set; }

    public int LexiconTypeId { get; set; }
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
