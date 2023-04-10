using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using BCMStrategy.Common.Unity;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.Provider;
using BCMStrategy.Data.Repository.Provider;

namespace BCMStrategy.API.Filter
{
  public class AuthenticationAttribute : AuthorizeAttribute
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

    protected override bool IsAuthorized(HttpActionContext actionContext)
    {
      IEnumerable<string> refreshToken;
      actionContext.Request.Headers.TryGetValues("RefreshToken", out refreshToken);

      if (refreshToken != null && refreshToken.Any())
      {
        string hashedTokenId = Helper.GetHash(refreshToken.FirstOrDefault());

        var token = AuthRepository.FindRefreshTokenAsync(hashedTokenId);

        if (token == null)
        {
          actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
        }
        else
        {
          var currentPrincipal = actionContext.RequestContext.Principal;
          if (currentPrincipal == null
              || currentPrincipal.Identity == null
              || !currentPrincipal.Identity.IsAuthenticated)
          {
            actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
          }
        }
      }
      else
      {
        actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
      }

      return true;
    }
  }
}