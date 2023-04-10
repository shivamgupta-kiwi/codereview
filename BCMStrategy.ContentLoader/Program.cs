using BCMStrategy.ContentLoader.API;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Threading;

namespace BCMStrategy.ContentLoader
{
  /// <summary>
  /// This project will be the master project for BCM Strategy project.
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

    public static void Main(string[] args)
    {
      try
      {
        if (args.Length > 0)
        {
          WebApi api = new WebApi();
          List<string> websiteTypeDetails = new List<string>();

          string outputResult = args[0];

          int pagesPerProcess = 0;
          int webSiteTypeId = 0;
          string applicationPath = string.Empty;

          if (outputResult != string.Empty)
          {
            string[] scraperDetails = outputResult.Split('-');

            int processId = scraperDetails[1] != null ? Convert.ToInt32(scraperDetails[1]) : 0;

            Events scraperEvents = new Events();

            scraperEvents.ProcessEventId = processId;
            scraperEvents.StartDateTime = Helper.GetSystemCurrentDateTime();
            scraperEvents.ProcessInstanceId = null;
            scraperEvents.ProcessTypeId = Convert.ToInt32(Helper.ProcessType.ContentLoaderService);

            int scraperEventId = api.SaveScraperEvent(scraperEvents);

            if (scraperDetails != null && scraperDetails[0] != null
              && (scraperDetails[0] == Helper.WebSiteType.OfficialSector.ToString() || scraperDetails[0] == Helper.WebSiteType.MediaSector.ToString()))
            {
              int siteId = 0;

              if (scraperDetails[0] == Helper.WebSiteType.MediaSector.ToString())
              {
                siteId = Convert.ToInt32(Helper.WebSiteType.MediaSector);
                websiteTypeDetails = api.GetWebSiteData(siteId, processId);
              }
              else if (scraperDetails[0] == Helper.WebSiteType.OfficialSector.ToString())
              {
                siteId = Convert.ToInt32(Helper.WebSiteType.OfficialSector);
                websiteTypeDetails = api.GetWebSiteData(siteId, processId);
              }

              foreach (string websiteType in websiteTypeDetails)
              {
                ////List<ProcessInstances> processInstanceList = new List<ProcessInstances>();

                ////string processArguments = string.Empty;
                
                if (websiteType.Equals(Helper.WebSiteCategory.ClickThroughPages.ToString()))
                {
                  pagesPerProcess = Convert.ToInt32(ConfigurationManager.AppSettings["HtmlWebPagesPerProcess"]);
                  webSiteTypeId = Convert.ToInt32(Helper.WebSiteCategory.ClickThroughPages);
                  applicationPath = ConfigurationManager.AppSettings["HtmlPagesApplicationPath"];

                  Thread.Sleep(5000);

                  api.WebSiteTypeInstanceExecution(siteId, processId, pagesPerProcess, webSiteTypeId, applicationPath, websiteType);
                }
                else if (websiteType.Equals(Helper.WebSiteCategory.PDFDocument.ToString()))
                {
                  pagesPerProcess = Convert.ToInt32(ConfigurationManager.AppSettings["DocumentsPerProcess"]);
                  webSiteTypeId = Convert.ToInt32(Helper.WebSiteCategory.PDFDocument);
                  applicationPath = ConfigurationManager.AppSettings["DocumentApplicationPath"];

                  Thread.Sleep(5000);

                  api.WebSiteTypeInstanceExecution(siteId, processId, pagesPerProcess, webSiteTypeId, applicationPath, websiteType);
                }
                else if (websiteType.Equals(Helper.WebSiteCategory.DynamicContent.ToString()))
                {
                  pagesPerProcess = Convert.ToInt32(ConfigurationManager.AppSettings["DynamicPagesPerProcess"]);
                  webSiteTypeId = Convert.ToInt32(Helper.WebSiteCategory.DynamicContent);
                  applicationPath = ConfigurationManager.AppSettings["DynamicPagesApplicationPath"];

                  Thread.Sleep(5000);

                  api.WebSiteTypeInstanceExecution(siteId, processId, pagesPerProcess, webSiteTypeId, applicationPath, websiteType);
                }
                else if (websiteType.Equals(Helper.WebSiteCategory.PDFDynamicContent.ToString()))
                {
                  pagesPerProcess = Convert.ToInt32(ConfigurationManager.AppSettings["DynamicPagesPerProcess"]);
                  webSiteTypeId = Convert.ToInt32(Helper.WebSiteCategory.PDFDynamicContent);
                  applicationPath = ConfigurationManager.AppSettings["DynamicPagesApplicationPath"];

                  Thread.Sleep(5000);

                  api.WebSiteTypeInstanceExecution(siteId, processId, pagesPerProcess, webSiteTypeId, applicationPath, websiteType);
                }
                else if(websiteType.Equals(Helper.WebSiteCategory.CustomAPI.ToString()))
                {
                  pagesPerProcess = Convert.ToInt32(ConfigurationManager.AppSettings["HtmlWebPagesPerProcess"]);
                  webSiteTypeId = Convert.ToInt32(Helper.WebSiteCategory.CustomAPI);
                  applicationPath = ConfigurationManager.AppSettings["CustomAPIProcess"];

                  Thread.Sleep(5000);

                  api.WebSiteTypeInstanceExecution(siteId, processId, pagesPerProcess, webSiteTypeId, applicationPath, websiteType);
                }
                else if (websiteType.Equals(Helper.WebSiteCategory.RSSFeeds.ToString()))
                {
                  pagesPerProcess = Convert.ToInt32(ConfigurationManager.AppSettings["RssFeedsPerProcess"]);
                  webSiteTypeId = Convert.ToInt32(Helper.WebSiteCategory.RSSFeeds);
                  applicationPath = ConfigurationManager.AppSettings["RSSFeedsApplicationPath"];

                  Thread.Sleep(5000);

                  api.WebSiteTypeInstanceExecution(siteId, processId, pagesPerProcess, webSiteTypeId, applicationPath, websiteType);
                }
              }
            }

            scraperEvents = new Events();

            scraperEvents.Id = scraperEventId;
            scraperEvents.ProcessEventId = processId;
            scraperEvents.EndDateTime = Helper.GetSystemCurrentDateTime();

            api.UpdateScraperEvent(scraperEvents);
          }
        }
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in Main method", ex, null);
      }
    }
  }
}