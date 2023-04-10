using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using BCMStrategy.API.Filter;
using BCMStrategy.Common.Kendo;
using BCMStrategy.Common.Unity;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Logger;
using Newtonsoft.Json;
using BCMStrategy.API.AuditLog;
using BCMStrategy.Common.AuditLog;
using BCMStrategy.Data.Abstract;

namespace BCMStrategy.API.Controllers
{
  [Authentication]
  [RoutePrefix("api/UserManagement")]
  public class UserManagementApiController : BaseApiController
  {
    private static readonly EventLogger<UserManagementApiController> _log = new EventLogger<UserManagementApiController>();
    private IUserMaster _userRepository;

    private IUserMaster UserRepository
    {
      get
      {
        if (_userRepository == null)
        {
          _userRepository = UnityHelper.Resolve<IUserMaster>();
        }

        return _userRepository;
      }
    }

    [HttpPost]
    [Route("UpdateUser")]
    public async Task<IHttpActionResult> UpdateUser(UserModel model)
    {
      try
      {
        bool isSave = false;
        
        if (!ModelState.IsValid)
        {
          return Ok(FormatResult(false, ModelState));
        }

        isSave = await UserRepository.UpdateUserMaster(model);
        if (string.IsNullOrEmpty(model.UserMasterHashId))
        {
          if (!string.IsNullOrEmpty(model.UserType) && model.UserType.ToLower() == "customer")
          {
            return Ok(FormatResult(isSave, (isSave ? Resources.Resource.CustomerAddedSuccessfully : Resources.Resource.ErrorWhileSaving)));
          }
          else
          {
            return Ok(FormatResult(isSave, (isSave ? Resources.Resource.AdminAddedSuccessfully : Resources.Resource.ErrorWhileSaving)));
          }
        }
        else
        {
          if (!string.IsNullOrEmpty(model.UserType) && model.UserType.ToLower() == "customer")
          {
            if (model.Status.ToLower() == model.OldStatus.ToLower())
              return Ok(FormatResult(isSave, (isSave ? Resources.Resource.CustomerUpdatedSuccessfully : Resources.Resource.ErrorWhileSaving)));
            else if (model.Status.ToLower() == "inactive")
              return Ok(FormatResult(isSave, (isSave ? Resources.Resource.CustomerInActiveSuccess : Resources.Resource.ErrorWhileSaving)));
            else
              return Ok(FormatResult(isSave, (isSave ? Resources.Resource.CustomerActiveSuccess : Resources.Resource.ErrorWhileSaving)));
          }
          else
          {
            if (model.Status.ToLower() == model.OldStatus.ToLower())
              return Ok(FormatResult(isSave, (isSave ? Resources.Resource.AdminUpdatedSuccessfully : Resources.Resource.ErrorWhileSaving)));
            else if (model.Status.ToLower() == "inactive")
              return Ok(FormatResult(isSave, (isSave ? Resources.Resource.AdminInActiveSuccess : Resources.Resource.ErrorWhileSaving)));
            else
              return Ok(FormatResult(isSave, (isSave ? Resources.Resource.AdminActiveSuccess : Resources.Resource.ErrorWhileSaving)));
          }
        }
        ////return Ok(FormatResult(isSave, (isSave ? Resources.Resource.SuccessMessageforUser : Resources.Resource.ErrorWhileSaving)));
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, model);
        if (string.IsNullOrEmpty(model.UserMasterHashId))
          AuditLogs.Write<UserModel, string>(model.UserType == "CUSTOMER" ? AuditConstants.CustomerUser : AuditConstants.AdminUser, AuditType.InsertFailure, model, (string)null, Helper.GetInnerException(ex));
        else
          AuditLogs.Write<UserModel, string>(model.UserType == "CUSTOMER" ? AuditConstants.CustomerUser : AuditConstants.AdminUser, AuditType.UpdateFailure, model, (string)null, Helper.GetInnerException(ex));

        return BadRequest(ex.Message);
      }
    }

    [Route("GetAllUserList")]
    [HttpGet]
    public async Task<IHttpActionResult> GetAllUserList(string parametersJson, string userType)
    {
      var parameters = JsonConvert.DeserializeObject<GridParameters>(parametersJson);
      ApiOutput apiOutput = await UserRepository.GetUserManagementList(parameters, userType);
      var result = new { Data = apiOutput.Data, Total = apiOutput.TotalRecords };
      return Json(result);
    }

    [Route("DeleteUser")]
    [HttpGet]
    public async Task<IHttpActionResult> DeleteUser(string userHashId, string userType)
    {
      try
      {
        
        bool isSave = false;

        isSave = await UserRepository.DeleteUser(userHashId);

        if (userType.ToLower() == "customer")
          return Ok(FormatResult(isSave, (isSave ? Resources.Resource.CustomerDeletedSuccessfully : Resources.Resource.ErrorWhileDeleting)));
        else
          return Ok(FormatResult(isSave, (isSave ? Resources.Resource.AdminDeletedSuccessfully : Resources.Resource.ErrorWhileDeleting)));
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, userHashId);
        AuditLogs.Write(AuditConstants.CustomerUser, AuditType.DeleteFailure, Helper.GetInnerException(ex));
        return BadRequest(ex.Message);
      }
    }

    /// <summary>
    /// Get User data By Hash Id for edit
    /// </summary>
    /// <param name="userHashId"></param>
    /// <returns></returns>
    [Route("GetUserByHashId")]
    [HttpGet]
    public async Task<IHttpActionResult> GetUserByHashId(string userHashId)
    {
      try
      {
        UserModel userModel = await UserRepository.GetUserByHashID(userHashId);
        return Ok(userModel);
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, userHashId);
        return BadRequest(ex.Message);
      }
    }

    [Route("GetDefaultLexiconList")]
    [HttpGet]
    public async Task<IHttpActionResult> GetDefaultLexiconList(string parametersJson, string userMasterHashId)
    {
      var parameters = JsonConvert.DeserializeObject<GridParameters>(parametersJson);
      ApiOutput apiOutput = await UserRepository.GetDefaultLexiconList(parameters, userMasterHashId);
      var result = new { Data = apiOutput.Data, Total = apiOutput.TotalRecords };
      return Json(result);
    }
  }
}
