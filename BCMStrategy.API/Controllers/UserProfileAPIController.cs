using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using BCMStrategy.API.Filter;
using BCMStrategy.Common.Unity;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Data.Repository;
using BCMStrategy.Logger;

namespace BCMStrategy.API.Controllers
{
  [Authentication]
  [RoutePrefix("api/UserProfile")]
  public class UserProfileApiController : BaseApiController
  {
    private static readonly EventLogger<UserProfileApiController> _log = new EventLogger<UserProfileApiController>();

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
 
    [Route("GetProfileDetail")]
    [HttpGet]
    public async Task<IHttpActionResult> GetProfileDetail()
    {
      var userDetail = await UserRepository.GetUserByHashID(UserAccessHelper.CurrentUserIdentity.ToEncrypt());
      return Ok(FormatResult(userDetail));
    }

    [HttpPost]
    [Route("SaveUserProfile")]
    public async Task<IHttpActionResult> SaveRegistrationDetail(UserModel model)
    {
      try
      {
        bool isSave = false;

        if (!ModelState.IsValid)
        {
          return Ok(FormatResult(false, ModelState));
        }

        isSave = await UserRepository.UpdateUserMaster(model);

        return Ok(FormatResult(isSave, (isSave ? Resources.Resource.SuccessfulMessageForUserProfile : Resources.Resource.ErrorWhileSaving)));
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, model);
        return BadRequest(ex.Message);
      }
    }

    [HttpPost]
    [Route("ChangePassword")]
    public async Task<IHttpActionResult> ChangePassword(ChangePasswordModel model)
    {
      try
      {
        

        if (!ModelState.IsValid)
        {
          return Ok(FormatResult(false, ModelState));
        }

        ApiOutput apiOutput = await UserRepository.ChangePassword(model);

        return Ok(apiOutput);
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, model);
        return BadRequest(ex.Message);
      }
    }
  }

}
