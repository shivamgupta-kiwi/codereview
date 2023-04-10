/// This is the Abstract file for IWebLink
namespace BCMStrategy.Data.Abstract.Abstract
{
  using System.Collections.Generic;
  using BCMStrategy.Data.Abstract.ViewModels;
  using System.Threading.Tasks;
  using BCMStrategy.Common.Kendo;

  /// <summary>
  /// Interface for Web Link
  /// </summary>
  public interface IWebLink
  {
    /// <summary>
    /// Get all Web Links by Type
    /// </summary>
    /// <param name="type">Type of web links</param>
    /// <param name="processId">Process Id</param>
    /// <returns>Boolean Value</returns>
    List<string> GetAllWebLinks(int type, int processId);

    /// <summary>
    /// Get Message details based upon their Type
    /// </summary>
    /// <param name="messageType">Message Type</param>
    /// <param name="processId">Process id of the message</param>
    /// <param name="processInstanceId">Process instance id of the message</param>
    /// <returns>List of messages returned</returns>
    List<LoaderLinkQueue> GetMessageBasedUponType(string messageType, int processId, int processInstanceId);

    /// <summary>
    /// Delete loader link queue record
    /// </summary>
    /// <param name="processLinkDetails">Process Link Details</param>
    /// <returns>Boolean value for the record being deleted</returns>
    bool DeleteLoaderLinkQueueRecord(ProcessLinkDetails processLinkDetails);

    /// <summary>
    /// Get Message details based upon their Type
    /// </summary>
    /// <param name="type">Type of the message</param>
    /// <param name="categoryName">Category Name</param>
    /// <returns>List of messages returned</returns>
    int GetWebLinkCount(int type, string categoryName);

    /// <summary>
    /// Insert link record to the database
    /// </summary>
    /// <param name="processLink">Process Link</param>
    /// <returns>returns true or false for the link records</returns>
    bool InsertLinkRecords(ProcessLinkDetails processLink);

    /// <summary>
    /// Delete loader link log record
    /// </summary>
    /// <param name="processLinkDetails">Process Link Details</param>
    /// <returns>returns true or false for the link records</returns>
    bool DeleteLoaderLinkLogRecord(ProcessLinkDetails processLinkDetails);

    /// <summary>
    /// Store Document records in the database
    /// </summary>
    /// <param name="listDocuments">Process list of documents</param>
    /// <returns>True or false for the document processed</returns>
    bool ProcessDocumentRecords(List<LoaderLinkQueue> listDocuments);

    /// <summary>
    /// insert Sub link Log Records
    /// </summary>
    /// <param name="processLink">Process Links</param>
    /// <returns>True or false for the document processed</returns>
    bool InsertSubLinkLogRecords(LoaderLinkQueue processLink);

    /// <summary>
    /// Update Loader Link Queue records
    /// </summary>
    /// <param name="linkDetails">Link Details</param>
    /// <returns>True or false for the document processed</returns>
    bool UpdateLoaderLinkLogRecordBytes(LoaderLinkQueue linkDetails);

    /// <summary>
    /// Update Loader Link Queue records
    /// </summary>
    /// <param name="linkDetails">Link Details</param>
    /// <returns>True or false for the document processed</returns>
    bool UpdateLoaderLinkLogMasterRecord(LoaderLinkQueue linkDetails);

    /// <summary>
    /// Get the count of all the processed contents
    /// </summary>
    /// <param name="processId"></param>
    /// <param name="processInstanceId"></param>
    /// <returns></returns>
    List<LoaderLinkQueue> GetWebLinkForLexicons(int processId, int processInstanceId);

    /// <summary>
    /// Get All Web Link DDL
    /// </summary>
    /// <returns></returns>
    Task<WebLinkViewModel> GetAllWebLinkPageDDL(bool isEdit = false);

    /// <summary>
    /// Update Content Loader Log
    /// </summary>
    /// <param name="contentLoaderViewModel"></param>
    /// <returns></returns>
    bool UpdateContentLoaderLog(ContentLoaderLogViewModel contentLoaderViewModel);

    /// <summary>
    /// Update Loader ErrorLog
    /// </summary>
    /// <param name="viewModel"></param>
    /// <returns></returns>
    bool UpdateLoaderErrorLog(LoaderErrorLogViewModel viewModel);

    /// <summary>
    /// Used to get All Entity FullName
    /// </summary>
    /// <param name="searchTerm"></param>
    /// <returns></returns>
    Task<List<DropdownMaster>> GetAllEntityFullName(string searchTerm);

    /// <summary>
    ///  Used to get All Individual Person
    /// </summary>
    /// <param name="searchTerm"></param>
    /// <returns></returns>
    Task<ApiOutput> GetAllIndividualPerson(GridParameters parameters);

    /// <summary>
    ///  Used to get All WebLinks
    /// </summary>
    /// <param name="searchTerm"></param>
    /// <returns></returns>
    Task<ApiOutput> GetAllWebLinks(GridParameters parametersJson);

    /// <summary>
    /// Compare data with the last execution
    /// </summary>
    /// <param name="previousProcessId">Previous Process Id</param>
    /// <param name="currentProcessId">Current Process id</param>
    /// <param name="linkLevel">Link Level</param>
    /// <returns>List of different records</returns>
    List<LoaderLinkQueue> GetAllLinksInnerLinks();

    /// <summary>
    /// Insert record in Scanning Link Queue table
    /// </summary>
    /// <param name="linkQueueRecord">Link Queue Record</param>
    /// <returns>returns Scanning Link ID</returns>
    int InsertInScanningLinkQueue(LoaderLinkQueue linkQueueRecord);

    /// <summary>
    /// Insert record in Scanning Link detail table
    /// </summary>
    /// <param name="linkQueueRecord">Scanning link Queue</param>
    /// <param name="scanningLinkId">Scanning Link ID</param>
    /// <returns>returns Scanning Link ID</returns>
    bool InsertInScanningLinkDetail(LoaderLinkQueue linkQueueRecord, int scanningLinkId);

    /// <summary>
    /// Update Web Link
    /// </summary>
    /// <param name="webLinkViewModel"></param>
    /// <returns></returns>
    Task<bool> UpdateWebLink(WebLinkViewModel webLinkViewModel);

    /// <summary>
    /// Delete Web Link
    /// </summary>
    /// <param name="webLinkHashId"></param>
    /// <returns></returns>
    Task<bool> DeleteWebLink(string webLinkHashId);

    /// <summary>
    /// Get WebLink Based on Hash Id
    /// </summary>
    /// <param name="webLinkHashId"></param>
    /// <returns></returns>
    Task<WebLinkViewModel> GetWebLinkBasedOnHashId(string webLinkHashId);

    /// <summary>
    /// IsDuplicateWebLink
    /// </summary>
    /// <param name="webLinkViewModel"></param>
    /// <returns></returns>
    Task<bool> IsDuplicateWebLink(WebLinkViewModel webLinkViewModel);

    /// <summary>
    /// IsFullScrapperActivityCompleted
    /// </summary>
    /// <param name="scrapperDetailId"></param>
    /// <returns></returns>
    bool IsFullScrapperActivityCompleted(int scrapperDetailId);

    /// <summary>
    /// IsFullScrapperActivityProcessCompleted
    /// </summary>
    /// <param name="processId"></param>
    /// <param name="ProcessInstanceId"></param>
    /// <returns></returns>
    bool IsFullScrapperActivityProcessCompleted(int processId, int ProcessInstanceId);

    /// <summary>
    /// GetActionTypeBasedOnWebsiteType
    /// </summary>
    /// <param name="websiteTypeHashId"></param>
    /// <returns></returns>
    Task<WebLinkViewModel> GetActionTypeBasedOnWebsiteType(WebLinkViewModel webLinkModel);

    /// <summary>
    /// GetActivityTypeBasedOnActionType
    /// </summary>
    /// <param name="actionTypeHashIds"></param>
    /// <returns></returns>
    Task<WebLinkViewModel> GetActivityTypeBasedOnActionType(List<string> actionTypeHashIds, bool isEdit);

    /// <summary>
    /// Get Web Links Html and RegEx details
    /// </summary>
    /// <param name="webLinkId">web Link Id</param>
    /// <returns>Loader Link Queue record</returns>
    LoaderLinkQueue GetWebLinkHtmlLinksRegEx(int webLinkId);

    /// <summary>
    /// Get Web Link Lexicon Count details
    /// </summary>
    /// <param name="webSiteId">webSiteId details</param>
    /// <returns>Lexicon count record</returns>
    int GetWebLinkLexiconCount(int webSiteId);

    /// <summary>
    /// Update Loader Link Log Lexicon Count
    /// </summary>
    /// <param name="linkDetails">link Details</param>
    /// <returns>Boolean value for the successful / un-successful deletion </returns>
    bool UpdateLoaderLinkLogLexiconCount(LoaderLinkQueue linkDetails);

    /// <summary>
    /// Delete Scanning Link Details
    /// </summary>
    /// <param name="linkDetails">linkDetails</param>
    /// <returns>Boolean value for the successful / un-successful deletion </returns>
    bool DeleteScanningLinkQueue(int scanningLinkQueueId);

    /// <summary>
    /// Get RSS Feed by Link details
    /// </summary>
    /// <param name="webLinkId">webLinkId</param>
    /// <returns>List of RSS Feed URLs</returns>
    List<RssFeedDetails> GetRSSFeedByWebLinkId(int webLinkId);

    List<ExcludedWebLinks> GetAllExcludedWebLinks();

    /// <summary>
    /// Get All based upon Website Id
    /// </summary>
    /// <param name="webSiteId">Website Id</param>
    /// <returns>List of records stored in the database</returns>
    bool GetAllLinksBasedUponWebSiteURL(string siteURL);

    /// <summary>
    /// Get regular expression for the given Website Id
    /// </summary>
    /// <param name="websiteId">Website Id</param>
    /// <returns>List of Regular expression for the given page</returns>
    List<WebLinkPageContentRegEx> GetRegularExpressionBasedUponWebsiteId(int websiteId);
  }
}