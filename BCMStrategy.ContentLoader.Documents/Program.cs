using BCMStrategy.ContentLoader.Documents.API;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Logger;
using iTextSharp.license;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BCMStrategy.ContentLoader.Documents
{
  /// <summary>
  /// This program will call the PDF document and will store the data in the SOLR database
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

    static void Main(string[] args)
    {
      try
      {
        HashSet<string> listOfURLs = new HashSet<string>();

        if (args != null && args[0] != null && args[1] != null)
        {
          // LicenseKey.LoadLicenseFile(Helper.GetItextSharpLicenseKeyPath());

          WebApi api = new WebApi();

          int processId = Convert.ToInt32(args[0]);
          int processInstanceId = Convert.ToInt32(args[1]);

          Events scraperEvents = new Events();

          scraperEvents.ProcessEventId = processId;
          scraperEvents.StartDateTime = Helper.GetSystemCurrentDateTime();
          scraperEvents.ProcessTypeId = Convert.ToInt32(Helper.ProcessType.Documents);
          scraperEvents.ProcessInstanceId = processInstanceId;

          int result = api.SaveScraperEvent(scraperEvents);

          if (result > 0)
          {
            List<LoaderLinkQueue> listOfRecords = api.GetLinkRecords();

            List<LoaderLinkQueue> pdfPageList = api.GetDocumentDetails(processId, processInstanceId);

            api.SolrDbLinkToSOLRList(pdfPageList, listOfRecords, listOfURLs);

            scraperEvents = new Events();

            scraperEvents.Id = result;
            scraperEvents.ProcessEventId = processId;
            scraperEvents.EndDateTime = Helper.GetSystemCurrentDateTime();
            scraperEvents.PagesProcessed = pdfPageList.Count;

            api.UpdateScraperEvent(scraperEvents);

            // Code to start calling Scrapper Activity Process for the given processId and processInstanceId
            Process pageApplicationProcess = new Process();
            string processArguments = Convert.ToString(Convert.ToInt32(processId)) + " " + Convert.ToString(processInstanceId);
            pageApplicationProcess.StartInfo.FileName = ConfigurationManager.AppSettings["ScraperActivityPath"];
            pageApplicationProcess.StartInfo.Arguments = processArguments;
            pageApplicationProcess.Start();
            pageApplicationProcess.PriorityClass = ProcessPriorityClass.Normal;
            // Code ends here
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