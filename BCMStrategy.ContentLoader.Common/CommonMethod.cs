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
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace BCMStrategy.ContentLoader.Common
{
  public class CommonMethod
  {
    private static readonly EventLogger<CommonMethod> log = new EventLogger<CommonMethod>();

    private IWebLink _webLinks;

    private IWebLink WebLinks
    {
      get
      {
        if (_webLinks == null)
        {
          _webLinks = new WebLinkRepository();
        }

        return _webLinks;
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

    private static ILexicon _lexicon;

    private static ILexicon Lexicon
    {
      get
      {
        if (_lexicon == null)
        {
          _lexicon = new LexiconRepository();
        }

        return _lexicon;
      }
    }


    private static IScrapperActivityRepository _scrapperActivity;

    private static IScrapperActivityRepository ScrapperActivity
    {
      get
      {
        if (_scrapperActivity == null)
        {
          _scrapperActivity = new ScrapperActivityRepository();
        }

        return _scrapperActivity;
      }
    }

    public void GetInnerChildContents(LoaderLinkQueue siteLink, List<LoaderLinkQueue> listOfRecords, ref int linkLevel, int scanningLinkQueueId, HashSet<string> listOfURLs, ref bool isPdfFound, string pageType = "")
    {
      List<PageDetails> solrPageAddDetails = new List<PageDetails>();
      List<PageDetails> solrPageUpdateDetails = new List<PageDetails>();
      string finalContent = string.Empty;
      bool isHttps = false;

      try
      {
        List<string> finalUrl = new List<string>();

        if (siteLink.SiteURL.IndexOf("https") != -1)
        {
          isHttps = true;
        }

        // Adding record in the scanning link queue table starts here
        if (linkLevel == 1)
        {
          scanningLinkQueueId = WebLinks.InsertInScanningLinkQueue(siteLink);
        }
        // Adding record in the scanning link queue table ends here

        if (linkLevel == 1 && pageType.Equals(Helper.WebSiteCategory.DynamicContent.ToString()))
        {
          if (siteLink.RegEx != null)
          {
            finalContent = siteLink.pageContent;

            Regex includeRegex = new Regex(siteLink.RegEx);

            MatchCollection includeMatches = includeRegex.Matches(finalContent);

            finalUrl = includeMatches.Cast<Match>().Where(x => x.Value != null
                                                    && x.Value.Length > 1 && x.Groups["url"].Value.Length > 1).Select(x => x.Groups["url"].Value).Distinct().ToList();
          }
        }
        else
        {
          using (var response = Helper.WebRequestResponse(siteLink.SiteURL))
          {
            if (response != null)
            {
              Stream stream = response.GetResponseStream();
              stream.ReadTimeout = -1;
              StreamReader reader = new StreamReader(stream);
              string pageContent = reader.ReadToEnd();

              Thread.Sleep(Convert.ToInt32(ConfigurationManager.AppSettings["ThreadSleepInterval"]));

              pageContent = Helper.RemoveUnWantedAnchorTags(pageContent);

              finalContent = Regex.Replace(pageContent, "<(?!a|\\/a\\s*\\/?)[^>]+>", string.Empty);

              finalContent = Helper.RemoveHeaderFooterTags(finalContent, siteLink);

              if (linkLevel == 1)
              {
                if ((finalContent.IndexOf("post-header") == -1) && (siteLink.SiteURL != "https://www.politico.eu/" && siteLink.SiteURL.IndexOf("www.suomenpankki.fi") == -1))
                {
                  finalContent = Regex.Replace(finalContent, "<header.*?>(.|\n)*?</header>", "");
                }

                finalContent = Regex.Replace(finalContent, "<em.*?>(.|\n)*?</em>", "");

                if (siteLink.SiteURL != "https://www.politico.eu/")
                {
                  finalContent = Regex.Replace(finalContent, "<footer.*?>(.|\n)*?</footer>", "");
                }

                finalContent = Helper.RemoveUnWantedHtmlSource(finalContent);
              }

              if (finalContent.IndexOf("www.imf.org") != -1)
              {
                finalContent = Regex.Replace(finalContent, "\\/en/Publications/Publications-By-Author(?'url'[^\"]+).*?", "");
                finalContent = Regex.Replace(finalContent, "\\/en/Publications/Publications-By-Subject(?'url'[^\"]+).*?", "");
              }

              finalContent = Regex.Replace(finalContent, "<nav\\s*id=\"main-navigation-menu\".*?>[\\s\\S]*?</nav>", "");

              string[] ingnorUrlArray = Helper.IgnoreExtNUrls.Split(',');

              string[] ignoreUrlExternal = Helper.IgnoreExtLinks.Split(',');

              if (linkLevel == 1 && siteLink.AllHtmlLinksFetch)
              {
                Regex includeRegex = new Regex("<a\\s+(?:[^>]*?\\s+)?href=\"(?'url'[^\"]*)\".*?</a>");

                MatchCollection includeMatches = includeRegex.Matches(finalContent);

                finalUrl = includeMatches.Cast<Match>().Where(x => x.Value != null
                                                            && x.Value.Length > 1
                                                            && !ignoreUrlExternal.Any(y => x.Groups["url"].Value.Contains(y))
                                                            && !ingnorUrlArray.Any(y => Regex.IsMatch(x.Groups["url"].Value, string.Format(@"\b{0}\b", y), RegexOptions.IgnoreCase))
                                                            && x.Groups["url"].Value.Length > 1).Select(x => x.Groups["url"].Value).Distinct().ToList();
              }
              else
              {
                Regex includeRegex = new Regex(".<a\\s+(?:[^>]*?\\s+)?href=\"(?'url'[^\"]*)\".*?</a>");

                MatchCollection includeMatches = includeRegex.Matches(finalContent);

                finalUrl = includeMatches.Cast<Match>().Where(x => x.Value != null
                                                          && x.Value.Length > 1
                                                          && !x.Value.Contains("><")
                                                          && !ignoreUrlExternal.Any(y => x.Groups["url"].Value.Contains(y))
                                                          && !ingnorUrlArray.Any(y => Regex.IsMatch(x.Groups["url"].Value, string.Format(@"\b{0}\b", y), RegexOptions.IgnoreCase))
                                                          && x.Groups["url"].Value.Length > 1).Select(x => x.Groups["url"].Value).Distinct().ToList();

              }
            }
          }
        }

        HashSet<string> finalURLs = new HashSet<string>(finalUrl);

        // check for the condition if there are links found on the main page
        if (finalURLs.Count > 0)
        {
          Uri pageURL = new Uri(siteLink.SiteURL);

          string filterPageURL = string.Empty;

          if (linkLevel == 1)
          {
            filterPageURL = pageURL.AbsoluteUri.Remove(pageURL.ToString().LastIndexOf("/"));
          }
          else
          {
            if (pageURL.AbsoluteUri.IndexOf(".") != -1)
            {
              filterPageURL = pageURL.AbsoluteUri.Remove(pageURL.ToString().LastIndexOf("/"));
            }
            else
            {
              filterPageURL = pageURL.IsAbsoluteUri.ToString();
            }
          }

          log.LogSimple(LoggingLevel.Information, "Reading of the inner pages starts here.- " + siteLink.SiteURL + " LinkLevel: " + linkLevel);

          int maxLinkLevel = Convert.ToInt32(ConfigurationManager.AppSettings["MaxLinkLevel"]);

          foreach (string urlString in finalURLs)
          {
            string pageDetailURl = urlString.Trim();

            Thread.Sleep(Convert.ToInt32(ConfigurationManager.AppSettings["ThreadSleepInterval"]));

            if (isPdfFound && linkLevel >= 2)
            {
              log.LogSimple(LoggingLevel.Information, "PDF found hence reading is completed for given page.- " + pageDetailURl + " LinkLevel: " + linkLevel);
              break;
            }

            if (maxLinkLevel > 0 && linkLevel >= maxLinkLevel)
            {
              break;
            }

            GetNestedChildContents(linkLevel, pageDetailURl, pageURL.Host, isHttps, listOfRecords, solrPageAddDetails, siteLink, scanningLinkQueueId, filterPageURL, listOfURLs, ref isPdfFound);

            Thread.Sleep(Convert.ToInt32(ConfigurationManager.AppSettings["ThreadSleepInterval"]));
          }

          if (linkLevel == 1 && solrPageAddDetails.Count == 0)
          {
            LoaderLinkQueue linkDetails = new LoaderLinkQueue();

            linkDetails.pageContent = siteLink.pageContent;
            linkDetails.Id = siteLink.Id;
            linkDetails.GUID = siteLink.GUID;
            linkDetails.SiteURL = siteLink.SiteURL;

            int lexiconCount = SearchLexiconTerm(linkDetails);

            // Compare it with the lexicon of this page earlier.
            int olderLexiconCount = WebLinks.GetWebLinkLexiconCount(siteLink.WebSiteId);

            // If previous and current lexicons are same then delete the entry from ScrappingLinkQueue table else keep it as it is.
            if (olderLexiconCount == lexiconCount)
            {
              WebLinks.DeleteScanningLinkQueue(scanningLinkQueueId);
            }
            else
            {
              linkDetails.LexiconCount = lexiconCount;
              WebLinks.UpdateLoaderLinkLogLexiconCount(linkDetails);
            }
          }

          if (solrPageAddDetails.Count > 0)
          {
            if (linkLevel == 1)
            {
              int lexiconCount = SearchLexiconTerm(siteLink);
              siteLink.LexiconCount = lexiconCount;

              WebLinks.UpdateLoaderLinkLogLexiconCount(siteLink);
            }

            log.LogSimple(LoggingLevel.Information, "Storing in the Solr DB Starts here. Total Records:- " + solrPageAddDetails.Count);

            this.InsertRecordToSolRDB(solrPageAddDetails);

            List<PageDetailHistory> pageInsertRecordHistory = solrPageAddDetails.Select(a => new PageDetailHistory()
            {
              ProcessId = siteLink.ProcessId,
              ProcessInstanceId = siteLink.ProcessInstanceId,
              GuidId = a.GuidId,
              AddedDateTime = a.AddedDateTime,
              PageSource = a.PageSource,
              URL = a.URL
            }).ToList();

            this.InsertRecordToSolrHistoryDB(pageInsertRecordHistory);

            log.LogSimple(LoggingLevel.Information, "Storing in the Solr DB Ends here.");
          }

          if (solrPageUpdateDetails.Count > 0)
          {
            log.LogSimple(LoggingLevel.Information, "Updating in the Solr DB Starts here. Total Records:- " + solrPageUpdateDetails.Count);

            this.InsertRecordToSolRDB(solrPageUpdateDetails);

            List<PageDetailHistory> pageInsertRecordHistory = solrPageUpdateDetails.Select(a => new PageDetailHistory()
            {
              ProcessId = siteLink.ProcessId,
              ProcessInstanceId = siteLink.ProcessInstanceId,
              GuidId = a.GuidId,
              AddedDateTime = a.AddedDateTime,
              PageSource = a.PageSource,
              URL = a.URL
            }).ToList();

            this.InsertRecordToSolrHistoryDB(pageInsertRecordHistory);

            log.LogSimple(LoggingLevel.Information, "Updating in the Solr DB Ends here.");
          }
        }
        else if (linkLevel == 1)
        {
          // No links found in the Main page
          var pageURLDetails = listOfRecords.Find(x => x.SiteURL == siteLink.SiteURL);

          // If the page is new then only store it otherwise don't show that page.
          if (pageURLDetails == null)
          {
            LoaderLinkQueue scanningLinkDetail = new LoaderLinkQueue();

            scanningLinkDetail.ProcessId = siteLink.ProcessId;
            scanningLinkDetail.ProcessInstanceId = siteLink.ProcessInstanceId;

            scanningLinkDetail.SiteURL = siteLink.SiteURL;
            scanningLinkDetail.GUID = siteLink.GUID;
            scanningLinkDetail.WebSiteId = siteLink.WebSiteId;

            WebLinks.InsertInScanningLinkDetail(scanningLinkDetail, scanningLinkQueueId);
          }
        }
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in GetInnerChildContents method", ex, null);
      }
    }

    private void GetNestedChildContents(int level, string urlString, string host, bool isHttps, List<LoaderLinkQueue> listOfRecords, List<PageDetails> solrPageAddDetails, LoaderLinkQueue siteLink, int scanningLinkQueueId, string filterPageURL, HashSet<string> listOfURLs, ref bool ispdfFound)
    {
      string finalPageURL = string.Empty;

      System.Text.Encoding textEncoding = System.Text.Encoding.UTF8;
      PageDetails pageDetails;

      try
      {
        List<ExcludedWebLinks> excludedPageList = WebLinks.GetAllExcludedWebLinks();
        List<WebLinkPageContentRegEx> webLinkPagesRegExList = WebLinks.GetRegularExpressionBasedUponWebsiteId(siteLink.WebSiteId);

        if (urlString.IndexOf("http") == -1 || (urlString.IndexOf(host) == -1 && urlString.IndexOf("http") == -1))
        {
          if (isHttps)
          {
            finalPageURL = "https://";

            if (host.IndexOf("www.fdic.gov") != -1 && siteLink.SiteURL != "https://www.fdic.gov/news/news/speeches/")
            {
              finalPageURL = finalPageURL + "www.fdic.gov/news/news/press/" + DateTime.Today.Year + "/" + urlString;
            }
            else if (host.IndexOf("www.sfc.hk") != -1)
            {
              finalPageURL = filterPageURL + "/" + urlString;
            }
            else if (host.IndexOf("www.fin.gc.ca") != -1)
            {
              finalPageURL = finalPageURL + host + "/" + urlString;
            }
            else
            {
              if (urlString.IndexOf("/") == -1)
              {
                urlString = "/" + urlString;
              }

              if (urlString.IndexOf(host) == -1)
              {
                if (host.IndexOf("financialservices.house.gov") != -1 && siteLink.SiteURL != "https://financialservices.house.gov/news/documentquery.aspx?DocumentTypeID=2092")
                {
                  finalPageURL = finalPageURL + host + "/calendar" + urlString;
                }
                else if (host.IndexOf("www.mof.go.jp") != -1 && siteLink.SiteURL != "https://www.mof.go.jp/english/whats_new/")
                {
                  finalPageURL = finalPageURL + host + "/english/international_policy/" + urlString;
                }
                else if (host.IndexOf("english.bmf.gv.at") != -1)
                {
                  finalPageURL = finalPageURL + host + "/" + urlString.Replace("../../", "");
                }
                else
                {
                  finalPageURL = finalPageURL + host + "/" + urlString;
                }

              }
              else
              {
                finalPageURL = finalPageURL + urlString;
              }
            }
          }
          else
          {
            if (host.IndexOf("www.sfc.hk") != -1)
            {
              if (siteLink.SiteURL.IndexOf("http://www.sfc.hk/edistributionWeb/gateway/EN/news-and-announcements/news/") != -1)
              {
                finalPageURL = siteLink.SiteURL + urlString;
              }
              else
              {
                finalPageURL = "http://www.sfc.hk/web/EN/" + urlString;
              }
            }
            else if (urlString.IndexOf(host) == -1)
            {
              if (urlString.IndexOf("/") == -1)
              {
                urlString = "/" + urlString;
              }
              if (siteLink.SiteURL.IndexOf("www.meti.go.jp") != -1 && siteLink.SiteURL != "http://www.meti.go.jp/english/press/index.html" && siteLink.SiteURL != "http://www.meti.go.jp/english/speeches/index.html")
              {
                finalPageURL = "http://" + host + "/english/statistics/" + urlString;
              }
              else
              {
                finalPageURL = "http://" + host + "/" + urlString;
              }
            }
            else
            {
              if (urlString.IndexOf("/") == -1)
              {
                urlString = "/" + urlString;
              }

              finalPageURL = "http://" + urlString;
            }
          }
        }
        else
        {
          if (urlString.IndexOf(host) != -1 && ("http://" + host != urlString || "https://" + host != urlString))
          {
            finalPageURL = urlString;
          }
          else
          {
            if (urlString.IndexOf("www.treasury.gov") != -1)
            {
              finalPageURL = urlString;
            }
            else if (host.IndexOf("www.bundesbank.de") != -1)
            {
              finalPageURL = "http://" + host + "/" + urlString;
            }
            else if (host.IndexOf("www.fin-fsa.fi") != -1)
            {
              finalPageURL = urlString;
            }
            else
            {
              return;
            }
          }
        }

        finalPageURL = finalPageURL.Replace("//", "/").Replace(":/", "://").Replace("..", "").Replace("/./", "/");

        finalPageURL = finalPageURL.Replace("&amp;", "&").Trim();

        if (level > 1 && (finalPageURL.IndexOf(filterPageURL) == -1))
        {
          return;
        }

        level = level + 1;

        bool recordExist = WebLinks.GetAllLinksBasedUponWebSiteURL(finalPageURL);

        if (listOfURLs.Contains(finalPageURL) && recordExist)
        {
          return;
        }
        else
        {
          listOfURLs.Add(finalPageURL);
        }

        if (!recordExist)
        {
          HttpWebRequest detailPageRequest = (HttpWebRequest)WebRequest.Create(finalPageURL);
          detailPageRequest.AllowAutoRedirect = true;
          detailPageRequest.Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["HttpRequestTimeOut"]);

          pageDetails = new PageDetails();

          detailPageRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.01; Windows NT 5.0)";

          ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                                      SecurityProtocolType.Tls11 |
                                      SecurityProtocolType.Ssl3 |
                                      SecurityProtocolType.Tls12;

          using (var pageResponse = (HttpWebResponse)detailPageRequest.GetResponse())
          {
            if (pageResponse != null)
            {
              Stream stream = pageResponse.GetResponseStream();
              stream.ReadTimeout = -1;
              StreamReader reader = new StreamReader(stream);
              string pageContent = reader.ReadToEnd();

              Thread.Sleep(Convert.ToInt32(ConfigurationManager.AppSettings["ThreadSleepInterval"]));

              if (pageContent.IndexOf(".pdf") != -1)
              {
                ispdfFound = true;
              }
              else
              {
                ispdfFound = false;
              }

              // Business logic added to fetch the Web Link Regular expression from the table and structure the content accordingly
              if (webLinkPagesRegExList != null && webLinkPagesRegExList.Count > 0)
              {
                StringBuilder pageContentFinalData = new StringBuilder();
                TimeSpan timeout = new TimeSpan(0, 1, 0);

                foreach (WebLinkPageContentRegEx webLinkPageRegEx in webLinkPagesRegExList)
                {
                  try
                  {
                    Regex webLinkRegEx = new Regex(webLinkPageRegEx.PageContentRegEx, RegexOptions.Compiled, timeout);

                    MatchCollection includeMatches = webLinkRegEx.Matches(pageContent);

                    WebPageRegExGroup webLinkRegExGroup = includeMatches.Cast<Match>()
                                                                  .Where(x => x.Value != null)
                                                                  .Select(z => new WebPageRegExGroup()
                                                                  {
                                                                    DateContent = z.Groups["date"] != null ? z.Groups["date"].Value : string.Empty,
                                                                    PageHeading = z.Groups["heading"] != null ? z.Groups["heading"].Value : string.Empty,
                                                                    PageContent = z.Groups["pageContent"] != null ? z.Groups["pageContent"].Value : string.Empty
                                                                  }).FirstOrDefault();

                    // create the final page Content based upon
                    if (webLinkRegExGroup != null)
                    {
                      if (webLinkRegExGroup.DateContent != string.Empty)
                      {
                        pageContentFinalData.Append(webLinkRegExGroup.DateContent + "<br/>");
                      }

                      if (webLinkRegExGroup.PageHeading != string.Empty)
                      {
                        pageContentFinalData.Append("<h1>" + webLinkRegExGroup.PageHeading + "</h1> ");
                      }

                      if (webLinkRegExGroup.PageContent != string.Empty)
                      {
                        pageContentFinalData.Append(webLinkRegExGroup.PageContent);
                      }
                    }
                  }
                  catch (Exception ex)
                  {
                    log.LogError(LoggingLevel.Error, "BadRequest", "TimeOut Occur: ", ex, null);
                  }
                }

                pageContent = pageContentFinalData.ToString();
              }

              if (level == 1 && (pageContent.IndexOf("post-header") == -1 && siteLink.SiteURL != "https://www.politico.eu/" && siteLink.SiteURL.IndexOf("www.suomenpankki.fi") == -1))
              {
                pageContent = Regex.Replace(pageContent, "<header.*?>(.|\n)*?</header>", "");
              }

              pageContent = Helper.RemoveUnWantedAnchorTags(pageContent);
              pageContent = Regex.Replace(pageContent, "<head.*?>(.|\n)*?</head>", "");
              pageContent = Helper.RemoveHeaderFooterTags(pageContent, siteLink);

              pageContent = Regex.Replace(pageContent, "<em.*?>(.|\n)*?</em>", "");
              pageContent = Regex.Replace(pageContent, "<footer.*?>(.|\n)*?</footer>", "");
              pageContent = Helper.RemoveUnWantedHtmlSource(pageContent);

              pageContent = Regex.Replace(pageContent, "<a\\s*href=\"(?'url'[^\"]+\\.(doc|docx|xls|xlsx|mp3|csv|mp4|ashx|wmv|txt|css|js|mov|flv|wmv|avi|qt|zip|ZIP|rar|RAR|RA|ra|cvs|bat|au|dif|eps|hqx|mac|mdb|p65|psd|psp|qxd|rtf|sit|tif|tar|wks|rss|atom|xml))", "");

              HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
              doc.LoadHtml(pageContent);

              if (doc.DocumentNode.SelectNodes("//*") != null)
              {
                foreach (var eachNode in doc.DocumentNode.SelectNodes("//*"))
                {
                  if (eachNode.Name != "a")
                  {
                    eachNode.Attributes.RemoveAll();
                  }
                }
              }

              string[] str = { doc.DocumentNode.OuterHtml };

              pageContent = Helper.RemoveAllHtmlContents(str[0]);

              byte[] arrayBytes = textEncoding.GetBytes(pageContent);

              bool isContentUpdated = false;
              bool isContentInserted = false;
              bool isRecordProcessed = false;

              if (!recordExist)
              {
                string scrapperName = ProcessEvents.CheckForScrapperEngine(siteLink.ProcessId);

                //// LOGIC STARTS HERE to check whether this page contains previous date or current date if contains date then only proceed further 

                // otherwise navigate to the next page.

                try
                {
                  int currentDay = DateTime.Now.Day;
                  int currentYear = DateTime.Now.Year;

                  var yesterday = DateTime.Today.AddDays(-1);

                  int previousDay = yesterday.Day;
                  int previousYear = yesterday.Year;

                  DateTime? dateSearch = null;

                  try
                  {
                    dateSearch = Helper.ExtractDate(pageContent);
                  }
                  catch (Exception exDate)
                  {
                    log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in the Generic Date Function ", exDate, null);
                  }

                  DateTime dayBeforeDate = DateTime.Today.AddDays(-2);

                  if (dateSearch == null || dateSearch == dayBeforeDate)
                  {
                    isRecordProcessed = false;

                    MatchCollection dateMatchCollection = Helper.DayMonthYearRegEx(pageContent, siteLink);

                    if (dateMatchCollection != null)
                    {
                      List<DateMonth> dateCollectionURL = (dateMatchCollection.Cast<Match>().Where(x => x.Value != null && x.Value.Length > 1
                                          && x.Groups["day"].Value.Length > 1 && x.Groups["year"].Value.Length > 1)
                                          .Select(z => new DateMonth()
                                          {
                                            Day = z.Groups["day"].Value != null ? Convert.ToInt32(z.Groups["day"].Value) : 0,
                                            Year = z.Groups["year"].Value != null ? Convert.ToInt32(z.Groups["year"].Value) : 0,
                                            Month = z.Groups["month"].Value != null ? z.Groups["month"].Value : string.Empty
                                          })).ToList();

                      if (dateCollectionURL.Count > 0)
                      {
                        foreach (DateMonth currentDateMonth in dateCollectionURL)
                        {
                          string currentURLYear = "";

                          if (currentDateMonth.Year.ToString().Length == 2)
                          {
                            currentURLYear = "20" + currentDateMonth.Year;
                          }
                          else
                          {
                            currentURLYear = currentDateMonth.Year.ToString();
                          }

                          string currentDate = currentURLYear + "-" + currentDateMonth.Month + "-" + currentDateMonth.Day;

                          DateTime currentDateCulture = Helper.GetDateOfAnyCulture(currentDate);

                          if (((currentDateMonth.Day == currentDay || currentDateMonth.Day == previousDay) && (currentURLYear.ToInt32() == currentYear || currentURLYear.ToInt32() == previousYear)) || currentDateCulture > DateTime.Today)
                          {

                            // Check whether the given page is in the exclusion list or not
                            foreach (ExcludedWebLinks excludedWebLink in excludedPageList)
                            {
                              if (finalPageURL.IndexOf(excludedWebLink.URLExcluded) != -1)
                              {
                                InsertRecordLoaderLinkLogLinkExcluded(siteLink, arrayBytes, pageDetails, finalPageURL, ref level);
                                return;
                              }
                            }
                            // code ends here

                            ////isRecordProcessed = true;

                            string[] pageLatestContents = str;
                            isContentInserted = true;
                            pageDetails.PageSource = pageLatestContents;
                            pageDetails.ItemId = siteLink.Id;
                            pageDetails.GuidId = Helper.GuidString();
                            pageDetails.AddedDateTime = Helper.GetCurrentDateTime();
                            pageDetails.Status = string.Empty;
                            pageDetails.URL = finalPageURL;

                            solrPageAddDetails.Add(pageDetails);

                            ReadPageContents(pageDetails, finalPageURL, siteLink, arrayBytes, ref level, ref ispdfFound, listOfRecords, scanningLinkQueueId, host, listOfURLs, currentDateCulture);
                            break;
                          }

                          if (!isRecordProcessed)
                          {
                            InsertRecordLoaderLinkLogRegExNotMatch(siteLink, arrayBytes, pageDetails, finalPageURL, ref level, null);
                            return;
                          }
                        }
                      }
                      else
                      {
                        InsertRecordLoaderLinkLogRegExNotMatch(siteLink, arrayBytes, pageDetails, finalPageURL, ref level, null);
                        return;
                      }
                    }
                    else
                    {
                      // Implement this change for euobserver.com like sites where complete date is not coming
                      if (scrapperName.Equals(Helper.WebSiteType.MediaSector.ToString()))
                      {
                        Regex currentDateReGex = new Regex("Today[,.]\\s\\d\\d[:]\\d\\d");

                        if (currentDateReGex.Matches(pageContent).Count > 0)
                        {
                          dateMatchCollection = currentDateReGex.Matches(pageContent);
                        }

                        if (dateMatchCollection != null && dateMatchCollection.Count > 0)
                        {

                          // Check whether the given page is in the exclusion list or not
                          foreach (ExcludedWebLinks excludedMediaSectorLink in excludedPageList)
                          {
                            if (finalPageURL.IndexOf(excludedMediaSectorLink.URLExcluded) != -1)
                            {
                              InsertRecordLoaderLinkLogLinkExcluded(siteLink, arrayBytes, pageDetails, finalPageURL, ref level);
                              return;
                            }
                          }
                          // code ends here

                          ////isRecordProcessed = true;

                          string[] pageContents = str;
                          ////isContentInserted = true;
                          pageDetails.PageSource = pageContents;
                          pageDetails.ItemId = siteLink.Id;
                          pageDetails.GuidId = Helper.GuidString();
                          pageDetails.AddedDateTime = Helper.GetCurrentDateTime();
                          pageDetails.Status = string.Empty;
                          pageDetails.URL = finalPageURL;

                          solrPageAddDetails.Add(pageDetails);

                          ReadPageContents(pageDetails, finalPageURL, siteLink, arrayBytes, ref level, ref ispdfFound, listOfRecords, scanningLinkQueueId, host, listOfURLs, Helper.GetCurrentDateTime());

                          return;
                        }
                        else
                        {
                          InsertRecordLoaderLinkLogRegExNotMatch(siteLink, arrayBytes, pageDetails, finalPageURL, ref level, null);
                          return;
                        }
                      }
                      // change ends here
                      else
                      {
                        InsertRecordLoaderLinkLogRegExNotMatch(siteLink, arrayBytes, pageDetails, finalPageURL, ref level, null);
                        return;
                      }
                    }
                  }
                  else
                  {
                    string currentDate = DateTime.Now.ToString("MM/dd/yyyy");
                    string previousDate = DateTime.Today.AddDays(-1).ToString("MM/dd/yyyy");

                    DateTime currentDateFetch;
                    DateTime previousDateFetch;

                    DateTime.TryParseExact(currentDate, "MM/dd/yyyy", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out currentDateFetch);

                    DateTime.TryParseExact(previousDate, "MM/dd/yyyy", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out previousDateFetch);

                    if (currentDateFetch == dateSearch || previousDateFetch == dateSearch)
                    {

                      // Check whether the given page is in the exclusion list or not
                      foreach (ExcludedWebLinks excludedNewLink in excludedPageList)
                      {
                        if (finalPageURL.IndexOf(excludedNewLink.URLExcluded) != -1)
                        {
                          InsertRecordLoaderLinkLogLinkExcluded(siteLink, arrayBytes, pageDetails, finalPageURL, ref level);
                          return;
                        }
                      }
                      // code ends here

                      string[] pageContents = str;
                      isContentInserted = true;
                      pageDetails.PageSource = pageContents;
                      pageDetails.ItemId = siteLink.Id;
                      pageDetails.GuidId = Helper.GuidString();
                      pageDetails.AddedDateTime = Helper.GetCurrentDateTime();
                      pageDetails.Status = string.Empty;
                      pageDetails.URL = finalPageURL;

                      solrPageAddDetails.Add(pageDetails);

                      ReadPageContents(pageDetails, finalPageURL, siteLink, arrayBytes, ref level, ref ispdfFound, listOfRecords, scanningLinkQueueId, host, listOfURLs, dateSearch);
                    }
                    else
                    {
                      InsertRecordLoaderLinkLogRegExNotMatch(siteLink, arrayBytes, pageDetails, finalPageURL, ref level, dateSearch);
                      return;
                    }
                  }
                  // Check for the date format using generic mechanism
                }
                catch (Exception exc)
                {
                  log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in the Date Month checking Method", exc, null);
                }
              }

              ContentLoaderLogViewModel viewModel = new ContentLoaderLogViewModel()
              {
                ProcessId = siteLink.ProcessId,
                ProcessInstanceId = siteLink.ProcessId,
                SiteUrl = finalPageURL,
                WebSiteId = siteLink.WebSiteId,
                LinkLevel = level,
                IsContentInserted = isContentInserted,
                IsContentUpdated = isContentUpdated
              };
              InsertDataToContentLoaderLog(viewModel);
            }
          }
        }
      }
      catch (Exception ex)
      {
        LoaderErrorLogViewModel viewModel = new LoaderErrorLogViewModel()
        {
          ProcessId = siteLink.ProcessId,
          ProcessInstanceId = siteLink.ProcessId,
          WebSiteId = siteLink.WebSiteId,
          SiteUrl = finalPageURL,
          ErrorDesc = "InnerException:" + ex.InnerException + "Message :" + ex.Message
        };
        InsertDataToLoaderErrorLog(viewModel);

        log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in the Page content fetching of GetInnerChildContents method", ex, null);
      }
    }

    private void ReadPageContents(PageDetails pageDetails, string finalPageURL, LoaderLinkQueue siteLink, byte[] arrayBytes, ref int level, ref bool ispdfFound, List<LoaderLinkQueue> listOfRecords, int scanningLinkQueueId, string host, HashSet<string> listOfURLs, DateTime? contentUploadedDate)
    {
      bool result = InsertLoaderLinkLog(siteLink, arrayBytes.Length, pageDetails.GuidId, true, finalPageURL, level);

      // Add record in the scanning link detail table
      LoaderLinkQueue scanningLinkDetail = new LoaderLinkQueue();

      scanningLinkDetail.ProcessId = siteLink.ProcessId;
      scanningLinkDetail.ProcessInstanceId = siteLink.ProcessInstanceId;

      scanningLinkDetail.SiteURL = finalPageURL;
      scanningLinkDetail.GUID = pageDetails.GuidId;
      scanningLinkDetail.WebSiteId = siteLink.WebSiteId;
      scanningLinkDetail.PageType = Convert.ToInt32(Helper.WebSiteCategory.PDFDocument);
      scanningLinkDetail.PublishDate = contentUploadedDate;

      WebLinks.InsertInScanningLinkDetail(scanningLinkDetail, scanningLinkQueueId);

      if (result && !ispdfFound)
      {
        Thread.Sleep(Convert.ToInt32(ConfigurationManager.AppSettings["ThreadSleepInterval"]));

        //// level = level + 1;

        Thread.Sleep(Convert.ToInt32(ConfigurationManager.AppSettings["ThreadSleepInterval"]));

        this.GetInnerChildContents(scanningLinkDetail, listOfRecords, ref level, scanningLinkQueueId, listOfURLs, ref ispdfFound);
      }
      else
      {
        // If PDF is found
        FetchPDFDocuments(scanningLinkDetail, listOfRecords, scanningLinkQueueId, host, listOfURLs);
        // Code ends here
      }
    }

    // Function to add Loader Link Log in case of date not match or historic date appear
    private void InsertRecordLoaderLinkLogRegExNotMatch(LoaderLinkQueue siteLink, byte[] arrayBytes, PageDetails pageDetails, string finalPageURL, ref int level, DateTime? dateSearch)
    {
      InsertLoaderLinkLog(siteLink, arrayBytes.Length, pageDetails.GuidId, false, finalPageURL, level);

      ContentLoaderLogViewModel historicExcludedPages = new ContentLoaderLogViewModel()
      {
        ProcessId = siteLink.ProcessId,
        ProcessInstanceId = siteLink.ProcessId,
        SiteUrl = "Excluded with date: " + dateSearch + " :" + finalPageURL,
        WebSiteId = siteLink.WebSiteId,
        LinkLevel = level,
        IsContentInserted = true,
        IsContentUpdated = false
      };

      InsertDataToContentLoaderLog(historicExcludedPages);
    }

    private void InsertRecordLoaderLinkLogLinkExcluded(LoaderLinkQueue siteLink, byte[] arrayBytes, PageDetails pageDetails, string finalPageURL, ref int level)
    {
      InsertLoaderLinkLog(siteLink, arrayBytes.Length, pageDetails.GuidId, false, finalPageURL, level);

      ContentLoaderLogViewModel historicExcludedPages = new ContentLoaderLogViewModel()
      {
        ProcessId = siteLink.ProcessId,
        ProcessInstanceId = siteLink.ProcessId,
        SiteUrl = "Excluded web link: " + finalPageURL,
        WebSiteId = siteLink.WebSiteId,
        LinkLevel = level,
        IsContentInserted = true,
        IsContentUpdated = false
      };

      InsertDataToContentLoaderLog(historicExcludedPages);
    }

    private PageDetailsView GetPageSourceIdByGuidId(LoaderLinkQueue loaderLinkQueue)
    {
      SolrSearchParameters searchParameters = new SolrSearchParameters();

      searchParameters.FreeSearch = "guidId:\"" + loaderLinkQueue.GUID + "\"";

      PageDetailsView pageDetails = SolrConfiguration.GetPageSourceIdByGuidId(searchParameters);

      return pageDetails;
    }

    public int SearchLexiconTerm(LoaderLinkQueue loaderLinkQueue)
    {
      int lexiconCount = 0;

      PageDetails pageDetails;
      List<PageDetails> solrPageDetails = new List<PageDetails>();

      PageDetailsView solrPageDetail = GetPageSourceIdByGuidId(loaderLinkQueue);

      ICollection<PageDetails> solrUpdatePageDetail = solrPageDetail.Products;

      if (solrUpdatePageDetail.Count > 0)
      {
        foreach (PageDetails solrPageUpdate in solrUpdatePageDetail)
        {
          string[] pageSource = { loaderLinkQueue.pageContent };
          solrPageUpdate.PageSource = pageSource;
          solrPageDetails.Add(solrPageUpdate);
        }
      }
      else
      {
        pageDetails = new PageDetails();
        string[] pageSource = { loaderLinkQueue.pageContent };

        pageDetails.PageSource = pageSource;
        pageDetails.ItemId = loaderLinkQueue.Id;
        pageDetails.GuidId = loaderLinkQueue.GUID;
        pageDetails.AddedDateTime = Helper.GetCurrentDateTime();
        pageDetails.Status = string.Empty;
        pageDetails.URL = loaderLinkQueue.SiteURL;

        solrPageDetails.Add(pageDetails);
      }

      InsertRecordToSolRDB(solrPageDetails);

      SolrSearchParameters searchParameters = new SolrSearchParameters();

      List<LexiconModel> lexiconList = Lexicon.GetLexiconListForScraping();

      if (lexiconList.Count > 0)
      {
        string[] lexiconeArray = lexiconList.Select(x => x.LexiconIssue.Trim()).OrderBy(p => p.Substring(0)).Distinct().ToArray();

        searchParameters.FreeSearch = "guidId:\"" + loaderLinkQueue.GUID + "\" AND pageSource:(" + "\"" + string.Join("\" OR \"", lexiconeArray) + "\"" + ")";

        PageDetailsView pageDetailsView = SolrConfiguration.Get(searchParameters);
        if (pageDetailsView.Highlight.Count > 0)
        {
          var highlightedResult = pageDetailsView.Highlight.Select(x => x).FirstOrDefault().Value.Values.FirstOrDefault().FirstOrDefault();
          ////highlightedResult = RemoveAllUnwantedAttributesFromAnchor(highlightedResult);
          string updatedHtmlContent = string.Empty;

          ScrappedContentMappingCounts htmlResult = ScrapperActivity.FetchLexiconResult(highlightedResult, lexiconList, out updatedHtmlContent);

          lexiconCount = htmlResult.ActaulLexiconCounts.Count;
        }
      }
      return lexiconCount;
    }

    private bool InsertLoaderLinkLog(LoaderLinkQueue siteLink, decimal arrayLength, string guid, bool isSuccessful, string pageURL, int level)
    {
      bool result = false;

      LoaderLinkQueue linkQueue = new LoaderLinkQueue();

      linkQueue.ProcessId = siteLink.ProcessId;
      linkQueue.ProcessInstanceId = siteLink.ProcessInstanceId;

      linkQueue.WebSiteId = siteLink.WebSiteId;
      linkQueue.SiteURL = pageURL;

      linkQueue.WebLinkBytes = arrayLength;
      linkQueue.IsSuccessful = isSuccessful;

      linkQueue.GUID = guid;
      linkQueue.LinkLevel = level;
      linkQueue.InstanceName = siteLink.InstanceName == null ? Convert.ToString(Helper.WebSiteCategory.ClickThroughPages) : siteLink.InstanceName;
      linkQueue.NewerProcessId = siteLink.ProcessId;

      result = WebLinks.InsertSubLinkLogRecords(linkQueue);

      return result;
    }

    private bool InsertDataToLoaderErrorLog(LoaderErrorLogViewModel viewModel)
    {
      return WebLinks.UpdateLoaderErrorLog(viewModel);
    }

    private bool InsertDataToContentLoaderLog(ContentLoaderLogViewModel viewModel)
    {
      return WebLinks.UpdateContentLoaderLog(viewModel);
    }

    public bool FetchPDFDocuments(LoaderLinkQueue linkDetails, List<LoaderLinkQueue> listOfRecords, int scanningLinkQueueId, string host, HashSet<string> listOfURLs)
    {
      bool result = false;

      try
      {
        List<LoaderLinkQueue> totalPDFPages = Helper.SearchPDFDocuments(linkDetails, listOfRecords);
        Encoding textEncoding = Encoding.UTF8;

        bool isContentInserted = false;
        bool isContentUpdated = false;

        List<PageDetails> solrPageDetails = new List<PageDetails>();
        List<PageDetailHistory> solrHistoryPageDetails = new List<PageDetailHistory>();

        if (totalPDFPages.Count > 0)
        {
          foreach (LoaderLinkQueue pdfFile in totalPDFPages)
          {
            // If the web links is of external site then it will be excluded
            if ((pdfFile.SiteURL.IndexOf(host) == -1) && (pdfFile.SiteURL.IndexOf("service.gov.uk") == -1))
              continue;

            pdfFile.SiteURL = pdfFile.SiteURL.Replace("//", "/").Replace(":/", "://");

            int lastIndex = pdfFile.SiteURL.LastIndexOf(".pdf");
            string finalURL = pdfFile.SiteURL;

            if (lastIndex > 0)
            {
              finalURL = finalURL.Substring(0, lastIndex + 4);
            }

            if (listOfURLs.Contains(finalURL))
            {
              continue;
            }
            else
            {
              listOfURLs.Add(finalURL);
            }

            pdfFile.SiteURL = finalURL;

            var pageDetail = listOfRecords.Find(x => x.SiteURL == pdfFile.SiteURL);

            if (pageDetail == null)
            {
              StringBuilder text = new StringBuilder();

              if (pdfFile.SiteURL.IndexOf("http") == -1)
              {
                pdfFile.SiteURL = "http://" + pdfFile.SiteURL;
              }

              text.Append("<html><body>");
              string pageContent = string.Empty;

              using (PdfReader reader = new PdfReader(pdfFile.SiteURL))
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

                string appendParagraphTag = text.ToString();
                appendParagraphTag = appendParagraphTag + "</p>";
                int paraGraphIndex = appendParagraphTag.IndexOf("</p>");

                if (appendParagraphTag.Length > 4)
                {
                  appendParagraphTag = appendParagraphTag.Remove(paraGraphIndex, 4);
                }

                appendParagraphTag = appendParagraphTag + "</body></html>";

                byte[] arrayBytes = textEncoding.GetBytes(appendParagraphTag);

                PageDetails pageDetails = new PageDetails();
                PageDetailHistory pageDetailHistory = new PageDetailHistory();

                string[] pageSource = { text.ToString() };

                pageDetails.PageSource = pageSource;
                pageDetails.ItemId = pdfFile.Id;
                pageDetails.GuidId = pdfFile.GUID;

                pageDetails.AddedDateTime = DateTime.Now;
                pageDetails.Status = string.Empty;
                pageDetails.URL = pdfFile.SiteURL;

                solrPageDetails.Add(pageDetails);

                // This page is coming from Dynamic Link Contents for Documents
                if (scanningLinkQueueId == 0)
                {
                  scanningLinkQueueId = WebLinks.InsertInScanningLinkQueue(linkDetails);
                }

                LoaderLinkQueue siteUpdateLoaderLinkPage = new LoaderLinkQueue();

                siteUpdateLoaderLinkPage.NewerProcessId = pdfFile.ProcessId;
                siteUpdateLoaderLinkPage.ProcessId = pdfFile.ProcessId;
                siteUpdateLoaderLinkPage.ProcessInstanceId = pdfFile.ProcessInstanceId;
                siteUpdateLoaderLinkPage.SiteURL = pdfFile.SiteURL;
                siteUpdateLoaderLinkPage.GUID = pdfFile.GUID;

                siteUpdateLoaderLinkPage.WebLinkBytes = Convert.ToDecimal(arrayBytes.Length);
                siteUpdateLoaderLinkPage.WebSiteId = pdfFile.WebSiteId;

                siteUpdateLoaderLinkPage.InstanceName = Convert.ToString(Helper.WebSiteCategory.PDFDocument);
                siteUpdateLoaderLinkPage.LinkLevel = 1;
                siteUpdateLoaderLinkPage.IsSuccessful = true;
                siteUpdateLoaderLinkPage.PageType = Convert.ToInt32(Helper.PageTypes.PDF);
                WebLinks.UpdateLoaderLinkLogMasterRecord(siteUpdateLoaderLinkPage);

                WebLinks.InsertInScanningLinkDetail(siteUpdateLoaderLinkPage, scanningLinkQueueId);

                isContentInserted = true;

                pageDetailHistory.PageSource = pageSource;
                pageDetailHistory.ProcessId = pdfFile.ProcessId;
                pageDetailHistory.ProcessInstanceId = pdfFile.ProcessInstanceId;

                pageDetailHistory.URL = pdfFile.SiteURL;
                pageDetailHistory.PageSource = pageSource;
                pageDetailHistory.GuidId = pdfFile.GUID;
                pageDetailHistory.AddedDateTime = DateTime.Now;

                solrHistoryPageDetails.Add(pageDetailHistory);

                ContentLoaderLogViewModel viewModel = new ContentLoaderLogViewModel()
                {
                  ProcessId = pdfFile.ProcessId,
                  ProcessInstanceId = pdfFile.ProcessInstanceId,
                  SiteUrl = pdfFile.SiteURL,
                  WebSiteId = pdfFile.WebSiteId,
                  LinkLevel = 1,
                  IsContentInserted = isContentInserted,
                  IsContentUpdated = isContentUpdated
                };

                result = WebLinks.UpdateContentLoaderLog(viewModel);
              }
            }
          }

          if (solrPageDetails.Count > 0)
          {
            InsertRecordToSolRDB(solrPageDetails);
          }

          if (solrHistoryPageDetails.Count > 0)
          {
            InsertRecordToSolrHistoryDB(solrHistoryPageDetails);
          }
        }
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in FetchPDFDocuments method", ex, null);
      }

      return result;
    }

    /// <summary>
    /// Insert Record to the SOLR Database
    /// </summary>
    /// <param name="solrList"></param>
    /// <returns></returns>
    private void InsertRecordToSolRDB(List<PageDetails> solrList)
    {
      try
      {
        SolrConfiguration.InsertInRange(solrList);
      }
      catch (SolrConnectionException ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in the InsertRecordToSolRDB method", ex, null);
      }
    }

    private void InsertRecordToSolrHistoryDB(List<PageDetailHistory> solrDetailList)
    {
      try
      {
        SolrPageDetail.InsertPageDetailHistory(solrDetailList);
      }
      catch (SolrConnectionException ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in the InsertRecordToSolrHistoryDB method", ex, null);
      }
    }

    public PageDetailsView GetPageSourceIdByGuidId(string guidString)
    {
      SolrSearchParameters searchParameters = new SolrSearchParameters();

      searchParameters.FreeSearch = "guidId:\"" + guidString + "\"";

      PageDetailsView pageDetails = SolrConfiguration.GetPageSourceIdByGuidId(searchParameters);

      return pageDetails;
    }
  }
}