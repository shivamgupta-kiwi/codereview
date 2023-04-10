using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Data.Repository.Concrete;
using BCMStrategy.Logger;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.ContentLoader.API
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

    private IWebLink _webLink;

    private IWebLink WebLink
    {
      get
      {
        if (_webLink == null)
        {
          _webLink = new WebLinkRepository();
        }

        return _webLink;
      }
    }

    /// <summary>
    /// Get Web Site Data
    /// </summary>
    /// <param name="engineType">Engine Type</param>
    /// <returns>Boolean value for fetching and store web site data to another table</returns>
    public List<string> GetWebSiteData(int engineType, int processId)
    {
      List<string> stringResult = WebLink.GetAllWebLinks(engineType, processId);

      return stringResult;
    }

    public bool InsertLinkDetails(ProcessLinkDetails processLink)
    {
      bool result = false;

      result = WebLink.InsertLinkRecords(processLink);

      return result;
    }

    public int SaveScraperEvent(Events scraperEvents)
    {
      try
      {
        int result = 0;

        Events eventObj = new Events()
        {
          ProcessEventId = scraperEvents.ProcessEventId,
          ProcessTypeId = scraperEvents.ProcessTypeId,
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

    public List<ProcessInstances> ProcessList(ProcessConfiguration processConfig)
    {
      List<ProcessInstances> processInstanceList = ProcessEvents.InsertProcesssInstances(processConfig, "p");

      return processInstanceList;
    }

    public int GetWebLinkCount(int type, string categoryName)
    {
      int count = WebLink.GetWebLinkCount(type, categoryName);

      return count;
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

    public void WebSiteTypeInstanceExecution(int siteId, int processId, int pagesPerProcess, int webSiteId, string applicationPath, string webSiteType)
    {
      bool result = false;
      string processArguments = string.Empty;

      List<ProcessInstances> processInstanceList;
      int totalWebPages = GetWebLinkCount(siteId, webSiteType);

      ProcessConfiguration config = new ProcessConfiguration();
      config.ProcessId = processId;
      config.ProcessName = webSiteType;
      config.TotalWebPages = totalWebPages;
      config.WebPagesPerProcess = Convert.ToInt32(pagesPerProcess);

      processInstanceList = ProcessList(config);

      for (int instance = 0; instance < processInstanceList.Count; instance++)
      {
        Process pageApplicationProcess = new Process();
        ProcessLinkDetails processLink = new ProcessLinkDetails();

        processLink.EngineCategory = Convert.ToInt32(webSiteId);
        processLink.Websitetypes = siteId;

        processLink.ProcessId = processId;
        processLink.ProcessInstanceId = Convert.ToInt32(processInstanceList[instance].Id);
        processLink.NoOfRecords = config.WebPagesPerProcess;
        processLink.PageNumber = instance;

        result = InsertLinkDetails(processLink);

        if (result)
        {
          processArguments = Convert.ToString(processLink.ProcessId) + " " + Convert.ToString(processLink.ProcessInstanceId);

          pageApplicationProcess.StartInfo.FileName = applicationPath;
          pageApplicationProcess.StartInfo.Arguments = processArguments;
          pageApplicationProcess.Start();
          pageApplicationProcess.PriorityClass = ProcessPriorityClass.Normal;
        }
      }
    }
  }
}