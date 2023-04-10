using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using BCMStrategy.Common.Unity;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Logger;
using BCMStrategy.Data.Repository.Concrete;
using BCMStrategy.API.Filter;
using BCMStrategy.Resources;

namespace BCMStrategy.API.Controllers
{
	[Authentication]
	[RoutePrefix("api/Dashboard")]
  public class DashboardApiController : BaseApiController
  {
    private static readonly EventLogger<StateHeadApiController> _log = new EventLogger<StateHeadApiController>();

    private IDashboard _dashboardRepository;

    private IDashboard DashboardRepository
    {
      get
      {
        if (_dashboardRepository == null)
        {
          _dashboardRepository = new DashboardRepository();
        }

        return _dashboardRepository;
      }
    }

    [HttpGet]
    [Route("GetAllDashboardLexiconTerms")]
    public async Task<IHttpActionResult> GetAllDashboardLexiconTerms(string selectedDate = "", string lexiconTypeHashId = "")
    {
      try
      {
        List<DashboardLexiconTypeViewModel> result = await DashboardRepository.GetLexiconsForDashboard(selectedDate, lexiconTypeHashId);
        return Ok(result);
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception occur while Getting DashBoard data", ex);
        return BadRequest(ex.Message);
      }
    }

    [HttpPost]
    [Route("GetChartLexiconValues")]
    public async Task<IHttpActionResult> GetChartLexiconValues(ReportViewModel model)
    {
      try
      {
        List<ReportViewModel> result = DashboardRepository.GetChartLexiconValues(model);
        return Ok(result);
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception occur while Getting DashBoard data", ex);
        return BadRequest(ex.Message);
      }
    }

    [HttpPost]
    [Route("PostChartLexiconValues")]
    public async Task<IHttpActionResult> PostChartLexiconValues(ReportViewModel model)
    {
      try
      {
        List<ReportViewModel> result = DashboardRepository.GetChartLexiconValues(model);
        return Ok(result);
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception occur while Getting DashBoard data", ex);
        return BadRequest(ex.Message);
      }
    }

    [HttpPost]
    [Route("UpdateLexiconDefaultFilter")]
    public async Task<IHttpActionResult> UpdateLexiconDefaultFilter(ReportViewModel model)
    {
      try
      {
        ApiOutput apiOutput = new ApiOutput();
        bool isSave = DashboardRepository.UpdateLexiconDefaultFilter(model);

        apiOutput.Data = isSave;
        apiOutput.TotalRecords = 0;
        apiOutput.ErrorMessage = isSave ? Resource.LblSuccessfulLexiconDefaultFilter : Resource.LblFailedLexiconDefaultFilter;

        return Ok(apiOutput);
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception occur while Updating Lexicon Default Filter ", ex);
        throw;
      }
    }

    [HttpGet]
    [Route("GetActivityTypeValues")]
    public async Task<IHttpActionResult> GetActivityTypeValues(string selectedDate, string actionHashId, string lexiconHashId)
    {
      try
      {
        ////List<ActivityType> result = await DashboardRepository.GetActivityTypeValues(processId, actionHashId, lexiconHashId);
        ////return Ok(result);
        ApiOutput apiOutput = await DashboardRepository.GetActivityTypeValues(selectedDate, actionHashId, lexiconHashId);
        var result = new { Data = apiOutput.Data, Total = apiOutput.TotalRecords };
        return Json(result);
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception occur while Getting DashBoard data", ex);
        return BadRequest(ex.Message);
      }
    }

    [HttpPost]
    [Route("AuthenticateUserForVirtualDashboard")]
    public async Task<bool> AuthenticateUserForVirtualDashboard(EmailServiceModel emailServiceModel)
    {
      try
      {
        bool result = await DashboardRepository.AuthenticateUserForVirtualDashboard(emailServiceModel);
        return result;
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception occur while Getting DashBoard data", ex);
        return false;
      }
    }

  }
}