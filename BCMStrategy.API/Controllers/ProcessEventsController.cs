using BCMStrategy.Common.Unity;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Logger;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace BCMStrategy.API.Controllers
{
  [RoutePrefix("api/ProcessEvents")]
  public class ProcessEventsController : BaseApiController
  {
    /// <summary>
    /// The log
    /// </summary>
    private static readonly EventLogger<ProcessEventsController> log = new EventLogger<ProcessEventsController>();

    /// <summary>
    /// The ProcessEvents repository
    /// </summary>
    private IProcessEvents _processEvents;

    private IProcessEvents ProcessEvents
    {
      get
      {
        if (_processEvents == null)
        {
          _processEvents = UnityHelper.Resolve<IProcessEvents>();
        }

        return _processEvents;
      }
    }

    /// <summary>
    /// Insert Events to the database
    /// </summary>
    /// <param name="scraperEvents">Event to insert in the database</param>
    /// <returns>Returns true or false for inserting events in the database</returns>
    [HttpPost]
    [Route("InsertEvents")]
    public async Task<IHttpActionResult> InsertEvents(Events scraperEvents)
    {
      try
      {
        int result = ProcessEvents.InsertEvents(scraperEvents);
        return Ok(result);
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception occur while inserting events in the database", ex);
        return BadRequest(ex.Message);
      }
    }

    [HttpPost]
    [Route("UpdateEvents")]
    public async Task<IHttpActionResult> UpdateEvents(Events scraperEvents)
    {
      try
      {
        bool result = ProcessEvents.UpdateEvents(scraperEvents);
        return Ok(result);
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception occur while updating Process Events in the database", ex);
        return BadRequest(ex.Message);
      }
    }

    [HttpPost]
    [Route("InsertProcessEvents")]
    public async Task<IHttpActionResult> InsertProcessEvents(ProcessEvents processEvents)
    {
      try
      {
        int processEventId = ProcessEvents.InsertProcessEvents(processEvents);
        return Ok(processEventId);
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception occur while inserting Process Events in the database", ex);
        return BadRequest(ex.Message);
      }
    }

    [HttpPost]
    [Route("InsertProcesssInstances")]
    public async Task<IHttpActionResult> InsertProcesssInstances(ProcessConfiguration processConfig)
    {
      try
      {
        List<ProcessInstances> instances = ProcessEvents.InsertProcesssInstances(processConfig);
        return Ok(instances);
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception occur while inserting Process Instances in the database", ex);
        return BadRequest(ex.Message);
      }
    }

    [HttpPost]
    [Route("InsertProcessEventLog")]
    public async Task<IHttpActionResult> InsertProcessEventLog(ProcessEventLog eventLog)
    {
      try
      {
        bool insertLog = ProcessEvents.InsertProcessEventLog(eventLog);
        return Ok(insertLog);
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception occur while inserting Process Instances in the database", ex);
        return BadRequest(ex.Message);
      }
    }

    [HttpGet]
    [Route("CheckIsContentLoaderCompleted")]
    public async Task<IHttpActionResult> CheckIsContentLoaderCompleted()
    {
      try
      {
        bool isProcessCompleted = ProcessEvents.CheckIsContentLoaderCompleted();
        return Ok(isProcessCompleted);
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception occur while checking Is Content Loader Completed", ex);
        return BadRequest(ex.Message);
      }
    }
  }
}
