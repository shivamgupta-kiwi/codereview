using System.Web.Optimization;

namespace BCMStrategy.App_Start
{
  public static class BundleConfig
  {
    /// <summary>
    /// Registers the bundles.
    /// </summary>
    /// <param name="bundles">The bundles.</param>
    public static void RegisterBundles(BundleCollection bundles)
    {
      string reqScriptsVariable = "~/bundles/reqScripts";
      string kendoScriptVariable = "~/bundles/KendoScript";

      string bundleContentVariable = "~/bundles/Content";
      string kendoStyleVariable = "~/Content/Kendo/styles";

      string bootStrapMobileVariable = "~/Content/bootstrapmobile";
      string customVariable = "~/Content/custom";
      string bundleLoginVariable = "~/bundles/Login";

      string kendoUiScript = "~/Scripts/js/kendo.all.min.js";
      string loginCssScript = "~/Content/css/login.css";
      string bootStrapMobileScript = "~/Content/css/kendo.bootstrap.mobile.min.css";

      //bundle for jquery and bootstrap js
      bundles.Add(new ScriptBundle(reqScriptsVariable).Include(
                          "~/Scripts/js/jquery-3.3.1.min.js",
                          "~/Scripts/js/popper.min.js",
                          "~/Scripts/js/bootstrap.min.js",
                          "~/Scripts/js/mdb.min.js",
                          "~/Scripts/js/jquery.validate.js",
                          "~/Scripts/common/common.js",
                          "~/Scripts/js/bootstrap-tagsinput.min.js"
                          ));
      //bundle for Kendo
      bundles.Add(new ScriptBundle(kendoScriptVariable).Include(
          kendoUiScript));

      ///css bundle for common
      bundles.Add(new StyleBundle(bundleContentVariable).Include(
              "~/Content/css/fontawesome-all.min.css",
              "~/Content/css/bootstrap.min.css",
              "~/Content/css/mdb.min.css",
              "~/Content/css/bootstrap-tagsinput.css"));

      bundles.Add(new StyleBundle(kendoStyleVariable).Include(
                "~/Content/css/kendo.bootstrap-v4.min.css",
                "~/Content/css/kendo.bootstrap.mobile.min.css"
               ));

      bundles.Add(new StyleBundle(bootStrapMobileVariable).Include(
                bootStrapMobileScript));

      bundles.Add(new StyleBundle(customVariable).Include(
                        "~/Content/css/style.css",
                        "~/Content/css/custom.css"
                        ));

      bundles.Add(new StyleBundle(bundleLoginVariable).Include(
                        loginCssScript
                        ));

#if DEBUG
      BundleTable.EnableOptimizations = false;
#else
      BundleTable.EnableOptimizations = true;
#endif
    }
  }
}