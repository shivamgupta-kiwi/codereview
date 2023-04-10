using BCMStrategy.API.Filter;
using BCMStrategy.Common.Kendo;
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
  [RoutePrefix("api/UserAccessRights")]
  public class UserAccessRightsController : BaseApiController
  {
    private static readonly EventLogger<UserAccessRightsController> _log = new EventLogger<UserAccessRightsController>();

    private IUserAccessRights _userAccessRightsRepository;

    private IUserAccessRights UserAccessRightsRepository
    {
      get
      {
        if (_userAccessRightsRepository == null)
        {
          _userAccessRightsRepository = new UserAccessRightsRepository();
        }

        return _userAccessRightsRepository;
      }
    }

    /// <summary>
    /// Get Drop down list of Lexicon Type
    /// </summary>
    /// <returns>Return lexicon type list</returns>
    [Route("GetDDUserList")]
    [HttpGet]
    public async Task<IHttpActionResult> GetDDUserList()
    {
      try
      {
        ApiOutput apiOutput = await UserAccessRightsRepository.GetDDUserList();
        return Ok(apiOutput);
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex);
        return BadRequest(ex.Message);
      }
    }

    [Route("GetAllMenuList")]
    [HttpGet]
    public async Task<IHttpActionResult> GetAllMenuList()
    {
      ApiOutput apiOutput = await UserAccessRightsRepository.GetAllMenuList();
      var result = new { Data = apiOutput.Data, Total = apiOutput.TotalRecords };
      return Json(result);
    }

    [Route("GetAllUserAccessRightsList")]
    [HttpGet]
    public async Task<IHttpActionResult> GetAllUserAccessRightsList(string parametersJson)
    {
      var parameters = JsonConvert.DeserializeObject<GridParameters>(parametersJson);
      ApiOutput apiOutput = await UserAccessRightsRepository.GetAllUserAccessRightsList(parameters);
      var result = new { Data = apiOutput.Data, Total = apiOutput.TotalRecords };
      return Json(result);
    }

    [Route("UpdateUserAccessRights")]
    [HttpPost]
    public async Task<IHttpActionResult> UpdateUserAccessRights(UserAccessRightsModel userAccessRightsModel)
    {
      try
      {
        bool isSave = false;

        if (!ModelState.IsValid)
        {
          return Ok(FormatResult(false, ModelState));
        }

        isSave = await UserAccessRightsRepository.UpdateUserAccessRights(userAccessRightsModel);

        return Ok(FormatResult(isSave, (isSave ? (string.IsNullOrEmpty(userAccessRightsModel.UserAccessRightsMasterHashId) ? Resources.Resource.UserAccessRightsSuccess : Resources.Resource.UserAccessRightsUpdatedSuccess) : Resources.Resource.ErrorWhileSaving)));
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, userAccessRightsModel);
        return BadRequest(ex.Message);
      }
    }

    [Route("DeleteUserAccessRights")]
    [HttpGet]
    public async Task<IHttpActionResult> DeleteUserAccessRights(string userAccessRightsMasterHashId)
    {
      try
      {
        bool isSave = false;

        isSave = await UserAccessRightsRepository.DeleteUserAccessRights(userAccessRightsMasterHashId);

        return Ok(FormatResult(isSave, (isSave ? Resources.Resource.ActivityTypeDeletedSuccessfully : Resources.Resource.ErrorWhileDeleting)));
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, userAccessRightsMasterHashId);
        return BadRequest(ex.Message);
      }
    }
  }
}
