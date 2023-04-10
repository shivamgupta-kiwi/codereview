using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using BCMStrategy.Common.Unity;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.Provider;

namespace BCMStrategy.API.Controllers
{
  [RoutePrefix("api/RefreshTokens")]
  public class RefreshTokensController : BaseApiController
  {
    /// <summary>
    /// Define Authentication Repository
    /// </summary>
    private IAuth _authRepository;

    /// <summary>
    /// Gets or set Authentication Repository
    /// </summary>
    private IAuth AuthRepository
    {
      get
      {
        if (_authRepository == null)
        {
          _authRepository = UnityHelper.Resolve<IAuth>();
        }

        return _authRepository;
      }
    }

    [HttpDelete]
    [Route("")]
    public async Task<IHttpActionResult> Delete(string id)
    {
      var status = await AuthRepository.RemoveRefreshToken(id);
      if (status)
      {
        return Ok();
      }
      return BadRequest("Token is not valid");
    }
  }
}
