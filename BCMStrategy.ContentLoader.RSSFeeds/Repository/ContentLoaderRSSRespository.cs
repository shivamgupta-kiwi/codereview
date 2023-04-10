using BCMStrategy.ContentLoader.Common;
using BCMStrategy.ContentLoader.RSSFeeds.Abstract;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Data.Repository.Concrete;
using BCMStrategy.Logger;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using SolrNet.Exceptions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BCMStrategy.ContentLoader.RSSFeeds.Repository
{
  public class ContentLoaderRSSRespository : IContentLoaderRss
  {
    readonly CommonMethod cmnMethod = new CommonMethod();
    private static readonly EventLogger<ContentLoaderRSSRespository> log = new EventLogger<ContentLoaderRSSRespository>();

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

    public void ContentLoaderRSSProcess(int processId, int processInstanceId)
    {
      Events scraperEvents = new Events();

      scraperEvents.ProcessEventId = processId;
      scraperEvents.ProcessInstanceId = processInstanceId;
      scraperEvents.ProcessTypeId = Convert.ToInt32(Helper.ProcessType.RSSFeeds);
      scraperEvents.StartDateTime = Helper.GetSystemCurrentDateTime();
      int result = SaveScraperEvent(scraperEvents);

      if (result > 0)
      {
        List<LoaderLinkQueue> listOfRecords = WebLink.GetAllLinksInnerLinks();

        List<LoaderLinkQueue> siteLinks = GetSimpleWebPageDetails(processId, processInstanceId);

        List<PageDetailHistory> solrPageDetailHistoryList = new List<PageDetailHistory>();

        List<PageDetails> solrRSSFeedURLStorage;

        foreach (LoaderLinkQueue siteDetails in siteLinks)
        {
          try
          {
            List<RssFeedDetails> rssInfo = WebLink.GetRSSFeedByWebLinkId(siteDetails.Id);
            int currentRecord = 0;
            int scanningLinkQueueId = 0;

            foreach (RssFeedDetails rssFeed in rssInfo)
            {
              try
              {
                currentRecord++;
                solrRSSFeedURLStorage = StoreWebSiteLinkToSOLRList(siteDetails, rssFeed, listOfRecords, processId, processInstanceId);

                foreach (PageDetails pageInfo in solrRSSFeedURLStorage)
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

                if (solrRSSFeedURLStorage.Count > 0)
                {
                  InsertRecordToSolRDB(solrRSSFeedURLStorage);

                  InsertRecordToSolrHistoryDB(solrPageDetailHistoryList);

                  solrPageDetailHistoryList = new List<PageDetailHistory>();

                  siteDetails.ProcessId = processId;
                  siteDetails.WebSiteId = siteDetails.Id;
                  siteDetails.SiteURL = rssFeed.RSSFeedURL;

                  if ((rssInfo.Count > 1 && currentRecord == 1) || rssInfo.Count == 1)
                  {
                    scanningLinkQueueId = WebLink.InsertInScanningLinkQueue(siteDetails);
                  }

                  FetchRSSFeedContents(rssFeed, solrRSSFeedURLStorage, processId, processInstanceId, scanningLinkQueueId);
                }
              }
              catch (Exception ex)
              {
                log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in the RSS For Each method", ex, null);
              }
            }
          }
          catch (Exception ex)
          {
            log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in ContentLoaderRSSProcess method", ex, null);
          }
        }

        Process pageApplicationProcess = new Process();
        string processArguments = Convert.ToString(Convert.ToInt32(processId)) + " " + Convert.ToString(processInstanceId);
        pageApplicationProcess.StartInfo.FileName = ConfigurationManager.AppSettings["ScraperActivityPath"];
        pageApplicationProcess.StartInfo.Arguments = processArguments;
        pageApplicationProcess.Start();
        pageApplicationProcess.PriorityClass = ProcessPriorityClass.Normal;

        int pagesProcessed = siteLinks.Count;

        scraperEvents = new Events();

        scraperEvents.Id = result;
        scraperEvents.ProcessEventId = processId;
        scraperEvents.EndDateTime = Helper.GetSystemCurrentDateTime();
        scraperEvents.PagesProcessed = pagesProcessed;

        UpdateScraperEvent(scraperEvents);
      }
    }

    /// <summary>
    /// Get Simple Web Page Details
    /// </summary>
    /// <param name="resultType">result Type</param>
    /// <returns></returns>
    public List<LoaderLinkQueue> GetSimpleWebPageDetails(int processId, int processInstanceId)
    {

      string messageType = Convert.ToString(Helper.WebSiteCategory.RSSFeeds);

      List<LoaderLinkQueue> webSiteLinks = WebLink.GetMessageBasedUponType(messageType, processId, processInstanceId);

      return webSiteLinks;
    }

    /// <summary>
    /// Save the Scraper Event
    /// </summary>
    /// <param name="scraperEvents">Scraper Events</param>
    /// <returns>boolean value for inserting record in the database</returns>
    private int SaveScraperEvent(Events scraperEvents)
    {
      int result = ProcessEvents.InsertEvents(scraperEvents);
      return result;
    }

    private bool UpdateScraperEvent(Events scraperEvents)
    {
      bool result = ProcessEvents.UpdateEvents(scraperEvents);
      return result;
    }

    /// <summary>
    /// Insert Record to the SOLR Database
    /// </summary>
    /// <param name="solrList"></param>
    /// <returns></returns>
    public async Task<bool> InsertRecordToSolRDB(List<PageDetails> solrList)
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

    public async Task<bool> InsertRecordToSolrHistoryDB(List<PageDetailHistory> solrDetailList)
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

    /// <summary>
    /// Stores Link to the SOLR Database
    /// </summary>
    /// <param name="siteLink">Web Site Links</param>
    /// <returns>boolean value for storing the link to the SOLR Database</returns>
    public List<PageDetails> StoreWebSiteLinkToSOLRList(LoaderLinkQueue siteLink, RssFeedDetails rssFeed, List<LoaderLinkQueue> listOfRecords, int processId, int processInstanceId)
    {
      string pageContent = string.Empty;
      List<PageDetails> solrPageDetails = new List<PageDetails>();

      PageDetails pageDetails;

      pageDetails = new PageDetails();

      using (var response = Helper.WebRequestResponse(rssFeed.RSSFeedURL))
      {
        if (response != null)
        {
          Stream stream = response.GetResponseStream();
          stream.ReadTimeout = -1;
          StreamReader reader = new StreamReader(stream);
          pageContent = reader.ReadToEnd();

          var newURLDetails = listOfRecords.Find(x => x.SiteURL.ToLower() == rssFeed.RSSFeedURL.ToLower());

          string[] pageSource = { pageContent };

          if (newURLDetails == null)
          {
            pageDetails.PageSource = pageSource;
            pageDetails.ItemId = siteLink.Id;
            pageDetails.GuidId = Helper.GuidString();
            pageDetails.AddedDateTime = DateTime.Now;
            pageDetails.Status = string.Empty;
            pageDetails.URL = rssFeed.RSSFeedURL;

            solrPageDetails.Add(pageDetails);

            LoaderLinkQueue linkRecordUpdate = new LoaderLinkQueue();

            linkRecordUpdate.ProcessId = processId;
            linkRecordUpdate.ProcessInstanceId = processInstanceId;
            linkRecordUpdate.WebSiteId = rssFeed.WebSiteId;
            linkRecordUpdate.SiteURL = rssFeed.RSSFeedURL;

            linkRecordUpdate.GUID = pageDetails.GuidId;

            linkRecordUpdate.WebLinkBytes = 0;
            linkRecordUpdate.NewerProcessId = 0;
            linkRecordUpdate.LexiconCount = 0;
            linkRecordUpdate.InstanceName = Helper.WebSiteCategory.RSSFeeds.ToString();
            linkRecordUpdate.NewerProcessId = processId;

            WebLink.UpdateLoaderLinkLogMasterRecord(linkRecordUpdate);
          }
          else
          {
            PageDetailsView solrPageDetail = cmnMethod.GetPageSourceIdByGuidId(newURLDetails.GUID);

            ICollection<PageDetails> solrUpdatePageDetail = solrPageDetail.Products;
            ////string solrPageSourceDetail = string.Empty;

            if (solrUpdatePageDetail.Count > 0)
            {
              foreach (PageDetails solrPageUpdate in solrUpdatePageDetail)
              {
                solrPageUpdate.PageSource = pageSource;
                solrPageDetails.Add(solrPageUpdate);
              }
            }
            else
            {
              pageDetails.PageSource = pageSource;
              pageDetails.ItemId = siteLink.Id;
              pageDetails.GuidId = newURLDetails.GUID;
              pageDetails.AddedDateTime = DateTime.Now;
              pageDetails.Status = string.Empty;
              pageDetails.URL = rssFeed.RSSFeedURL;

              solrPageDetails.Add(pageDetails);
            }
          }
        }
      }

      return solrPageDetails;
    }

    public void FetchRSSFeedContents(RssFeedDetails rssFeed, List<PageDetails> solrRSSFeedURLStorage, int processId, int processInstanceId, int scanningLinkQueueId)
    {
      try
      {
        PageDetails rssFeedPageDetails;
        PageDetailHistory rssFeedDetailHistory;

        List<PageDetails> solrPageDetails = new List<PageDetails>();
        List<PageDetailHistory> solrPageDetailHistoryList = new List<PageDetailHistory>();

        CommonMethod method = new CommonMethod();
        HashSet<string> listOfURLs = new HashSet<string>();
        LoaderLinkQueue linkDetails = new LoaderLinkQueue();

        List<WebLinkPageContentRegEx> webLinkPagesRegExList = WebLink.GetRegularExpressionBasedUponWebsiteId(rssFeed.WebSiteId);

        string pageContent = string.Empty;

        DateTime currentDate = DateTime.Today;
        DateTime previousDate = DateTime.Today.AddDays(-1);

        string[] pageSource = solrRSSFeedURLStorage[0].PageSource;
        bool isHttps = false;

        Regex rssFeedRegEx = new Regex(rssFeed.RegEx);

        MatchCollection includeMatches = rssFeedRegEx.Matches(pageSource[0]);

        List<RssFeedWebSiteUrlDetails> rssFeedWebSiteDetails = includeMatches.Cast<Match>()
                                                                .Where(x => x.Value != null && x.Value.Length > 1)
                                                                .Select(z => new RssFeedWebSiteUrlDetails()
                                                                {
                                                                  PublishDate = z.Groups["date"] != null ? z.Groups["date"].Value : null,
                                                                  SiteURL = z.Groups["link"].Value != null ? z.Groups["link"].Value : string.Empty
                                                                }).ToList();

        try
        {
          foreach (RssFeedWebSiteUrlDetails rssFeedWebsite in rssFeedWebSiteDetails)
          {
            try
            {
              DateTime? dateSearch = null;

              if (rssFeedWebsite.PublishDate != string.Empty)
              {
                dateSearch = Helper.ExtractDate(rssFeedWebsite.PublishDate);
              }

              if (rssFeed.RSSFeedURL.IndexOf("https") != -1)
              {
                isHttps = true;
              }

              rssFeedWebsite.SiteURL = rssFeedWebsite.SiteURL.Replace("&amp;", "&");

              if (rssFeedWebsite.SiteURL.IndexOf("http") == -1)
              {
                if (isHttps)
                {
                  rssFeedWebsite.SiteURL = "https://" + rssFeedWebsite.SiteURL;
                }
                else
                {
                  rssFeedWebsite.SiteURL = "http://" + rssFeedWebsite.SiteURL;
                }
              }
              else
              {
                if (isHttps)
                {
                  rssFeedWebsite.SiteURL = rssFeedWebsite.SiteURL.Replace("http://", "https://");
                }
              }

              rssFeedWebsite.SiteURL = rssFeedWebsite.SiteURL.Trim();
              Uri pageURL = new Uri(rssFeedWebsite.SiteURL);

              bool recordExist = WebLink.GetAllLinksBasedUponWebSiteURL(rssFeedWebsite.SiteURL);
              List<LoaderLinkQueue> listOfRecords = WebLink.GetAllLinksInnerLinks();

              if (rssFeedWebsite.SiteURL.ToLower().IndexOf(".pdf") == -1)
              {
                string[] ignoreUrlArray = Helper.IgnoreExtNUrls.Split(',');

                bool isUnwantedURLExist = ignoreUrlArray.Any(y => Regex.IsMatch(rssFeedWebsite.SiteURL, string.Format(@"\b{0}\b", y), RegexOptions.IgnoreCase));

                // For Html sites
                if (!recordExist)
                {
                  if ((rssFeedWebsite.PublishDate == string.Empty || dateSearch == currentDate || dateSearch == previousDate || dateSearch > currentDate) && !isUnwantedURLExist)
                  {
                    using (var response = Helper.WebRequestResponse(rssFeedWebsite.SiteURL))
                    {
                      if (response != null)
                      {
                        Stream stream = response.GetResponseStream();
                        stream.ReadTimeout = -1;
                        StreamReader reader = new StreamReader(stream);
                        pageContent = reader.ReadToEnd();

                        string guid = Helper.GuidString();

                        InsertUpdateLoaderLinkLogRecord(processId, processInstanceId, rssFeed, true, rssFeedWebsite.SiteURL, guid);

                        LoaderLinkQueue scanningLinkDetail = new LoaderLinkQueue();

                        scanningLinkDetail.ProcessId = processId;
                        scanningLinkDetail.ProcessInstanceId = processInstanceId;

                        scanningLinkDetail.SiteURL = rssFeedWebsite.SiteURL;
                        scanningLinkDetail.GUID = guid;
                        scanningLinkDetail.WebSiteId = rssFeed.WebSiteId;
                        scanningLinkDetail.PublishDate = dateSearch;

                        WebLink.InsertInScanningLinkDetail(scanningLinkDetail, scanningLinkQueueId);

                        if (webLinkPagesRegExList != null && webLinkPagesRegExList.Count > 0)
                        {
                          string pageContentFinalData = string.Empty;
                          TimeSpan timeout = new TimeSpan(0, 1, 0);

                          foreach (WebLinkPageContentRegEx webLinkPageRegEx in webLinkPagesRegExList)
                          {
                            try
                            {
                              Regex webLinkRegEx = new Regex(webLinkPageRegEx.PageContentRegEx, RegexOptions.Compiled, timeout);

                              MatchCollection mathCollectionRegEx = webLinkRegEx.Matches(pageContent);

                              WebPageRegExGroup pageRegExGroup = mathCollectionRegEx.Cast<Match>()
                                                              .Where(x => x.Value != null)
                                                              .Select(z => new WebPageRegExGroup()
                                                              {
                                                                DateContent = z.Groups["date"] != null ? z.Groups["date"].Value : string.Empty,
                                                                PageHeading = z.Groups["heading"] != null ? z.Groups["heading"].Value : string.Empty,
                                                                PageContent = z.Groups["pageContent"] != null ? z.Groups["pageContent"].Value : string.Empty
                                                              }).FirstOrDefault();

                              // create the final page Content based upon
                              if (pageRegExGroup != null)
                              {
                                if (pageRegExGroup.DateContent != string.Empty)
                                {
                                  pageContentFinalData = pageContentFinalData + pageRegExGroup.DateContent + "<br/>";
                                }

                                if (pageRegExGroup.PageHeading != string.Empty)
                                {
                                  pageContentFinalData = pageContentFinalData + "<h1>" + pageRegExGroup.PageHeading + "</h1> ";
                                }

                                if (pageRegExGroup.PageContent != string.Empty)
                                {
                                  pageContentFinalData = pageContentFinalData + pageRegExGroup.PageContent;
                                }
                              }
                            }
                            catch (Exception ex)
                            {
                              log.LogError(LoggingLevel.Error, "BadRequest", "TimeOut Occur: ", ex, null);
                            }
                          }

                          pageContent = pageContentFinalData;
                        }

                        pageContent = Helper.RemoveUnWantedAnchorTags(pageContent);
                        pageContent = Helper.RemoveHeaderFooterTags(pageContent, scanningLinkDetail);

                        if (rssFeed.RSSFeedURL.IndexOf("thehill.com") == -1)
                        {
                          pageContent = Regex.Replace(pageContent, "<head.*?>(.|\n)*?</head>", "");
                        }

                        pageContent = Regex.Replace(pageContent, "<header.*?>(.|\n)*?</header>", "");
                        pageContent = Regex.Replace(pageContent, "<em.*?>(.|\n)*?</em>", "");
                        pageContent = Regex.Replace(pageContent, "<footer.*?>(.|\n)*?</footer>", "");
                        
                        pageContent = Helper.RemoveUnWantedHtmlSource(pageContent);

                        // Check for the File with PDF Extension starts here
                        linkDetails.pageContent = pageContent;
                        linkDetails.GUID = guid;
                        linkDetails.InstanceName = Helper.WebSiteCategory.RSSFeeds.ToString();
                        linkDetails.ProcessId = processId;
                        linkDetails.ProcessInstanceId = processInstanceId;
                        linkDetails.SiteURL = rssFeedWebsite.SiteURL;
                        linkDetails.WebSiteId = rssFeed.WebSiteId;

                        method.FetchPDFDocuments(linkDetails, listOfRecords, scanningLinkQueueId, pageURL.Host.ToString(), listOfURLs);
                        // File with PDF Extension ends here

                        rssFeedPageDetails = CreateSolrPageDetail(pageContent, guid, rssFeedWebsite.SiteURL, rssFeed.Id);
                        solrPageDetails.Add(rssFeedPageDetails);

                        rssFeedDetailHistory = CreateSolrPageDetailHistory(processId, processInstanceId, pageContent, guid, rssFeedWebsite.SiteURL);
                        solrPageDetailHistoryList.Add(rssFeedDetailHistory);
                      }
                    }
                  }
                  else
                  {
                    InsertUpdateLoaderLinkLogRecord(processId, processInstanceId, rssFeed, false, rssFeedWebsite.SiteURL, "");
                  }
                }
              }
              else
              {
                if (!recordExist)
                {
                  StringBuilder text = new StringBuilder();
                  if (dateSearch == currentDate || dateSearch == previousDate || dateSearch > currentDate)
                  {
                    text.Append("<html><body>");

                    using (PdfReader reader = new PdfReader(rssFeedWebsite.SiteURL))
                    {
                      for (int page = 1; page <= reader.NumberOfPages; page++)
                      {
                        ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                        pageContent = PdfTextExtractor.GetTextFromPage(reader, page, strategy);

                        pageContent = Regex.Replace(pageContent, "/\t+/g", "");

                        pageContent = Regex.Replace(pageContent, "s/^\\s+//", "");
                        pageContent = Regex.Replace(pageContent, "s/\\s+$//", "");

                        pageContent = Regex.Replace(pageContent, "\\n\\s\\n*", "\n\n");

                        pageContent = Regex.Replace(pageContent, "\n\n", "</p><p>").Replace("\n", "<br>");

                        text.Append(pageContent);
                      }
                    }

                    string guid = Helper.GuidString();

                    InsertUpdateLoaderLinkLogRecord(processId, processInstanceId, rssFeed, true, rssFeedWebsite.SiteURL, guid);

                    LoaderLinkQueue scanningLinkDetail = new LoaderLinkQueue();

                    scanningLinkDetail.ProcessId = processId;
                    scanningLinkDetail.ProcessInstanceId = processInstanceId;

                    scanningLinkDetail.SiteURL = rssFeedWebsite.SiteURL;
                    scanningLinkDetail.GUID = guid;
                    scanningLinkDetail.WebSiteId = rssFeed.WebSiteId;
                    scanningLinkDetail.PublishDate = dateSearch;

                    WebLink.InsertInScanningLinkDetail(scanningLinkDetail, scanningLinkQueueId);

                    string appendParagraphTag = text.ToString();
                    appendParagraphTag = appendParagraphTag + "</p>";
                    int paraGraphIndex = appendParagraphTag.IndexOf("</p>");

                    if (appendParagraphTag.Length > 4)
                    {
                      appendParagraphTag = appendParagraphTag.Remove(paraGraphIndex, 4);
                    }

                    appendParagraphTag = appendParagraphTag + "</body></html>";

                    rssFeedPageDetails = CreateSolrPageDetail(appendParagraphTag, guid, rssFeedWebsite.SiteURL, rssFeed.Id);
                    solrPageDetails.Add(rssFeedPageDetails);

                    rssFeedDetailHistory = CreateSolrPageDetailHistory(processId, processInstanceId, appendParagraphTag, guid, rssFeedWebsite.SiteURL);
                    solrPageDetailHistoryList.Add(rssFeedDetailHistory);
                  }
                  else
                  {
                    InsertUpdateLoaderLinkLogRecord(processId, processInstanceId, rssFeed, false, rssFeedWebsite.SiteURL, "");
                  }
                }
                // For PDF sites
              }
            }
            catch (Exception rssFeedEx)
            {
              log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in single record of FetchRSSFeedContents method", rssFeedEx, null);
            }
          }

          if (solrPageDetails.Count > 0)
          {
            InsertRecordToSolRDB(solrPageDetails);
          }

          if (solrPageDetailHistoryList.Count > 0)
          {
            InsertRecordToSolrHistoryDB(solrPageDetailHistoryList);
          }
        }
        catch (Exception ex)
        {
          log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown while calling regular expression of FetchRSSFeedContents method", ex, null);
        }
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in the FetchRSSFeedContents method", ex, null);
      }
    }

    private PageDetails CreateSolrPageDetail(string pageContent, string guid, string siteURL, int id)
    {
      PageDetails rssFeedPageDetails = new PageDetails();

      string[] rssPageSource = { pageContent };

      rssFeedPageDetails.PageSource = rssPageSource;
      rssFeedPageDetails.ItemId = id;
      rssFeedPageDetails.GuidId = guid;

      rssFeedPageDetails.AddedDateTime = DateTime.Now;
      rssFeedPageDetails.Status = string.Empty;
      rssFeedPageDetails.URL = siteURL;

      return rssFeedPageDetails;
    }

    private PageDetailHistory CreateSolrPageDetailHistory(int processId, int processInstanceId, string pageContent, string guid, string siteURL)
    {
      PageDetailHistory rssFeedDetailHistory = new PageDetailHistory();

      string[] rssPageSource = { pageContent };

      rssFeedDetailHistory.ProcessId = processId;
      rssFeedDetailHistory.ProcessInstanceId = processInstanceId;
      rssFeedDetailHistory.URL = siteURL;

      rssFeedDetailHistory.PageSource = rssPageSource;
      rssFeedDetailHistory.GuidId = guid;
      rssFeedDetailHistory.AddedDateTime = Helper.GetCurrentDateTime();

      return rssFeedDetailHistory;
    }

    private bool InsertUpdateLoaderLinkLogRecord(int processId, int processInstanceId, RssFeedDetails rssFeed, bool isSuccessful, string siteURL, string guid)
    {
      LoaderLinkQueue linkPageRecords = new LoaderLinkQueue();

      linkPageRecords.ProcessId = processId;
      linkPageRecords.ProcessInstanceId = processInstanceId;

      linkPageRecords.WebSiteId = rssFeed.WebSiteId;
      linkPageRecords.SiteURL = siteURL;

      linkPageRecords.IsSuccessful = isSuccessful;

      linkPageRecords.GUID = guid;
      linkPageRecords.LinkLevel = 1;
      linkPageRecords.InstanceName = Helper.WebSiteCategory.RSSFeeds.ToString();
      linkPageRecords.NewerProcessId = processId;

      bool result = WebLink.InsertSubLinkLogRecords(linkPageRecords);

      return result;
    }
  }
}