using BCMStrategy.ContentLoader.Common;
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
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BCMStrategy.ContentLoader.Documents.API
{
  public class WebApi
  {
    private static readonly EventLogger<WebApi> log = new EventLogger<WebApi>();

    readonly CommonMethod cmnMethod = new CommonMethod();

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

    /// <summary>
    /// Get Simple Web Page Document Details
    /// </summary>
    /// <param name="resultType">result Type</param>
    /// <returns></returns>
    public List<LoaderLinkQueue> GetDocumentDetails(int processId, int processInstanceId)
    {
      try
      {
        List<LoaderLinkQueue> webDocumentLinks = WebDocument.GetDocumentList(processId, processInstanceId);

        return webDocumentLinks;
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in GetDocumentDetails method", ex, null);
        throw;
      }
    }

    public int SaveScraperEvent(Events scraperEvents)
    {
      try
      {
        int result = 0;

        result = ProcessEvents.InsertEvents(scraperEvents);

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

        result = ProcessEvents.UpdateEvents(scraperEvents);

        return result;
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in UpdateScraperEvent method", ex, null);
        throw;
      }
    }

    public Task<bool> SolrDbLinkToSOLRList(List<LoaderLinkQueue> pdfDetails, List<LoaderLinkQueue> listOfRecords, HashSet<string> listOfURLs)
    {
      Task<bool> solrInsert = null;
      bool newerContent = true;
      bool notNewerContent = false;

      try
      {
        Encoding textEncoding = Encoding.UTF8;

        PageDetails pageDetails;
        PageDetailHistory pageDetailHistory;

        foreach (LoaderLinkQueue pdfPages in pdfDetails)
        {   
          if (listOfURLs.Contains(pdfPages.SiteURL))
          {
            continue;
          }
          else
          {
            listOfURLs.Add(pdfPages.SiteURL);
          }

          List<PageDetails> solrPageDetails = new List<PageDetails>();
          List<PageDetailHistory> solrHistoryPageDetails = new List<PageDetailHistory>();

          LoaderLinkQueue specificPageDetail = listOfRecords.Find(x => x.SiteURL == pdfPages.SiteURL && x.LinkLevel == 0);

          PageDetailsView solrPageDetail = GetPageSourceIdByGuidId(specificPageDetail);

          ICollection<PageDetails> solrUpdatePageDetail = solrPageDetail.Products;
          string solrPageSourceDetail = string.Empty;

          if (solrUpdatePageDetail.Count > 0)
          {
            foreach (PageDetails solrPageUpdate in solrUpdatePageDetail)
            {
              solrPageSourceDetail = solrPageUpdate.PageSource[0];
              pdfPages.pageContent = solrPageSourceDetail;
            }
          }
          else
          {
            HttpWebResponse webResponse = Helper.WebRequestResponse(pdfPages.SiteURL);

            if (webResponse != null)
            {
              Stream stream = webResponse.GetResponseStream();
              stream.ReadTimeout = -1;
              StreamReader reader = new StreamReader(stream);
              string pageContent = reader.ReadToEnd();

              string[] pageSource = { pageContent };

              pageDetails = new PageDetails();

              pageDetails.PageSource = pageSource;
              pageDetails.ItemId = pdfPages.Id;
              pageDetails.GuidId = specificPageDetail.GUID;
              pageDetails.AddedDateTime = Helper.GetCurrentDateTime();
              pageDetails.Status = string.Empty;
              pageDetails.URL = pdfPages.SiteURL;

              solrPageDetails.Add(pageDetails);

              SolrConfiguration.InsertInRange(solrPageDetails);

              pdfPages.pageContent = pageContent;
              solrPageDetails = new List<PageDetails>();
            }
          }

          List<LoaderLinkQueue> totalPDFPages = Helper.SearchPDFDocuments(pdfPages, listOfRecords);

          if (totalPDFPages.Count > 0)
          {
            LoaderLinkQueue loaderMasterPageRecord = new LoaderLinkQueue()
            {
              InstanceName = pdfPages.InstanceName,
              IsReadTaken = true,
              IsSuccessful = true,
              GUID = pdfPages.GUID,
              LinkLevel = 0,
              SiteURL = pdfPages.SiteURL,
              ProcessId = pdfPages.ProcessId,
              ProcessInstanceId = pdfPages.ProcessInstanceId,
              NewerProcessId = pdfPages.ProcessId,
              WebLinkBytes = pdfPages.WebLinkBytes,
              WebLinkTypeId = pdfPages.WebLinkTypeId,
              WebSiteId = pdfPages.WebSiteId
            };

            int scanningLinkQueueId = WebLink.InsertInScanningLinkQueue(loaderMasterPageRecord);

            ContentLoaderLogViewModel viewModel = new ContentLoaderLogViewModel()
            {
              ProcessId = pdfPages.ProcessId,
              ProcessInstanceId = pdfPages.ProcessInstanceId,
              SiteUrl = pdfPages.SiteURL,
              WebSiteId = pdfPages.WebSiteId,
              LinkLevel = 1,
              IsContentInserted = pdfPages.NewerProcessId == null ? newerContent : notNewerContent,
              IsContentUpdated = pdfPages.NewerProcessId != null ? newerContent : notNewerContent
            };

            WebLink.UpdateContentLoaderLog(viewModel);

            WebLink.UpdateLoaderLinkLogMasterRecord(loaderMasterPageRecord);

            foreach (LoaderLinkQueue pdfFile in totalPDFPages)
            {
              if (listOfURLs.Contains(pdfFile.SiteURL))
              {
                continue;
              }
              else
              {
                listOfURLs.Add(pdfFile.SiteURL);
              }

              try
              {
                pdfFile.SiteURL = pdfFile.SiteURL.Replace("//", "/").Replace(":/", "://").Replace("&amp;", "&").Replace("><span class=", "");

                int lastIndex = pdfFile.SiteURL.LastIndexOf(".pdf");
                string finalURL = pdfFile.SiteURL;

                if (lastIndex > 0)
                {
                  finalURL = finalURL.Substring(0, lastIndex + 4);
                  pdfFile.SiteURL = finalURL;
                }

                var pageDetail = listOfRecords.Find(x => x.SiteURL == pdfFile.SiteURL);

                if (pageDetail == null)
                {
                  StringBuilder text = new StringBuilder();

                  bool isContentInserted = false;
                  bool isContentUpdated = false;

                  if (pdfFile.SiteURL.IndexOf("http") == -1)
                  {
                    pdfFile.SiteURL = "http://" + pdfFile.SiteURL;
                  }

                  text.Append("<html><body>");
                  string pageContent = string.Empty;

                  #region Manual Methods
                  HttpWebRequest request = (HttpWebRequest)WebRequest.Create(pdfFile.SiteURL);
                  request.Method = "GET";

                  request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-gb,en;q=0.5");
                  request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
                  request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                  request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:12.0) Gecko/20100101 Firefox/12.0";
                  request.KeepAlive = true;
                  HttpWebResponse resp = (HttpWebResponse)request.GetResponse();
                  #endregion

                  using (Stream stream = resp.GetResponseStream())
                  {
                    MemoryStream memoryStream = ConvertStreamToMemoryStream(stream);
                    using (PdfReader reader = new PdfReader((byte[])memoryStream.ToArray())) ////
                    {
                      for (int page = 1; page <= reader.NumberOfPages; page++)
                      {
                        ITextExtractionStrategy strategy = new LocationTextExtractionStrategy();
                        pageContent = PdfTextExtractor.GetTextFromPage(reader, page, strategy);

                        pageContent = Regex.Replace(pageContent, "/\t+/g", "");

                        pageContent = Regex.Replace(pageContent, "s/^\\s+//", "");
                        pageContent = Regex.Replace(pageContent, "s/\\s+$//", "");

                        pageContent = Regex.Replace(pageContent, "\\n\\s\\n*", "\n\n");

                        pageContent = Regex.Replace(pageContent, "\n\n", "</p><p>").Replace("\n", "<br>");

                        text.Append(pageContent);
                      }

                      string appendParagraphTag = text.ToString();
                      appendParagraphTag = appendParagraphTag + "</p>";
                      int paraGraphIndex = appendParagraphTag.IndexOf("</p>");

                      if (appendParagraphTag.Length > 4)
                      {
                        appendParagraphTag = appendParagraphTag.Remove(paraGraphIndex, 4);
                      }

                      appendParagraphTag = appendParagraphTag + "</body></html>";

                      byte[] arrayBytes = textEncoding.GetBytes(appendParagraphTag);

                      pageDetails = new PageDetails();
                      pageDetailHistory = new PageDetailHistory();

                      string[] pageSource = { text.ToString() };

                      pageDetails.PageSource = pageSource;
                      pageDetails.ItemId = pdfFile.Id;
                      pageDetails.GuidId = pdfFile.GUID;

                      pageDetails.AddedDateTime = DateTime.Now;
                      pageDetails.Status = string.Empty;
                      pageDetails.URL = pdfFile.SiteURL;

                      solrPageDetails.Add(pageDetails);

                      LoaderLinkQueue siteLink = new LoaderLinkQueue();

                      siteLink.NewerProcessId = pdfFile.ProcessId;
                      siteLink.ProcessId = pdfFile.ProcessId;
                      siteLink.ProcessInstanceId = pdfFile.ProcessInstanceId;
                      siteLink.SiteURL = pdfFile.SiteURL;
                      siteLink.GUID = pdfFile.GUID;

                      siteLink.WebLinkBytes = Convert.ToDecimal(arrayBytes.Length);
                      siteLink.WebSiteId = pdfFile.WebSiteId;

                      siteLink.InstanceName = Convert.ToString(Helper.WebSiteCategory.PDFDocument);
                      siteLink.LinkLevel = 1;
                      siteLink.IsSuccessful = true;
                      siteLink.PageType = Convert.ToInt32(Helper.PageTypes.PDF);
                      WebLink.UpdateLoaderLinkLogMasterRecord(siteLink);

                      WebLink.InsertInScanningLinkDetail(siteLink, scanningLinkQueueId);

                      isContentInserted = true;

                      pageDetailHistory.PageSource = pageSource;
                      pageDetailHistory.ProcessId = pdfFile.ProcessId;
                      pageDetailHistory.ProcessInstanceId = pdfFile.ProcessInstanceId;

                      pageDetailHistory.URL = pdfFile.SiteURL;
                      pageDetailHistory.PageSource = pageSource;

                      if (pageDetail == null)
                      {
                        pageDetailHistory.GuidId = pdfFile.GUID;
                      }

                      pageDetailHistory.AddedDateTime = DateTime.Now;

                      solrHistoryPageDetails.Add(pageDetailHistory);

                      viewModel = new ContentLoaderLogViewModel()
                      {
                        ProcessId = pdfFile.ProcessId,
                        ProcessInstanceId = pdfFile.ProcessInstanceId,
                        SiteUrl = pdfFile.SiteURL,
                        WebSiteId = pdfFile.WebSiteId,
                        LinkLevel = 1,
                        IsContentInserted = isContentInserted,
                        IsContentUpdated = isContentUpdated
                      };

                      WebLink.UpdateContentLoaderLog(viewModel);
                    }
                  }
                }
              }
              catch (Exception ex)
              {
                LoaderErrorLogViewModel errorLogViewModel = new LoaderErrorLogViewModel()
                {
                  ProcessId = pdfFile.ProcessId,
                  ProcessInstanceId = pdfFile.ProcessInstanceId,
                  WebSiteId = pdfFile.WebSiteId,
                  SiteUrl = pdfFile.SiteURL,
                  ErrorDesc = "InnerException:" + ex.InnerException + "Message :" + ex.Message
                };

                WebLink.UpdateLoaderErrorLog(errorLogViewModel);
              }
            }

            if (solrPageDetails.Count == 0)
            {
              int lexiconCount = cmnMethod.SearchLexiconTerm(pdfPages);

              // Compare it with the lexicon of this page earlier.
              int olderLexiconCount = WebLink.GetWebLinkLexiconCount(pdfPages.WebSiteId);

              if (olderLexiconCount == lexiconCount)
              {
                WebLink.DeleteScanningLinkQueue(scanningLinkQueueId);
              }
              else
              {
                pdfPages.LexiconCount = lexiconCount;
                WebLink.UpdateLoaderLinkLogLexiconCount(pdfPages);
              }
            }

            if (solrPageDetails.Count > 0)
            {
              solrInsert = InsertRecordToSolRDB(solrPageDetails);

              int lexiconCount = cmnMethod.SearchLexiconTerm(pdfPages);
              pdfPages.LexiconCount = lexiconCount;
              WebLink.UpdateLoaderLinkLogLexiconCount(pdfPages);
            }

            if (solrHistoryPageDetails.Count > 0)
            {
              InsertRecordToSolrHistoryDB(solrHistoryPageDetails);
            }
          }

        }
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in SolrDbLinkToSOLRList method", ex, null);
      }

      return solrInsert;
    }

    private MemoryStream ConvertStreamToMemoryStream(Stream stream)
    {
      MemoryStream memoryStream = new MemoryStream();

      if (stream != null)
      {

        byte[] buffer = ReadFully(stream);

        if (buffer != null)
        {
          var binaryWriter = new BinaryWriter(memoryStream);
          binaryWriter.Write(buffer);
        }
      }
      return memoryStream;
    }

    private byte[] ReadFully(Stream input)
    {

      byte[] buffer = new byte[16 * 1024];
      using (MemoryStream ms = new MemoryStream())
      {
        int read;
        while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
        {
          ms.Write(buffer, 0, read);
        }
        return ms.ToArray();
      }
    }

    public PageDetailsView GetPageSourceIdByGuidId(LoaderLinkQueue loaderLinkQueue)
    {
      SolrSearchParameters searchParameters = new SolrSearchParameters();

      searchParameters.FreeSearch = "guidId:\"" + loaderLinkQueue.GUID + "\"";

      PageDetailsView pageDetails = SolrConfiguration.GetPageSourceIdByGuidId(searchParameters);

      return pageDetails;
    }

    /// <summary>
    /// Insert Record to the SOLR Database
    /// </summary>
    /// <param name="solrList">list of documents to be inserted in the SOLR Database</param>
    /// <returns>boolean value for inserting records to the database</returns>
    public async Task<bool> InsertRecordToSolRDB(List<PageDetails> solrList)
    {
      try
      {

        SolrConfiguration.InsertInRange(solrList);

        return true;
      }
      catch (SolrConnectionException e)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in InsertRecordToSolRDB method", e, null);
        throw;
      }
    }

    private void InsertRecordToSolrHistoryDB(List<PageDetailHistory> solrDetailList)
    {
      try
      {
        SolrPageDetail.InsertPageDetailHistory(solrDetailList);
      }
      catch (SolrConnectionException e)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in InsertRecordToSolrHistoryDB method", e, null);
      }
    }

    public async Task<bool> InsertSingleRecordToSolRDB(PageDetails solrRecord)
    {
      try
      {
        List<PageDetails> solrRecordList = new List<PageDetails>();

        solrRecordList.Add(solrRecord);

        SolrConfiguration.InsertInRange(solrRecordList);

        return true;
      }
      catch (SolrConnectionException e)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in InsertSingleRecordToSolRDB method", e, null);
        throw;
      }
    }

    public List<LoaderLinkQueue> GetLinkRecords()
    {
      List<LoaderLinkQueue> linkRecords = WebLink.GetAllLinksInnerLinks();

      return linkRecords;
    }
  }
}