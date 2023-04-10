using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BCMStrategy.Resources;

namespace BCMStrategy.Controllers
{
  public class BaseController : Controller
  {
    protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
    {
      string language = null;
      HttpCookie langCookie = Request.Cookies["currentCulture"];

      if (langCookie != null)
      {
        language = langCookie.Value;
      }
      else
      {
        var userLanguages = Request.UserLanguages;
        var userLang = userLanguages != null && userLanguages.Count() > 0 ? userLanguages[0] : "";
        if (!string.IsNullOrWhiteSpace(userLang))
        {
          language = userLang;
        }
        else
        {
          language = LanguageDetector.GetDefaultLanguage();
        }
      }

      new LanguageDetector().SetLanguage(language);
      ViewBag.CurrentLanguage = language;
      return base.BeginExecuteCore(callback, state);
    }
  }
}