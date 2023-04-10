using BCMStrategy.ContentLoader.Common;
using BCMStrategy.ContentLoader.DynamicContents.Abstract;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Data.Repository.Concrete;
using BCMStrategy.Logger;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace BCMStrategy.ContentLoader.DynamicContents.Repository
{
  public class DynamicContentRepository : IDynamicContent
  {
    private static readonly EventLogger<DynamicContentRepository> log = new EventLogger<DynamicContentRepository>();

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

    private IWebDocument _webDocument;

    private IWebDocument WebDocument
    {
      get
      {
        if (_webDocument == null)
        {
          _webDocument = new WebDocumentRepository();
        }

        return _webDocument;
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

    public void DynamicContentLoaderProcess(int processId, int processInstanceId, IWebDriver webDriver)
    {
      int linkLevel = 1;
      bool isPdfFile = false;

      HashSet<string> listOfURLs = new HashSet<string>();
      List<PageDetailHistory> solrPageDetailHistoryList = new List<PageDetailHistory>();

      List<LoaderLinkQueue> linkRecords = WebDocument.GetDocumentList(processId, processInstanceId);

      string pageType = linkRecords.Select(x => x.InstanceName).FirstOrDefault();

      ////string pageType = Convert.ToString(Helper.WebSiteCategory.DynamicContent);

      List<LoaderLinkQueue> listOfRecords = WebLink.GetAllLinksInnerLinks();

      List<LoaderLinkQueue> siteLinks = WebLink.GetMessageBasedUponType(pageType, processId, processInstanceId);

      List<PageDetails> solrDynamicPageList = FetchSiteContentSolrInsert(siteLinks, processId, processInstanceId, listOfRecords, listOfURLs, webDriver);

      if (solrDynamicPageList.Count > 0)
      {
        foreach (PageDetails pageInfo in solrDynamicPageList)
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

      if (solrDynamicPageList.Count > 0)
      {
        SolrConfiguration.InsertInRange(solrDynamicPageList);

        SolrPageDetail.InsertPageDetailHistory(solrPageDetailHistoryList);

        foreach (PageDetails siteLink in solrDynamicPageList)
        {
          LoaderLinkQueue pageRegExLinkAvailable = WebLink.GetWebLinkHtmlLinksRegEx(siteLink.ItemId);

          log.LogSimple(LoggingLevel.Information, "Reading from the Parent Web site Starts here. - " + siteLink.URL);

          LoaderLinkQueue webPageDetails = new LoaderLinkQueue();

          webPageDetails.ProcessId = processId;
          webPageDetails.ProcessInstanceId = processInstanceId;

          webPageDetails.SiteURL = siteLink.URL;
          webPageDetails.WebSiteId = siteLink.ItemId;
          webPageDetails.GUID = siteLink.GuidId;
          webPageDetails.RegEx = pageRegExLinkAvailable.RegEx;
          webPageDetails.pageContent = siteLink.PageSource[0];

          if (pageRegExLinkAvailable != null)
          {
            webPageDetails.RegEx = pageRegExLinkAvailable.RegEx;
            webPageDetails.AllHtmlLinksFetch = pageRegExLinkAvailable.AllHtmlLinksFetch;
          }

          string instanceName = siteLinks.Where(x => x.SiteURL == siteLink.URL).Select(x => x.InstanceName).FirstOrDefault();

          if (instanceName.Equals(Helper.WebSiteCategory.DynamicContent.ToString()))
          {
            cmnMethod.GetInnerChildContents(webPageDetails, listOfRecords, ref linkLevel, 0, listOfURLs, ref isPdfFile, pageType);
          }
          else if (instanceName.Equals(Helper.WebSiteCategory.PDFDynamicContent.ToString()))
          {
            Uri pageURL = new Uri(siteLink.URL);

            cmnMethod.FetchPDFDocuments(webPageDetails, listOfRecords, 0, pageURL.Host, listOfURLs);
          }

          log.LogSimple(LoggingLevel.Information, "Reading from the Parent Web site Ends here.- " + siteLink.URL);
        }

        log.LogSimple(LoggingLevel.Information, "Completion for all the sites ends here.");
      }

    }

    private List<PageDetails> FetchSiteContentSolrInsert(List<LoaderLinkQueue> webSiteLinks, int processId, int processInstanceId, List<LoaderLinkQueue> listOfRecords, HashSet<string> listOfURLs, IWebDriver webDriver)
    {
      string pageHtmlContent = string.Empty;
      ////bool result = false;

      List<PageDetails> solrPageDetails = new List<PageDetails>();
      PageDetails pageDetailData;

      foreach (LoaderLinkQueue link in webSiteLinks)
      {
        try
        {
          listOfURLs.Add(link.SiteURL);

          webDriver.Navigate().GoToUrl(link.SiteURL);

          Thread.Sleep(Int32.Parse(ConfigurationManager.AppSettings["WebDriverTimeOut"]));
          pageDetailData = new PageDetails();

          bool isContentInserted = false;
          bool isContentUpdated = false;

          pageHtmlContent = webDriver.PageSource;

          pageHtmlContent = Helper.RemoveUnWantedAnchorTags(pageHtmlContent);
          pageHtmlContent = Regex.Replace(pageHtmlContent, "<head.*?>(.|\n)*?</head>", "");
          pageHtmlContent = Helper.RemoveUnWantedHtmlSource(pageHtmlContent);

          var newURLDetails = listOfRecords.Find(x => x.SiteURL == link.SiteURL && x.LinkLevel == 0);

          if (newURLDetails == null)
          {
            string[] pageSource = { pageHtmlContent };

            pageDetailData.PageSource = pageSource;
            pageDetailData.ItemId = link.Id;
            pageDetailData.GuidId = link.GUID;
            pageDetailData.AddedDateTime = Helper.GetCurrentDateTime();
            pageDetailData.Status = string.Empty;
            pageDetailData.URL = link.SiteURL;

            solrPageDetails.Add(pageDetailData);

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
            if (solrPageSourceDetail != pageHtmlContent)
            {
              if (solrUpdatePageDetail.Count > 0)
              {
                foreach (PageDetails solrPageUpdate in solrUpdatePageDetail)
                {
                  string[] pageSource = { pageHtmlContent };
                  solrPageUpdate.PageSource = pageSource;
                  solrPageDetails.Add(solrPageUpdate);
                }
              }
              else
              {
                string[] pageSource = { pageHtmlContent };

                pageDetailData.PageSource = pageSource;
                pageDetailData.ItemId = link.Id;
                pageDetailData.GuidId = newURLDetails.GUID;
                pageDetailData.AddedDateTime = Helper.GetCurrentDateTime();
                pageDetailData.Status = string.Empty;
                pageDetailData.URL = newURLDetails.SiteURL;

                solrPageDetails.Add(pageDetailData);
              }

              LoaderLinkQueue linkRecordUpdate = new LoaderLinkQueue();

              linkRecordUpdate.ProcessId = processId;
              linkRecordUpdate.ProcessInstanceId = processInstanceId;
              linkRecordUpdate.WebSiteId = link.Id;
              linkRecordUpdate.SiteURL = link.SiteURL;

              linkRecordUpdate.GUID = link.GUID;

              linkRecordUpdate.WebLinkBytes = 0;
              linkRecordUpdate.NewerProcessId = processId;

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

          WebLink.UpdateContentLoaderLog(viewModel);
        }
        catch (Exception ex)
        {
          ProcessEventLog processEventLog = new ProcessEventLog();

          processEventLog.ProcessEventId = processId;
          processEventLog.ProcessInstanceId = processInstanceId;

          processEventLog.SiteUrl = link.SiteURL;
          processEventLog.Created = Helper.GetCurrentDateTime();
          processEventLog.CreatedBy = Helper.ShowScrapperName();

          ProcessEvents.InsertProcessEventLog(processEventLog);

          LoaderErrorLogViewModel viewModel = new LoaderErrorLogViewModel()
          {
            ProcessId = processId,
            ProcessInstanceId = processInstanceId,
            WebSiteId = link.Id,
            SiteUrl = link.SiteURL,
            ErrorDesc = "InnerException:" + ex.InnerException + "Message :" + ex.Message
          };
          WebLink.UpdateLoaderErrorLog(viewModel);

        }
      }

      return solrPageDetails;
    }

    private PageDetailsView GetPageSourceIdByGuidId(LoaderLinkQueue loaderLinkQueue)
    {
      SolrSearchParameters searchParameters = new SolrSearchParameters();

      searchParameters.FreeSearch = "guidId:\"" + loaderLinkQueue.GUID + "\"";

      PageDetailsView pageDetails = SolrConfiguration.GetPageSourceIdByGuidId(searchParameters);

      return pageDetails;
    }
  }
}