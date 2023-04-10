using System.Web.Mvc;
using System.Web.Routing;

namespace BCMStrategy
{
  public static class RouteConfig
  {
    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

      routes.MapRoute(
          name: "Default",
          url: "{controller}/{action}/{id}",
          defaults: new { controller = "Login", action = "Index", id = UrlParameter.Optional }
      ).DataTokens.Add("area", "Account");
    }
  }
}