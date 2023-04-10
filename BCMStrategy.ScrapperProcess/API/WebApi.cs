//-----------------------------------------------------------------------
// <copyright file="WebApi.cs" company="Company">
//     Copyright (c) Company. All rights reserved.
// </copyright>
// <author>Vatsal Shah</author>
//-----------------------------------------------------------------------
namespace BCMStrategy.ScrapperProcess.API
{
  using BCMStrategy.Data.Abstract.Abstract;
  using BCMStrategy.Data.Abstract.ViewModels;
  using BCMStrategy.Data.Repository.Concrete;
  using BCMStrategy.Logger;
  using System;

  /// <summary>
  /// Web API for Scrapper Process
  /// </summary>
  public class WebApi
  {
    private static readonly EventLogger<WebApi> log = new EventLogger<WebApi>();

    private IProcessEvents _processEvents;

    private IProcessEvents ProcessEvents
    {
      get
      {
        if (_processEvents == null)
        {
          _processEvents = new ProcessEventsRepository();
        }

        return _processEvents;
      }
    }

    private IMessageQueue _messageQueue;

    private IMessageQueue MessageQueue
    {
      get
      {
        if (_messageQueue == null)
        {
          _messageQueue = new MessageQueueRepository();
        }

        return _messageQueue;
      }
    }

    /// <summary>
    /// Save Scrapper Engine Details
    /// </summary>
    /// <param name="engineName"> Engine Name</param>
    /// <param name="type"> Type of the Engine</param>
    /// <returns>Boolean Value for scraper update</returns>
    public bool SaveScrapperEngineDetails(string engineName, int type)
    {
      try
      {

        MessageQueue awsQueue = new MessageQueue();

        awsQueue.MessageBody = engineName;
        awsQueue.QueueType = type;

        bool result = MessageQueue.SendMessage(awsQueue);

        return result;
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in SaveScrapperEngineDetails method", ex, null);
        throw;
      }
    }

    /// <summary>
    /// Save Process Event
    /// </summary>
    /// <param name="processEvents">process Events</param>
    /// <returns>Integer value returning the Process id</returns>
    public int SaveProcessEvent(ProcessEvents processEvents)
    {
      try
      {
        int processId = ProcessEvents.InsertProcessEvents(processEvents);

        return processId;
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in SaveProcessEvent method", ex, null);
        throw;
      }
    }

    /// <summary>
    /// Save Scraper Event
    /// </summary>
    /// <param name="scraperEvents">scraper Events</param>
    /// <returns>Boolean Value for scraper update</returns>
    public int SaveScraperEvent(Events scraperEvents)
    {
      try
      {
        int result = 0;

        Events eventObj = new Events()
        {
          ProcessEventId = scraperEvents.ProcessEventId,
          ProcessTypeId = 1,
          StartDateTime = scraperEvents.StartDateTime
        };

        result = ProcessEvents.InsertEvents(eventObj);

        return result;
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in SaveScraperEvent method", ex, null);
        throw;
      }
    }

    /// <summary>
    /// Update Scraper Event
    /// </summary>
    /// <param name="scraperEvents">Scraper Events</param>
    /// <returns>Boolean Value for scraper update</returns>
    public bool UpdateScraperEvent(Events scraperEvents)
    {
      try
      {
        bool result = false;

        Events eventObj = new Events()
        {
          Id = scraperEvents.Id,
          ProcessEventId = scraperEvents.ProcessEventId,
          EndDateTime = scraperEvents.EndDateTime
        };

        result = ProcessEvents.UpdateEvents(eventObj);

        return result;
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in UpdateScraperEvent method", ex, null);
        throw;
      }
    }
  }
}
