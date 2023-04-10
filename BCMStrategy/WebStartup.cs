using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;

[assembly: OwinStartup(typeof(BCMStrategy.WebStartup))]

namespace BCMStrategy
{
  public class WebStartup
  {
    public void Configuration(IAppBuilder app)
    {
      app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

      app.UseCookieAuthentication(new CookieAuthenticationOptions
      {
        AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
        LoginPath = new PathString("/Account/Login"),
        Provider = new CookieAuthenticationProvider(),
        CookieName = DefaultAuthenticationTypes.ApplicationCookie,
        CookieHttpOnly = true,
      });
    }
  }
}
