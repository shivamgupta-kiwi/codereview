using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace BCMStrategy.Resources
{
  public class LanguageDetector
  {
    private readonly static List<Language> AvailableLanguages = new List<Language>()
    {
      new Language(){ LanguageFullName = "English", LanguageCultureName="en" },
    };

    private static bool IsLanguageAvailable(string language)
    {
      return AvailableLanguages.Any(x => x.LanguageCultureName.Equals(language));
    }

    public static string GetDefaultLanguage()
    {
      return AvailableLanguages.Select(x => x.LanguageCultureName).FirstOrDefault();
    }

    public void SetLanguage(string language)
    {
      try
      {
        if (!IsLanguageAvailable(language))
          language = GetDefaultLanguage();

        var cultureInfo = new CultureInfo(language, true);

        Thread.CurrentThread.CurrentUICulture = cultureInfo;
        Thread.CurrentThread.CurrentCulture = cultureInfo;
        HttpCookie langCookie = new HttpCookie("currentCulture", language);
        langCookie.Expires = DateTime.Now.AddYears(1);
        HttpContext.Current.Response.Cookies.Add(langCookie);
      }
      finally
      {
        
      }
    }
  }

  public class Language
  {
    public string LanguageFullName { get; set; }
    public string LanguageCultureName { get; set; }
  }
}
