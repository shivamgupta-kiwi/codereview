using System.Collections.Generic;
using System.Linq;
using BCMStrategy.DAL.Context;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using System;

namespace BCMStrategy.Data.Repository.Concrete
{
	/// <summary>
	/// Process Events Repository for inserting events and process events
	/// </summary>
	public class ProcessEventsRepository : IProcessEvents
	{
    ////private IMessageQueue _messageQueue;

    ////private IMessageQueue MessageQueue
    ////{
    ////  get
    ////  {
    ////    if (_messageQueue == null)
    ////    {
    ////      _messageQueue = new MessageQueueRepository();
    ////    }

    ////    return _messageQueue;
    ////  }
    ////}

		/// <summary>
		/// Insert Events to the database
		/// </summary>
		/// <param name="scraperEvents">Event to insert in the database</param>
		/// <returns>Returns true or false for inserting events in the database</returns>
		public int InsertEvents(Events scraperEvents)
		{
			int eventId = 0;

			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{
				events dbEvents = new events();

				dbEvents.ProcessEventId = scraperEvents.ProcessEventId;
				dbEvents.StartDateTime = scraperEvents.StartDateTime;

				dbEvents.ProcessTypeId = scraperEvents.ProcessTypeId;
				dbEvents.ProcessInstanceId = scraperEvents.ProcessInstanceId;

				db.events.Add(dbEvents);

				db.SaveChanges();

				eventId = dbEvents.Id;
			}

			return eventId;
		}

		/// <summary>
		/// Update Events to the database
		/// </summary>
		/// <param name="scraperEvents">Event to update in the database</param>
		/// <returns>Returns true or false for inserting events in the database</returns>
		public bool UpdateEvents(Events scraperEvents)
		{
			bool result = false;

			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{
				events dbEvents = db.events.Where(x => x.Id == scraperEvents.Id && x.ProcessEventId == scraperEvents.ProcessEventId).FirstOrDefault();

				if (dbEvents != null)
				{
					DateTime startDate = dbEvents.StartDateTime;

					if (scraperEvents.PagesProcessed > 0)
					{
						dbEvents.PagesProcessed = scraperEvents.PagesProcessed;
					}

					TimeSpan diffTicks = (scraperEvents.EndDateTime - startDate);

					dbEvents.EndDateTime = scraperEvents.EndDateTime;
					dbEvents.TimeTaken = Convert.ToDecimal(diffTicks.TotalSeconds);

					result = db.SaveChanges() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
				}
			}

			return result;
		}

		/// <summary>
		/// Insert Process Events to the database
		/// </summary>
		/// <param name="processEvents">Process Event to insert in the database</param>
		/// <returns>Returns true or false for inserting events in the database</returns>
		public int InsertProcessEvents(ProcessEvents processEvents)
		{
			int processId = 0;

			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{
				processevents dbProcessEvents = new processevents();

				dbProcessEvents.ScraperName = processEvents.ScraperName;
				dbProcessEvents.StartDateTime = processEvents.StartDateTime;

				db.processevents.Add(dbProcessEvents);

				db.SaveChanges();

				processId = dbProcessEvents.Id;
			}

			return processId;
		}

		/// <summary>
		/// Insert Process Instances
		/// </summary>
		/// <param name="processConfig">Process Configurations</param>
		/// <returns>List of Processes</returns>
		public List<ProcessInstances> InsertProcesssInstances(ProcessConfiguration processConfig, string instanceName = "p")
		{
			List<ProcessInstances> processInstancesList = new List<ProcessInstances>();

			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{
				int totalProcess = 0;

				totalProcess = processConfig.TotalWebPages / processConfig.WebPagesPerProcess;

				if (processConfig.TotalWebPages % processConfig.WebPagesPerProcess != 0)
				{
					totalProcess++;
				}

				for (int instance = 0; instance < totalProcess; instance++)
				{
					processinstances processInstance = new processinstances();

					processInstance.ProcessId = processConfig.ProcessId;
					processInstance.ProcessInstanceName = instanceName + instance;
					processInstance.Created = Helper.GetCurrentDateTime();
					processInstance.CreatedBy = Helper.ShowScrapperName();

					db.processinstances.Add(processInstance);

					db.SaveChanges();

					ProcessInstances processModel = new ProcessInstances();

					processModel.Id = processInstance.Id;
					processModel.ProcessId = processConfig.ProcessId;
					processModel.ProcessInstanceName = instanceName + instance;
					processModel.Created = Helper.GetCurrentDateTime();
					processModel.CreatedBy = Helper.ShowScrapperName();

					processInstancesList.Add(processModel);
				}
			}

			return processInstancesList;
		}

		/// <summary>
		/// Insert Process Event Log
		/// </summary>
		/// <param name="eventLog">event log</param>
		/// <returns>Returns boolean value containing whether insert has been done successful or not</returns>
		public bool InsertProcessEventLog(ProcessEventLog eventLog)
		{
			bool result = false;

			processeventlog processEventLogDB = new processeventlog();

			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{
				processEventLogDB.ProcessEventId = eventLog.ProcessEventId;
				processEventLogDB.ProcessInstanceId = eventLog.ProcessInstanceId;

				processEventLogDB.SiteUrl = eventLog.SiteUrl;
				processEventLogDB.Created = Helper.GetCurrentDateTime();
				processEventLogDB.CreatedBy = Helper.ShowScrapperName();

				db.processeventlog.Add(processEventLogDB);

				result = db.SaveChanges() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
			}

			return result;
		}

		/// <summary>
		/// Get the List of Process Instance Id's Based on Process Id
		/// </summary>
		/// <param name="processId">Pass parameter As ProcessId</param>
		/// <returns>List of ProcessInstance Id</returns>
		public int[] GetListOfProcessInstanceId(int processId)
		{
			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{
				return db.processinstances.Where(x => x.ProcessId == processId).Select(x => x.Id).ToArray();
			}
		}

		public bool CheckIsContentLoaderCompleted()
		{
			bool result = false;

			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{
				result = db.events.Any(x => x.ProcessEventId == (db.processevents.Max(y => y.Id)) && x.ProcessTypeId == 3 && x.EndDateTime != null);
			}

			return result;
		}

		public string CheckForScrapperEngine(int processId)
		{
			string scrapperName = string.Empty;

			ProcessEvents processEvent;

			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{
				processEvent = (from pro in db.processevents
												join proInst in db.processinstances on pro.Id equals proInst.ProcessId
												where pro.Id == processId
												select new ProcessEvents
												{
													ScraperName = pro.ScraperName
												}).FirstOrDefault();
			}

			if (processEvent != null)
			{
				scrapperName = processEvent.ScraperName;
			}

			return scrapperName;
		}

    // Commented code of Future enhancement

    ////public bool CheckFullProcessCompleted(int processId, int processInstanceId)
    ////{
    ////  using (BCMStrategyEntities db = new BCMStrategyEntities())
    ////  {
    ////    var processInstance = db.processinstances.Where(x => x.ProcessId == processId && x.Id == processInstanceId).FirstOrDefault();

    ////    DateTime reportDate = processInstance.Created.Value;

    ////    if (processInstance != null)
    ////    {
    ////      //processInstance.IsProcessCompleted = true;
    ////      db.SaveChanges();
    ////    }

    ////    var query = (from process in db.processevents
    ////                 where process.StartDateTime.Day == reportDate.Day &&
    ////                       process.StartDateTime.Month == reportDate.Month &&
    ////                       process.StartDateTime.Year == reportDate.Year
    ////                 select process).ToList();

    ////    var processDetail = query.Select(x=> new { ScraperName = x.ScraperName }).Distinct().Select(x => new
    ////    {
    ////      ScrapperName = x.ScraperName,
    ////      ProcessIds = query.Where(y => y.ScraperName == x.ScraperName).Select(y => y.Id).ToList()
    ////    }).Distinct().ToList();

    ////    List<int> processIds = processDetail.SelectMany(x => x.ProcessIds.Select(y => y)).ToList();

    ////    var emailGeneration = IsEmailGenerated(reportDate);
    ////    bool result  = processDetail.Count == 2 && !emailGeneration ? db.processinstances.Any(x => processIds.Contains(x.ProcessId)) : true;
    ////    return result;
    ////  }
    ////}

    ////public bool IsEmailGenerated(DateTime reportDate)
    ////{
    ////  using (BCMStrategyEntities db = new BCMStrategyEntities())
    ////  {
    ////    return db.emailgenerationstatus.Any(x => x.CreatedAt.Day == reportDate.Day &&
    ////                                                            x.CreatedAt.Month == reportDate.Month &&
    ////                                                            x.CreatedAt.Year == reportDate.Year);
    ////  }
    ////}

    ////public bool SaveToSQS(string engineName, int type)
    ////{
    ////  MessageQueue awsQueue = new MessageQueue();

    ////  awsQueue.MessageBody = engineName;
    ////  awsQueue.QueueType = type;

    ////  bool result = MessageQueue.SendMessage(awsQueue);

    ////  return result;
    ////}

    ////public int GetMessageCount(string engineName, int type)
    ////{
    ////  MessageQueue awsQueue = new MessageQueue();

    ////  awsQueue.MessageBody = engineName;
    ////  awsQueue.QueueType = type;

    ////  int result = MessageQueue.ReadQueueMessageCount(awsQueue);

    ////  return result;
    ////}
	}
}