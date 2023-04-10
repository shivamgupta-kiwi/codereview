using BCMStrategy.API.AuditLog;
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
  [RoutePrefix("api/ActivityType")]
  public class ActivityTypeController : BaseApiController
  {
    private static readonly EventLogger<ActivityTypeController> _log = new EventLogger<ActivityTypeController>();

    private IActivityType _activityTypeRepository;

    private IActivityType ActivityTypeRepository
    {
      get
      {
        if (_activityTypeRepository == null)
        {
          _activityTypeRepository = new ActivityTypeRepository();
        }

        return _activityTypeRepository;
      }
    }

    [Route("UpdateActivityType")]
    [HttpPost]
    public async Task<IHttpActionResult> UpdateActivityType(ActivityTypeModel activityTypeModel)
    {
      try
      {
        bool isSave = false;
        var test = false;
        if (!ModelState.IsValid)
        {
          return Ok(FormatResult(false, ModelState));
        }

        isSave = await ActivityTypeRepository.UpdateActivityType(activityTypeModel);

        return Ok(FormatResult(isSave, (isSave ? (string.IsNullOrEmpty(activityTypeModel.ActivityTypeMasterHashId) ? Resources.Resource.ActivityTypeAddedSuccess : Resources.Resource.ActivityTypeUpdatedSuccess) : Resources.Resource.ErrorWhileSaving)));
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, activityTypeModel);
        if (string.IsNullOrEmpty(activityTypeModel.ActivityTypeMasterHashId))
          AuditLogs.Write<ActivityTypeModel, string>(AuditConstants.ActivityType, AuditType.InsertFailure, activityTypeModel, (string)null, Helper.GetInnerException(ex));
        else
          AuditLogs.Write<ActivityTypeModel, string>(AuditConstants.ActivityType, AuditType.UpdateFailure, activityTypeModel, (string)null, Helper.GetInnerException(ex));
        return BadRequest(ex.Message);
      }
    }

    [Route("DeleteActivityType")]
    [HttpGet]
    public async Task<IHttpActionResult> DeleteActivityType(string activityTypeMasterHashId)
    {
      try
      {
        ApiOutput apiOutput = await ActivityTypeRepository.DeleteActivityType(activityTypeMasterHashId);
        return Ok(apiOutput);
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, activityTypeMasterHashId);
        AuditLogs.Write(AuditConstants.ActivityType, AuditType.DeleteFailure, Helper.GetInnerException(ex));
        return BadRequest(ex.Message);
      }
    }

    [Route("GetAllActivityTypeList")]
    [HttpGet]
    public async Task<IHttpActionResult> GetAllActivityTypeList(string parametersJson)
    {
      var parameters = JsonConvert.DeserializeObject<GridParameters>(parametersJson);
      ApiOutput apiOutput = await ActivityTypeRepository.GetAllActivityTypeList(parameters);
      var result = new { Data = apiOutput.Data, Total = apiOutput.TotalRecords };
      return Json(result);
    }

    /// <summary>
    /// Get Drop down list of Metadata Dynamic table Noun plus Verb
    /// </summary>
    /// <returns>Return meta data DynamicTableNounplusVerb list</returns>
    [Route("GetDDActivityTypeList")]
    [HttpGet]
    public async Task<IHttpActionResult> GetDDActivityTypeList(string actionTypeMasterHashId)
    {
      try
      {
        ApiOutput apiOutput = await ActivityTypeRepository.GetDDActivityTypeList(actionTypeMasterHashId);
        return Ok(apiOutput);
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, actionTypeMasterHashId);
        return BadRequest(ex.Message);
      }
    }

    /// <summary>
    /// Get Metadata Type By Hash Id
    /// </summary>
    /// <param name="activityTypeHashId"></param>
    /// <returns></returns>
    [Route("GetActivityTypeByHashId")]
    [HttpGet]
    public async Task<IHttpActionResult> GetActivityTypeByHashId(string activityTypeHashId)
    {
      try
      {
        ActivityTypeModel activitTypeModel = await ActivityTypeRepository.GetActivityTypeByHashId(activityTypeHashId);
        return Ok(activitTypeModel);
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, activityTypeHashId);
        return BadRequest(ex.Message);
      }
    }
  }
}
