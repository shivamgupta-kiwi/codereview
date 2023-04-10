using System;
using System.Collections.Generic;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class ProcessDetailModel
  {
    public int ProcessId { get; set; }
    public string ScraperName { get; set; }
    public System.DateTime StartDateTime { get; set; }

    public List<EventsData> EventsList { get; set; }
  }

  public class EventsData
  {
    private string _processHashId;

    public int ProcessEventId { get; set; }

    public string ProcessHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_processHashId))
        {
          _processHashId = this.ProcessEventId == 0 ? string.Empty : this.ProcessEventId.ToEncrypt();
        }
        return _processHashId;
      }
      set
      {
        _processHashId = value;
      }
    }

    public int ProcessTypeId { get; set; }
    public Nullable<int> ProcessInstanceId { get; set; }

    ////public string StartDateTime { get; set; }
    ////public string EndDateTime { get; set; }
    public Nullable<decimal> TimeTaken { get; set; }

    public Nullable<int> PagesProcessed { get; set; }

    public DateTime? StartDateTime { get; set; }

    public string StartDateTimeString
    {
      get
      {
        if (StartDateTime.HasValue)
        {
          return (CommonUtilities.ToESTTimezone((DateTime)StartDateTime)).ToString("d-MMM-yy HH:mm:ss");
        }
        else
        {
          return string.Empty;
        }
      }
    }

    public DateTime? EndDateTime { get; set; }

    public string EndDateTimeString
    {
      get
      {
        if (EndDateTime.HasValue)
        {
          return (CommonUtilities.ToESTTimezone((DateTime)EndDateTime)).ToString("d-MMM-yy HH:mm:ss");
        }
        else
        {
          return string.Empty;
        }
      }
    }

    public string ScraperName { get; set; }

    public string status { get; set; }

    public string second { get; set; }
  }
}