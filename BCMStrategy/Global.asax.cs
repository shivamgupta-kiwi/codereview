﻿using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using BCMStrategy.App_Start;

namespace BCMStrategy
{
  public class Global : HttpApplication
  {
    protected void Application_BeginRequest(object sender, EventArgs e)
    {
      HttpContext.Current.Response.AddHeader("x-frame-options", "DENY");
    }

    private void Application_Start(object sender, EventArgs e)
    {
      // Code that runs on application startup
      AreaRegistration.RegisterAllAreas();
      GlobalConfiguration.Configure(WebApiConfig.Register);
      RouteConfig.RegisterRoutes(RouteTable.Routes);
      BundleConfig.RegisterBundles(BundleTable.Bundles);
    }
  }
}