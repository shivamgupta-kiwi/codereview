using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using System.Collections.Generic;
using System.Linq;
using DALContext = BCMStrategy.DAL.Context;

namespace BCMStrategy.Data.Repository.Concrete
{
  /// <summary>
  /// Web Document Repository
  /// </summary>
  public class WebDocumentRepository : IWebDocument
  {
    /// <summary>
    /// Get the document link count
    /// </summary>
    /// <param name="processId">Process Id of the respective Process</param>
    /// <returns>Count of documents</returns>
    public int WebDocumentCount(int processId)
    {
      int documentLinkCount = 0;

      using (DALContext.BCMStrategyEntities db = new DALContext.BCMStrategyEntities())
      {
        documentLinkCount = (from w in db.loaderlinkqueue.AsNoTracking()
                               .Where(x => x.ProcessId == processId)
                               select w.Id
                            ).Count();
      }

      return documentLinkCount;
    }

    /// <summary>
    /// Get document list
    /// </summary>
    /// <param name="processId">ProcessId of the document</param>
    /// <param name="processInstanceId">Process Instance Id of the document</param>
    /// <returns>List of Document links to be scrapped</returns>
    public List<LoaderLinkQueue> GetDocumentList(int processId, int processInstanceId)
    {
      List<LoaderLinkQueue> documentLinkCount;

      using (DALContext.BCMStrategyEntities db = new DALContext.BCMStrategyEntities())
      {
        documentLinkCount = (from w in db.loaderlinkqueue.AsNoTracking()
                               .Where(x => x.ProcessId == processId && x.ProcessInstanceId == processInstanceId)
                             select new LoaderLinkQueue
                               {
                                 SiteURL = w.SiteURL,
                                 ProcessId = w.ProcessId,
                                 ProcessInstanceId = w.ProcessInstanceId,
                                 WebSiteId = w.WebSiteId != null ? w.WebSiteId.Value : 0,
                                 InstanceName = w.InstanceName,
                                 GUID = w.Guid
                               }).ToList();
      }

      return documentLinkCount;
    }

  }
}