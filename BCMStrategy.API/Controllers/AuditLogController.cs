using BCMStrategy.API.Filter;
using BCMStrategy.Common.AuditLog;
using BCMStrategy.Common.Kendo;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Data.Repository.Concrete;
using BCMStrategy.Logger;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace BCMStrategy.API.Controllers
{
  [Authentication]
  [RoutePrefix("api/AuditLog")]
  public class AuditLogController : BaseApiController
  {
    private static readonly EventLogger<AuditLogController> _log = new EventLogger<AuditLogController>();

    private IAuditLog _auditLogRepository;

    private IAuditLog AuditLogRepository
    {
      get
      {
        if (_auditLogRepository == null)
        {
          _auditLogRepository = new AuditLogRepository();
        }

        return _auditLogRepository;
      }
    }

    /// <summary>
    /// Get Drop down list of Metadata Dynamic table Noun plus Verb
    /// </summary>
    /// <returns>Return meta data DynamicTableNounplusVerb list</returns>
    [Route("GetDDAuditTableList")]
    [HttpGet]
    public async Task<IHttpActionResult> GetDDAuditTableList()
    {
      try
      {
        ApiOutput apiOutput = await AuditLogRepository.GetDDAuditTableList();
        return Ok(apiOutput);
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex);
        return BadRequest(ex.Message);
      }
    }

    /// <summary>
    /// Get Drop down list of Metadata Dynamic table Noun plus Verb
    /// </summary>
    /// <returns>Return meta data DynamicTableNounplusVerb list</returns>
    [Route("GetDDCustomerTableList")]
    [HttpGet]
    public async Task<IHttpActionResult> GetDDCustomerTableList()
    {
      try
      {
        ApiOutput apiOutput = await AuditLogRepository.GetDDCustomerList();
        return Ok(apiOutput);
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex);
        return BadRequest(ex.Message);
      }
    }

    [Route("GetAllAuditLogList")]
    [HttpGet]
    public async Task<IHttpActionResult> GetAllAuditLogList(string parametersJson, string auditPageHash="")
    {
      var parameters = JsonConvert.DeserializeObject<GridParameters>(parametersJson);
      ApiOutput apiOutput = await AuditLogRepository.GetAllAuditLogList(parameters, auditPageHash);
      var result = new { Data = apiOutput.Data, Total = apiOutput.TotalRecords };
      return Json(result);
    }

    [Route("GetAllCustomerAuditLogList")]
    [HttpGet]
    public async Task<IHttpActionResult> GetAllCustomerAuditLogList(string parametersJson, string customerHashId = "")
    {
      var parameters = JsonConvert.DeserializeObject<GridParameters>(parametersJson);
      ApiOutput apiOutput = await AuditLogRepository.GetAllCustomerAuditLogList(parameters, customerHashId);
      var result = new { Data = apiOutput.Data, Total = apiOutput.TotalRecords };
      return Json(result);
    }

    [Route("WriteAudit")]
    [HttpPost]
    public async Task<IHttpActionResult> WriteAudit()
    {
      try
      {
        bool result = true;
        int user = UserAccessHelper.CurrentUserIdentity.ToString().ToInt32();
        AuditLogRepository.WriteAudit<UserModel>(CustomerAuditConstants.CustomerLogout, AuditType.Logout, null, null, AuditConstants.LogOutSuccessful, user);

        return Json(result);
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex);
        return BadRequest(ex.Message);
      }
    }
  }
}