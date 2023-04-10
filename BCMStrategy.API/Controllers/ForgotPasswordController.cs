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

namespace BCMStrategy.API.Controllers
{
  [RoutePrefix("api/ForgotPassword")]
  public class ForgotPasswordController : BaseApiController
  {
    /// <summary>
    /// The interface _userRepository master repository
    /// </summary>
    private IUserMaster _userRepository;

    /// /// <summary>
    /// Gets the _userRepository repository.
    /// </summary>
    /// <value>
    /// The _userRepository repository.
    /// </value>
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

    /// <summary>
    /// send email
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns>Forgot Password Master.</returns>
    [HttpPost]
    [Route("Reset")]
    public async Task<IHttpActionResult> Post(ForgotPasswordModel model)
    {
      if (!ModelState.IsValid)
      {
        return Ok(FormatResult(false, ModelState));
      }

      bool isSave = await UserRepository.ForgotPassword(model);

      return Ok(FormatResult(isSave , (isSave ? Resources.Resource.ForgotPasswordResetMailSent : Resources.Resource.ForgotPasswordResetMailNotSent)));
    }
  }
}
