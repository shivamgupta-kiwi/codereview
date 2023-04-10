using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using BCMStrategy.API;
using BCMStrategy.Common.Unity;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Logger;
using BCMStrategy.Resources;

namespace BCMStrategy.API.Controllers
{
  [RoutePrefix("api/Registration")]
  public class RegistrationApiController : BaseApiController
  {
    private static readonly EventLogger<RegistrationApiController> _log = new EventLogger<RegistrationApiController>();
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
    [Route("SaveRegistration")]
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

        return Ok(FormatResult(isSave, (isSave ? Resources.Resource.SuccessfulMessageForRegistration : Resources.Resource.ErrorWhileSaving)));
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, model);
        return BadRequest(ex.Message);
      }
    }

    [HttpGet]
    [Route("ActivationCodeExist")]
    public async Task<IHttpActionResult> ActivationCodeExist(string activationCode)
    {
      var userDetail = await UserRepository.GetUserByActivationCode(activationCode);
      return Ok(FormatResult(userDetail, string.Empty));
    }

    [HttpPost]
    [Route("SetPassword")]
    public async Task<IHttpActionResult> SetPassword(SetPasswordModel model)
    {
      try
      {
        ApiOutput apiOutput;

        if (!ModelState.IsValid)
        {
          return Ok(FormatResult(false, ModelState));
        }

        apiOutput = await UserRepository.UpdatePassword(model);

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
