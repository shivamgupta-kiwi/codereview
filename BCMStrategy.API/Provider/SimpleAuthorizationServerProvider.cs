using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BCMStrategy.Common.Unity;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.Provider;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Logger;
using BCMStrategy.Resources;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using BCMStrategy.Common.AuditLog;
using BCMStrategy.Data.Abstract.Abstract;

namespace BCMStrategy.API.Provider
{
  /// <summary>
  /// class SimpleAuthorizationServerProvider  Repository
  /// </summary>
  public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
  {
    /// <summary>
    /// Event logger for logging
    /// </summary>
    private static readonly EventLogger<SimpleAuthorizationServerProvider> _Log = new EventLogger<SimpleAuthorizationServerProvider>();

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

    /// <summary>
    /// The _audit repository
    /// </summary>
    private IAuditLog _auditRepository;

    /// <summary>
    /// Gets the audit repository.
    /// </summary>
    /// <value>
    /// The audit repository.
    /// </value>
    private IAuditLog AuditRepository
    {
      get
      {
        if (this._auditRepository == null)
        {
          this._auditRepository = UnityHelper.Resolve<IAuditLog>();
        }

        return this._auditRepository;
      }
    }
    /// <summary>
    /// Validate Client With Token
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>result for find client id and client secretId</returns>
    public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
    {
      try
      {
        string clientId = string.Empty;
        string clientSecret = string.Empty;
        ApiClient client = null;

        if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
        {
          context.TryGetFormCredentials(out clientId, out clientSecret);
        }

        string language = null;
        language = LanguageDetector.GetDefaultLanguage();

        new LanguageDetector().SetLanguage(language);

        if (context.ClientId == null)
        {
          ////Remove the comments from the below line context.SetError, and invalidate context
          ////if you want to force sending clientId/secrets once obtain access tokens.
          context.Validated();
          ////context.SetError("invalid_clientId", "ClientId should be sent.");
          context.SetError("invalid_clientId", Resource.ValidateClientId);
          return Task.FromResult<object>(null);
        }

        client = AuthRepository.FindClient(context.ClientId);

        if (client == null)
        {
          context.SetError("invalid_clientId", string.Format(Resource.ClientRegister, context.ClientId));
          return Task.FromResult<object>(null);
        }

        if (client.ApplicationType == (int)Helper.ApplicationType.NativeConfidential)
        {
          if (string.IsNullOrWhiteSpace(clientSecret))
          {
            context.SetError("invalid_clientId", Resource.ClientSecret);
            return Task.FromResult<object>(null);
          }
          else
          {
            if (client.SecretHash != Helper.GetHash(clientSecret))
            {
              context.SetError("invalid_clientId", Resource.InvalidClientSecret);
              return Task.FromResult<object>(null);
            }
          }
        }

        if (!client.IsActive)
        {
          context.SetError("invalid_clientId", Resource.ClientInactive);
          return Task.FromResult<object>(null);
        }

        context.OwinContext.Set<string>("as:clientAllowedOrigin", client.AllowedOrigin);
        context.OwinContext.Set<string>("as:clientRefreshTokenLifeTime", client.RefreshTokenLifeTime.ToString());

        context.Validated();
        return Task.FromResult<object>(null);
      }
      catch (Exception ex)
      {
        _Log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, context);
        context.SetError("error_process", "Error is occur while processing.");
        return Task.FromResult<object>(null);
      }
    }

    public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
    {
      try
      {
        var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");

        if (allowedOrigin == null) allowedOrigin = "*";

        context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });
        if (string.IsNullOrWhiteSpace(context.UserName) || string.IsNullOrWhiteSpace(context.Password))
        {
          context.SetError("invalid_grant", Resource.ValidateUserNamePassword);
          return;
        }

        UserModel userInfo = await AuthRepository.FindUser(context.UserName, context.Password);
        ApiClient clientInfo = AuthRepository.FindClient(context.ClientId);
        
        if (userInfo == null || clientInfo == null)
        {
          context.SetError("invalid_grant", Resource.ValidateUserNamePassword);
          return;
        }
        else if (!userInfo.Active)
        {
          context.SetError("invalid_grant", Resource.ValidateInactiveUserMessage);
          return;
        }

        if (userInfo != null && userInfo.UserType == "CUSTOMER")
        {
          AuditRepository.WriteAudit<SearchablePdfParameters>(CustomerAuditConstants.CustomerLogIn, AuditType.Login, null, null, AuditConstants.LoginSuccessful, userInfo.UserId);
        }

        var identity = new ClaimsIdentity(DefaultAuthenticationTypes.ApplicationCookie, ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userInfo.UserMasterHashId));
        identity.AddClaim(new Claim("sub", context.UserName));
        identity.AddClaim(new Claim(ClaimTypes.UserData, Newtonsoft.Json.JsonConvert.SerializeObject(userInfo)));
        identity.AddClaim(new Claim(ClaimTypes.Role, userInfo.UserType));
        identity.AddClaim(new Claim("userMasterHashId", userInfo.UserMasterHashId));
        identity.AddClaim(new Claim("userName", context.UserName));
        identity.AddClaim(new Claim("userFullName", string.Format("{0} {1}", userInfo.FirstName, userInfo.LastName)));

        var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    {
                      "as:client_id", (context.ClientId == null) ? string.Empty : context.ClientId
                    },
                    {
                      "userMasterHashId", userInfo.UserMasterHashId
                    },
                    {
                       "userType", userInfo.UserType
                    },
                    {
                      "userName", context.UserName
                    },
                    {
                      "userFullName",string.Format("{0} {1}", userInfo.FirstName, userInfo.LastName)
                    }
              });

        var ticket = new AuthenticationTicket(identity, props);
        context.Validated(ticket);
      }
      catch (System.Exception ex)
      {
        _Log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, context);
        context.SetError("error_process", "Error is occur while processing.");
        return;
      }
    }

    /// <summary>
    /// Token End Point For Response parameters
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>Response for Token End Point </returns>
    public override Task TokenEndpoint(OAuthTokenEndpointContext context)
    {
      foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
      {
        context.AdditionalResponseParameters.Add(property.Key, property.Value);
      }

      return Task.FromResult<object>(null);
    }
  }
}