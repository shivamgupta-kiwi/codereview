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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BCMStrategy.API.Controllers
{
  [Authentication]
  [RoutePrefix("api/Scheduler")]
  public class SchedulerController : BaseApiController
  {
    private static readonly EventLogger<SchedulerController> _log = new EventLogger<SchedulerController>();


    private IScheduler _schedulerRepository;

    private IScheduler SchedulerRepository
    {
      get
      {
        if (_schedulerRepository == null)
        {
          _schedulerRepository = new SchedulerRepository();
        }

        return _schedulerRepository;
      }
    }


    /// <summary>
    /// Get Drop down list of Lexicon Type
    /// </summary>
    /// <returns>Return lexicon type list</returns>
    [Route("GetDDSchedulerFrequencyList")]
    [HttpGet]
    public async Task<IHttpActionResult> GetDDSchedulerFrequencyList()
    {
      try
      {
        ApiOutput apiOutput = await SchedulerRepository.GetDDSchedulerFrequencyList();
        return Ok(apiOutput);
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex);
        return BadRequest(ex.Message);
      }
    }

    [Route("UpdateScheduler")]
    [HttpPost]
    public async Task<IHttpActionResult> UpdateScheduler(SchedulerModel schedulerModel)
    {
      try
      {
        bool isSave = false;
        int decryptSchedulerId = schedulerModel.FrequencyTypeMasterHashId.ToDecrypt().ToInt32();
        if (decryptSchedulerId == 0)
        {
          ModelState.Remove("schedulerModel.WeekdaysCheckbox");
        }
        if (decryptSchedulerId == 2)
        {
          ModelState.Remove("schedulerModel.WeekdaysCheckbox");
        }
        if (!ModelState.IsValid)
        {
          return Ok(FormatResult(false, ModelState));
        }

        isSave = await SchedulerRepository.UpdateScheduler(schedulerModel);

        return Ok(FormatResult(isSave, (isSave ? (string.IsNullOrEmpty(schedulerModel.SchedulerMasterHashId) ? Resources.Resource.SchedulerAddedSuccess : Resources.Resource.SchedulerUpdatedSuccess) : Resources.Resource.ErrorWhileSaving)));
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, schedulerModel);

        AuditLogs.Write<SchedulerModel, string>(AuditConstants.Scheduler, AuditType.UpdateFailure, schedulerModel, (string)null, Helper.GetInnerException(ex));
        return BadRequest(ex.Message);
      }
    }

    [Route("DeleteScheduler")]
    [HttpGet]
    public async Task<IHttpActionResult> DeleteScheduler(string schedulerMasterHashId)
    {
      try
      {
        bool isSave = false;

        isSave = await SchedulerRepository.DeleteScheduler(schedulerMasterHashId);

        return Ok(FormatResult(isSave, (isSave ? Resources.Resource.SchedulerDeletedSuccessfully : Resources.Resource.ErrorWhileDeleting)));
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, schedulerMasterHashId);
        AuditLogs.Write(AuditConstants.Scheduler, AuditType.DeleteFailure, Helper.GetInnerException(ex));
        return BadRequest(ex.Message);
      }
    }

    [Route("GetAllSchedulerList")]
    [HttpGet]
    public async Task<IHttpActionResult> GetAllSchedulerList(string parametersJson)
    {
      var parameters = JsonConvert.DeserializeObject<GridParameters>(parametersJson);
      ApiOutput apiOutput = await SchedulerRepository.GetAllSchedulerList(parameters);
      var result = new { Data = apiOutput.Data, Total = apiOutput.TotalRecords };
      return Json(result);
    }

    [Route("GetProcessDetailBasedOnScheduler")]
    [HttpGet]
    public async Task<IHttpActionResult> GetProcessDetailBasedOnScheduler(string parametersJson , string schedulerMasterHashId)
    {
      var parameters = JsonConvert.DeserializeObject<GridParameters>(parametersJson);
      ApiOutput apiOutput = await SchedulerRepository.GetProcessDetailBasedOnScheduler(parameters, schedulerMasterHashId);
      var result = new { Data = apiOutput.Data, Total = apiOutput.TotalRecords };
      return Json(result);
    }

    /// <summary>
    /// Get Scheduler By Hash Id for edit
    /// </summary>
    /// <param name="schedulerHashId"></param>
    /// <returns></returns>
    [Route("GetSchedulerByHashId")]
    [HttpGet]
    public async Task<IHttpActionResult> GetSchedulerByHashId(string schedulerHashId)
    {
      try
      {
        SchedulerModel schedulerModel = await SchedulerRepository.GetSchedulerByHashId(schedulerHashId);
        return Ok(schedulerModel);
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, schedulerHashId);
        return BadRequest(ex.Message);
      }
    }
  }
}
