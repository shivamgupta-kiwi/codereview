using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Data.Repository.Concrete;
using BCMStrategy.Logger;

namespace BCMStrategy.ContentLoader.HtmlPages.API
{
  public class WebApi
  {
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

    public bool InsertLoaderLinkLog(LoaderLinkQueue siteLink, decimal arrayLength, string guid, bool isSuccessful, string pageURL, int level)
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
      linkQueue.InstanceName = siteLink.InstanceName == null ? "ClickThroughLinks" : siteLink.InstanceName;
      linkQueue.NewerProcessId = siteLink.ProcessId;

      result = WebLink.InsertSubLinkLogRecords(linkQueue);

      return result;
    }
  }
}