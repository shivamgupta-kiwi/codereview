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
  public class SchedulerModel
  {
    private string _schedulerMasterHashId;
    [JsonIgnore]
    public int SchedulerMasterId { get; set; }

    public string SchedulerMasterHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_schedulerMasterHashId))
        {
          _schedulerMasterHashId = this.SchedulerMasterId == 0 ? string.Empty : this.SchedulerMasterId.ToEncrypt();
        }
        return _schedulerMasterHashId;
      }
      set
      {
        _schedulerMasterHashId = value;
      }
    }

    private string _frequencyTypeMasterHashId;


    public int FrequencyTypeMasterId { get; set; }

    [Required(ErrorMessageResourceName = "ValidateRequireDropDown", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblFrequencyType", ResourceType = typeof(Resource))]
    public string FrequencyTypeMasterHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_frequencyTypeMasterHashId))
        {
          _frequencyTypeMasterHashId = this.FrequencyTypeMasterId == 0 ? string.Empty : this.FrequencyTypeMasterId.ToEncrypt();
        }
        return _frequencyTypeMasterHashId;
      }
      set
      {
        _frequencyTypeMasterHashId = value;
      }
    }

    public string FrequencyType { get; set; }

    [IsSchedulerExistAttribute(ErrorMessageResourceName = "ValidateSchedulerExist", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Required(ErrorMessageResourceName = "ValidateRequireDropDown", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblWebsiteTypes", ResourceType = typeof(Resource))]
    public string WebsiteType { get; set; }

    public string Description { get; set; }

  
    ////[Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    ////[Display(Name = "LblStartDate", ResourceType = typeof(Resource))]
    public string StartDate { get; set; }

    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblStartDate", ResourceType = typeof(Resource))]
    public string StartDateFinal
    {
      get
      {
        DateTime dDate;
        return !DateTime.TryParse(StartDate, out dDate) ? string.Empty : StartDate;
      }
    }


    ////[EndDateGreaterThanStartDate(ErrorMessageResourceName = "ValidateEnddateGreaterthenStartdate", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    public string EndDate { get; set; }

    [EndDateGreaterThanStartDateAttribute(ErrorMessageResourceName = "ValidateEnddateGreaterthenStartdate", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    public string EndDateFinal
    {
      get
      {
        DateTime dDate;
        return !DateTime.TryParse(EndDate, out dDate) ? string.Empty : EndDate;
      }
    }

    public bool IsEnabled { get; set; }
    private string _status;
    public string Status
    {
      get
      {
        if (string.IsNullOrEmpty(_status))
        {
          _status = this.IsEnabled ? Enums.Status.Yes.ToString() : Enums.Status.No.ToString();
        }
        return _status;
      }
      set
      {
        _status = value;
      }
    }

    ////[Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    ////[Display(Name = "LblStartTime", ResourceType = typeof(Resource))]
    public string StartTime { get; set; }

    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblStartTime", ResourceType = typeof(Resource))]
    public string StartTimeFinal
    {
      get
      {

        DateTime dDate;
        return !DateTime.TryParse(StartTime,out dDate) ? string.Empty : StartTime;
         
      }
    }
    
    [Required(ErrorMessageResourceName = "ValidateRequiredField", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [Display(Name = "LblRepeatEvery", ResourceType = typeof(Resource))]
    [Range(1, 24, ErrorMessageResourceName = "ValidationDaysWeeks", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    [ValidateMinimumHoursAttribute(ErrorMessageResourceName = "ValidateEndTimeGreaterthenStartTime", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    public string RepeatEveryHour { get; set; }


    public bool Sunday { get; set; }
    public bool Monday { get; set; }
    public bool Tuesday { get; set; }
    public bool Wednesday { get; set; }
    public bool Thursday { get; set; }
    public bool Friday { get; set; }
    public bool Saturday { get; set; }

    [Required(ErrorMessageResourceName = "LblCheckboxValidation", ErrorMessageResourceType = typeof(Resource), ErrorMessage = null)]
    public string weekdaysCheckbox { get; set; }


    public string RecurEvery { get; set; }

    public string Details { get; set; }

    [JsonIgnore]
    public int FrequencyTypeValue { get; set; }
    public string Name { get; set; }
    [JsonIgnore]
    public Nullable<System.DateTime> ModifiedDate { get; set; }

    [JsonIgnore]
    public bool IsDeleted { get; set; }

    public string CreatedBy { get; set; }

    public string ModifiedBy { get; set; }
  }
}
