using BCMStrategy.CustomAPI.Abstract;
using BCMStrategy.CustomAPI.ViewModel;
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BCMStrategy.CustomAPI.Repository
{
	public class FTOperationRepository : IFTApiOperation
	{
		#region GENERAL PROPERTIES

		////private const string commonJSONPath = @"E:\Projects\BCM\BCMStrategy_4_0_0\BCMStrategy.CustomAPI\Common\jsonCollection.json";
		private readonly string commonJSONPath = ConfigurationManager.AppSettings["commonJSONPath"].ToString();

		private static readonly EventLogger<FTOperationRepository> log = new EventLogger<FTOperationRepository>();

		private readonly JToken commonFileContent;

		//// private static readonly EventLogger<Program> log = new EventLogger<Program>();

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

		////private ISolrPageDetail _solrPageDetail;

		////private ISolrPageDetail SolrPageDetail
		////{
		////  get
		////  {
		////    if (_solrPageDetail == null)
		////    {
		////      _solrPageDetail = new SolrPageDetailRepository();
		////    }

		////    return _solrPageDetail;
		////  }
		////}

		#endregion GENERAL PROPERTIES

		public FTOperationRepository()
		{
			using (JsonTextReader reader = new JsonTextReader(File.OpenText(commonJSONPath)))
			{
				commonFileContent = (JObject)JToken.ReadFrom(reader);
			}
		}

		#region Public  METHOD

		public void GetHeadLinesURLList(int processId, int processInstanceId)
		{
			List<HeadLineViewModel> headLineList = new List<HeadLineViewModel>();

			try
			{

				////if (isScrapperEventSaved > 0)
				////{
				string fTurl = ConfigurationManager.AppSettings["FTApiUrl"];
				LoaderLinkQueue link = GetSimpleWebPageDetails(processId, processInstanceId).Where(w => w.SiteURL == fTurl).Select(s => new LoaderLinkQueue
				{
					Id = s.Id,
					GUID = s.GUID,
					InstanceName = s.InstanceName,
					SiteURL = s.SiteURL,
					WebSiteId = s.Id,
					ProcessId = processId
				}).FirstOrDefault();

				if (link == null)
				{
					log.LogSimple(LoggingLevel.Information, "no loader link found");
					return;
				}

				int scanningLinkQueueId = WebLink.InsertInScanningLinkQueue(link);

				string url = Helper.GetFTHeadLineURL();
				double timeIntervalForLatestNEWS = Convert.ToDouble(ConfigurationManager.AppSettings["FTAPIIntervalInHours"]);
				////Get sector name list.
				List<string> sectorList = SolrConfiguration.GetAllSectorList();
				if (sectorList.Count > 0)
				{
					foreach (var sector in sectorList)
					{
						commonFileContent["FTQueryString"]["queryString"] = sector + " AND lastPublishDateTime:>" + Helper.GetJsonDateWithTimeZone(Helper.GetCurrentDateTime(), -timeIntervalForLatestNEWS).Replace("\"", "");

						//// Get data by hitting API
						HttpResponseMessage messge = HitAPI(url, Helper.GetFTHeadLineLicenceKey(), commonFileContent["FTQueryString"].ToString(), BCMStrategy.Resources.Enums.ApiMetodCall.POST);

						log.LogSimple(LoggingLevel.Information, "Http response of '" + url + "' is as below : \n " + messge);

						if (messge.IsSuccessStatusCode)
						{
							string result = messge.Content.ReadAsStringAsync().Result;
							JToken data = JObject.Parse(result);
							var jsonData = data["results"][0]["results"];
							if (jsonData != null)
							{
								log.LogSimple(LoggingLevel.Information, "New details found for sector '" + sector + "'");

								foreach (var item in jsonData)
								{
									HeadLineViewModel model = new HeadLineViewModel();
									model.Id = item["id"] != null ? item["id"].ToString() : string.Empty;
									model.URL = item["apiUrl"] != null ? item["apiUrl"].ToString() : string.Empty;
									if (headLineList.Count(s => s.Id == model.Id) == 0)
									{
										headLineList.Add(model);
									}
								}
							}
						}
					}
					if (headLineList.Any())
					{

						List<HeadLineContentViewModel> headLineContentList = GetContentOfHedLines(headLineList);
						List<LoaderLinkQueue> listOfRecords = GetLinkRecords();

						List<PageDetails> solrList = StoreWebSiteLinkToSOLRList(headLineContentList, processId, processInstanceId, listOfRecords, link, scanningLinkQueueId);


						if (solrList.Count > 0)
						{
							InsertRecordToSolRDB(solrList);
						}

						log.LogSimple(LoggingLevel.Information, "process completed");


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
				log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in GetInnerChildContents method", ex, null);
			}
		}

		private List<HeadLineContentViewModel> GetContentOfHedLines(List<HeadLineViewModel> modelList)
		{
			List<HeadLineContentViewModel> headLineContentList = new List<HeadLineContentViewModel>();
			try
			{
				log.LogSimple(LoggingLevel.Information, "Getting content from " + modelList.Count + " headlines news.");
				string url = Helper.GetFTB2BContentURL();
				string autorizationKey = Helper.GetFTB2BLineLicenceKey();
				foreach (HeadLineViewModel data in modelList)
				{
					Thread.Sleep(Helper.ThreadSleepInterval.ToString().ToInt32());

					string formatedURL = string.Format(url, data.Id, autorizationKey);
					HttpResponseMessage messge = HitAPI(formatedURL, autorizationKey, null, BCMStrategy.Resources.Enums.ApiMetodCall.GET);
					log.LogSimple(LoggingLevel.Information, "Http response of '" + url + "' is as below : \n " + messge);

					if (messge.IsSuccessStatusCode)
					{
						string result = messge.Content.ReadAsStringAsync().Result;
						JToken jsonObj = JObject.Parse(result);

						HeadLineContentViewModel model = new HeadLineContentViewModel();
						model.Id = data.Id;
						model.URL = jsonObj.Root["webUrl"] != null ? jsonObj.Root["webUrl"].ToString() : string.Empty;
						model.Title = jsonObj.Root["title"] != null ? jsonObj.Root["title"].ToString() : string.Empty;
						model.PublishedDate = jsonObj.Root["publishedDate"] != null ? jsonObj.Root["publishedDate"].ToString() : string.Empty;
						model.BodyContent = jsonObj.Root["bodyXML"] != null ? "<html>" + jsonObj.Root["bodyXML"].ToString() + "</html>" : string.Empty;
						headLineContentList.Add(model);
					}
				}
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in GetContentOfHedLines method", ex, null);
			}
			return headLineContentList;
		}

		public int SaveScraperEvent(int processId, int processInstanceId)
		{
			Events scraperEvents = new Events()
			{
				ProcessEventId = processId,
				StartDateTime = Helper.GetSystemCurrentDateTime(),
				ProcessTypeId = Convert.ToInt32(Helper.ProcessType.CustomAPI),
				ProcessInstanceId = processInstanceId,
			};
			return ProcessEvents.InsertEvents(scraperEvents);
		}

		public bool UpdateScraperEvent(int eventSavedresult, int processId, int solrCount)
		{
			Events scraperEvents = new Events()
			{
				Id = eventSavedresult,
				ProcessEventId = processId,
				EndDateTime = Helper.GetSystemCurrentDateTime(),
				PagesProcessed = solrCount
			};
			return ProcessEvents.UpdateEvents(scraperEvents);
		}

		#endregion Public  METHOD

		#region CODE MOVE TO COMMON PROJECT

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

		/// <summary>
		/// Get Simple Web Page Details
		/// </summary>
		/// <param name="resultType">result Type</param>
		/// <returns></returns>
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

		private HttpResponseMessage HitAPI(string url, string autorizationKey, string queryString, BCMStrategy.Resources.Enums.ApiMetodCall apiCallType)
		{
			System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();

			HttpResponseMessage response = new HttpResponseMessage();
			try
			{
				client.BaseAddress = new System.Uri(url);
				client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
				if (apiCallType == BCMStrategy.Resources.Enums.ApiMetodCall.POST && queryString != null)
				{
					client.DefaultRequestHeaders.Add("x-api-key", autorizationKey);
					System.Net.Http.HttpContent content = new StringContent(queryString, UTF8Encoding.UTF8, "application/json");
					response = client.PostAsync(url, content).Result;
				}
				else if (apiCallType == BCMStrategy.Resources.Enums.ApiMetodCall.GET)
				{
					response = client.GetAsync(url).Result;
				}
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

		/// <summary>
		/// Insert Record to the SOLR Database
		/// </summary>
		/// <param name="solrList"></param>
		/// <returns></returns>
		private async Task<bool> InsertRecordToSolRDB(List<PageDetails> solrList)
		{
			try
			{
				SolrConfiguration.InsertInRange(solrList);
				return true;
			}
			catch (SolrConnectionException ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in InsertRecordToSolRDB method", ex, null);
				return false;
			}
		}

		/// <summary>
		/// Stores Link to the SOLR Database
		/// </summary>
		/// <param name="webSiteLinks">Web Site Links</param>
		/// <returns>boolean value for storing the link to the SOLR Database</returns>
		private List<PageDetails> StoreWebSiteLinkToSOLRList(List<HeadLineContentViewModel> contentList, int processId, int processInstanceId, List<LoaderLinkQueue> listOfRecords, LoaderLinkQueue linkQueueModel, int scanningLinkQueueId)
		{
			string pageContent = string.Empty;
			List<PageDetails> solrPageDetails = new List<PageDetails>();

			foreach (HeadLineContentViewModel content in contentList)
			{
				if (content != null)
				{
					try
					{
						var newURLDetail = listOfRecords.Find(x => x.SiteURL == content.URL && x.InstanceName == Convert.ToString(Helper.WebSiteCategory.CustomAPI));
						if (newURLDetail == null)
						{
							string guid = Helper.GuidString();
							PageDetails pageDetails = new PageDetails();

							pageContent = Helper.RemoveUnWantedHtmlSource(content.BodyContent);
							string[] pageSource = { pageContent };
							pageDetails.PageSource = pageSource;
							pageDetails.ItemId = linkQueueModel.Id;
							pageDetails.GuidId = guid;
							pageDetails.AddedDateTime = Helper.GetCurrentDateTime();
							pageDetails.Status = string.Empty;
							pageDetails.URL = content.URL;
							solrPageDetails.Add(pageDetails);

							LoaderLinkQueue linkRecordUpdate = new LoaderLinkQueue();
							linkRecordUpdate.ProcessId = processId;
							linkRecordUpdate.ProcessInstanceId = processInstanceId;
							linkRecordUpdate.WebSiteId = linkQueueModel.Id;
							linkRecordUpdate.InstanceName = Convert.ToString(Helper.WebSiteCategory.CustomAPI);
							linkRecordUpdate.SiteURL = content.URL;
							linkRecordUpdate.GUID = guid;
							linkRecordUpdate.WebLinkBytes = 0;
							linkRecordUpdate.NewerProcessId = processId;
							linkRecordUpdate.PublishDate = DateTime.Parse(content.PublishedDate);

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

		#endregion CODE MOVE TO COMMON PROJECT
	}
}