using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using BCMStrategy.Common.Kendo;
using BCMStrategy.DAL.Context;
using BCMStrategy.Data.Abstract.ViewModels;
using Newtonsoft.Json;

namespace BCMStrategy.Data.Abstract
{
	public static class Helper
	{
		/// <summary>
		/// Encryption Key
		/// </summary>
		private static string EncryptionKey = "bcmstrategy";

		/// <summary>
		/// Assembly Version
		/// </summary>
		private static string _assemblyVersion = string.Empty;

		public static readonly string IgnoreExtNUrls = ConfigurationManager.AppSettings["ignoreextnurl"];

		public static readonly string IgnoreExtLinks = ConfigurationManager.AppSettings["ignoreexternalLinks"];

		public static readonly string ThreadSleepInterval = ConfigurationManager.AppSettings["ThreadSleepInterval"];

		public static readonly string AllowFirstFewWordsForActivityType = ConfigurationManager.AppSettings["AllowFirstFewWordsForActivityType"];

		public static readonly string AllowFirstFewLinesForActivityType = ConfigurationManager.AppSettings["AllowFirstFewLinessForActivityType"];

		public static readonly string DatePattern = @"((^(?:(?:(?:(?:(?:[1-9]\d)(?:0[48]|[2468][048]|[13579][26])|(?:(?:[2468][048]|[13579][26])00))(\/|-|\.)(?:0?2\1(?:29)))|(?:(?:[1-9]\d{3})(\/|-|\.)(?:(?:(?:0?[13578]|1[02])\2(?:31))|(?:(?:0?[13-9]|1[0-2])\2(?:29|30))|(?:(?:0?[1-9])|(?:1[0-2]))\2(?:0?[1-9]|1\d|2[0-8])))))$)|((?:\d{1,2}[- ,./]*)(?:Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)[a-z]*[- ,./]*\d{4})|((?:Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)[a-z]*[ ,./-]*\d{1,2}[ ,./-]*\d{4})|((?:\d{1,2}[/-]\d{1,2}[/-](?:\d{4}|\d{2})))|(?:(((Jan(uary)?|Ma(r(ch)?|y)|Jul(y)?|Aug(ust)?|Oct(ober)?|Dec(ember)?)\ 31(st)?)|((Jan(uary)?|Ma(r(ch)?|y)|Apr(il)?|Ju((ly?)|(ne?))|Aug(ust)?|Oct(ober)?|(Sept|Nov|Dec)(ember)?)\ (0?[1-9]|([12]\d)|30))(st|nd|rd|th)?|(Feb(ruary)?\ (0?[1-9]|1\d|2[0-8]|(29(th)?(?=,\ ((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00)))))(st|nd|rd|th)?))\,\ ((1[6-9]|[2-9]\d)\d{2})))";

		public static readonly bool saveChangesSuccessful = true;

		public static readonly bool saveChangesNotSuccessful = false;

		#region AWS

		public static readonly string BucketName = ConfigurationManager.AppSettings["BucketName"];

		public static readonly string AWSAccessKey = ConfigurationManager.AppSettings["AWSAccessKey"];

		public static readonly string StoringURL = ConfigurationManager.AppSettings["StoringAWSURL"];

		#endregion AWS

		public static string RemoveHeaderFooterNavBarFromHtml(string htmlContent)
		{
			HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
			doc.LoadHtml(htmlContent);
			try
			{
				if (doc.DocumentNode.SelectNodes("//aside") != null)
					doc.DocumentNode.SelectNodes("//aside").Where(x => x.GetAttributeValue("class", "").Contains("footer-nav")).ToList().ForEach(x => { x.Remove(); });

				if (doc.DocumentNode.SelectNodes("//ul") != null)
					doc.DocumentNode.SelectNodes("//ul").Where(x => x.GetAttributeValue("class", "").Contains("footer-nav")).ToList().ForEach(x => { x.Remove(); });

				if (doc.DocumentNode.SelectNodes("//nav") != null)
					doc.DocumentNode.SelectNodes("//nav").ToList().ForEach(x => { x.Remove(); });

				if (doc.DocumentNode.SelectNodes("//script") != null)
					doc.DocumentNode.SelectNodes("//script").ToList().ForEach(x => { x.Remove(); });

				if (doc.DocumentNode.SelectNodes("//noscript") != null)
					doc.DocumentNode.SelectNodes("//noscript").ToList().ForEach(x => { x.Remove(); });
			}
			finally
      {

      }
			return doc.DocumentNode.OuterHtml;
		}

		public static List<string> DateFormats()
		{
			return new List<string>(){                    "yyyy/MM/dd",
																										"yyyy.MM.dd",
																										"yyyy-MM-dd",
																										"yyyy-MMM-dd",
																										"yyyy/MMM/dd",
																										"d/M/yy",
																										"dd/MM/yyyy",
																										"dd/MMM/yyyy",
																										"d/MM/yyyy",
																										"d/M/yyyy",
																										"d.M.yy",
																										"dd.MM.yyyy",
																										"d.MM.yyyy",
																										"d.M.yyyy",
																										"d-M-yy",
																										"dd-MM-yyyy",
																										"dd-MMM-yyyy",
																										"d-MM-yyyy",
																										"d-M-yyyy",
																										"M/d/yy",
																										"M/dd/yy",
																										"M/dd/yyyy",
																										"MM/dd/yyyy",
																										"M-d-yy",
																										"MM-dd-yyyy",
																										"MMM-dd-yyyy",
																										"MMM/dd/yyyy",
																										"MMM dd yyyy",
																										"MMM dd yy",
																										"d MMM yy",
																										"d MMM yyyy",
																										"d MMMM yy",
																										"d MMMM yyyy",
																										"dd MMM yy",
																										"dd MMM yyyy",
																										"dd MMM, yyyy",
																										"dd MMM., yyyy",
																										"dd MMMM yyyy",
																										"dd MMMM, yyyy",
																										"MMMM dd,yyyy",
																										"MMMM d, yyyy",
																										"MMMM dd, yyyy",
																										"MMM. d, yyyy",
																										"MMM. dd, yyyy",
																										"MMM dd, yyyy",
																										"yyyy MMM, dd",
																										"yyyy, MMM dd"
                                                    //,"MMMddyyyy",
                                                    //"ddMMMyyyy",
                                                    //"yyyyMMMdd",
                                                    //"yyyyMMdd",
                                                    //"MMddyyyy",
                                                    //"ddMMyyyy"
                                                  };
		}

		/// <summary>
		/// Assembly Version
		/// </summary>
		public static string AssemblyVersion
		{
			get
			{
				if (string.IsNullOrEmpty(_assemblyVersion))
				{
					_assemblyVersion = GetAssemblyVersion();
				}
				return _assemblyVersion;
			}
		}

		#region Enums

		/// <summary>
		/// Activation Status
		/// </summary>
		public enum Activation
		{
			/// <summary>
			/// Profile Tag
			/// </summary>
			PROFILE,

			/// <summary>
			/// Password Tag
			/// </summary>
			PASSWORD
		}

		/// <summary>
		/// Role Status
		/// </summary>
		public enum Role
		{
			/// <summary>
			/// Super Admin Tag
			/// </summary>
			SUPERADMIN,

			/// <summary>
			/// Admin Tag
			/// </summary>
			ADMIN,

			/// <summary>
			/// Advertiser Tag
			/// </summary>
			ADVERTISER,

			/// <summary>
			/// Consumer Tag
			/// </summary>
			CONSUMER,

			/// <summary>
			/// Sponsor Tag
			/// </summary>
			SPONSOR,

			/// <summary>
			/// Print Service Provider Tag
			/// </summary>
			PRINTSERVICEPROVIDER,

			/// <summary>
			/// Person Tag
			/// </summary>
			PERSON
		}

		/// <summary>
		/// Application Types
		/// </summary>
		public enum ApplicationTypes
		{
			/// <summary>
			/// JavaScript Tag
			/// </summary>
			JavaScript = 0,

			/// <summary>
			/// Native Confidential Tag
			/// </summary>
			NativeConfidential = 1
		}

		/// <summary>
		/// Application Client
		/// </summary>
		public enum ApplicationClient
		{
			/// <summary>
			/// MLD Back Office Tag
			/// </summary>
			MLDBACKOFFICE,

			/// <summary>
			/// MLD Mobile App
			/// </summary>
			MLDMOBILEAPP,

			/// <summary>
			/// MLD Consumer
			/// </summary>
			MLDCONSUMER
		}

		/// <summary>
		/// Order Status
		/// </summary>
		public enum OrderStatus
		{
			/// <summary>
			/// New Order
			/// </summary>
			NEWORDER = 1,

			/// <summary>
			/// In Process Order
			/// </summary>
			INPROCESS = 2,

			/// <summary>
			/// In Printing Order
			/// </summary>
			INPRINTING = 3,

			/// <summary>
			/// Cancelled Order
			/// </summary>
			CANCELLED = 4,

			/// <summary>
			/// Printed Order
			/// </summary>
			PRINTED = 5,

			/// <summary>
			/// Shipped Order
			/// </summary>
			SHIPPED = 6,

			/// <summary>
			/// Completed Order
			/// </summary>
			COMPLETED = 7,

			/// <summary>
			/// Rejected Order
			/// </summary>
			REJECTED = 8,

			/// <summary>
			/// The sent for print
			/// </summary>
			SENTFORPRINT = 9
		}

		/// <summary>
		/// Link Status
		/// </summary>
		public enum LinkStatus
		{
			/// <summary>
			/// Valid Status
			/// </summary>
			Valid = 1,

			/// <summary>
			/// InValid Status
			/// </summary>
			Invalid = 2,

			/// <summary>
			/// Expired Status
			/// </summary>
			Expired = 3,

			/// <summary>
			/// Used Status
			/// </summary>
			Used = 4,

			/// <summary>
			/// Email In Use Status
			/// </summary>
			EmailInUse = 5
		}

		/// <summary>
		/// Background Process Status
		/// </summary>
		public enum BackgroundProcessStatus
		{
			/// <summary>
			/// In Progress Status
			/// </summary>
			INPROGRESS = 1,

			/// <summary>
			/// Completed Status
			/// </summary>
			COMPLETED = 2,

			/// <summary>
			/// Failed Status
			/// </summary>
			FAILED = 3,

			/// <summary>
			/// The questionable
			/// </summary>
			QUESTIONABLE = 4,

			/// <summary>
			/// The manual process
			/// </summary>
			MANUALPROCESS = 5
		}

		/// <summary>
		/// Order Delete Status
		/// </summary>
		public enum OrderDeleteStatus
		{
			/// <summary>
			/// Success Status
			/// </summary>
			SUCCESS = 1,

			/// <summary>
			/// Validation Status
			/// </summary>
			VALIDATION = 2,

			/// <summary>
			/// Failed Status
			/// </summary>
			FAILED = 3
		}

		public enum EmailParameters
		{
			/// <summary>
			/// Subjects
			/// </summary>
			SUBJECT,

			/// <summary>
			/// Email Body
			/// </summary>
			BODY
		}

		public enum ApplicationType
		{
			/// <summary>
			/// Javascript
			/// </summary>
			JavaScript = 0,

			/// <summary>
			/// NativeConfidential
			/// </summary>
			NativeConfidential = 1
		};

		public enum EMailTemplateName
		{
			/// <summary>
			/// Registration Template
			/// </summary>
			REGISTRATION_TEMPLATE,

			/// <summary>
			/// Forgot Password Templates
			/// </summary>
			FORGOTPASSWORD_TEMPLATE,

			/// <summary>
			/// Report Template
			/// </summary>
			REPORT_TEMPLATE
		}

		public enum WebProcessNames
		{
			/// <summary>
			/// Lexicon Process
			/// </summary>
			LEXICONPROCESS,
		}

		public enum ScrappingTypes
		{
			/// <summary>
			/// By Hard Coded
			/// </summary>
			HARDCODED,

			/// <summary>
			/// By Activity Type
			/// </summary>
			BY_ACTIVITY_TYPE,

			/// <summary>
			/// By Phrase
			/// </summary>
			BY_PHRASE,

			/// <summary>
			/// By Noun + Verb
			/// </summary>
			BY_NOUN_VERB,

			/// <summary>
			/// By DYNAMIC
			/// </summary>
			DYNAMIC
		}

		public enum DdlFrequency
		{
			Daily = 2,
			Weekly = 3
		}

		public enum WebSiteCategory
		{
			RSSFeeds = 11,
			DynamicContent = 6,
			ClickThroughPages = 7,
			PDFDocument = 8,
			CustomAPI = 9,
			PDFDynamicContent = 10
		}

		public enum WebSiteType
		{
			MediaSector = 1,
			OfficialSector = 2
		}

		public enum WebSiteTypes
		{
			MediaSector = 100,
			OfficialSector = 200
		}

		public enum ProcessType
		{
			WebCrawlerService = 1,
			ContentLoaderService = 2,
			HtmlPages = 3,
			RSSFeeds = 4,
			Documents = 5,
			ScraperActivity = 6,
			PDFGeneration = 7,
			CustomAPI = 8
		}

		public enum PageTypes
		{
			Official = 0,
			PDF = 1,
			Media = 2,
			RSS = 3,
			API = 4
		}

		public enum SqsTypes
		{
			EmailGeneration = 2
		}

		public enum Sectors
		{
			FT_SECTOR
		}

		public enum EmailServiceStatus
		{
			PENDING,
			FAILED,
			EXPIRED,
			SUCCESS,
			NA
		}

		public enum ThomsonReutersApiStatus
		{
			Expired_authentication_token
		}

		public enum InstitutionTypes
		{
			Multilateral,
			Legislator,
			Regulator,
			Data_Protection_Regulator,
			Economic_Policy
		}

		#endregion Enums

		public static string GetPathForSetPassword
		{
			get
			{
				return "Common/Common?activationLink=";
			}
		}

		public static int ScheduleInterval
		{
			get
			{
				return Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ServiceDuration"]) * 60;
			}
		}

		/// <summary>
		/// Get Assembly Version
		/// </summary>
		/// <returns>Assembly Version</returns>
		private static string GetAssemblyVersion()
		{
			System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
			FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
			string version = fvi.FileVersion;
			return version;
		}

		public static string GuidString()
		{
			return Guid.NewGuid().ToString("N");
		}

		public static string GetHash(this string input)
		{
			HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();

			byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(input);

			byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);

			return Convert.ToBase64String(byteHash);
		}

		#region Encrypt - Decrypt

		/// <summary>
		/// Encryption of String
		/// </summary>
		/// <summary>
		/// get the encrypted data of given string value
		/// </summary>
		/// <param name="plainText">data which we want to encrypt</param>
		/// <returns>encrypted data</returns>
		public static string ToEncrypt(this object plainText)
		{
			string pText = plainText.ToString();
			string encrptText = string.Empty;
			try
			{
				if (!string.IsNullOrEmpty(pText))
				{
					Aes des = CreateAES(EncryptionKey);
					ICryptoTransform ct = des.CreateEncryptor();
					byte[] input = Encoding.Unicode.GetBytes(pText);
					encrptText = Convert.ToBase64String(ct.TransformFinalBlock(input, 0, input.Length));
					encrptText = encrptText.Replace("=", string.Empty).Replace('+', '-').Replace('/', '_');
				}
			}
			catch (Exception)
			{
				throw;
			}

			return encrptText;
		}

		/// <summary>
		/// get the decrypted data of given encrypted text
		/// </summary>
		/// <param name="cypherText">data which we want to decrypt</param>
		/// <returns>decrypted data</returns>
		public static string ToDecrypt(this object cypherText)
		{
			string cText = cypherText.ToString();
			string dycryptText = string.Empty;

			try
			{
				if (!string.IsNullOrEmpty(cText))
				{
					cText = cText.PadRight(cText.Length + (4 - cText.Length % 4) % 4, '=');
					cText = cText.Trim().Replace('-', '+').Replace('_', '/');
					byte[] b = Convert.FromBase64String(cText.Trim());
					Aes des = CreateAES(EncryptionKey);
					ICryptoTransform ct = des.CreateDecryptor();
					byte[] output = ct.TransformFinalBlock(b, 0, b.Length);
					dycryptText = Encoding.Unicode.GetString(output);
				}
			}
			catch (Exception ex)
			{
				throw;
			}

			return dycryptText;
		}

		/// <summary>
		/// Creates the AES.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public static Aes CreateAES(string key)
		{
			Aes des = new AesCryptoServiceProvider();
			try
			{
				MD5 md5 = new MD5CryptoServiceProvider();
				des.Key = md5.ComputeHash(Encoding.Unicode.GetBytes(key));
				des.IV = new byte[des.BlockSize / 8];
			}
			catch
			{
				throw;
			}

			return des;
		}

		#endregion Encrypt - Decrypt

		public static string GetRandomPasswordSalt()
		{
			var rng = new RNGCryptoServiceProvider();
			var buffer = new byte[32];
			rng.GetBytes(buffer);
			return Convert.ToBase64String(buffer);
		}

		public static string GetRandomPassword()
		{
			Random r = new Random();
			string newPassword = System.Web.Security.Membership.GeneratePassword(8, 0);
			newPassword = Regex.Replace(newPassword, @"[^a-zA-Z0-9]", m => ((char)r.Next(65, 90)).ToString());
			return newPassword;
		}

		public static string GetPasswordHash(string password, string salt)
		{
			string saltAndPassword = String.Concat(password, salt);
			MD5 algorithm = MD5.Create();
			byte[] data = algorithm.ComputeHash(Encoding.UTF8.GetBytes(saltAndPassword));

			StringBuilder mdstring = new StringBuilder();

			for (int i = 0; i < data.Length; i++)
			{
				mdstring.Append(data[i].ToString("x2").ToUpperInvariant());
			}

			return Convert.ToString(mdstring);
		}

		public static IQueryable<T> ModifyList<T>(this IQueryable<T> query, GridParameters input, out int totalRecord)
		{
			query = query.FullTextWhereSearch(input);

			if (input.Sort != null && input.Sort.Count > 0)
			{
				var sortResult = input.Sort.FirstOrDefault();
				bool isReverse = sortResult.Dir == "asc" ? Helper.saveChangesNotSuccessful : Helper.saveChangesSuccessful;
				query = query.CustomOrderBy(sortResult.Field, isReverse);
			}

			totalRecord = query.Count();

			if (input.Skip == 0 && input.Take == 0)
			{
				return query;
			}
			else
			{
				return query.Skip(input.Skip).Take(input.Take);
			}
		}

		public static DateTime GetCurrentDateTime()
		{
			return DateTime.Now;
			////return DateTime.UtcNow;
		}

		public static DateTime GetCurrentDate()
		{
			return DateTime.Today;
			////return DateTime.UtcNow;
		}

		public static List<ChartColors> GetProprietaryColorList()
		{
			List<ChartColors> chartColors = new List<ChartColors>() {
				new ChartColors()
				{
					ProprietaryName = "Rhetoric",
					ProprietaryColor = "#0070C0"
				},
				new ChartColors()
				{
					ProprietaryName = "Action",
					ProprietaryColor = "#00B050"
				},
				new ChartColors()
				{
					ProprietaryName = "Judicial",
					ProprietaryColor = "#7030A0"
				},
				new ChartColors()
				{
					ProprietaryName = "Data",
					ProprietaryColor = "#FFC000"
				},
				new ChartColors()
				{
					ProprietaryName = "Leaks",
					ProprietaryColor = "#C00000"
				}
			};
			return chartColors;
		}

		public static string GetCurrentFormatedDateTime()
		{
			return DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
			////return DateTime.UtcNow;
		}

		public static string ToFormatedDateTime(this DateTime? date)
		{
			return ((DateTime)date).ToString("dd-MM-yyyy hh:mm:ss");
			////return DateTime.UtcNow;
		}

		public static DateTime? FromFormatedDateTime(string input, string format = "MM/dd/yyyy HH:mm")
		{
			DateTime output;
			DateTime.TryParseExact(input, format, System.Globalization.CultureInfo.InvariantCulture,
			DateTimeStyles.None, out output);
			return output;
		}

		public static string ShowScrapperName()
		{
			return "Scrapper";
		}

		public static DateTime SubstarctDaysFromCurrentDate(int days)
		{
			return DateTime.UtcNow.AddDays(days);
		}

		public static DateTime GetSystemCurrentDateTime()
		{
			return DateTime.Now;
		}

		public static string GetJsonDateWithTimeZone(DateTime date, double hours)
		{
			////TimeSpan span = date.AddHours(hours).TimeOfDay;
			var settings = new JsonSerializerSettings { DateFormatString = "yyyy-MM-ddThh:mm:ssZ" };
			var json = JsonConvert.SerializeObject(date.AddHours(hours), settings);
			return json;
		}

		public static string GetSystemIPAddress()
		{
			string ipAddress = string.Empty;
			string hostName = System.Net.Dns.GetHostName();

			for (int i = 0; i <= Dns.GetHostEntry(hostName).AddressList.Length - 1; i++)
			{
				if (!Dns.GetHostEntry(hostName).AddressList[i].IsIPv6LinkLocal)
				{
					ipAddress = Dns.GetHostEntry(hostName).AddressList[i].ToString();
				}
			}

			return ipAddress;
		}

		public static string ToSepratedString<T>(this IList<T> input, string seprator)
		{
			string output = string.Empty;
			try
			{
				var inputArray = input.ToArray();
				output = string.Join(seprator, inputArray);
			}
			catch
			{
				////output = string.Empty; ////"Error in converting list to string.";
				throw;
			}
			return output;
		}

		#region Linq Extention Methods

		public static IQueryable<T> RetrieveImageList<T>(this IQueryable<T> query, int start, int end, out int totalCount)
		{
			totalCount = query.Count();
			return query.Skip(start).Take(end);
		}

		public static IQueryable<T> WhereSearch<T>(this IQueryable<T> query, string searchField, string searchTerm)
		{
			if (string.IsNullOrEmpty(searchField) || string.IsNullOrEmpty(searchTerm))
				return query;

			MethodInfo methodSearch = null;
			ConstantExpression constantExpression = null;
			var parameterType = Expression.Parameter(typeof(T), "type");
			var propertyExpressionValue = Expression.Property(parameterType, searchField);

			if (propertyExpressionValue.Type.Name.ToLower() == "string")
			{
				methodSearch = typeof(string).GetMethod("Contains", new[] { typeof(string) });
				constantExpression = Expression.Constant(searchTerm, typeof(string));
			}
			else if (propertyExpressionValue.Type.Name.ToLower() == "int32")
			{
				methodSearch = typeof(int).GetMethod("Equals", new[] { typeof(int) });
				constantExpression = Expression.Constant(searchTerm.ToInt32(), typeof(int));
			}
			else if (propertyExpressionValue.Type.Name.ToLower() == "int64")
			{
				methodSearch = typeof(long).GetMethod("Equals", new[] { typeof(long) });
				constantExpression = Expression.Constant(searchTerm.ToInt64(), typeof(long));
			}
			else if (propertyExpressionValue.Type.Name.ToLower() == "boolean")
			{
				methodSearch = typeof(bool).GetMethod("Equals", new[] { typeof(bool) });
				constantExpression = Expression.Constant(searchTerm.ToBool(), typeof(bool));
			}

			var containsExpression = Expression.Call(propertyExpressionValue, methodSearch, constantExpression);
			return query.Where(Expression.Lambda<Func<T, bool>>(containsExpression, parameterType));
		}

		public static IQueryable<T> FullTextWhereSearch<T>(this IQueryable<T> query, GridParameters gridParameters)
		{
			if (gridParameters.Filter == null || gridParameters.Filter.Filters == null || !gridParameters.Filter.Filters.Any())
				return query;

			var mainParameter = Expression.Parameter(typeof(T), "type");
			Expression mainExpression = null;

			foreach (var searchField in gridParameters.Filter.Filters)
			{
				MethodInfo method = null;
				ConstantExpression someValue = null;
				var propertyExpression = Expression.Property(mainParameter, searchField.Field);
				string searchTerm = searchField.Value.ToString();
				if (propertyExpression.Type.Name.ToLower() == "string")
				{
					method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
					someValue = Expression.Constant(searchTerm, typeof(string));
				}
				else if (propertyExpression.Type.Name.ToLower() == "int32")
				{
					method = typeof(int).GetMethod("Equals", new[] { typeof(int) });
					someValue = Expression.Constant(searchTerm.ToInt32(), typeof(int));
				}
				else if (propertyExpression.Type.Name.ToLower() == "int64")
				{
					method = typeof(long).GetMethod("Equals", new[] { typeof(long) });
					someValue = Expression.Constant(searchTerm.ToInt64(), typeof(long));
				}
				else if ((propertyExpression.Type.Name.ToLower() == "boolean") && (searchTerm.Contains("true") || searchTerm.Contains("false")))
				{
					method = typeof(bool).GetMethod("Equals", new[] { typeof(bool) });
					someValue = Expression.Constant(searchTerm.ToBool(), typeof(bool));
				}
				if (someValue != null)
				{
					var callContainsMethod = Expression.Call(propertyExpression, method, someValue);

					if (mainExpression == null)
					{
						mainExpression = callContainsMethod;
					}
					else
					{
						mainExpression = Expression.And(mainExpression, callContainsMethod);
					}
				}
			}

			MethodCallExpression whereCallExpression = Expression.Call(
																														typeof(Queryable),
																														"Where",
																														new Type[] { query.ElementType },
																														query.Expression,
																														Expression.Lambda<Func<T, bool>>(mainExpression, new ParameterExpression[] { mainParameter }));

			// Create an executable query from the expression tree.
			IQueryable<T> results = query.Provider.CreateQuery<T>(whereCallExpression);

			return results;
		}

		public static IOrderedQueryable<T> CustomOrderBy<T>(this IQueryable<T> query, string sortType, bool? sortReverse)
		{
			string orderType = sortReverse == true ? "OrderByDescending" : "OrderBy";

			var parameter = Expression.Parameter(typeof(T), "type");
			var propertyExpression = Expression.Property(parameter, sortType);
			var propertyType = propertyExpression.Type;
			var lambda = Expression.Lambda(propertyExpression, new[] { parameter });

			return typeof(Queryable).GetMethods()
															.Where(m => m.Name == orderType && m.GetParameters().Length == 2)
															.Single()
															.MakeGenericMethod(new[] { typeof(T), propertyType })
															.Invoke(null, new object[] { query, lambda }) as IOrderedQueryable<T>;
		}

		/// <summary>
		/// Not the null or empty. Null and any condition combined
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source">The source.</param>
		/// <returns>Boolean Flag</returns>
		public static bool NotNullOrEmpty<T>(this IEnumerable<T> source)
		{
			return source != null && source.Any();
		}

		/// <summary>
		/// Gets the batch from list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="query">The query.</param>
		/// <param name="start">The start.</param>
		/// <param name="length">The length.</param>
		/// <returns>Batch List</returns>
		public static IEnumerable<T> GetBatchFromList<T>(this List<T> query, int start, int length)
		{
			if (length == -1)
			{
				return query;
			}
			else
			{
				return query.Skip(start * length).Take(length);
			}
		}

		#endregion Linq Extention Methods

		#region To View Model

		public static ApiClient ToViewModel(this apiclient input)
		{
			if (input == null)
				return null;

			var result = new ApiClient()
			{
				ID = input.Id,
				SecretHash = input.SecretHash,
				EmailId = input.Name,
				ApplicationType = input.ApplicationType,
				IsActive = input.IsActive,
				RefreshTokenLifeTime = input.RefreshTokenLifeTime,
				AllowedOrigin = input.AllowedOrigin,
			};

			return result;
		}

		public static RefreshToken ToViewModel(this refreshtoken input)
		{
			if (input == null)
				return null;

			var result = new RefreshToken()
			{
				ID = input.Id,
				UserID = input.UserId,
				ClientID = input.ClientId,
				IssuedDateTime = input.IssuedDateTime,
				ExpiresDateTime = input.ExpiresDateTime,
				ProtectedTicket = input.ProtectedKey
			};

			return result;
		}

		#endregion To View Model

		#region To DB Model

		public static refreshtoken ToDBModel(this RefreshToken input)
		{
			if (input == null)
				return null;

			var result = new refreshtoken()
			{
				Id = input.ID,
				UserId = input.UserID,
				ClientId = input.ClientID,
				IssuedDateTime = input.IssuedDateTime,
				ExpiresDateTime = input.ExpiresDateTime,
				ProtectedKey = input.ProtectedTicket
			};

			return result;
		}

		public static string RemoveSpecialCharacters(string str)
		{
			return Regex.Replace(str, "[^0-9a-zA-Z]+", "", RegexOptions.Compiled);
		}

		public static string RemoveSpecialCharacterswithSpace(string str)
		{
			return Regex.Replace(str, "[^0-9a-zA-Z]+", " ", RegexOptions.Compiled);
		}

		public static string GetEmailTemplate(EMailTemplateName emailTemplateName)
		{
			string emailBody = string.Empty;
			switch (emailTemplateName)
			{
				case EMailTemplateName.REGISTRATION_TEMPLATE:
					emailBody = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "", "ConnectionString.txt"));
					break;
			}

			return emailBody;
		}

		public static lexiconissuelinker ToDBModel(this string input, int lexiconId = 0)
		{
			if (input == null)
				return null;

			var result = new lexiconissuelinker()
			{
				Linkers = input,
				LexiconIssueId = lexiconId
			};

			return result;
		}

		public static relatedactivitytypes ToDBModelForActivityType(this string input, int activityTypeId = 0)
		{
			if (input == null)
				return null;

			var result = new relatedactivitytypes()
			{
				RelatedActvity = input,
				ActivityTypeId = activityTypeId
			};

			return result;
		}

		public static List<lexiconissuelinker> ToDBModel(this IEnumerable<string> input, int lexiconId = 0)
		{
			if (input == null)
				return null;

			var result = input.ToList().Select(x => x.ToDBModel(lexiconId)).ToList();

			return result;
		}

		public static List<relatedactivitytypes> ToDBModelForActivityType(this IEnumerable<string> input, int activityTypeId = 0)
		{
			if (input == null)
				return null;

			var result = input.ToList().Select(x => x.ToDBModelForActivityType(activityTypeId)).ToList();

			return result;
		}

		#endregion To DB Model

		#region Function to Get PDF and RSS Feeds Pages

		public static string RemoveUnWantedHtmlSource(string pageContent)
		{
			string finalContent = Regex.Replace(pageContent, "/\t+/g", "");
			finalContent = Regex.Replace(finalContent, "/(\r\n)+|\r+|\n+|\t+/i", "");

			finalContent = Regex.Replace(finalContent, "s/^\\s+//", "");
			finalContent = Regex.Replace(finalContent, "s/\\s+$//", "");

			finalContent = Regex.Replace(finalContent, "</a>\\s*<a", "</a><a");
			finalContent = Regex.Replace(finalContent, "</a>\\s*/\\s*<a", "</a><a");

			finalContent = Regex.Replace(finalContent, "<input.*?>", "");

			finalContent = finalContent.Replace("'", "\"");

			return finalContent;
		}

		public static string RemoveUnWantedAnchorTags(string pageContent)
		{
			string finalContent = pageContent.Replace("  ", " ");

			finalContent = Regex.Replace(finalContent, "<nav.*?>(.|\n)*?</nav>", "");
			finalContent = Regex.Replace(finalContent, "<style.*?>(.|\n)*?</style>", "");

			finalContent = Regex.Replace(finalContent, "<script.*?>(.|\n)*?</script>", "");
			finalContent = Regex.Replace(finalContent, "<script>(.|\n)*?</script>", "");

			finalContent = Regex.Replace(finalContent, "<noscript>(.|\n)*?</noscript>", "");
			finalContent = Regex.Replace(finalContent, "<video.*?>(.|\n)*?</video>", "");

			finalContent = Regex.Replace(finalContent, "<meta.*?>(.|\n)*?</meta>", "");
			finalContent = Regex.Replace(finalContent, "<map.*?>(.|\n)*?</map>", "");

			finalContent = Regex.Replace(finalContent, "<embed.*?>(.|\n)*?</embed>", "");

			finalContent = Regex.Replace(finalContent, "<button.*?>(.|\n)*?</button>", "");
			finalContent = Regex.Replace(finalContent, "<!--(.|\n)*?-->", "");

			finalContent = Regex.Replace(finalContent, "<div\\s*class=\"footer_menu\">(.|\n)*?</div>", "");
			finalContent = Regex.Replace(finalContent, "<div\\s*id=\"Footer\">(.|\n)*?</div>", "");
			finalContent = Regex.Replace(finalContent, "<div\\s*id=\"footer\"\\s*class=\"footer\">(.|\n)*?</div>", "");

			finalContent = Regex.Replace(finalContent, "<div\\s*id=\"footer\">(.|\n)*?</div>", "");
			finalContent = Regex.Replace(finalContent, "<div\\s*class=\"footer\">(.|\n)*?</div>", "");

			finalContent = Regex.Replace(finalContent, "<div\\s*class=\"l-sidebar\">(.|\n)*?</div>", "");
			finalContent = Regex.Replace(finalContent, "<div\\s*class=\"header\">(.|\n)*?</div>", "");
			finalContent = Regex.Replace(finalContent, "<div\\s*id=\"header\">(.|\n)*?</div>", "");

			finalContent = Regex.Replace(finalContent, "<div\\s*class=\"leftMenuBox\">(.|\n)*?</div>", "");

			finalContent = Regex.Replace(finalContent, "<div\\s*id=\"footer\".*?>(.|\n)*?</div>", "");

			finalContent = Regex.Replace(finalContent, "<div\\sid=\"seo\"\\sclass=\"clearfix\">(.|\n)*?</div>", "");

			finalContent = Regex.Replace(finalContent, "<div\\s*class=\"menu\">(.|\n)*?</div>", "");
			finalContent = Regex.Replace(finalContent, "<div\\s*class=\"bg_header\">(.|\n)*?</div>", "");

			finalContent = Regex.Replace(finalContent, "<div\\s*id=\"footer__sidebars\".*?>(.|\n)*?</div>", "");
			finalContent = Regex.Replace(finalContent, "<div\\s*id=\"orb-footer\".*?>(.|\n)*?</div>", "");

			finalContent = Regex.Replace(finalContent, "<div\\s*id=\"FooterWrapper\">(.|\n)*?</div>", "");
			finalContent = Regex.Replace(finalContent, "<div\\s*id=\"HeaderContainer\">(.|\n)*?</div>", "");

			finalContent = Regex.Replace(finalContent, "<div\\s*class=\"navTeaserGroup\">(.|\n)*?</div>", "");
			finalContent = Regex.Replace(finalContent, "<div\\s*class=\"navTeaser\">(.|\n)*?</div>", "");

			// Removing below tag from the official sector site http://www.treasury.gov.my/index.php?lang=en //
			finalContent = Regex.Replace(finalContent, "<span\\s*class=\"label\\s*label-warning\">(.|\n)*?</span>", "");

			return finalContent;
		}

		public static string RemoveHeaderFooterTags(string pageContent, LoaderLinkQueue siteLink)
		{
			string finalContent = pageContent;

			finalContent = Regex.Replace(finalContent, "<fieldset.*?>(.|\n)*?</fieldset>", "");

			if (siteLink.SiteURL != "https://www.politico.com/tag/us-china-relations")
			{
				finalContent = Regex.Replace(finalContent, "<aside.*?>(.|\n)*?</aside>", "");
			}

			return finalContent;
		}

		public static string RemoveAllHtmlContents(string pageContent)
		{
			pageContent = Regex.Replace(pageContent, "<[^>]+>", "");

			return pageContent;
		}

		public static List<LoaderLinkQueue> SearchPDFDocuments(LoaderLinkQueue loaderRecords, List<LoaderLinkQueue> listOfRecords)
		{
			List<LoaderLinkQueue> linkLogQueueList = new List<LoaderLinkQueue>();

			try
			{
				var pageURLDetails = listOfRecords.Find(x => x.SiteURL == loaderRecords.SiteURL);

				////System.Text.Encoding textEncoding = System.Text.Encoding.UTF8;
				string documentURL = string.Empty;

				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(loaderRecords.SiteURL);
				request.AllowAutoRedirect = true;
				request.KeepAlive = true;

				bool isHttps = false;

				if (loaderRecords.SiteURL.IndexOf("https") != -1)
				{
					isHttps = true;
				}

				request.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.01; Windows NT 5.0)";

				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
																			 SecurityProtocolType.Tls11 |
																			 SecurityProtocolType.Ssl3 |
																			 SecurityProtocolType.Tls12;

				using (var response = (HttpWebResponse)request.GetResponse())
				{
					if (response != null)
					{
						Stream stream = response.GetResponseStream();
						stream.ReadTimeout = -1;
						StreamReader reader = new StreamReader(stream);
						string pageContent = reader.ReadToEnd();

						if (pageURLDetails == null || loaderRecords.pageContent != pageContent)
						{
							string regExValue = "<a.*?href=\"(?'url'[^\"]+\\.(bbs|pdf|PDF)(.|\n)*?)";

							LoaderLinkQueue linkQueue;

							Regex regex = new Regex(regExValue);

							MatchCollection matches = regex.Matches(pageContent);

							if (matches.Count > 0)
							{
								foreach (Match match in matches)
								{
									try
									{
										////StringBuilder text = new StringBuilder();
										documentURL = match.Groups["url"].Value;

										if (documentURL.LastIndexOf("\"") != -1)
										{
											documentURL = documentURL.Remove(documentURL.LastIndexOf("\""));
										}

										string finalPageURL = string.Empty;

										Uri pageURL = new Uri(loaderRecords.SiteURL);
										string host = pageURL.Host;

										if (documentURL.IndexOf("http") == -1)
										{
											if (isHttps)
											{
												if (documentURL.IndexOf(host) == -1)
												{
													if (host.IndexOf("www.bundesbank.de") != -1)
													{
														documentURL = "http://" + host + "/" + documentURL + "?__blob=publicationFile";
													}
													if (host.IndexOf("www.sfc.hk") != -1)
													{
														documentURL = "http://" + host + "/web/EN/" + documentURL;
													}
													else
													{
														documentURL = "http://" + host + "/" + documentURL;
													}
												}
												else
												{
													documentURL = "https://" + documentURL;
												}
											}
											else
											{
												if (documentURL.IndexOf(host) == -1)
												{
													documentURL = "http://" + host + "/" + documentURL;
												}
												else
												{
													documentURL = finalPageURL + documentURL;
												}
											}
										}

										if (isHttps && documentURL.IndexOf("http://") != -1)
										{
											documentURL = documentURL.Replace("http://", "https://");
										}

										documentURL = documentURL.Replace("../../../", "/");
										documentURL = documentURL.Replace("../", "/");
										documentURL = documentURL.Replace("..", "").Replace("&amp;", "&").Replace("target=", "").Replace("\"", "").Trim();
										documentURL = documentURL.Replace("../../../", "/");
										documentURL = documentURL.Replace("../", "/");
										documentURL = documentURL.Replace("..", "").Replace("&amp;", "&").Replace("target=", "").Replace("\"", "").Trim();

										linkQueue = new LoaderLinkQueue();

										var pageDocumentDetails = listOfRecords.Find(x => x.SiteURL == documentURL);

										if (pageDocumentDetails == null)
										{
											linkQueue.GUID = Helper.GuidString();

											linkQueue.ProcessId = loaderRecords.ProcessId;
											linkQueue.ProcessInstanceId = loaderRecords.ProcessInstanceId;

											linkQueue.SiteURL = documentURL;
											linkQueue.WebSiteId = loaderRecords.WebSiteId;

											linkQueue.InstanceName = WebSiteCategory.PDFDocument.ToString();
											linkQueue.LinkLevel = 1;

											linkLogQueueList.Add(linkQueue);
										}
									}
									catch (Exception ex)
									{
									}
								}
							}
						}
					}
				}
			}
      finally
      {

      }

			return linkLogQueueList;
		}

		#endregion Function to Get PDF and RSS Feeds Pages

		/// <summary>
		/// Get Itextsharp License Key From DB
		/// </summary>
		/// <returns>
		/// Get Itextsharp key path
		/// </returns>
		public static string GetItextSharpLicenseKeyPath()
		{
			using (var context = new BCMStrategyEntities())
			{
				var result = context.globalconfiguration.Where(x => x.Name == GlobalConfigurationKeys.ITextSharp_LincenceKey).Select(y => y.Value).FirstOrDefault();

				return result.ToString();
			}
		}

		/// <summary>
		/// Get Itextsharp License Key From DB
		/// </summary>
		/// <returns>
		/// Get Itextsharp key path
		/// </returns>
		public static string GetNumberOfChartColumns()
		{
			using (var context = new BCMStrategyEntities())
			{
				var result = context.globalconfiguration.Where(x => x.Name == GlobalConfigurationKeys.NumberOfChartColumns).Select(y => y.Value).FirstOrDefault();

				return result.ToString();
			}
		}

		public static string SerializeObjectTojson<T>(T obj)
		{
			string jsonStr = string.Empty;
			if (obj != null)
			{
				var properties = obj.GetType()
									.GetProperties().Where(a => a.PropertyType == typeof(string) || a.PropertyType == typeof(int) || a.PropertyType == typeof(bool) || a.PropertyType == typeof(decimal) || a.PropertyType == typeof(DateTime) || a.PropertyType == typeof(Nullable<int>) || a.PropertyType == typeof(Nullable<bool>) || a.PropertyType == typeof(Nullable<System.DateTime>));

				Dictionary<object, object> dict = new Dictionary<object, object>();
				List<string> propsToExclude = Helper.PropsToExcludeOnSerialization();
				properties.ToList().ForEach(property =>
				{
					if (!propsToExclude.Contains(property.Name))
					{
						var propVal = obj.GetType().GetProperty(property.Name).GetValue(obj, null);
						dict.Add(property.Name, propVal == null ? string.Empty : propVal);
					}
				});

				jsonStr = JsonConvert.SerializeObject(dict);
			}
			return jsonStr;
		}

		public static HttpWebResponse WebRequestResponse(string siteURL)
		{
			ServicePointManager.Expect100Continue = true;

			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
																	 SecurityProtocolType.Tls11 |
																	 SecurityProtocolType.Ssl3 |
																	 SecurityProtocolType.Tls12;
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(siteURL);
			request.Method = "GET";
			request.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.01; Windows NT 5.0)";
			request.KeepAlive = true;
			request.AllowAutoRedirect = true;
			////request.Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["HttpRequestTimeOut"]);
			return (HttpWebResponse)request.GetResponse();
		}

		/// <summary>
		/// Get FT API query string parameter
		/// </summary>
		/// <returns>
		/// Get FT API query string parameter
		/// </returns>
		public static string GetFTQueryStringParameter()
		{
			using (var context = new BCMStrategyEntities())
			{
				var result = context.globalconfiguration.Where(x => x.Name == GlobalConfigurationKeys.FTAPI_Querstring_Param).Select(y => y.Value).FirstOrDefault();

				return result.ToString();
			}
		}

		public static List<string> PropsToExcludeOnSerialization()
		{
			return new List<string> {
				"PasswordHash",
				"PasswordSalt",
				"CompositionBody"
			};
		}

		public static string GetInnerException(Exception ex)
		{
			if (ex == null || string.IsNullOrEmpty(ex.Message))
			{
				return string.Empty;
			}
			else
			{
				return ex.Message.ToString();
			}
		}

		public static void TestException()
		{
			throw new Exception("Path Not Found");
		}

		/// <summary>
		/// Get FT Licence Key For HeadLine News
		/// </summary>
		/// <returns>
		/// FT Licence Key For HeadLine News
		/// </returns>
		public static string GetFTHeadLineLicenceKey()
		{
			using (var context = new BCMStrategyEntities())
			{
				var result = context.globalconfiguration.Where(x => x.Name == GlobalConfigurationKeys.FT_Headline_Licence_Key).Select(y => y.Value).FirstOrDefault();

				return result.ToString();
			}
		}

		/// <summary>
		/// Get FT Licence Key For ALL News
		/// </summary>
		/// <returns>
		/// FT Licence Key For ALL News
		/// </returns>
		public static string GetFTB2BLineLicenceKey()
		{
			using (var context = new BCMStrategyEntities())
			{
				var result = context.globalconfiguration.Where(x => x.Name == GlobalConfigurationKeys.FT_B2B_All_Licence_Key).Select(y => y.Value).FirstOrDefault();

				return result.ToString();
			}
		}

		/// <summary>
		/// Get FT B2B Content URL
		/// </summary>
		/// <returns>
		/// FT B2B Content URL
		/// </returns>
		public static string GetFTB2BContentURL()
		{
			using (var context = new BCMStrategyEntities())
			{
				var result = context.globalconfiguration.Where(x => x.Name == GlobalConfigurationKeys.FT_B2B_Content_API_URL).Select(y => y.Value).FirstOrDefault();

				return result.ToString();
			}
		}

		/// <summary>
		/// Get FT HeadLine API URL
		/// </summary>
		/// <returns>
		/// FT HeadLine API URL
		/// </returns>
		public static string GetFTHeadLineURL()
		{
			using (var context = new BCMStrategyEntities())
			{
				var result = context.globalconfiguration.Where(x => x.Name == GlobalConfigurationKeys.FT_HeadLine_API_URL).Select(y => y.Value).FirstOrDefault();

				return result.ToString();
			}
		}

		public static DateTime? ExtractDate(string htmlContent)
		{
			List<StandardTagDates> dateList = new List<StandardTagDates>();
			List<string> dateTimeFormats = DateTimeFormats.find_date(htmlContent).Distinct().ToList();
			dateTimeFormats.ForEach(x =>
			{
				try
				{
					string date = DateTime.ParseExact(x.Trim().ReplaceOrdinals(), Helper.DateFormats().ToArray(), CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
					DateTime dateTime2;
					if (DateTime.TryParse(date, out dateTime2))
					{
						StandardTagDates standardTagDates = new StandardTagDates()
						{
							ActualDate = x,
							FormatedDate = dateTime2
						};
						dateList.Add(standardTagDates);
					}
				}
				catch (Exception ex)
				{
					//// log.LogError(LoggingLevel.Error, "BadRequest", "ExtractDate: Exception is thrown in Main method", ex, null);
				}
			});
			var inputDate = DateTime.Now;
			var allDates = dateList.Distinct().Where(x => x.FormatedDate <= inputDate).Distinct().OrderBy(d => d.FormatedDate).ToList();
			DateTime? closestDate = null;
			if (allDates != null)
			{
				closestDate = allDates.Count > 0 ? inputDate >= allDates.Last().FormatedDate
								 ? allDates.Last().FormatedDate
								 : inputDate <= allDates.First().FormatedDate
										 ? allDates.First().FormatedDate
										 : allDates.First(x => x.FormatedDate >= inputDate).FormatedDate : (DateTime?)null;
			}
			return closestDate;
		}

		public static DateTime GetDateOfAnyCulture(string date)
		{
			List<CultureInfo> cinfo = CultureInfo.GetCultures(CultureTypes.AllCultures & CultureTypes.NeutralCultures).ToList();
			DateTime MyDateTime = new DateTime();

			foreach (CultureInfo x in cinfo)
			{
				try
				{
					CultureInfo MyCultureInfo = new CultureInfo(x.Name);
					MyDateTime = DateTime.Parse(date, MyCultureInfo);
					break;
				}
				catch (Exception ex)
				{
          
				}
			}

			return MyDateTime;
		}

		// Get Day Month Year Regular Expression
		public static MatchCollection DayMonthYearRegEx(string pageContent, LoaderLinkQueue siteLink)
		{
			MatchCollection dateMatchCollection = null;

			Regex monthDayYearRegEx = new Regex("\\s((?'month'\\b[a-zA-Z]+\\b)[ ,./-](?'day'(\\d)?\\d)[ ,./-](?'year'((?:19|20))?\\d{2}))");

			Regex yearMonthDayRegEx = new Regex("\\s((?'year'((?:19|20))?\\d{2})[ ,./-](?'month'\\b[a-zA-Z]+\\b)[ ,./-][\\ ](?'day'(\\d)?\\d))");

			Regex dayMonthYearRegEx = new Regex("\\s((?'day'(\\d)?\\d)[ ,./-](?'month'\\b[a-zA-Z0-9_]+\\b)[ ,./-](?'year'((?:19|20))?\\d{2}))");

			if (siteLink.SiteURL.IndexOf("https://www.politico.eu") != -1)
			{
				monthDayYearRegEx = new Regex("((?'month'\\b[a-zA-Z0-9]+\\b)[ ,./-](?'day'(\\d)?\\d)[ ,./-](?'year'((?:19|20))?\\d{2}))");

				if (monthDayYearRegEx.Matches(pageContent).Count > 0)
				{
					dateMatchCollection = monthDayYearRegEx.Matches(pageContent);
				}
				else
				{
					dateMatchCollection = dayMonthYearRegEx.Matches(pageContent);
				}
			}
			else
			{
				if (monthDayYearRegEx.Matches(pageContent).Count > 0)
				{
					dateMatchCollection = monthDayYearRegEx.Matches(pageContent);
				}
				else if (yearMonthDayRegEx.Matches(pageContent).Count > 0)
				{
					dateMatchCollection = yearMonthDayRegEx.Matches(pageContent);
				}
				else if (dayMonthYearRegEx.Matches(pageContent).Count > 0)
				{
					dateMatchCollection = dayMonthYearRegEx.Matches(pageContent);
				}
			}

			return dateMatchCollection;
		}

		/// <summary>
		/// get token for Thomson reuters api
		/// </summary>
		/// <returns>saved token from database</returns>
		public static string GetTrToken()
		{
			using (var context = new BCMStrategyEntities())
			{
				string result = context.globalconfiguration.Where(x => x.Name == GlobalConfigurationKeys.TR_TOKEN).Select(y => y.Value).FirstOrDefault();
				return result;
			}
		}

		/// <summary>
		/// get thomson reuters head line url
		/// </summary>
		/// <returns>Thomson Reuters Url</returns>
		public static string GetTRTokenURL()
		{
			using (var context = new BCMStrategyEntities())
			{
				var result = context.globalconfiguration.Where(x => x.Name == GlobalConfigurationKeys.TR_GET_TOKEN_URL).Select(y => y.Value).FirstOrDefault();

				return result.ToString();
			}
		}

		public static ThomsonReutersViewModel GetTrCredentials()
		{
			ThomsonReutersViewModel model = new ThomsonReutersViewModel();
			using (var context = new BCMStrategyEntities())
			{
				var userName = context.globalconfiguration.Where(x => x.Name == GlobalConfigurationKeys.TR_USERNAME).Select(y => y.Value).FirstOrDefault();
				model.UserName = userName.ToString();

				var password = context.globalconfiguration.Where(x => x.Name == GlobalConfigurationKeys.TR_PASSWORD).Select(y => y.Value).FirstOrDefault();
				model.Password = password.ToString();
			}
			return model;
		}

		public static string GetTRContentURL()
		{
			using (var context = new BCMStrategyEntities())
			{
				var result = context.globalconfiguration.Where(x => x.Name == GlobalConfigurationKeys.TR_GET_CONTENT_URL).Select(y => y.Value).FirstOrDefault();

				return result.ToString();
			}
		}

		public static List<TopicCodeViewModel> GetTopicCodeLIst()
		{
			List<TopicCodeViewModel> listTopicCode;
			using (var context = new BCMStrategyEntities())
			{
				listTopicCode = context.topiccodes
						 .Select(x => new TopicCodeViewModel()
						 {
							 TopicCode = x.TopicCode
						 }).ToList();

				return listTopicCode;
			}
		}

		public static string GetArchivalProcessFrequency()
		{
			using (var context = new BCMStrategyEntities())
			{
				var result = context.globalconfiguration.Where(x => x.Name == GlobalConfigurationKeys.ARCHIVAL_PROCESS_FREQUENCY_DAYS).Select(y => y.Value).FirstOrDefault();

				return result.ToString();
			}
		}

		public static string GetMySQLRetentionInDays()
		{
			using (var context = new BCMStrategyEntities())
			{
				var result = context.globalconfiguration.Where(x => x.Name == GlobalConfigurationKeys.MYSQL_DATARETENTION_DAYS).Select(y => y.Value).FirstOrDefault();

				return result.ToString();
			}
		}

		public static string GetSolrRetentionInDays()
		{
			using (var context = new BCMStrategyEntities())
			{
				var result = context.globalconfiguration.Where(x => x.Name == GlobalConfigurationKeys.SOLR_DATARETENTION_DAYS).Select(y => y.Value).FirstOrDefault();

				return result.ToString();
			}
		}

		/// <summary>
		/// Get Email Generation URL of SQS AWS
		/// </summary>
		/// <returns></returns>
		public static string GetSQSEmailGeneration()
		{
			using (var context = new BCMStrategyEntities())
			{
				var result = context.globalconfiguration.Where(x => x.Name == GlobalConfigurationKeys.SQS_EmailGeneration).Select(y => y.Value).FirstOrDefault();

				return result.ToString();
			}
		}

		/// <summary>
		/// get access key for AWS SQS
		/// </summary>
		/// <returns></returns>
		public static string GetSQSAWSAccessKeyId()
		{
			using (var context = new BCMStrategyEntities())
			{
				var result = context.globalconfiguration.Where(x => x.Name == GlobalConfigurationKeys.SQS_AWSAccessKeyId).Select(y => y.Value).FirstOrDefault();

				return result.ToString();
			}
		}

		/// <summary>
		/// get access secret key for AWS SQS
		/// </summary>
		/// <returns></returns>
		public static string GetSQSAWSSecretAccessKey()
		{
			using (var context = new BCMStrategyEntities())
			{
				var result = context.globalconfiguration.Where(x => x.Name == GlobalConfigurationKeys.SQS_AWSSecretAccessKey).Select(y => y.Value).FirstOrDefault();

				return result.ToString();
			}
		}

		/// <summary>
		/// get SQS URL for content loader
		/// </summary>
		/// <returns></returns>
		public static string GetSQSContentLoader()
		{
			using (var context = new BCMStrategyEntities())
			{
				var result = context.globalconfiguration.Where(x => x.Name == GlobalConfigurationKeys.SQS_ContentLoader).Select(y => y.Value).FirstOrDefault();

				return result.ToString();
			}
		}

		/// <summary>
		/// Retrieval of Hours from Configuration for the execution of EmailGeneration process
		/// </summary>
		/// <returns></returns>
		public static int GetAutoEmailGenerationHours_FailOver()
		{
			using (var context = new BCMStrategyEntities())
			{
				var result = context.globalconfiguration.Where(x => x.Name == GlobalConfigurationKeys.Auto_EmailGeneration_Hours_FailOver).Select(y => y.Value).FirstOrDefault();

				return result.ToInt32();
			}
		}
	}
}