//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Company">
//     Copyright (c) Company. All rights reserved.
// </copyright>
// <author>Vatsal Shah</author>
//-----------------------------------------------------------------------
namespace BCMStrategy.ScrapperProcess
{
  using System;
  using BCMStrategy.Data.Abstract;
  using BCMStrategy.Data.Abstract.ViewModels;
  using BCMStrategy.ScrapperProcess.API;
  using BCMStrategy.Logger;

  /// <summary>
  /// This is the Scrapper Process
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
    
    /// <summary>
    /// Main function of the execution
    /// </summary>
    /// <param name="args">Arguments passed as the input parameter</param>
    private static void Main(string[] args)
    {
      try
      {
        if (args != null && args[0] != null && args[1] != null)
        {
          WebApi api = new WebApi();
          ProcessEvents processEvent = new ProcessEvents();

          processEvent.ScraperName = args[0];
          processEvent.StartDateTime = DateTime.Now;
          processEvent.EndDateTime = DateTime.MinValue;

          int processId = api.SaveProcessEvent(processEvent);

          Events scraperEvents = new Events();

          scraperEvents.ProcessEventId = processId;
          scraperEvents.StartDateTime = Helper.GetSystemCurrentDateTime();
          scraperEvents.ProcessTypeId = Convert.ToInt32(Helper.ProcessType.WebCrawlerService);

          int result = api.SaveScraperEvent(scraperEvents);

          if (result > 0)
          {
            api.SaveScrapperEngineDetails(args[0] + "-" + processId, Convert.ToInt32(args[1]));
          }

          scraperEvents = new Events();

          scraperEvents.Id = result;
          scraperEvents.ProcessEventId = processId;
          scraperEvents.EndDateTime = Helper.GetSystemCurrentDateTime();

          api.UpdateScraperEvent(scraperEvents);
        }
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in Main method of Scraper Process", ex, null);
        throw;
      }
    }
  }
}