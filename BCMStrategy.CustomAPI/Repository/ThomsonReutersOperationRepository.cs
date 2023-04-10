using BCMStrategy.CustomAPI.Abstract;
using BCMStrategy.CustomAPI.ViewModel;
using BCMStrategy.DAL.Context;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Data.Repository.Concrete;
using BCMStrategy.Logger;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SolrNet.Exceptions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace BCMStrategy.CustomAPI.Repository
{
	public class ThomsonReutersOperationRepository : IThomsonReutersOperation
	{
		private static readonly EventLogger<ThomsonReutersOperationRepository> log = new EventLogger<ThomsonReutersOperationRepository>();

		private readonly string commonJSONPath = ConfigurationManager.AppSettings["commonJSONPath"].ToString();
		////private readonly string ValidateTakenurl = "http://rmb.reuters.com/rmd/soap/RmdService.wsdl";
		private string aPiUrl = "";
		private string aPiContentUrl = "";
		private string userName = "";
		private string password = "";
		private string Token = "";

		private readonly JToken commonFileContent;

		public ThomsonReutersOperationRepository()
		{
			try
			{
				using (JsonTextReader reader = new JsonTextReader(File.OpenText(commonJSONPath)))
				{
					commonFileContent = (JObject)JToken.ReadFrom(reader);
				}
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "ThomsonReutersOperationRepository reading common json file method", ex, null);
			}
		}
		private IProcessEvents _processEvents;

		private IProcessEvents ProcessEvents
		{
			get
			{
				if (_processEvents == null)
				{
					_processEvents = new ProcessEventsRepository();
				}

				return _processEvents;
			}
		}

		private IWebLink _webLink;

		private IWebLink WebLink
		{
			get
			{
				if (_webLink == null)
				{
					_webLink = new WebLinkRepository();
				}

				return _webLink;
			}
		}

		private IGlobalConfiguration _solrConfiguration;

		private IGlobalConfiguration SolrConfiguration
		{
			get
			{
				if (_solrConfiguration == null)
				{
					_solrConfiguration = new GlobalConfigurationRepository();
				}

				return _solrConfiguration;
			}
		}

		public void GetTRIdList(int processId, int processInstanceId)
		{
			try
			{
				ThomsonReutersViewModel thomsonReutersViewModel = Helper.GetTrCredentials();
				if (thomsonReutersViewModel != null)
				{
					userName = thomsonReutersViewModel.UserName;
					password = thomsonReutersViewModel.Password;
				}
				Token = Helper.GetTrToken();
				aPiUrl = Helper.GetTRTokenURL();
				aPiContentUrl = Helper.GetTRContentURL();

				////int isScrapperEventSaved = SaveScraperEvent(processId, processInstanceId);

				////if (isScrapperEventSaved > 0)
				////{
				string thomsonUrl = ConfigurationManager.AppSettings["TRApiUrl"];
				LoaderLinkQueue thomsonLinks = GetSimpleWebPageDetails(processId, processInstanceId).Where(w => w.SiteURL == thomsonUrl).Select(s => new LoaderLinkQueue
				{
					Id = s.Id,
					GUID = s.GUID,
					InstanceName = s.InstanceName,
					SiteURL = s.SiteURL,
					WebSiteId = s.Id,
					ProcessId = processId
				}).FirstOrDefault();

				if (thomsonLinks == null)
				{
					log.LogSimple(LoggingLevel.Information, "no loader link found");
					return;
				}

				int scanningLinkQueueId = WebLink.InsertInScanningLinkQueue(thomsonLinks);

				////Topic Code List
				List<TopicCodeViewModel> topicCodeList = Helper.GetTopicCodeLIst();
				List<ChannelViewModel> ChannelList = GetChannelList();
				List<ThomsonReutersOperationViewModel> ItemsIdList = GetItemsIdList(ChannelList, topicCodeList);
				if (ItemsIdList.Any())
				{
					List<ItemList> ItemList = GetItemList(ItemsIdList);
					List<LoaderLinkQueue> listOfRecords = GetLinkRecords();
					List<PageDetails> solrList = StoreWebSiteLinkToSOLRList(ItemList, processId, processInstanceId, listOfRecords, thomsonLinks, scanningLinkQueueId);

					if (solrList.Any())
					{
						InsertRecordToSolRDB(solrList);
					}
				}
				////  Events scraperEvents = new Events();
				////  scraperEvents.Id = isScrapperEventSaved;
				////  scraperEvents.ProcessEventId = processId;
				////  scraperEvents.EndDateTime = Helper.GetSystemCurrentDateTime();
				////  ProcessEvents.UpdateEvents(scraperEvents);
				////}
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in Repository GetTRIdList method", ex, null);
			}
		}

		////private bool ValidateToken(string token)
		////{
		////  bool isValid = false;
		////  string tokenResult = "";
		////  try
		////  {
		////    string url = string.Format(commonFileContent["TRQueryString"]["ValidateTokenURL"].ToString(), ValidateTakenurl, token);
		////    //// Get data by hitting API 
		////    HttpResponseMessage messge = HitAPI(url);
		////    string result = messge.Content.ReadAsStringAsync().Result;
		////    XmlDocument xmlDoc = new XmlDocument();
		////    xmlDoc.LoadXml(result);
		////    string xpath = "status/error";

		////    var nodes = xmlDoc.SelectNodes(xpath);

		////    foreach (XmlNode childrenNode in nodes)
		////    {
		////      tokenResult = childrenNode.FirstChild.Value;
		////    }
		////    if (tokenResult == "Expired authentication token")
		////    {
		////      isValid = false;
		////    }
		////    else isValid = true;
		////  }
		////  catch (Exception ex)
		////  {
		////    log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in ValidateToken method", ex, null);
		////  }
		////  return isValid;
		////}

		public string GetToken()
		{
			string authToken = "";
			try
			{
				string url = string.Format(commonFileContent["TRQueryString"]["TokenURL"].ToString(), aPiUrl, userName, password);
				//// Get data by hitting API 
				HttpResponseMessage messge = HitAPI(url);

				if (messge.IsSuccessStatusCode)
				{
					string result = messge.Content.ReadAsStringAsync().Result;

					JToken data = JObject.Parse(result);
					var jsonData = data["authToken"]["authToken"];
					if (jsonData != null)
					{
						authToken = data["authToken"]["authToken"].ToString();
						UpdateNewToken(authToken);
					}
				}
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in Thomsan reuters repository.GetToken() method", ex, null);
			}
			return authToken;
		}

		private static HttpResponseMessage HitAPI(string url)
		{
			System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();

			HttpResponseMessage response = new HttpResponseMessage();
			try
			{
				client.BaseAddress = new System.Uri(url);
				client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
				response = client.GetAsync(url).Result;
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in HitAPI method", ex, null);
			}
			finally
			{
				////Dispose once all HttpClient calls are complete. This is not necessary if the containing object will be disposed of; for example in this case the HttpClient instance will be disposed automatically when the application terminates so the following call is superfluous.
				client.Dispose();
			}
			return response;

		}

		private void UpdateNewToken(string authToken)
		{
			try
			{
				using (BCMStrategyEntities db = new BCMStrategyEntities())
				{
					DateTime currentTimeStamp = Helper.GetCurrentDateTime();
					var objConfigToken = db.globalconfiguration.Where(x => x.Name == Data.Abstract.ViewModels.GlobalConfigurationKeys.TR_TOKEN).FirstOrDefault();
					if (objConfigToken != null)
					{
						objConfigToken.Value = authToken;
						objConfigToken.Modified = currentTimeStamp;
					}
					db.SaveChanges();
				}
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in UpdateNewToken method", ex, null);
			}
		}

		private List<ChannelViewModel> GetChannelList()
		{
			List<ChannelViewModel> channelViewModelList = new List<ChannelViewModel>();
			try
			{
				Found:
				string formattedUrl = string.Format(commonFileContent["TRQueryString"]["ChannelURL"].ToString(), aPiContentUrl, Token);
				HttpResponseMessage messge = HitAPI(formattedUrl);
				log.LogSimple(LoggingLevel.Information, "Http response of channel is as below : \n " + messge);
				if (messge.IsSuccessStatusCode)
				{
					string result = messge.Content.ReadAsStringAsync().Result;

					XmlDocument xmlDoc = new XmlDocument();
					string myXML = result;
					xmlDoc.LoadXml(myXML);

					string xpath = "availableChannels/channelInformation";
					var nodes = xmlDoc.SelectNodes(xpath);
					ChannelViewModel model = new ChannelViewModel();
					foreach (XmlNode childrenNode in nodes)
					{
						model.ChannelCode = childrenNode.SelectSingleNode("alias").InnerText;
					}
					channelViewModelList.Add(model);
				}
				else
				{
					string messgeResult = ConvertResultXmlToString(messge.Content.ReadAsStringAsync().Result);
					if (messgeResult == Helper.ThomsonReutersApiStatus.Expired_authentication_token.ToString().Replace("_", " "))
					{
						Token = GetToken();
						goto Found;
					}
				}
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in Thomsan reuters GetChannelList method", ex, null);
			}
			return channelViewModelList;
		}


		private List<ThomsonReutersOperationViewModel> GetItemsIdList(List<ChannelViewModel> channelList, List<TopicCodeViewModel> topicCodeList)
		{
			List<ThomsonReutersOperationViewModel> itemsList = new List<ThomsonReutersOperationViewModel>();
			double timeIntervalHours = Convert.ToDouble(ConfigurationManager.AppSettings["FTAPIIntervalInHours"]);
			foreach (var itemChannel in channelList)
			{
				try
				{
					foreach (var objTopicCode in topicCodeList)
					{
						Found:
						string formattedUrl = string.Format(commonFileContent["TRQueryString"]["ItemsURL"].ToString(), aPiContentUrl, objTopicCode.TopicCode, itemChannel.ChannelCode, timeIntervalHours, Token);

						HttpResponseMessage messge = HitAPI(formattedUrl);
						log.LogSimple(LoggingLevel.Information, "Http response of retrieving Id : \n " + messge);
						if (messge.IsSuccessStatusCode)
						{
							string result = messge.Content.ReadAsStringAsync().Result;
							string xmlResult = XmlToJSON(result);
							JToken data = JObject.Parse(xmlResult);

							int recordFound = Convert.ToInt32(data["results"]["numFound"]);

							if (recordFound > 1)
							{
								var jsonDataMultiple = data["results"]["result"];
								if (jsonDataMultiple != null)
								{
									foreach (var item in jsonDataMultiple)
									{
										ThomsonReutersOperationViewModel model = new ThomsonReutersOperationViewModel();
										model.ItemsId = item["id"] != null ? item["id"].ToString() : string.Empty;
										model.PublishedDate = item["dateCreated"] != null ? item["dateCreated"].ToString() : string.Empty;

										if (!itemsList.Any(x => x.ItemsId == model.ItemsId))
										{
											itemsList.Add(model);
										}
									}
								}
							}
							else if (recordFound > 0)
							{
								ThomsonReutersOperationViewModel model = new ThomsonReutersOperationViewModel();
								model.ItemsId = data["results"]["result"]["id"].ToString();
								model.PublishedDate = data["results"]["result"]["dateCreated"].ToString();
								if (!itemsList.Any(x => x.ItemsId == model.ItemsId))
								{
									itemsList.Add(model);
								}
							}

						}
						else
						{
							string messgeResult = ConvertResultXmlToString(messge.Content.ReadAsStringAsync().Result);
							if (messgeResult == Helper.ThomsonReutersApiStatus.Expired_authentication_token.ToString().Replace("_", " "))
							{
								Token = GetToken();
								goto Found;
							}
						}
					}
				}
				catch (Exception ex)
				{
					log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in GetItemsIdList method", ex, null);
				}
			}

			return itemsList;
		}

		private List<ItemList> GetItemList(List<ThomsonReutersOperationViewModel> itemsIdList)
		{
			List<ItemList> itemLIst = new List<ItemList>();
			try
			{
				foreach (var item in itemsIdList)
				{
					Found:
					string formattedUrl = string.Format(commonFileContent["TRQueryString"]["ItemURL"].ToString(), aPiContentUrl, item.ItemsId, Token);
					HttpResponseMessage messge = HitAPI(formattedUrl);
					log.LogSimple(LoggingLevel.Information, "Http response of Contents : \n " + messge);
					if (messge.IsSuccessStatusCode)
					{
						string pageContent = messge.Content.ReadAsStringAsync().Result;

						string htmlPageRegex = "<inlineXML.*?>(?'pageContent'(.|\n)*?)</inlineXML>";
						string htmlpageHeadLineRegex = "<headline>(?'headLine'.*?)</headline>";
						string htmlpageURLRegex = "<remoteContent.*?href=\"(?'pageURL'(.|\n)*?)\">";

						string htmlSource = string.Empty;
						string htmlHeadLine = string.Empty;
						string pageURL = string.Empty;

						Regex pageSourceRegex = new Regex(htmlPageRegex);
						Regex headlineRegex = new Regex(htmlpageHeadLineRegex);
						Regex pageURLRegex = new Regex(htmlpageURLRegex);

						Match matches = pageSourceRegex.Match(pageContent);
						Match matchesHeadLine = headlineRegex.Match(pageContent);
						Match matchPageURLRegEx = pageURLRegex.Match(pageContent);

						try
						{
							if (matches != null && matches.Groups["pageContent"] != null)
							{
								htmlSource = matches.Groups["pageContent"].Value;
							}

							if (matchesHeadLine != null && matchesHeadLine.Groups["headLine"] != null)
							{
								htmlHeadLine = matchesHeadLine.Groups["headLine"].Value;
							}

							if (matchPageURLRegEx != null && matchPageURLRegEx.Groups["pageURL"] != null)
							{
								pageURL = matchPageURLRegEx.Groups["pageURL"].Value;
							}

							if (pageURL != null)
							{
								pageURL = pageURL.Replace("&amp;", "&");
							}

							htmlSource = htmlSource.Replace("<body>", "<body><h1>" + htmlHeadLine + "</h1>");
						}
						catch (Exception ex)
						{
							log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in Regex method", ex, null);
						}
						////string xmlResult = XmlToJSON(pageContent);
						////JToken data = JObject.Parse(xmlResult);
						ItemList iListModel = new ItemList();

						iListModel.ItemId = item.ItemsId;
						iListModel.PublishedDate = item.PublishedDate;
						iListModel.BodyContent = htmlSource;
						iListModel.Title = htmlHeadLine;
						iListModel.URL = pageURL;
						////iListModel.BodyContent = data["newsMessage"]["itemSet"]["newsItem"]["contentSet"]["inlineXML"]["html"]["body"] != null ? JsonToXml(data["newsMessage"]["itemSet"]["newsItem"]["contentSet"]["inlineXML"]["html"]["body"].ToString()) : string.Empty;
						itemLIst.Add(iListModel);
					}
					else
					{
						string messgeResult = ConvertResultXmlToString(messge.Content.ReadAsStringAsync().Result);
						if (messgeResult == Helper.ThomsonReutersApiStatus.Expired_authentication_token.ToString().Replace("_", " "))
						{
							Token = GetToken();
							goto Found;
						}
					}
				}
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in GetItemList method", ex, null);
			}
			return itemLIst;

		}

		public static string XmlToJSON(string xml)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xml);

			return XmlToJSON(doc);
		}
		public static string XmlToJSON(XmlDocument xmlDoc)
		{
			StringBuilder sbJSON = new StringBuilder();
			sbJSON.Append("{ ");
			XmlToJSONnode(sbJSON, xmlDoc.DocumentElement, true);
			sbJSON.Append("}");
			return sbJSON.ToString();
		}

		////  XmlToJSONnode:  Output an XmlElement, possibly as part of a higher array
		private static void XmlToJSONnode(StringBuilder sbJSON, XmlElement node, bool showNodeName)
		{
			if (showNodeName)
				sbJSON.Append("\"" + SafeJSON(node.Name) + "\": ");
			sbJSON.Append("{");
			//// Build a sorted list of key-value pairs
			////  where   key is case-sensitive nodeName
			////          value is an ArrayList of string or XmlElement
			////  so that we know whether the nodeName is an array or not.
			SortedList<string, object> childNodeNames = new SortedList<string, object>();

			////  Add in all node attributes
			if (node.Attributes != null)
				foreach (XmlAttribute attr in node.Attributes)
					StoreChildNode(childNodeNames, attr.Name, attr.InnerText);

			////  Add in all nodes
			foreach (XmlNode cnode in node.ChildNodes)
			{
				if (cnode is XmlText)
					StoreChildNode(childNodeNames, "value", cnode.InnerText);
				else if (cnode is XmlElement)
					StoreChildNode(childNodeNames, cnode.Name, cnode);
			}

			//// Now output all stored info
			foreach (string childname in childNodeNames.Keys)
			{
				List<object> alChild = (List<object>)childNodeNames[childname];
				if (alChild.Count == 1)
					OutputNode(childname, alChild[0], sbJSON, true);
				else
				{
					sbJSON.Append(" \"" + SafeJSON(childname) + "\": [ ");
					foreach (object Child in alChild)
						OutputNode(childname, Child, sbJSON, false);
					sbJSON.Remove(sbJSON.Length - 2, 2);
					sbJSON.Append(" ], ");
				}
			}
			sbJSON.Remove(sbJSON.Length - 2, 2);
			sbJSON.Append(" }");
		}

		////  StoreChildNode: Store data associated with each nodeName
		////                  so that we know whether the nodeName is an array or not.
		private static void StoreChildNode(SortedList<string, object> childNodeNames, string nodeName, object nodeValue)
		{
			//// Pre-process contraction of XmlElement-s
			if (nodeValue is XmlElement)
			{
				//// Convert  <aa></aa> into "aa":null
				////          <aa>xx</aa> into "aa":"xx"
				XmlNode cnode = (XmlNode)nodeValue;
				if (cnode.Attributes.Count == 0)
				{
					XmlNodeList children = cnode.ChildNodes;
					if (children.Count == 0)
						nodeValue = null;
					else if (children.Count == 1 && (children[0] is XmlText))
						nodeValue = ((XmlText)(children[0])).InnerText;
				}
			}
			//// Add nodeValue to ArrayList associated with each nodeName
			//// If nodeName doesn't exist then add it
			List<object> ValuesAL;

			if (childNodeNames.ContainsKey(nodeName))
			{
				ValuesAL = (List<object>)childNodeNames[nodeName];
			}
			else
			{
				ValuesAL = new List<object>();
				childNodeNames[nodeName] = ValuesAL;
			}
			ValuesAL.Add(nodeValue);
		}

		private static void OutputNode(string childname, object alChild, StringBuilder sbJSON, bool showNodeName)
		{
			if (alChild == null)
			{
				if (showNodeName)
					sbJSON.Append("\"" + SafeJSON(childname) + "\": ");
				sbJSON.Append("null");
			}
			else if (alChild is string)
			{
				if (showNodeName)
					sbJSON.Append("\"" + SafeJSON(childname) + "\": ");
				string sChild = (string)alChild;
				sChild = sChild.Trim();
				sbJSON.Append("\"" + SafeJSON(sChild) + "\"");
			}
			else
				XmlToJSONnode(sbJSON, (XmlElement)alChild, showNodeName);
			sbJSON.Append(", ");
		}

		//// Make a string safe for JSON
		private static string SafeJSON(string sIn)
		{
			StringBuilder sbOut = new StringBuilder(sIn.Length);
			foreach (char ch in sIn)
			{
				if (Char.IsControl(ch) || ch == '\'')
				{
					int ich = (int)ch;
					sbOut.Append(@"\u" + ich.ToString("x4"));
					continue;
				}
				else if (ch == '\"' || ch == '\\' || ch == '/')
				{
					sbOut.Append('\\');
				}
				sbOut.Append(ch);
			}
			return sbOut.ToString();
		}

		////public static string GetDatetimeForTrApi(DateTime Date)
		////{
		////  ////sample: yyyy.MM.dd.hh.mm
		////  ////        2018.10.18.12.20
		////  return string.Format("{0}.{1}.{2}.{3}.{4}", Date.Year, Date.Month, Date.Day, Date.Hour, Date.Minute);
		////}

		private string ConvertResultXmlToString(string input)
		{
			string ouputString = "";
			try
			{
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.LoadXml(input);
				string xpath = "status/error";
				var nodes = xmlDoc.SelectNodes(xpath);
				foreach (XmlNode childrenNode in nodes)
				{
					ouputString = childrenNode.FirstChild.Value;
				}
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in ConvertResultXmlToString method", ex, null);
			}
			return ouputString;
		}

		private int SaveScraperEvent(int processId, int processInstanceId)
		{
			Events scraperEvents = new Events()
			{
				ProcessEventId = processId,
				StartDateTime = Helper.GetSystemCurrentDateTime(),
				ProcessTypeId = Convert.ToInt32(Helper.ProcessType.HtmlPages),
				ProcessInstanceId = processInstanceId,
			};
			return ProcessEvents.InsertEvents(scraperEvents);
		}

		private List<LoaderLinkQueue> GetSimpleWebPageDetails(int processId, int processInstanceId)
		{
			string messageType = Convert.ToString(Helper.WebSiteCategory.CustomAPI);

			List<LoaderLinkQueue> webSiteLinks = WebLink.GetMessageBasedUponType(messageType, processId, processInstanceId);
			return webSiteLinks;
		}

		private List<LoaderLinkQueue> GetLinkRecords()
		{
			List<LoaderLinkQueue> linkRecords = WebLink.GetAllLinksInnerLinks();

			return linkRecords;
		}

		private List<PageDetails> StoreWebSiteLinkToSOLRList(List<ItemList> itemList, int processId, int processInstanceId, List<LoaderLinkQueue> listOfRecords, LoaderLinkQueue linkQueueModel, int scanningLinkQueueId)
		{
			string pageContent = string.Empty;
			List<PageDetails> solrPageDetails = new List<PageDetails>();

			foreach (ItemList items in itemList)
			{
				if (items != null)
				{
					try
					{
						var newURLDetail = listOfRecords.Find(x => x.SiteURL == items.URL && x.InstanceName == Convert.ToString(Helper.WebSiteCategory.CustomAPI));
						if (newURLDetail == null)
						{
							string guid = Helper.GuidString();
							PageDetails pageDetails = new PageDetails();

							pageContent = Helper.RemoveUnWantedHtmlSource(items.BodyContent);
							string[] pageSource = { pageContent };
							pageDetails.PageSource = pageSource;
							pageDetails.ItemId = linkQueueModel.Id;
							pageDetails.GuidId = guid;
							pageDetails.AddedDateTime = Helper.GetCurrentDateTime();
							pageDetails.Status = string.Empty;
							pageDetails.URL = items.URL;
							solrPageDetails.Add(pageDetails);

							LoaderLinkQueue linkRecordUpdate = new LoaderLinkQueue();
							linkRecordUpdate.ProcessId = processId;
							linkRecordUpdate.ProcessInstanceId = processInstanceId;
							linkRecordUpdate.WebSiteId = linkQueueModel.Id;
							linkRecordUpdate.InstanceName = Convert.ToString(Helper.WebSiteCategory.CustomAPI);
							linkRecordUpdate.SiteURL = items.URL;
							linkRecordUpdate.GUID = guid;
							linkRecordUpdate.WebLinkBytes = 0;
							linkRecordUpdate.NewerProcessId = processId;
							linkRecordUpdate.PublishDate = DateTime.Parse(items.PublishedDate);

							WebLink.UpdateLoaderLinkLogMasterRecord(linkRecordUpdate);

							////insert data into scanninglinkdetails
							WebLink.InsertInScanningLinkDetail(linkRecordUpdate, scanningLinkQueueId);
						}
					}
					catch (Exception ex)
					{
						log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in StoreWebSiteLinkToSOLRList method", ex, null);
					}
				}
			}
			return solrPageDetails;
		}

		private void InsertRecordToSolRDB(List<PageDetails> solrList)
		{
			try
			{
				SolrConfiguration.InsertInRange(solrList);
			}
			catch (SolrConnectionException ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in InsertRecordToSolRDB method", ex, null);
			}
		}

	}
}
