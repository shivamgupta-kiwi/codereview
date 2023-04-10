using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Data.Repository.Concrete;
using BCMStrategy.Logger;
using BCMStrategy.ScrapperActivity.Abstract;
using HtmlAgilityPack;

namespace BCMStrategy.ScrapperActivity.Repository
{
	public class ScrapperActivityProcessRepository : IScrapperActivityProcess
	{
		#region General Variables

		private static readonly EventLogger<ScrapperActivityProcessRepository> log = new EventLogger<ScrapperActivityProcessRepository>();

		#region No Use Code

		/////// <summary>
		/////// The ProcessEvents repository
		/////// </summary>
		////private IProcessEvents _processEvents;

		////private IProcessEvents ProcessEvent
		////{
		////  get
		////  {
		////    if (_processEvents == null)
		////    {
		////      _processEvents = new ProcessEventsRepository();
		////    }

		////    return _processEvents;
		////  }
		////}

		////public int SaveScraperEvent(Events scraperEvents)
		////{
		////  return ProcessEvent.InsertEvents(scraperEvents);
		////}

		////public int GetWebLinkCount(int processId, int processInstanceId)
		////{
		////  return WebLink.GetWebLinkCountForLexicons(processId, processInstanceId);
		////}

		////public List<ProcessInstances> InsertProcesssInstances(ProcessConfiguration processConfig)
		////{
		////  List<ProcessInstances> instances = ProcessEvent.InsertProcesssInstances(processConfig, "LSP");
		////  return instances;
		////}

		////public bool UpdateScraperEvent(Events scraperEvents)
		////{
		////  return ProcessEvent.UpdateEvents(scraperEvents);
		////}

		#endregion No Use Code

		private static ILexicon _lexicon;

		private static ILexicon Lexicon
		{
			get
			{
				if (_lexicon == null)
				{
					_lexicon = new LexiconRepository();
				}

				return _lexicon;
			}
		}

		private static IGlobalConfiguration _globalConfiguration;

		private static IGlobalConfiguration GlobalConfiguration
		{
			get
			{
				if (_globalConfiguration == null)
				{
					_globalConfiguration = new GlobalConfigurationRepository();
				}

				return _globalConfiguration;
			}
		}

		private static IScrapperActivityRepository _scrapperActivity;

		private static IScrapperActivityRepository ScrapperActivity
		{
			get
			{
				if (_scrapperActivity == null)
				{
					_scrapperActivity = new ScrapperActivityRepository();
				}

				return _scrapperActivity;
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

		/// <summary>
		/// The WebLink repository
		/// </summary>
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

		/// <summary>
		/// The Meta TAgging and Quantification
		/// </summary>
		private IMetaTaggingAndQuantification _metaTaggingAndQuantification;

		private IMetaTaggingAndQuantification MetaTaggingAndQuantification
		{
			get
			{
				if (_metaTaggingAndQuantification == null)
				{
					_metaTaggingAndQuantification = new MetaTaggingAndQuantification();
				}

				return _metaTaggingAndQuantification;
			}
		}

		#endregion General Variables

		#region Main Process

		/// <summary>
		/// Read Lexicons From Solr
		/// </summary>
		/// <param name="processId">ProcessId</param>
		/// <param name="processInstanceId">Process Instance Id</param>
		public void ReadLexiconFromSolr(int processId, int processInstanceId)
		{
			try
			{
				int result = SaveScraperEvent(processId, processInstanceId);
				if (result > 0)
				{
					log.LogEntry("ReadLexiconFromSolr :", processId, processInstanceId);
					List<LoaderLinkQueue> listOfWebURLtoTagLexicons = WebLink.GetWebLinkForLexicons(processId, processInstanceId);

					foreach (LoaderLinkQueue loaderLinkQueue in listOfWebURLtoTagLexicons)
					{
						ProcessLexiconSearch(loaderLinkQueue);
					}
					log.LogEntry("End Of ReadLexiconFromSolr ");
					UpdateScraperEvent(result, processId, listOfWebURLtoTagLexicons.Count);

          // Commenting the code of future enhancement

          ////if (WebLink.IsFullScrapperActivityProcessCompleted(processId, processInstanceId))
          ////{
          ////  // Code to start calling Scrapper Activity Process for the given processId and processInstanceId
          ////  Process pageApplicationProcess = new Process();
          ////  string processArguments = Convert.ToString(Convert.ToInt32(processId)) + " " + Convert.ToString(processInstanceId);
          ////  pageApplicationProcess.StartInfo.FileName = ConfigurationManager.AppSettings["PDFGeneratorPath"];
          ////  pageApplicationProcess.StartInfo.Arguments = processArguments;
          ////  pageApplicationProcess.Start();
          ////  pageApplicationProcess.PriorityClass = ProcessPriorityClass.Normal;
          ////}
          ////else
          ////{
          ////  if (!ProcessEvents.CheckFullProcessCompleted(processId, processInstanceId) &&
          ////       ProcessEvents.GetMessageCount(Helper.SQSTypes.EmailGeneration.ToString(), (int)Helper.SQSTypes.EmailGeneration) == 0)
          ////  {
          ////    ProcessEvents.SaveToSQS(Helper.SQSTypes.EmailGeneration.ToString(), (int)Helper.SQSTypes.EmailGeneration);
          ////  }
          ////}
				}
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "ReadLexiconFromSolr: Exception is thrown in Main method", ex, null);
			}
		}

		#endregion Main Process

		#region Lexicon Process

		/// <summary>
		/// Lexicon Search From Solr
		/// </summary>
		/// <param name="loaderLinkQueue"></param>
		private void ProcessLexiconSearch(LoaderLinkQueue loaderLinkQueue)
		{
			try
			{
				log.LogEntry("ProcessLexiconSearch: ", loaderLinkQueue);
				SolrSearchParameters searchParameters = new SolrSearchParameters();
				List<LexiconModel> lexiconList = LexiconList();

				if (lexiconList.Count > 0)
				{
					string[] lexiconeArray = lexiconList.Select(x => x.LexiconIssue.Trim()).OrderBy(p => p.Substring(0)).Distinct().ToArray();

					searchParameters.FreeSearch = "guidId:\"" + loaderLinkQueue.GUID + "\" AND pageSource:(" + "\"" + string.Join("\" OR \"", lexiconeArray) + "\"" + ")";

					PageDetailsView pageDetailsView = GlobalConfiguration.Get(searchParameters);
					if (pageDetailsView.Highlight.Count > 0)
					{
						var highlightedResult = pageDetailsView.Highlight.Select(x => x).FirstOrDefault().Value.Values.FirstOrDefault().FirstOrDefault();
						////highlightedResult = RemoveAllUnwantedAttributesFromAnchor(highlightedResult);
						string updatedHtmlContent = string.Empty;

						ScrappedContentMappingCounts htmlResult = ScrapperActivity.FetchLexiconResult(highlightedResult, lexiconList, out updatedHtmlContent);

						bool result = AddLexiconResults(loaderLinkQueue, htmlResult, lexiconList);
						if (result)
						{
							MetaTaggingAndQuantification.MetaTaggingAndQuantificationForActivity(loaderLinkQueue);
						}

						if (WebLink.IsFullScrapperActivityCompleted(loaderLinkQueue.Id))
						{
							Thread.Sleep(Helper.ThreadSleepInterval.ToString().ToInt32());

							string[] pageSource = { updatedHtmlContent };
							List<LexiconDetails> lexiconDetail = new List<LexiconDetails>()
							{
								new LexiconDetails() {
									GuidId = loaderLinkQueue.GUID,
									PageSource = pageSource,
									ProcessId = loaderLinkQueue.ProcessId,
									ProcessInstanceId =loaderLinkQueue.ProcessInstanceId,
									ScrapperDetailId = loaderLinkQueue.Id,
									AddedDateTime = Helper.GetCurrentDateTime()
								}
							};

							GlobalConfiguration.InsertInRangeLexicons(lexiconDetail);
							Thread.Sleep(Helper.ThreadSleepInterval.ToString().ToInt32());
						}

						log.LogEntry("End of ProcessLexiconSearch");
					}
				}
				else
				{
					log.LogEntry("No Lexicon Found!!!");
				}
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "ProcessLexiconSearch: Exception is thrown in Main method", ex, null);
			}
		}

		/// <summary>
		/// Get the list of Lexicons
		/// </summary>
		/// <returns></returns>
		private List<LexiconModel> LexiconList()
		{
			return Lexicon.GetLexiconListForScraping();
		}



		/// <summary>
		/// Insert the tagged paragraphs in the DAtabase
		/// </summary>
		/// <param name="loaderLinkQueue">List of DAta</param>
		/// <param name="htmlResult">Tagged Paragraphs.</param>
		/// <returns></returns>
		private bool AddLexiconResults(LoaderLinkQueue loaderLinkQueue, ScrappedContentMappingCounts htmlResult, List<LexiconModel> lexiconList)
		{
			bool isSave = false;
			try
			{
				log.LogEntry("AddLexiconResults: ", loaderLinkQueue, htmlResult);
				if (!string.IsNullOrEmpty(htmlResult.HtmlContents.ToString()) && htmlResult.ActaulLexiconCounts.Count > 0)
				{
					////List<ScrappedContentMapping> accutalIssueCountList = ScrapperActivity.GetLexiconsIssueCounts(htmlResult, lexiconList);
					isSave = ScrapperActivity.AddLexiconResults(loaderLinkQueue, htmlResult.HtmlContents, htmlResult.ActaulLexiconCounts);
				}
				log.LogEntry("End Of AddLexiconResults");
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "AddLexiconResults: Exception is thrown in Main method", ex, null);
			}
			return isSave;
		}



		#endregion Lexicon Process

		#region Genaral Functions

		////private string RemoveAllUnwantedAttributesFromAnchor(string htmlResult)
		////{
		////	var htmlDoc = new HtmlDocument();
		////	htmlDoc.LoadHtml(htmlResult);

		////	var removedUnwantedHighlightedTags = htmlDoc.DocumentNode.OuterHtml.Replace("<b style=\" background:#7fffd4\"=\"\">", "");

		////	var actualDocument = new HtmlDocument();
		////	actualDocument.LoadHtml(removedUnwantedHighlightedTags);
		////	var node = actualDocument.DocumentNode.SelectNodes("//a");
		////	List<string> paraCollections = new List<string>();
		////	string actualHtml = actualDocument.DocumentNode.OuterHtml;
		////	foreach (var nod in node)
		////	{
		////		string oldValue = nod.OuterHtml;

		////		if (nod.HasAttributes)
		////		{
		////			foreach (var attribute in nod.Attributes.ToList())
		////			{
		////				if (attribute.Name.ToLower() != "href" && attribute.Name.ToLower() != "class")
		////				{
		////					attribute.Remove();
		////				}
		////			}
		////		}
		////		string newValue = nod.OuterHtml;
		////		if (actualHtml != newValue)
		////		{
		////			actualHtml = actualHtml.Replace(oldValue, newValue);
		////		}
		////	}
		////	return actualHtml;
		////}

		private int SaveScraperEvent(int processId, int processInstanceId)
		{
			Events scraperEvents = new Events()
			{
				ProcessEventId = processId,
				StartDateTime = Helper.GetSystemCurrentDateTime(),
				ProcessTypeId = Convert.ToInt32(Helper.ProcessType.ScraperActivity),
				ProcessInstanceId = processInstanceId,
			};
			return ProcessEvents.InsertEvents(scraperEvents);
		}

		private bool UpdateScraperEvent(int eventSavedresult, int processId, int solrCount)
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
		#endregion Genaral Functions
	}
}