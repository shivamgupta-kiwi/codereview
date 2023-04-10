using System;
using System.Web.Http;
using BCMStrategy.API.Provider;
using BCMStrategy.Data.Abstract;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;

[assembly: OwinStartup(typeof(BCMStrategy.API.Startup))]

namespace BCMStrategy.API
{
  public class Startup
  {
    public void Configuration(IAppBuilder app)
    {
      // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
      HttpConfiguration config = new HttpConfiguration();
      ConfigureOAuth(app);
      UnityConfig.RegisterComponents();
      GlobalConfiguration.Configure(WebApiConfig.Register);
      app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
      app.UseWebApi(config);
    }

    private void ConfigureOAuth(IAppBuilder app)
    {
      // Configurable path & Expiry Time.
      OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
      {
        AllowInsecureHttp = true,
        TokenEndpointPath = new PathString("/token"),
        AccessTokenExpireTimeSpan = TimeSpan.FromDays(100),
        Provider = new SimpleAuthorizationServerProvider(),
        RefreshTokenProvider = new SimpleRefreshTokenProvider()
      };

      // Token Generation
      app.UseOAuthAuthorizationServer(OAuthServerOptions);
      app.UseCookieAuthentication(new CookieAuthenticationOptions());
      app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
    }
  }
}