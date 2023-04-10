using BCMStrategy.DAL.Context;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Logger;
using BCMStrategy.Schedular.API;
using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using BCMStrategy.Resources;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Repository.Concrete;

namespace BCMStrategy.Schedular
{
  /// <summary>/// This scheduler will be invoke every minute to check whether entry is there in the queue or not.
  /// </summary>
  public class Program
  {

    /// <summary>
    /// Default Constructor
    /// </summary>
    protected Program()
    {

    }

    private static readonly EventLogger<Program> log = new EventLogger<Program>();

    static string mediaOfficialSectorPath = @ConfigurationManager.AppSettings["MediaOfficialSectorPath"];

		#region DECLARE VARIABLES
    ////private static IScheduler _schedular;

    ////private static IScheduler Scheduler
    ////{
    ////  get
    ////  {
    ////    if (_schedular == null)
    ////    {
    ////      _schedular = new SchedulerRepository();
    ////    }

    ////    return _schedular;
    ////  }
    ////}


    ////private static IProcessEvents _processEvents;

    ////private static IProcessEvents ProcessEvents
    ////{
    ////  get
    ////  {
    ////    if (_processEvents == null)
    ////    {
    ////      _processEvents = new ProcessEventsRepository();
    ////    }

    ////    return _processEvents;
    ////  }
    ////}
		#endregion

		/// <summary>
		/// Main method of the scheduler event to take place
		/// </summary>
		/// <param name="args"></param>
		private static void Main(string[] args)
    {
      try
      {
        WebApi api = new WebApi();

        ////string queueMessageEmailGeneration = api.GetMessageQueueData((int)Helper.SQSTypes.EmailGeneration);
        ////if (!string.IsNullOrEmpty(queueMessageEmailGeneration))
        ////{
        ////  Process pProcess = new Process();
        ////  pProcess.StartInfo.FileName = ConfigurationManager.AppSettings["EmailGenerationPath"];
        ////  pProcess.Start();
        ////  pProcess.PriorityClass = ProcessPriorityClass.Normal;
        ////}
        ////else {
        ////  DateTime datMaxtime = Scheduler.RetrieveMaxTime();
        ////  DateTime currentDateTime = Helper.GetCurrentDateTime();
        ////  if (datMaxtime < currentDateTime) {

        ////    if (!ProcessEvents.IsEmailGenerated(currentDateTime) &&
        ////           ProcessEvents.GetMessageCount(Helper.SQSTypes.EmailGeneration.ToString(), (int)Helper.SQSTypes.EmailGeneration) == 0)
        ////    {
        ////      ProcessEvents.SaveToSQS(Helper.SQSTypes.EmailGeneration.ToString(), (int)Helper.SQSTypes.EmailGeneration);
        ////    }
        ////  }
        ////}

				string queueMessage = api.GetMessageQueueData(1);


        if (queueMessage != string.Empty)
        {
          string[] scraperDetails = queueMessage.Split('-');

          int processId = scraperDetails[1] != null ? Convert.ToInt32(scraperDetails[1]) : 0;

          Events scraperEvents = new Events();

          scraperEvents.ProcessEventId = processId;
          scraperEvents.ProcessTypeId = Convert.ToInt32(Helper.ProcessType.WebCrawlerService);
          scraperEvents.StartDateTime = Helper.GetSystemCurrentDateTime();

          int result = api.SaveScraperEvent(scraperEvents);

          Process pProcess = new Process();
          pProcess.StartInfo.FileName = ConfigurationManager.AppSettings["ContentLoaderPath"];
          pProcess.StartInfo.Arguments = queueMessage;
          pProcess.Start();
          pProcess.PriorityClass = ProcessPriorityClass.Normal;

          scraperEvents = new Events();

          scraperEvents.Id = result;
          scraperEvents.ProcessEventId = processId;
          scraperEvents.EndDateTime = Helper.GetSystemCurrentDateTime();

          api.UpdateScraperEvent(scraperEvents);
        }
        else
        {
          //// Check whether Schedular has been updated or not. If updated then update the current schedular for Official Sector and Media Sector
          //// Code starts here
           
          List<SchedulerModel> schedulerList;
          using (BCMStrategyEntities db = new BCMStrategyEntities())
          {
            var query = db.schedular.Where(z => z.IsUpdated).GroupBy(s => s.Name).Select(g => g.OrderByDescending(c => c.Id).FirstOrDefault()) /*Where(x => !x.IsUpdated).*/
                .Select(x => new
                {
                  SchedulerMasterId = x.Id,
                  Name = x.Name,
                  Description = x.Description,
                  FrequencyTypeMasterId = x.FrequencyType,
                  IsDeleted = x.IsDeleted,
                  IsEnabled = x.IsEnabled,
                  StartDate = x.StartDate,
                  EndDate = x.EndDate,
                  StartTime = x.StartTime,
                  EndTime = x.EndTime,
                  RecurEvery = x.schedularfrequency.Where(a => a.SchedularId == x.Id).Select(s => s.RecurEvery).FirstOrDefault().ToString(),
                  Sunday = x.schedularfrequency.Where(a => a.SchedularId == x.Id).Select(s => s.Sunday).FirstOrDefault(),
                  Monday = x.schedularfrequency.Where(a => a.SchedularId == x.Id).Select(s => s.Monday).FirstOrDefault(),
                  Tuesday = x.schedularfrequency.Where(a => a.SchedularId == x.Id).Select(s => s.Tuesday).FirstOrDefault(),
                  Wednesday = x.schedularfrequency.Where(a => a.SchedularId == x.Id).Select(s => s.Wednesday).FirstOrDefault(),
                  Thursday = x.schedularfrequency.Where(a => a.SchedularId == x.Id).Select(s => s.Thursday).FirstOrDefault(),
                  Friday = x.schedularfrequency.Where(a => a.SchedularId == x.Id).Select(s => s.Friday).FirstOrDefault(),
                  Saturday = x.schedularfrequency.Where(a => a.SchedularId == x.Id).Select(s => s.Saturday).FirstOrDefault(),
                });
            var result = query.ToList();

            schedulerList = result
                .Select(x => new SchedulerModel()
                {
                  SchedulerMasterId = x.SchedulerMasterId,
                  Name = x.Name,
                  Description = x.Description,
                  FrequencyTypeMasterId = x.FrequencyTypeMasterId,
                  IsEnabled = x.IsEnabled,
                  IsDeleted = x.IsDeleted,
                  StartDate = x.StartDate.ToString("MM-dd-yyyy"),
                  EndDate = x.EndDate.HasValue ? x.EndDate.Value.ToString("MM-dd-yyyy HH:mm:ss ") : string.Empty,
                  StartTime = x.StartTime.HasValue ? DateTime.Today.Add(x.StartTime.Value).ToString("hh:mm tt") : string.Empty, //// StartTime = x.StartTime.HasValue ? CommonUtilities.ToESTTimezone(DateTime.Today.Add(x.StartTime.Value)).ToString("hh:mm tt") : string.Empty,
                  RecurEvery = x.RecurEvery,
                  Sunday = x.Sunday,
                  Monday = x.Monday,
                  Tuesday = x.Tuesday,
                  Wednesday = x.Wednesday,
                  Thursday = x.Thursday,
                  Friday = x.Friday,
                  Saturday = x.Saturday
                }).ToList();
          }
          foreach (var item in schedulerList)
          {
            if (!item.IsDeleted)
            {
              if (item.FrequencyTypeMasterId == 2)
              {
                CreateTaskForDaily(item.Name, item.Description, item.StartDate, item.EndDate, item.StartTime, Convert.ToInt16(item.RecurEvery), item.IsEnabled);
              }
              else if (item.FrequencyTypeMasterId == 3)
              {
                List<int> WeekDays = new List<int>();
                if (item.Sunday)
                {
                  WeekDays.Add(1);
                }
                if (item.Monday)
                {
                  WeekDays.Add(2);
                }
                if (item.Tuesday)
                {
                  WeekDays.Add(4);
                }
                if (item.Wednesday)
                {
                  WeekDays.Add(8);
                }
                if (item.Thursday)
                {
                  WeekDays.Add(16);
                }
                if (item.Friday)
                {
                  WeekDays.Add(32);
                }
                if (item.Saturday)
                {
                  WeekDays.Add(64);
                }
                CreateTaskForWeekly(WeekDays, item.Name, item.Description, item.StartDate, item.EndDate, item.StartTime, Convert.ToInt16(item.RecurEvery), item.IsEnabled);
              }
            }
            else
            {
              ////delete scheduler
              DeleteTask(item.Name);
            }

          }

          //// Code ends here
          
        }
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in Main method of Scheduler Process", ex, null);
        throw;
      }
    }
    private static void CreateTaskForDaily(string name, string desc, string startDate, string endDate, string startTime, short recureEvery, bool Enable)
    {
      using (TaskService ts = new TaskService())
      {
        string argName = "";
        if (name == Resources.Enums.Status.Official_Sector_Pages.ToString().Replace("_", " "))
        {
          argName = Resources.Enums.Status.OfficialSector_1.ToString().Replace("_", " ");
        }
        if (name == Resources.Enums.Status.Media_Pages.ToString().Replace("_", " "))
        {
          argName = Resources.Enums.Status.MediaSector_1.ToString().Replace("_", " ");
        }
        TaskDefinition td = ts.NewTask();
        td.RegistrationInfo.Author = WindowsIdentity.GetCurrent().Name;
        td.Principal.LogonType = TaskLogonType.S4U;
        td.RegistrationInfo.Description = desc;
        DailyTrigger daily = new DailyTrigger();
        daily.StartBoundary = Convert.ToDateTime(startDate + " " + startTime);
        if (endDate != null && endDate != "")
        {
          daily.EndBoundary = Convert.ToDateTime(endDate);
        }
        daily.DaysInterval = 1;
        daily.Repetition.Interval = TimeSpan.FromHours(recureEvery);
        mediaOfficialSectorPath = @mediaOfficialSectorPath.Replace("\\\\", "\\");
        td.Settings.Enabled = Enable;
        td.Triggers.Add(daily);
        td.Actions.Add(new ExecAction(mediaOfficialSectorPath, argName, null));
        ts.RootFolder.RegisterTaskDefinition(name, td);
        updateScheduler(name);
      }
    }

    private static void CreateTaskForWeekly(List<int> weekDays, string name, string desc, string startDate, string endDate, string startTime, short recureEvery, bool Enable)
    {
      using (TaskService ts = new TaskService())
      {
        string argName = "";
        if (name == Resources.Enums.Status.Official_Sector_Pages.ToString().Replace("_", " "))
        {
          argName = Resources.Enums.Status.OfficialSector_1.ToString().Replace("_", " ");
        }
        if (name == Resources.Enums.Status.Media_Pages.ToString().Replace("_", " "))
        {
          argName = Resources.Enums.Status.MediaSector_1.ToString().Replace("_", " ");
        }
        TaskDefinition td = ts.NewTask();
        td.RegistrationInfo.Author = WindowsIdentity.GetCurrent().Name;
        td.Principal.LogonType = TaskLogonType.S4U;
        td.RegistrationInfo.Description = desc;
        WeeklyTrigger week = new WeeklyTrigger();
        week.StartBoundary = Convert.ToDateTime(startDate + " " + startTime);
        if (endDate != null && endDate != "")
        {
          week.EndBoundary = Convert.ToDateTime(endDate);
        }
        week.WeeksInterval = 1;
        week.Repetition.Interval = TimeSpan.FromHours(recureEvery);

        week.DaysOfWeek = Microsoft.Win32.TaskScheduler.DaysOfTheWeek.Monday;
        if (weekDays.Any())
        {
          week.DaysOfWeek = (DaysOfTheWeek)weekDays[0];
        }
        foreach (int day in weekDays)
        {
          week.DaysOfWeek |= (DaysOfTheWeek)day;
        }
        td.Triggers.Add(week);
        td.Settings.Enabled = Enable;
        mediaOfficialSectorPath = @mediaOfficialSectorPath.Replace("\\\\", "\\");
        td.Actions.Add(new ExecAction(mediaOfficialSectorPath, argName, null));

        ts.RootFolder.RegisterTaskDefinition(name, td);
        updateScheduler(name);
      }
    }

    private static void DeleteTask(string name)
    {
      using (TaskService ts = new TaskService())
      {
        if (ts.GetTask(name) != null)
        {
          ts.RootFolder.DeleteTask(name);
          updateScheduler(name);
        }
      }
    }

    public static void updateScheduler(string name)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (!string.IsNullOrEmpty(name.ToString()))
        {
          var objScheduler = db.schedular.Where(x => x.Name == name).ToList();
          if (objScheduler != null)
          {
            foreach (var item in objScheduler)
            {
              item.IsUpdated = false;
            }
          }
        }
        db.SaveChanges();
      }
    }
    
  }
}