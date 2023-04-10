using BCMStrategy.API.AuditLog;
using BCMStrategy.API.Filter;
using BCMStrategy.Common.AuditLog;
using BCMStrategy.Common.Kendo;
using BCMStrategy.Common.Unity;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Logger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.ModelBinding;

namespace BCMStrategy.API.Controllers
{
  [Authentication]
  /// <summary>
  /// This controller contains API methods for Web Link Controller
  /// </summary>
  [RoutePrefix("api/WebLink")]
  public class WebLinkController : BaseApiController
  {
    /// <summary>
    /// The log
    /// </summary>
    private static readonly EventLogger<WebLinkController> log = new EventLogger<WebLinkController>();

    /// <summary>
    /// The WebLink repository
    /// </summary>
    private IWebLink _webLink;

    private IWebLink WebLink
    {
      get
      {
        if (_webLink == null)
        {
          _webLink = UnityHelper.Resolve<IWebLink>();
        }

        return _webLink;
      }
    }

    /// <summary>
    /// Get the List of all the WebLinks based upon type defined.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("GetWebLinkList")]
    public async Task<IHttpActionResult> GetWebLinkList(int type, int processId)
    {
      try
      {
        List<string> result = WebLink.GetAllWebLinks(type, processId);

        return Ok(result);
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception occur while assigning link to queue", ex);

        return BadRequest(ex.Message);
      }
    }

    /// <summary>
    /// Get All Messages based upon their Type
    /// </summary>
    /// <param name="messageType">Message Type</param>
    /// <param name="type">Stores the Type of Message</param>
    /// <returns>All the webLinks for the given Message Type</returns>
    [HttpGet]
    [Route("GetMessageBasedUponType")]
    public async Task<IHttpActionResult> GetMessageBasedUponType(string messageType, int processId, int processInstanceId)
    {
      try
      {
        List<LoaderLinkQueue> result = WebLink.GetMessageBasedUponType(messageType, processId, processInstanceId);
        return Ok(result);
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception occur while assigning link to queue", ex);
        return BadRequest(ex.Message);
      }
    }

    /// <summary>
    /// Delete record from Loader Link Queue Table
    /// </summary>
    /// <param name="guid">GUID of the given website link</param>
    /// <param name="processId">Process Id</param>
    /// <returns>Boolean value for deleting the record</returns>
    [HttpPost]
    [Route("DeleteLoaderLinkQueue")]
    public async Task<IHttpActionResult> DeleteLoaderLinkQueue(ProcessLinkDetails linkDetails)
    {
      try
      {
        bool result = WebLink.DeleteLoaderLinkQueueRecord(linkDetails);
        return Ok(result);
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception occur while deleting link from the Loader Link queue table", ex);
        return BadRequest(ex.Message);
      }
    }

    [HttpPost]
    [Route("DeleteLoaderLinkLog")]
    public async Task<IHttpActionResult> DeleteLoaderLinkLog(ProcessLinkDetails linkDetails)
    {
      try
      {
        bool result = WebLink.DeleteLoaderLinkLogRecord(linkDetails);
        return Ok(result);
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception occur while deleting link from the Loader Link log table", ex);
        return BadRequest(ex.Message);
      }
    }

    /// <summary>
    /// Get Message details based upon their Type
    /// </summary>
    /// <param name="type">Type of the message</param>
    /// <returns>List of messages returned</returns>
    [HttpGet]
    [Route("GetWebLinkCount")]
    public async Task<IHttpActionResult> GetWebLinkCount(int type, string categoryName)
    {
      try
      {
        int countWebSites = WebLink.GetWebLinkCount(type, categoryName);
        return Ok(countWebSites);
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception occur while reading web links from the database", ex);
        return BadRequest(ex.Message);
      }
    }

    /// <summary>
    /// Insert link record to the database
    /// </summary>
    /// <param name="processLink">ProcessLink</param>
    /// <returns>returns true or false for the link records</returns>
    [HttpPost]
    [Route("InsertLinkRecords")]
    public async Task<IHttpActionResult> InsertLinkRecords(ProcessLinkDetails processLink)
    {
      try
      {
        bool result = WebLink.InsertLinkRecords(processLink);
        return Ok(result);
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception occur while inserting records in the database", ex);
        return BadRequest(ex.Message);
      }
    }

    /// <summary>
    /// Insert document record to the database
    /// </summary>
    /// <param name="listDocuments">List of documents to be processed</param>
    /// <returns>returns true or false for the link records</returns>
    [HttpPost]
    [Route("ProcessDocumentRecords")]
    public async Task<IHttpActionResult> ProcessDocumentRecords(List<LoaderLinkQueue> listDocuments)
    {
      try
      {
        bool result = WebLink.ProcessDocumentRecords(listDocuments);
        return Ok(result);
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception occur while inserting documents in the database", ex);
        return BadRequest(ex.Message);
      }
    }

    [HttpPost]
    [Route("UpdateLoaderLinkQueueRecordBytes")]
    public async Task<IHttpActionResult> UpdateLoaderLinkQueueRecordBytes(LoaderLinkQueue linkDetails)
    {
      try
      {
        bool result = WebLink.UpdateLoaderLinkLogRecordBytes(linkDetails);
        return Ok(result);
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception occur while inserting documents in the database", ex);
        return BadRequest(ex.Message);
      }
    }

    [HttpPost]
    [Route("InsertSubLinkLogRecords")]
    public async Task<IHttpActionResult> InsertSubLinkLogRecords(LoaderLinkQueue linkDetails)
    {
      try
      {
        bool result = WebLink.InsertSubLinkLogRecords(linkDetails);
        return Ok(result);
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception occur while inserting documents in the database", ex);
        return BadRequest(ex.Message);
      }
    }

    /// <summary>
    /// Get the List of all the WebLinks Page DDL
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IHttpActionResult> GetAllWebLinkManagementDDL()
    {
      try
      {
        WebLinkViewModel result = await WebLink.GetAllWebLinkPageDDL();
        return Ok(result);
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception occur while Getting WebLink page DDL", ex);
        return BadRequest(ex.Message);
      }
    }

    /// <summary>
    /// Get the List of Individual Person
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [Route("GetAllIndividualPersons")]
    [HttpGet]
    public async Task<IHttpActionResult> GetAllIndividualPersons(string parametersJson)
    {
      try
      {
        var parameters = JsonConvert.DeserializeObject<GridParameters>(parametersJson);
        ApiOutput apiOutput = await WebLink.GetAllIndividualPerson(parameters);
        var result = new { Data = apiOutput.Data, Total = apiOutput.TotalRecords };
        return Json(result);
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception occur while Getting All Individual Person DDL Data", ex);
        return BadRequest(ex.Message);
      }
    }

    /// <summary>
    /// Get the List web Links
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [Route("GetAllWebLinks")]
    [HttpGet]
    public async Task<IHttpActionResult> GetAllWebLinks(string parametersJson)
    {
      try
      {
        var parameters = JsonConvert.DeserializeObject<GridParameters>(parametersJson);
        ApiOutput apiOutput = await WebLink.GetAllWebLinks(parameters);
        var result = new { Data = apiOutput.Data, Total = apiOutput.TotalRecords };
        return Json(result);
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception occur while Getting webLinks", ex);
        return BadRequest(ex.Message);
      }
    }

    [HttpPost]
    [Route("UpdateWebLink")]
    public async Task<IHttpActionResult> UpdateWebLink(WebLinkViewModel webLinkModel)
    {
      try
      {
        bool isSave = false;
        bool isDuplicate = false;
        if (!string.IsNullOrEmpty(webLinkModel.WebLinkUrl))
        {
          isDuplicate = await WebLink.IsDuplicateWebLink(webLinkModel);
        }
        HandleModelStateForWebLink(webLinkModel, ModelState, isDuplicate);
        if (!ModelState.IsValid)
        {
          return Ok(FormatResult(false, ModelState));
        }

        isSave = await WebLink.UpdateWebLink(webLinkModel);

        return Ok(FormatResult(isSave, (isSave ? (string.IsNullOrEmpty(webLinkModel.WebLinkMasterHashId) ? Resources.Resource.WebLinkAddedSuccess : Resources.Resource.WebLinkUpdatedSuccess) : Resources.Resource.ErrorWhileSaving)));
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, webLinkModel);
        if (string.IsNullOrEmpty(webLinkModel.WebLinkMasterHashId))
          AuditLogs.Write<WebLinkViewModel, string>(AuditConstants.WebLinks, AuditType.InsertFailure, webLinkModel, (string)null, Helper.GetInnerException(ex));
        else
          AuditLogs.Write<WebLinkViewModel, string>(AuditConstants.WebLinks, AuditType.UpdateFailure, webLinkModel, (string)null, Helper.GetInnerException(ex));
        return BadRequest(ex.Message);
      }
    }

    public void HandleModelStateForWebLink(WebLinkViewModel webLinkModel, System.Web.Http.ModelBinding.ModelStateDictionary modelState, bool isDuplicate)
    {
      if (string.IsNullOrEmpty(webLinkModel.WebLinkUrl))
      {
        ModelState.AddModelError("webLinkModel.WebLinkUrl", string.Format(Resources.Resource.ValidateRequiredField, Resources.Resource.LblWebLinkUrl));
      }
      else
      {
        if (!IsUrlValid(webLinkModel.WebLinkUrl))
          ModelState.AddModelError("webLinkModel.WebLinkUrl", Resources.Resource.LblUrlValidation);
        else if (isDuplicate)
        {
          ModelState.AddModelError("webLinkModel.WebLinkUrl", "Weblink URL already exists");
        }
      }
      if (webLinkModel.IsRSSFeedAvailable && webLinkModel.RSSFeedURLs.Count == 0)
      {
        ModelState.AddModelError("webLinkModel.RSSFeedURLs", string.Format(Resources.Resource.ValidateRequiredField, Resources.Resource.LblRSSFeedURL));
      }
      else if (webLinkModel.RSSFeedURLs.Count > 0)
      {
        foreach (var url in webLinkModel.RSSFeedURLs)
        {
          if (!IsUrlValid(url))
          {
            ModelState.AddModelError("webLinkModel.RSSFeedURLs", Resources.Resource.LblRSSFeedUrlValidation);
            break;
          }
        }
      }

      if (webLinkModel.IsSearchFunctionality && webLinkModel.SearchKeywords.Count == 0)
      {
        ModelState.AddModelError("webLinkModel.SearchKeyWord", string.Format(Resources.Resource.ValidateRequiredField, Resources.Resource.LblSearchKeyWord));
      }
    }

    private bool IsUrlValid(string url)
    {
      if (string.IsNullOrEmpty(url))
        return false;

      return Uri.IsWellFormedUriString(url, UriKind.Absolute);
    }

    /// <summary>
    /// Get the List of Entity Full Name
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IHttpActionResult> GetAllEntityFullName(string searchTerm)
    {
      try
      {
        List<DropdownMaster> result = await WebLink.GetAllEntityFullName(searchTerm);
        return Ok(result);
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception occur while Getting All Entity Full Name DDL Data", ex);
        return BadRequest(ex.Message);
      }
    }

    [Route("DeleteWebLink")]
    [HttpGet]
    public async Task<IHttpActionResult> DeleteWebLink(string webLinkHashId)
    {
      try
      {
        bool isSave = false;

        isSave = await WebLink.DeleteWebLink(webLinkHashId);

        return Ok(FormatResult(isSave, (isSave ? Resources.Resource.WebLinkDeletedSuccessfully : Resources.Resource.ErrorWhileDeleting)));
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, webLinkHashId);
        AuditLogs.Write(AuditConstants.WebLinks, AuditType.DeleteFailure, Helper.GetInnerException(ex));
        return BadRequest(ex.Message);
      }
    }

    /// <summary>
    /// Get Web Link By Hash Id
    /// </summary>
    /// <param name="webLinkHashId"></param>
    /// <returns></returns>
    [Route("GetWebLinkByHashId")]
    [HttpGet]
    public async Task<IHttpActionResult> GetWebLinkByHashId(string webLinkHashId, string guiId = "")
    {
      try
      {
        WebLinkViewModel webLinkModel = await WebLink.GetWebLinkBasedOnHashId(webLinkHashId);
        return Ok(webLinkModel);
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, webLinkHashId);
        return BadRequest(ex.Message);
      }
    }

    /// <summary>
    /// GetActionTypeBasedOnWebsiteType
    /// </summary>
    /// <param name="websiteTypeHashId"></param>
    /// <returns></returns>
    [Route("GetActionTypeBasedOnWebsiteType")]
    [HttpPost]
    public async Task<IHttpActionResult> GetActionTypeBasedOnWebsiteType(WebLinkViewModel webLinkModel)
    {
      try
      {
        WebLinkViewModel model = await WebLink.GetActionTypeBasedOnWebsiteType(webLinkModel);
        return Ok(model);
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, webLinkModel.WebSiteTypeHashId);
        return BadRequest(ex.Message);
      }
    }

    /// <summary>
    /// GetActivityTypeBasedOnActionType
    /// </summary>
    /// <param name="actionTypeHashIds"></param>
    /// <returns></returns>
    [Route("GetActivityTypeBasedOnActionType")]
    [HttpPost]
    public async Task<IHttpActionResult> GetActivityTypeBasedOnActionType(WebLinkViewModel webLinkModel)
    {
      try
      {
        WebLinkViewModel model = await WebLink.GetActivityTypeBasedOnActionType(webLinkModel.ProprietaryHashIds, webLinkModel.IsEdit);
        return Ok(model);
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, string.Empty);
        return BadRequest(ex.Message);
      }
    }

    /// <summary>
    /// Get Web Link By Hash Id
    /// </summary>
    /// <param name="webLinkHashId"></param>
    /// <returns></returns>
    ////[Route("GetWebLinkByHashId")]
    ////[HttpGet]
    ////public async Task<IHttpActionResult> GetWebLinkByHashId(string webLinkHashId)
    ////{
    ////  try
    ////  {
    ////    WebLinkViewModel webLinkModel = await WebLink.GetWebLinkBasedOnHashId(webLinkHashId);
    ////    return Ok(webLinkModel);
    ////  }
    ////  catch (Exception ex)
    ////  {
    ////    log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, webLinkHashId);
    ////    return BadRequest(ex.Message);
    ////  }
    ////}
  }
}