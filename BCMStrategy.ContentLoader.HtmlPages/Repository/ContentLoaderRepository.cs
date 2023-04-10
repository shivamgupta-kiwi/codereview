using BCMStrategy.ContentLoader.Common;
using BCMStrategy.ContentLoader.HtmlPages.Abstract;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Data.Repository.Concrete;
using BCMStrategy.Logger;
using SolrNet.Exceptions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BCMStrategy.ContentLoader.HtmlPages.Repository
{
  public class ContentLoaderRepository : IContentLoader
  {
    private static readonly EventLogger<ContentLoaderRepository> log = new EventLogger<ContentLoaderRepository>();

    readonly CommonMethod cmnMethod = new CommonMethod();

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

    private IGlobalConfiguration _solrConfiguration;

    private IGlobalConfiguration SolrConfiguration
    {
      get
      {
        if (_solrConfiguration == null)
        {
          _solrConfiguration = new GlobalConfigurationRepository();
        }

        return _solrConfiguration;
      }
    }

    private ISolrPageDetail _solrPageDetail;

    private ISolrPageDetail SolrPageDetail
    {
      get
      {
        if (_solrPageDetail == null)
        {
          _solrPageDetail = new SolrPageDetailRepository();
        }

        return _solrPageDetail;
      }
    }

    public void ContentLoaderProcess(int processId, int processInstanceId)
    {
      int linkLevel = 1;
      bool isPdfFile = false;
      HashSet<string> listOfURLs = new HashSet<string>();

      int result = SaveScraperEvent(processId, processInstanceId);

      if (result > 0)
      {
        List<LoaderLinkQueue> listOfRecords = GetLinkRecords();

        List<LoaderLinkQueue> siteLinks = GetSimpleWebPageDetails(processId, processInstanceId);

        List<PageDetails> solrList = StoreWebSiteLinkToSOLRList(siteLinks, processId, processInstanceId, listOfRecords, listOfURLs);

        List<PageDetailHistory> solrPageDetailHistoryList = new List<PageDetailHistory>();

        if (solrList.Count > 0)
        {
          foreach (PageDetails pageInfo in solrList)
          {
            PageDetailHistory historyDetail = new PageDetailHistory();

            historyDetail.ProcessId = processId;
            historyDetail.ProcessInstanceId = processInstanceId;

            historyDetail.GuidId = pageInfo.GuidId;
            historyDetail.PageSource = pageInfo.PageSource;
            historyDetail.URL = pageInfo.URL;
            historyDetail.AddedDateTime = pageInfo.AddedDateTime;

            solrPageDetailHistoryList.Add(historyDetail);
          }
        }

        if (solrList.Count > 0)
        {
          InsertRecordToSolRDB(solrList);

          InsertRecordToSolrHistoryDB(solrPageDetailHistoryList);

          foreach (PageDetails siteLink in solrList)
          {
            LoaderLinkQueue pageRegExLinkAvailable = WebLink.GetWebLinkHtmlLinksRegEx(siteLink.ItemId);

            log.LogSimple(LoggingLevel.Information, "Reading from the Parent Web site Starts here. - " + siteLink.URL);

            LoaderLinkQueue webPageDetails = new LoaderLinkQueue();

            webPageDetails.ProcessId = processId;
            webPageDetails.ProcessInstanceId = processInstanceId;

            webPageDetails.SiteURL = siteLink.URL;
            webPageDetails.WebSiteId = siteLink.ItemId;
            webPageDetails.GUID = siteLink.GuidId;
            webPageDetails.pageContent = siteLink.PageSource != null ? siteLink.PageSource[0] : null;

            if (pageRegExLinkAvailable != null)
            {
              webPageDetails.RegEx = pageRegExLinkAvailable.RegEx;
              webPageDetails.AllHtmlLinksFetch = pageRegExLinkAvailable.AllHtmlLinksFetch;
            }

            cmnMethod.GetInnerChildContents(webPageDetails, listOfRecords, ref linkLevel, 0, listOfURLs, ref isPdfFile);

            log.LogSimple(LoggingLevel.Information, "Reading from the Parent Web site Ends here.- " + siteLink.URL);
          }

          log.LogSimple(LoggingLevel.Information, "Completion for all the sites ends here.");
        }

        int solrListCount = solrList.Count;

        UpdateScraperEvent(result, processId, solrListCount);

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

    private List<LoaderLinkQueue> GetLinkRecords()
    {
      List<LoaderLinkQueue> linkRecords = WebLink.GetAllLinksInnerLinks();

      return linkRecords;
    }

    private bool UpdateScraperEvent(int eventSavedresult, int processId, int solrCount)
    {
      Events scraperEvents = new Events()
      {
        Id = eventSavedresult,
        ProcessEventId = processId,
        EndDateTime = Helper.GetSystemCurrentDateTime(),
        PagesProcessed = solrCount
      };
      return ProcessEvents.UpdateEvents(scraperEvents);
    }

    private int SaveScraperEvent(int processId, int processInstanceId)
    {
      Events scraperEvents = new Events()
      {
        ProcessEventId = processId,
        StartDateTime = Helper.GetSystemCurrentDateTime(),
        ProcessTypeId = Convert.ToInt32(Helper.ProcessType.HtmlPages),
        ProcessInstanceId = processInstanceId,
      };
      return ProcessEvents.InsertEvents(scraperEvents);
    }

    /// <summary>
    /// Get Simple Web Page Details
    /// </summary>
    /// <param name="resultType">result Type</param>
    /// <returns></returns>
    private List<LoaderLinkQueue> GetSimpleWebPageDetails(int processId, int processInstanceId)
    {
      string messageType = Convert.ToString(Helper.WebSiteCategory.ClickThroughPages);

      List<LoaderLinkQueue> webSiteLinks = WebLink.GetMessageBasedUponType(messageType, processId, processInstanceId);
      return webSiteLinks;
    }

    /// <summary>
    /// Stores Link to the SOLR Database
    /// </summary>
    /// <param name="webSiteLinks">Web Site Links</param>
    /// <returns>boolean value for storing the link to the SOLR Database</returns>
    private List<PageDetails> StoreWebSiteLinkToSOLRList(List<LoaderLinkQueue> webSiteLinks, int processId, int processInstanceId, List<LoaderLinkQueue> listOfRecords, HashSet<string> listOfURLs)
    {
      string pageContent = string.Empty;

      List<PageDetails> solrPageDetails = new List<PageDetails>();
      PageDetails pageDetails;

      foreach (LoaderLinkQueue link in webSiteLinks)
      {
        try
        {
          listOfURLs.Add(link.SiteURL);

          pageDetails = new PageDetails();
          bool isContentInserted = false;
          bool isContentUpdated = false;

          using (var response = Helper.WebRequestResponse(link.SiteURL))
          {
            if (response != null)
            {
              Stream stream = response.GetResponseStream();
              stream.ReadTimeout = -1;
              StreamReader reader = new StreamReader(stream);
              pageContent = reader.ReadToEnd();

              ////System.Text.Encoding encoding = System.Text.Encoding.UTF8;

              pageContent = Helper.RemoveUnWantedAnchorTags(pageContent);
              pageContent = Regex.Replace(pageContent, "<head.*?>(.|\n)*?</head>", "");

              pageContent = Regex.Replace(pageContent, "<em.*?>(.|\n)*?</em>", "");

              if (pageContent.IndexOf("post-header") == -1 && link.SiteURL != "https://www.politico.eu/")
              {
                pageContent = Regex.Replace(pageContent, "<header.*?>(.|\n)*?</header>", "");
                pageContent = Regex.Replace(pageContent, "<footer.*?>(.|\n)*?</footer>", "");
              }

              pageContent = Helper.RemoveHeaderFooterTags(pageContent, link);
              pageContent = Helper.RemoveUnWantedHtmlSource(pageContent);

              var newURLDetails = listOfRecords.Find(x => x.SiteURL == link.SiteURL && x.LinkLevel == 0);

              if (newURLDetails == null)
              {
                string[] pageSource = { pageContent };

                pageDetails.PageSource = pageSource;
                pageDetails.ItemId = link.Id;
                pageDetails.GuidId = link.GUID;
                pageDetails.AddedDateTime = Helper.GetCurrentDateTime();
                pageDetails.Status = string.Empty;
                pageDetails.URL = link.SiteURL;

                solrPageDetails.Add(pageDetails);

                LoaderLinkQueue linkRecordUpdate = new LoaderLinkQueue();

                linkRecordUpdate.ProcessId = processId;
                linkRecordUpdate.ProcessInstanceId = processInstanceId;
                linkRecordUpdate.WebSiteId = link.Id;
                linkRecordUpdate.SiteURL = link.SiteURL;

                linkRecordUpdate.GUID = link.GUID;

                linkRecordUpdate.WebLinkBytes = 0;
                linkRecordUpdate.NewerProcessId = processId;
                linkRecordUpdate.LexiconCount = 0;

                WebLink.UpdateLoaderLinkLogMasterRecord(linkRecordUpdate);
                isContentInserted = true;
              }
              else
              {
                PageDetailsView solrPageDetail = GetPageSourceIdByGuidId(newURLDetails);

                ICollection<PageDetails> solrUpdatePageDetail = solrPageDetail.Products;
                string solrPageSourceDetail = string.Empty;

                if (solrUpdatePageDetail.Count > 0)
                {
                  foreach (PageDetails solrPageUpdate in solrUpdatePageDetail)
                  {
                    solrPageSourceDetail = solrPageUpdate.PageSource[0];
                  }
                }

                // update the content of the Solr instance
                if (solrPageSourceDetail != pageContent)
                {
                  if (solrUpdatePageDetail.Count > 0)
                  {
                    foreach (PageDetails solrPageUpdate in solrUpdatePageDetail)
                    {
                      string[] pageSource = { pageContent };
                      solrPageUpdate.PageSource = pageSource;
                      solrPageDetails.Add(solrPageUpdate);
                    }
                  }
                  else
                  {
                    string[] pageSource = { pageContent };

                    pageDetails.PageSource = pageSource;
                    pageDetails.ItemId = link.Id;
                    pageDetails.GuidId = newURLDetails.GUID;
                    pageDetails.AddedDateTime = Helper.GetCurrentDateTime();
                    pageDetails.Status = string.Empty;
                    pageDetails.URL = newURLDetails.SiteURL;

                    solrPageDetails.Add(pageDetails);
                  }

                  LoaderLinkQueue linkRecordUpdate = new LoaderLinkQueue();

                  linkRecordUpdate.ProcessId = processId;
                  linkRecordUpdate.ProcessInstanceId = processInstanceId;
                  linkRecordUpdate.WebSiteId = link.Id;
                  linkRecordUpdate.SiteURL = link.SiteURL;

                  linkRecordUpdate.GUID = link.GUID;

                  linkRecordUpdate.WebLinkBytes = 0;
                  linkRecordUpdate.NewerProcessId = processId;
                  linkRecordUpdate.LexiconCount = 0;

                  WebLink.UpdateLoaderLinkLogMasterRecord(linkRecordUpdate);
                  isContentUpdated = true;
                }
              }

              ContentLoaderLogViewModel viewModel = new ContentLoaderLogViewModel()
              {
                ProcessId = processId,
                ProcessInstanceId = processInstanceId,
                SiteUrl = link.SiteURL,
                WebSiteId = link.Id,
                LinkLevel = 0,
                IsContentInserted = isContentInserted,
                IsContentUpdated = isContentUpdated
              };
              InsertDataToContentLoaderLog(viewModel);
            }
          }
        }
        catch (Exception ex)
        {
          ProcessEventLog processLog = new ProcessEventLog();

          processLog.ProcessEventId = processId;
          processLog.ProcessInstanceId = processInstanceId;

          processLog.SiteUrl = link.SiteURL;
          processLog.Created = Helper.GetCurrentDateTime();
          processLog.CreatedBy = Helper.ShowScrapperName();

          ProcessEvents.InsertProcessEventLog(processLog);

          LoaderErrorLogViewModel viewModel = new LoaderErrorLogViewModel()
          {
            ProcessId = processId,
            ProcessInstanceId = processInstanceId,
            WebSiteId = link.Id,
            SiteUrl = link.SiteURL,
            ErrorDesc = "InnerException:" + ex.InnerException + "Message :" + ex.Message
          };
          InsertDataToLoaderErrorLog(viewModel);

        }
      }

      return solrPageDetails;
    }

    private bool InsertDataToLoaderErrorLog(LoaderErrorLogViewModel viewModel)
    {
      return WebLink.UpdateLoaderErrorLog(viewModel);
    }

    private bool InsertDataToContentLoaderLog(ContentLoaderLogViewModel viewModel)
    {
      return WebLink.UpdateContentLoaderLog(viewModel);
    }
    
    private PageDetailsView GetPageSourceIdByGuidId(LoaderLinkQueue loaderLinkQueue)
    {
      SolrSearchParameters searchParameters = new SolrSearchParameters();

      searchParameters.FreeSearch = "guidId:\"" + loaderLinkQueue.GUID + "\"";

      PageDetailsView pageDetails = SolrConfiguration.GetPageSourceIdByGuidId(searchParameters);

      return pageDetails;
    }

    /// <summary>
    /// Insert Record to the SOLR Database
    /// </summary>
    /// <param name="solrList"></param>
    /// <returns></returns>
    private async Task<bool> InsertRecordToSolRDB(List<PageDetails> solrList)
    {
      try
      {
        SolrConfiguration.InsertInRange(solrList);
        return true;
      }
      catch (SolrConnectionException ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in the InsertRecordToSolRDB method", ex, null);
        return false;
      }
    }

    private async Task<bool> InsertRecordToSolrHistoryDB(List<PageDetailHistory> solrDetailList)
    {
      try
      {
        SolrPageDetail.InsertPageDetailHistory(solrDetailList);
        return true;
      }
      catch (SolrConnectionException ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in the InsertRecordToSolrHistoryDB method", ex, null);
        return false;
      }
    }
  }
}