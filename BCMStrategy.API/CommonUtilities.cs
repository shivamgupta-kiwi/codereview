using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCMStrategy.API
{
  public static class CommonUtilities
  {
    public static List<KeyValuePair<string, string>> HandleModelState(System.Web.Http.ModelBinding.ModelStateDictionary modelState, bool removePretext = true)
    {
      return modelState.Select(x => x.Value.Errors
                .Select(y => new KeyValuePair<string, string>(removePretext ? x.Key.PostSeprator('.') : x.Key, y.ErrorMessage)).First()).ToList();
    }

    public static string PostSeprator(this string input, char seprator)
    {
      int l = input.IndexOf(seprator);
      if (l > 0)
      {
        return input.Substring((l + 1), (input.Length - l - 1));
      }
      return input;
    }

    public static List<KeyValuePair<string, string>> HandleModelStateForImport(System.Web.Http.ModelBinding.ModelStateDictionary modelState)
    {
      return modelState.SelectMany(x => x.Value.Errors
                .Select(y => new KeyValuePair<string, string>(x.Key, y.ErrorMessage))).ToList();
    }
  }
}