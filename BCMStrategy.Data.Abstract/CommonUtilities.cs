using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.ModelBinding;
using System.Globalization;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.DAL.Context;

namespace BCMStrategy.Data.Abstract
{
  public static class CommonUtilities
  {
    /// <summary>
    /// Gets or sets the application base path.
    /// </summary>
    /// <value>
    /// The application base path.
    /// </value>
    public static string ApplicationBasePath { get; set; }

    public static string DefaultTimezone
    {
      get
      {
        return "Central Europe Standard Time";
      }
    }

    /// <summary>
    /// convert the input in to Int32
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static int ToInt32(this string input)
    {
      int output;
      Int32.TryParse(input, out output);
      return output;
    }

    /// <summary>
    /// convert the input in to Int64
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static long ToInt64(this string input)
    {
      long output;
      Int64.TryParse(input, out output);
      return output;
    }

    /// <summary>
    /// convert the input in to NullableInt32
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static int? ToNullableInt32(this string input)
    {
      int output;

      if (Int32.TryParse(input, out output))
        return output;

      return null;
    }

    /// <summary>
    /// convert the input in to NullableInt64
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static long? ToNullableInt64(this string input)
    {
      long output;

      if (Int64.TryParse(input, out output))
        return output;

      return null;
    }

    /// <summary>
    /// convert the input in to Float
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static float ToFloat(this string input)
    {
      float output;
      float.TryParse(input, out output);
      return output;
    }

    /// <summary>
    /// convert the input in to NullableFloat
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static float? ToNullableFloat(this string input)
    {
      float output;

      if (float.TryParse(input, out output))
        return output;

      return null;
    }

		public static GlobalSettingModel ToViewModel(this List<globalconfiguration> objGlobalConfiguration)
		{
			GlobalSettingModel globalConfig = new GlobalSettingModel();

			if (objGlobalConfiguration.NotNullOrEmpty())
			{
				foreach (globalconfiguration gbl in objGlobalConfiguration)
				{
					switch (gbl.Name)
					{
						case GlobalConfigurationKeys.SMTPDetails:
							globalConfig.SMTPDetails = gbl.Value;
							break;

					}
				}
			}

			return globalConfig;
		}

		/// <summary>
		/// convert the input in to Bool
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static bool ToBool(this string input)
    {
      bool output;
      Boolean.TryParse(input, out output);
      return output;
    }

    /// <summary>
    /// convert the input in to ToRound with specified round length
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static float ToRound(this float input, int round)
    {
      return (float)Math.Round(input, round);
    }

    /// <summary>
    /// Remove Ordinals from numbers
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string ReplaceOrdinals(this string input)
    {
      return input.Replace(" - ", " ").Replace("Sept. ", "Sep. ").Replace(@"\b(\d+)(?:st|nd|rd|th)\b", "");
    }

    /// <summary>
    /// Remove Special html content
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string ReplaceHtmlContents(this string input)
    {
      return input.Replace("\n"," ").Replace("&nbsp;", " ").Replace("\t", " ");
    }

    public enum Months
    {
      January = 1,
      February = 2,
      March = 3,
      april = 4,
      May = 5,
      Jun = 6,
      July = 7,
      August = 8,
      September = 9,
      October = 10,
      November = 11,
      December = 12,
    }

    /// <summary>
    /// Auction Status
    /// </summary>
    public enum Status
    {
      NotAvailable = 0,
      Draft = 1
    }

    /// <summary>
    /// convert the input in to ToFormatedDateTime
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string ToFormatedDateTime(this DateTime input, string format = "MM/dd/yyyy HH:mm")
    {
      return input.ToString(format);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="modelState"></param>
    /// <param name="removePretext"></param>
    /// <returns></returns>
    public static List<KeyValuePair<string, string>> HandleModelState(ModelStateDictionary modelState, bool removePretext = true)
    {
      return modelState.SelectMany(x => x.Value.Errors
                .Select(y => new KeyValuePair<string, string>(removePretext ? x.Key.PostSeprator('.') : x.Key, y.ErrorMessage))).ToList();
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

    public static string ToFormatedDateTime(this DateTime? input, string format = "dd-MM-yyyy HH:mm")
    {
      return input.HasValue ? input.Value.ToString(format) : string.Empty;
    }

    ////public static DateTime ToUCTTimezone(this DateTime input)
    ////{
    ////  if (input == null)
    ////    return input;

    ////  return TimeZoneInfo.ConvertTimeToUtc(input);
    ////}

    ////public static DateTime ToUCTTimezone(this DateTime input)
    ////{
    ////  if (input == null)
    ////    return input;

    ////  ////TimeZoneInfo utcTimeZone = TimeZoneInfo.Utc;
    ////  ////TimeZoneInfo userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(DefaultTimezone);
    ////  ////DateTime utc = DateTime.SpecifyKind(input, DateTimeKind.Unspecified);
    ////  ////return TimeZoneInfo.ConvertTime(utc, userTimeZone, utcTimeZone);

    ////  ////TimeZoneInfo utcTimeZone = TimeZoneInfo.Utc;
    ////  ////TimeZoneInfo userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(DefaultTimezone);
    ////  ////return TimeZoneInfo.ConvertTime(input, userTimeZone, utcTimeZone);

    ////  return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(input, DefaultTimezone);

    ////}

    ////public static DateTime ToUserTimezone(this DateTime input)
    ////{
    ////  System.Globalization.CultureInfo.CurrentCulture.ClearCachedData();
    ////  if (input == null)
    ////    return input;

    ////  return TimeZoneInfo.ConvertTimeToUtc(input);
    ////}

    public static DateTime? ToDefaultTimeZone(DateTime? datetime)
    {
      if (datetime.HasValue)
      {
        var timeUtc = (DateTime)datetime;
        TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById(CommonUtilities.DefaultTimezone);
        DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, easternZone);
        return easternTime;
      }
      else
      {
        return datetime;
      }
       
    }

    public static DateTime ToUTCTimezone(DateTime input)
    {
      System.Globalization.CultureInfo.CurrentCulture.ClearCachedData();
      var timezoneObject = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(x => x.Id == Resources.Enums.TimeZone.Eastern_Standard_Time.ToString().Replace("_", " "));
      var timeUtc = input;
      DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc(timeUtc, timezoneObject);
      return utcTime;
    }

    public static DateTime ToESTTimezone(DateTime input)
    {
      System.Globalization.CultureInfo.CurrentCulture.ClearCachedData();
      var timezoneObject = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(x => x.Id == Resources.Enums.TimeZone.Eastern_Standard_Time.ToString().Replace("_"," "));
      var timeUtc =  DateTime.SpecifyKind(input, DateTimeKind.Unspecified);
      DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, timezoneObject);
      return cstTime;
    }

    public static DateTime? FromFormatedDateTime(this string input, string format = "dd-MM-yyyy HH:mm")
    {
      DateTime output;
      DateTime.TryParseExact(input, format, System.Globalization.CultureInfo.InvariantCulture,
      DateTimeStyles.None, out output);
      return output;
    }
  }
}
