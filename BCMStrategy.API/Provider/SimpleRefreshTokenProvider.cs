using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using BCMStrategy.Common.Unity;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.Provider;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Logger;
using Microsoft.Owin.Security.Infrastructure;

namespace BCMStrategy.API.Provider
{
  public class SimpleRefreshTokenProvider : IAuthenticationTokenProvider
  {
    private static readonly EventLogger<SimpleRefreshTokenProvider> _log = new EventLogger<SimpleRefreshTokenProvider>();

    private IAuth _authRepository;

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


    public async Task CreateAsync(AuthenticationTokenCreateContext context)
    {
      try
      {
        var clientid = context.Ticket.Properties.Dictionary["as:client_id"];
        var userMasterHashId = context.Ticket.Properties.Dictionary["userMasterHashId"];

        if (string.IsNullOrEmpty(clientid))
        {
          return;
        }
        var refreshTokenId = Guid.NewGuid().ToString("n");

        var refreshTokenLifetime = context.OwinContext.Get<string>("as:clientRefreshTokenLifeTime");

        var token = new RefreshToken()
        {
          ID = refreshTokenId.GetHash(),
          ClientID = clientid,
          UserID = userMasterHashId.ToDecrypt().ToInt32(),
          IssuedDateTime = DateTime.UtcNow,
          ExpiresDateTime = DateTime.UtcNow.AddSeconds(Convert.ToDouble(refreshTokenLifetime))
        };

        context.Ticket.Properties.IssuedUtc = token.IssuedDateTime;
        context.Ticket.Properties.ExpiresUtc = token.ExpiresDateTime;

        token.ProtectedTicket = context.SerializeTicket();
        bool result = await AuthRepository.AddRefreshToken(token);

        if (result)
        {
          context.SetToken(refreshTokenId);
        }
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, context);
      }
    }


    public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
    {
      try
      {
        var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");
        context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

        string hashedTokenId = Helper.GetHash(context.Token);

        var refreshToken = await AuthRepository.FindRefreshTokenAsync(hashedTokenId);

        if (refreshToken != null)
        {
          //Get protectedTicket from refreshToken class
          context.DeserializeTicket(refreshToken.ProtectedTicket);
          await AuthRepository.RemoveRefreshToken(hashedTokenId);
        }
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, context);
      }
    }

    public void Create(AuthenticationTokenCreateContext context)
    {
      throw new NotImplementedException();
    }

    public void Receive(AuthenticationTokenReceiveContext context)
    {
      throw new NotImplementedException();
    }
  }
}