using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Data.Repository.Concrete;
using BCMStrategy.Logger;
using System;

namespace BCMStrategy.Schedular.API
{
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
    /// Get Web Site Data
    /// </summary>
    /// <param name="engineType">Engine Type</param>
    /// <returns>Boolean value for fetching and store web site data to another table</returns>
    public string GetMessageQueueData(int type)
    {
      try
      {
        string queueMessage = string.Empty;
        MessageQueue queue = new Data.Abstract.ViewModels.MessageQueue()
        {
          QueueType = type
        };
        queueMessage = MessageQueue.ReadAndDeleteMessage(queue);

        return queueMessage;
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in GetMessageQueueData method", ex, null);
        throw;
      }
    }

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