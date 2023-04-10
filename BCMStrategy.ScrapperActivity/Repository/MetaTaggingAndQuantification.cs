using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Data.Repository.Concrete;
using BCMStrategy.Logger;
using BCMStrategy.ScrapperActivity.Abstract;
using HtmlAgilityPack;

namespace BCMStrategy.ScrapperActivity.Repository
{
	public class MetaTaggingAndQuantification : IMetaTaggingAndQuantification
	{
		private static readonly EventLogger<MetaTaggingAndQuantification> log = new EventLogger<MetaTaggingAndQuantification>();

		#region General Variables

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

		private static IMetaData _metaData;

		private static IMetaData MetaData
		{
			get
			{
				if (_metaData == null)
				{
					_metaData = new MetaDataRepository();
				}

				return _metaData;
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

		private static IStandardTagsAndQuantification _standardTagsAndQuantification;

		private static IStandardTagsAndQuantification StandardTagsAndQuantification
		{
			get
			{
				if (_standardTagsAndQuantification == null)
				{
					_standardTagsAndQuantification = new StandardTagsAndQuantification();
				}

				return _standardTagsAndQuantification;
			}
		}

		#endregion General Variables

		public void MetaTaggingAndQuantificationForActivity(LoaderLinkQueue loaderLinkQueue)
		{
			try
			{
				log.LogEntry("MetaTaggingAndQuantification: ", loaderLinkQueue);
				List<WebLinkMetaDataModel> metaDataList = MetaDataList(loaderLinkQueue);
				if (!loaderLinkQueue.IsHardCoded && (metaDataList.Count != 1 || metaDataList.Any(x => !x.IsActivityTypeAssignedWebLink)))
				{
					ActivityTypeInHeadings(loaderLinkQueue, metaDataList);
				}
				else
				{
					AddActivityTypeResultsForHardCoded(loaderLinkQueue, metaDataList);
				}
				log.LogEntry("End of ProcessLexiconSearch");
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "ProcessLexiconSearch: Exception is thrown in Main method", ex, null);
			}
		}

		#region Activity Type in Headings

		/// <summary>
		/// Search Activity Types in Heading of the Web Page
		/// </summary>
		/// <param name="loaderLinkQueue">Details of Web Page</param>
		/// <param name="metaDataList">List of Proprietary and Activity Types list</param>
		private void ActivityTypeInHeadings(LoaderLinkQueue loaderLinkQueue, List<WebLinkMetaDataModel> metaDataList)
		{
			SolrSearchParameters searchParameters = new SolrSearchParameters();
			List<ProrietarySearchModel> prorietarySearchList = new List<ProrietarySearchModel>();
			searchParameters.FreeSearch = "guidId:\"" + loaderLinkQueue.GUID + "\"";
			PageDetailsView pageDetailsView = GlobalConfiguration.Get(searchParameters);
			bool isFullSearchRequired = false;
			if (pageDetailsView.Products.Count > 0)
			{
				var pageSource = pageDetailsView.Products.Select(x => x).FirstOrDefault().PageSource[0];

				/*Activity Type Search*/

				var prorietarySearchActivity = FetchHeaderAndHilightResult(loaderLinkQueue, pageSource, metaDataList, out isFullSearchRequired);
				if (prorietarySearchActivity != null)
				{
					prorietarySearchList.Add(prorietarySearchActivity);
				}
				#region Original Code
				////if (!string.IsNullOrEmpty(htmlResult.ToString()))
				////{
				////	bool result = AddActivityTypesResults(loaderLinkQueue, htmlResult, metaDataList);
				////	if (result)
				////	{
				////		StandardTagsAndQuantification.StandardTags(loaderLinkQueue);
				////	}
				////}
				////else
				////{
				////	MetaTaggingAndQuantificationPhrase(loaderLinkQueue);
				////}
				#endregion
			}

			#region Original Code
			////else
			////{
			////	MetaTaggingAndQuantificationPhrase(loaderLinkQueue);
			////}
			#endregion

			/*Phrase Search*/
			var proprietarySearchPhrase = isFullSearchRequired || !prorietarySearchList.Any() || loaderLinkQueue.WebLinkTypeId == (int)Helper.WebSiteTypes.MediaSector ? MetaTaggingAndQuantificationPhrase(loaderLinkQueue) : null;
			if (proprietarySearchPhrase != null)
			{
				prorietarySearchList.Add(proprietarySearchPhrase);
			}

			var proprietarySearchNounPlusVerb = isFullSearchRequired || !prorietarySearchList.Any() || loaderLinkQueue.WebLinkTypeId == (int)Helper.WebSiteTypes.MediaSector ? MetaTaggingAndQuantificationNounPlus(loaderLinkQueue) : null;
			if (proprietarySearchNounPlusVerb != null)
			{
				prorietarySearchList.Add(proprietarySearchNounPlusVerb);
			}
			if (prorietarySearchList.Any())
			{
				InsertProrietoryTags(loaderLinkQueue, prorietarySearchList);
			}
		}

		#endregion Activity Type in Headings

		#region Activity Search Block

		/// <summary>
		/// Store Activity Type Result
		/// </summary>
		/// <param name="loaderLinkQueue"></param>
		/// <param name="htmlResult"></param>
		/// <param name="metaDataList"></param>
		/// <returns></returns>
    ////private bool AddActivityTypesResults(LoaderLinkQueue loaderLinkQueue, StringBuilder htmlResult, List<WebLinkMetaDataModel> metaDataList)
    ////{
    ////  log.LogEntry("AddActivityTypesResults: ", loaderLinkQueue, htmlResult);
    ////  bool isSave = false;
    ////  try
    ////  {
    ////    if (!string.IsNullOrEmpty(htmlResult.ToString()))
    ////    {
    ////      List<ScrappedProprietoryTagsMapping> accutalProrietoryTagCountList = GetActivityTypesCounts(htmlResult, metaDataList);
    ////      isSave = ScrapperActivity.InsertTaggedResults(loaderLinkQueue, accutalProrietoryTagCountList);
    ////    }
    ////    log.LogEntry("End Of AddActivityTypesResults");
    ////  }
    ////  catch (Exception ex)
    ////  {
    ////    log.LogError(LoggingLevel.Error, "BadRequest", "AddActivityTypesResults: Exception is thrown in Main method", ex, null);
    ////  }
    ////  return isSave;
    ////}

		/// <summary>
		/// Store Activity Type Result for HardCoded Sites
		/// </summary>
		/// <param name="loaderLinkQueue">Website Lisst</param>
		/// <returns>Is Saved or not</returns>
		private void AddActivityTypeResultsForHardCoded(LoaderLinkQueue loaderLinkQueue, List<WebLinkMetaDataModel> metaDataList)
		{
			bool isSave = false;
			log.LogEntry("AddActivityTypeResultsForHardCoded: ", loaderLinkQueue);
			try
			{
				isSave = ScrapperActivity.AddActivityTypeResultsForHardCoded(loaderLinkQueue, metaDataList);
				if (isSave)
				{
					StandardTagsAndQuantification.StandardTags(loaderLinkQueue);
				}
				log.LogEntry("End Of AddActivityTypeResultsForHardCoded");
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "AddActivityTypeResultsForHardCoded: Exception is thrown in Main method", ex, null);
			}
			////return isSave;
		}

		/// <summary>
		/// Get the ActivityType Count based on Html Content
		/// </summary>
		/// <param name="htmlResult">Pass html Content</param>
		/// <param name="metaDataList">List of Meta Data List</param>
		/// <returns>Get the List of Count for Proprietory Tags</returns>
		private List<ScrappedProprietoryTagsMapping> GetActivityTypesCounts(StringBuilder htmlResult, List<WebLinkMetaDataModel> metaDataList)
		{
			try
			{
				log.LogEntry("GetActivityTypesCounts: ", htmlResult);
				var htmlDoc = new HtmlDocument();
				htmlDoc.LoadHtml(htmlResult.ToString());
				var activityTypeCountList = htmlDoc.DocumentNode.Descendants("b").Select(x => x.InnerHtml.ToLower()).GroupBy(x => x).Select(x => new { Activity = x.Key, ActivityCount = x.Count() }).ToList();
				List<ScrappedProprietoryTagsMapping> accutalProrietoryTagCountList;

				accutalProrietoryTagCountList = metaDataList.Join(activityTypeCountList, metaData => metaData.ActivityName.ToLower(), activityType => activityType.Activity.ToLower(), (metaData, activityType) => new
				{
					ActivityTypeId = metaData.ActivityTypeMasterId,
					MetaDataId = metaData.MetaDataMasterId,
					ActivityCount = activityType.ActivityCount,
					MetaDataName = metaData.MetaDataName,
					ActivityType = metaData.ActivityName,
					ActivityValue = metaData.ActivityValue,
					IsfullSearchRequired = metaData.IsFullSearchRequired,
				}).Distinct()
			 .Select(x => new ScrappedProprietoryTagsMapping()
			 {
				 MetaDataName = x.MetaDataName,
				 MetaDataId = x.MetaDataId,
				 SearchValue = x.ActivityValue,
				 SearchCount = x.ActivityCount,
				 SearchType = BCMStrategy.Data.Abstract.Helper.ScrappingTypes.BY_ACTIVITY_TYPE.ToString(),
				 SearchTypeId = x.ActivityTypeId,
				 HtmlResult = htmlResult,
				 IsFullSearchRequired = x.IsfullSearchRequired
			 }).Distinct().ToList();

				log.LogEntry("End Of GetActivityTypesCounts");
				return accutalProrietoryTagCountList;
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "GetActivityTypesCounts: Exception is thrown in Main method", ex, null);
				return new List<ScrappedProprietoryTagsMapping>();
			}
		}

		#endregion Activity Search Block

		#region Phrases Search Block

		/// <summary>
		/// Phrase Quantification
		/// </summary>
		/// <param name="loaderLinkQueue"></param>
		private ProrietarySearchModel MetaTaggingAndQuantificationPhrase(LoaderLinkQueue loaderLinkQueue)
		{
			try
			{

				log.LogEntry("AddPhraseSearchResult: ", loaderLinkQueue);
				SolrSearchParameters searchParameters = new SolrSearchParameters();
				List<WebLinkPhraseModel> phraseList = PhraseList(loaderLinkQueue);
				string[] phraseArray = phraseList.Where(x => !string.IsNullOrEmpty(x.Phrase.Trim())).Select(x => x.Phrase.Trim()).OrderBy(p => p.Substring(0)).Distinct().ToArray();
				if (phraseArray.Any())
				{
					searchParameters.FreeSearch = "guidId:\"" + loaderLinkQueue.GUID + "\" AND pageSource:(" + "\"" + string.Join("\" OR \"", phraseArray) + "\"" + ")";
					PageDetailsView pageDetailsView = GlobalConfiguration.Get(searchParameters);
					if (pageDetailsView.Highlight.Count > 0)
					{
						var highlightedResult = pageDetailsView.Highlight.Select(x => x).FirstOrDefault().Value.Values.FirstOrDefault().FirstOrDefault();
						StringBuilder htmlResult = StandardTagsAndQuantification.FetchHighlightedResult(highlightedResult);
						if (!string.IsNullOrEmpty(htmlResult.ToString()))
						{
							return new ProrietarySearchModel()
							{
								SearchType = Helper.ScrappingTypes.BY_PHRASE.ToString(),
								HtmlResult = htmlResult,
								ObjectList = phraseList
							};
						}
						////bool result = AddPhraseResults(loaderLinkQueue, htmlResult, phraseList);
						////if (result)
						////{
						////	StandardTagsAndQuantification.StandardTags(loaderLinkQueue);
						////}
					}
					////else
					////{
					////	MetaTaggingAndQuantificationNounPlus(loaderLinkQueue);
					////}
				}
				////else
				////{
				////	MetaTaggingAndQuantificationNounPlus(loaderLinkQueue);
				////}
				log.LogEntry("End Of AddPhraseSearchResult");
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "AddPhraseSearchResult: Exception is thrown in Main method", ex, null);
			}
			return null;
		}

		/// <summary>
		/// Store Phrases result  with count into the Database
		/// </summary>
		/// <param name="loaderLinkQueue">Website Details</param>
		/// <param name="htmlResult">Html content</param>
		/// <param name="phraseList">List of Phrases</param>
		/// <returns></returns>
    ////private bool AddPhraseResults(LoaderLinkQueue loaderLinkQueue, StringBuilder htmlResult, List<WebLinkPhraseModel> phraseList)
    ////{
    ////  log.LogEntry("AddPhraseResults: ", loaderLinkQueue, htmlResult);
    ////  bool isSave = false;
    ////  try
    ////  {
    ////    if (!string.IsNullOrEmpty(htmlResult.ToString()))
    ////    {
    ////      List<ScrappedProprietoryTagsMapping> accutalProrietoryTagCountList = GetPhrasesCounts(htmlResult, phraseList);
    ////      isSave = ScrapperActivity.InsertTaggedResults(loaderLinkQueue, accutalProrietoryTagCountList);
    ////    }
    ////    log.LogEntry("End Of AddPhraseResults");
    ////  }
    ////  catch (Exception ex)
    ////  {
    ////    log.LogError(LoggingLevel.Error, "BadRequest", "AddPhraseResults: Exception is thrown in Main method", ex, null);
    ////  }
    ////  return isSave;
    ////}

		/// <summary>
		/// Get the Phrase Count based on Html Content
		/// </summary>
		/// <param name="htmlResult">Pass html Content</param>
		/// <param name="phrasesList">List of phrases List</param>
		/// <returns>Get the List of Count for Proprietory Tags</returns>
		private List<ScrappedProprietoryTagsMapping> GetPhrasesCounts(StringBuilder htmlResult, List<WebLinkPhraseModel> phrasesList)
		{
			try
			{
				log.LogEntry("GetPhrasesCounts: ", htmlResult, phrasesList);
				var htmlDoc = new HtmlDocument();
				htmlDoc.LoadHtml(htmlResult.ToString());
				var phrasesCountList = htmlDoc.DocumentNode.Descendants("b").Select(x => x.InnerHtml.ToLower()).GroupBy(x => x).Select(x => new { Phrases = x.Key, ActivityCount = x.Count() }).ToList();
				List<ScrappedProprietoryTagsMapping> accutalProrietoryTagCountList;

				accutalProrietoryTagCountList = phrasesList.Join(phrasesCountList, phraseData => phraseData.Phrase.ToLower(), phrase => phrase.Phrases.ToLower(), (phraseData, phrase) => new
				{
					PhraseId = phraseData.PhraseMasterId,
					MetaDataId = phraseData.MetaDataMasterId,
					PhraseCount = phrase.ActivityCount,
					MetaDataName = phraseData.MetaDataName,
					MetaDataValue = phraseData.MetaDataValue
				}).Distinct()
			 .Select(x => new ScrappedProprietoryTagsMapping()
			 {
				 MetaDataName = x.MetaDataName,
				 MetaDataId = x.MetaDataId,
				 SearchValue = x.MetaDataValue,
				 SearchCount = x.PhraseCount,
				 SearchType = BCMStrategy.Data.Abstract.Helper.ScrappingTypes.BY_PHRASE.ToString(),
				 SearchTypeId = x.PhraseId,
				 HtmlResult = htmlResult,
				 IsFullSearchRequired = false
			 }).Distinct().ToList();

				log.LogEntry("End Of GetPhrasesCounts");
				return accutalProrietoryTagCountList;
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "GetPhrasesCounts: Exception is thrown in Main method", ex, null);
				return new List<ScrappedProprietoryTagsMapping>();
			}
		}

		#endregion Phrases Search Block

		#region Noun Plus Block

		/// <summary>
		/// Meta data Tagging and Quantification Noun Plus
		/// </summary>
		/// <param name="loaderLinkQueue">Web Page Details</param>
		private ProrietarySearchModel MetaTaggingAndQuantificationNounPlus(LoaderLinkQueue loaderLinkQueue)
		{
			try
			{
				log.LogEntry("AddNounPlusSearchResult: ", loaderLinkQueue);
				SolrSearchParameters searchParameters = new SolrSearchParameters();
				List<WebLinkNounPVerbModel> nounPVerbList = NounPVerbList(loaderLinkQueue);
				string[] VerbArray = nounPVerbList.Where(x => !string.IsNullOrEmpty(x.Verb.Trim())).Select(x => x.Verb.Trim()).OrderBy(p => p.Substring(0)).Distinct().ToArray();
				if (VerbArray.Any())
				{
					searchParameters.FreeSearch = "guidId:\"" + loaderLinkQueue.GUID + "\" AND pageSource:(" + "\"" + string.Join("\" OR \"", VerbArray) + "\"" + ")";
					PageDetailsView pageDetailsView = GlobalConfiguration.Get(searchParameters);
					if (pageDetailsView.Highlight.Count > 0)
					{
						var highlightedResult = pageDetailsView.Highlight.Select(x => x).FirstOrDefault().Value.Values.FirstOrDefault().FirstOrDefault();
						List<string> searchedBy;
						StringBuilder htmlResult = FetchHighlightedNounPVerbResult(highlightedResult, nounPVerbList, out searchedBy);
						if (!string.IsNullOrEmpty(htmlResult.ToString()))
						{
							return new ProrietarySearchModel()
							{
								HtmlResult = htmlResult,
								SearchType = Helper.ScrappingTypes.BY_NOUN_VERB.ToString(),
								ObjectList = nounPVerbList,
								SearchedBy = searchedBy
							};
						}
						#region Original Code
						////bool result = AddNounPVerbResults(loaderLinkQueue, htmlResult, nounPVerbList, searchedBy);
						////if (result)
						////{
						////	StandardTagsAndQuantification.StandardTags(loaderLinkQueue);
						////}
						#endregion
					}
				}
				log.LogEntry("End Of AddNounPlusSearchResult");
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "AddNounPlusSearchResult: Exception is thrown in Main method", ex, null);
			}
			return null;
		}

		/// <summary>
		/// Add Noun Plus Verb Results
		/// </summary>
		/// <param name="loaderLinkQueue">Web Page Details</param>
		/// <param name="htmlResult">Html Results </param>
		/// <param name="nounPVerbList">List of Noun plus Verb</param>
		/// <returns></returns>
    ////private bool AddNounPVerbResults(LoaderLinkQueue loaderLinkQueue, StringBuilder htmlResult, List<WebLinkNounPVerbModel> nounPVerbList, List<string> searchedBy)
    ////{
    ////  bool isSave = false;
    ////  try
    ////  {
    ////    log.LogEntry("AddNounPVerbResults: ", loaderLinkQueue, nounPVerbList);
    ////    if (!string.IsNullOrEmpty(htmlResult.ToString()))
    ////    {
    ////      List<ScrappedProprietoryTagsMapping> accutalProrietoryTagCountList = GetNounPVerbCounts(htmlResult, nounPVerbList, searchedBy);
    ////      isSave = ScrapperActivity.InsertTaggedResults(loaderLinkQueue, accutalProrietoryTagCountList);
    ////    }
    ////    log.LogEntry("End Of AddNounPVerbResults");
    ////  }
    ////  catch (Exception ex)
    ////  {
    ////    log.LogError(LoggingLevel.Error, "BadRequest", "AddNounPVerbResults: Exception is thrown in Main method", ex, null);
    ////  }
    ////  return isSave;
    ////}

		/// <summary>
		/// Get the list of Noun Plus Verb For Insertion
		/// </summary>
		/// <param name="htmlResult">Tagged Result of Html content</param>
		/// <param name="nounPVerbList">List of Noun plus Verb</param>
		/// <returns></returns>
		private List<ScrappedProprietoryTagsMapping> GetNounPVerbCounts(StringBuilder htmlResult, List<WebLinkNounPVerbModel> nounPVerbList, List<string> searchedBy)
		{
			try
			{
				log.LogEntry("GetNounPVerbCounts: ", htmlResult, nounPVerbList);
				var htmlDoc = new HtmlDocument();
				htmlDoc.LoadHtml(htmlResult.ToString());
				var nounPVerbCountList = htmlDoc.DocumentNode.Descendants("b").Select(x => x.InnerHtml.ToLower()).GroupBy(x => x).Select(x => new { NounPVerb = x.Key, NounPVerbCount = x.Count() }).ToList();
				List<ScrappedProprietoryTagsMapping> accutalProrietoryTagCountList;
				accutalProrietoryTagCountList = nounPVerbList.Where(x => searchedBy.Distinct().Any(y => y.ToLower() == x.Noun.ToLower())).Join(nounPVerbCountList, nounPVList => nounPVList.Verb.ToLower(), nounPV => nounPV.NounPVerb.ToLower(), (nounPVList, nounPV) => new
				{
					NounPVerbMasterId = nounPVList.NounPVerbMasterId,
					MetaDataId = nounPVList.MetaDataMasterId,
					PhraseCount = nounPV.NounPVerbCount,
					MetaDataName = nounPVList.MetaDataName,
					MetaDataValue = nounPVList.MetaDataValue,
					ActivityTypeId = nounPVList.ActivityTypeId
				}).Distinct()
			 .Select(x => new ScrappedProprietoryTagsMapping()
			 {
				 MetaDataName = x.MetaDataName,
				 MetaDataId = x.MetaDataId,
				 SearchValue = x.MetaDataValue,
				 SearchCount = x.PhraseCount,
				 SearchType = BCMStrategy.Data.Abstract.Helper.ScrappingTypes.BY_NOUN_VERB.ToString(),
				 SearchTypeId = x.NounPVerbMasterId,
				 ActivityTypeId = x.ActivityTypeId,
				 HtmlResult = htmlResult,
				 IsFullSearchRequired = false
			 }).Distinct().ToList();

				log.LogEntry("End Of GetNounPVerbCounts");
				return accutalProrietoryTagCountList;
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "GetNounPVerbCounts: Exception is thrown in Main method", ex, null);
				return new List<ScrappedProprietoryTagsMapping>();
			}
		}

		/// <summary>
		/// Fetch Noun plus results
		/// </summary>
		/// <param name="htmlContent">Html Content</param>
		/// <param name="nounPVerbList">Noun Plus list</param>
		/// <returns></returns>
		private StringBuilder FetchHighlightedNounPVerbResult(string htmlContent, List<WebLinkNounPVerbModel> nounPVerbList, out List<string> SearchedBy)
		{
			try
			{
				List<string> searchedBy = new List<string>();
				log.LogEntry("FetchActivityTypesResult", htmlContent);
				var htmlDoc = new HtmlDocument();
				htmlDoc.LoadHtml(htmlContent);
				StringBuilder htmlResult = new StringBuilder();

				var node = htmlDoc.DocumentNode.SelectNodes("//b");
				if (node != null)
				{
					List<string> paraCollections = new List<string>();
					foreach (var nod in node)
					{
						if (nod.HasAttributes)
						{
							HtmlNode parentNode = nod.ParentNode;
							paraCollections.Add(parentNode.InnerHtml);
						}
					}
					HashSet<string> hSet = new HashSet<string>(paraCollections);
					foreach (var paragraph in hSet)
					{
						if (!string.IsNullOrEmpty(htmlResult.ToString()))
						{
							htmlResult.Append(Environment.NewLine + "<br/>");
							htmlResult.Append(Environment.NewLine + "<br/>");
						}

						DynamicValuesOfNounPVerb updatedParagraph = CheckForDynamicValues(paragraph, nounPVerbList);

						htmlResult.Append(updatedParagraph.PageContent);
						if (updatedParagraph.SearchedBy.Any())
						{
							searchedBy = updatedParagraph.SearchedBy;
							break;
						}
					}
					log.LogEntry("Output Result FetchActivityTypesResult: ", htmlResult);
					log.LogEntry("End Of FetchActivityTypesResult");
				}
				SearchedBy = searchedBy;
				return htmlResult;
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "FetchActivityTypesResult: Exception is thrown in Main method", ex, null);
				SearchedBy = new List<string>();
				return new StringBuilder();
			}
		}

		/// <summary>
		/// Check for Dynamic Values based on the Noun
		/// </summary>
		/// <param name="paragraph">Content of the html</param>
		/// <param name="nounPVerbList">Noun plus verb list</param>
		/// <returns></returns>
		private DynamicValuesOfNounPVerb CheckForDynamicValues(string paragraph, List<WebLinkNounPVerbModel> nounPVerbList)
		{
			List<string> SearchedBy = new List<string>();
			try
			{
				log.LogEntry("CheckForDynamicValues", paragraph, nounPVerbList);
				var doc = new HtmlDocument();
				doc.LoadHtml(paragraph);
				string newParagraph = paragraph;

				var pTags = doc.DocumentNode.Descendants("b").Select(x => x.InnerHtml.ToLower()).Distinct().ToArray();

				foreach (string verbTagged in pTags)
				{
					List<WebLinkNounPVerbModel> nextedNounPVerb = nounPVerbList.Where(x => x.Verb.ToLower() == verbTagged.ToLower()).Select(x => x).ToList();
					bool isAnyLexiconMatched = false;
					foreach (WebLinkNounPVerbModel nounPVerbModel in nextedNounPVerb)
					{
						if (nounPVerbModel.IsHardCode)
						{
							////var isAllMatched = nounPVerbModel.Noun.Any(c => paragraph.Contains(string.Format("\b{0}\b", c)));
							var isAllMatched = Regex.IsMatch(paragraph, string.Format(@"\b{0}\b", nounPVerbModel.Noun), RegexOptions.IgnoreCase);

							if (isAllMatched)
							{
								SearchedBy.Add(nounPVerbModel.Noun);
								isAnyLexiconMatched = true;
								////newParagraph = string.IsNullOrEmpty(nounPVerbModel.Noun) ? newParagraph : newParagraph.Replace(nounPVerbModel.Noun, "<i style=\"background:#7fffd4\">" + nounPVerbModel.Noun + "</i>");

								newParagraph = string.IsNullOrEmpty(nounPVerbModel.Noun) ? newParagraph : Regex.Replace(newParagraph, string.Format(@"\b{0}\b", nounPVerbModel.Noun), "<i style=\"background:#7fffd4\">" + nounPVerbModel.Noun + "</i>", RegexOptions.IgnoreCase);
								break;
							}
						}
						else
						{
							var getDynamicTables = MetaData.GetListOfDynmicTable(nounPVerbModel);
							////var isAllMatched = getDynamicTables.Any(c => !string.IsNullOrEmpty(c.Name) && paragraph.ToLower().Contains(string.Format(@"\b{0}\b", c.Name.ToLower())));
							var isAllMatched = getDynamicTables.Any(c => !string.IsNullOrEmpty(c.Name) && Regex.IsMatch(paragraph, string.Format(@"\b{0}\b", c.Name), RegexOptions.IgnoreCase));
							if (isAllMatched)
							{
								isAnyLexiconMatched = true;
								SearchedBy.Add(nounPVerbModel.Noun);
								////var nounPVerb = getDynamicTables.Where(c => !string.IsNullOrEmpty(c.Name) && paragraph.ToLower().Contains(string.Format("\b{0}\b", c.Name.ToLower()))).Select(x => x).ToList();
								var nounPVerb = getDynamicTables.Where(c => !string.IsNullOrEmpty(c.Name) && Regex.IsMatch(paragraph, string.Format(@"\b{0}\b", c.Name.ToLower()), RegexOptions.IgnoreCase)).Select(x => x).ToList();
								nounPVerb.ForEach(nestedNoun =>
								{
									////newParagraph = string.IsNullOrEmpty(nounPVerbModel.Noun) ? newParagraph : newParagraph.Replace(nestedNoun.Name, "<i style=\"background:#7fffd4\">" + nestedNoun.Name + "</i>");
									newParagraph = string.IsNullOrEmpty(nounPVerbModel.Noun) ? newParagraph : Regex.Replace(newParagraph, string.Format(@"\b{0}\b", nestedNoun.Name), "<i style=\"background:#7fffd4\">" + nestedNoun.Name + "</i>", RegexOptions.IgnoreCase);
								});
								break;
							}
						}
					}
					if (!isAnyLexiconMatched && nextedNounPVerb.Count > 0)
					{
						////newParagraph = paragraph.Replace("<b style=\"background:#7fffd4\">" + verbTagged + "</b>", verbTagged);

						newParagraph = Regex.Replace(newParagraph, "<b style=\"background:#7fffd4\">" + verbTagged + "</b>", verbTagged, RegexOptions.IgnoreCase);
					}
				}
				var newDoc = new HtmlDocument();
				newDoc.LoadHtml(newParagraph);
				log.LogEntry("Ouput of CheckForDynamicValues: ", newParagraph);
				log.LogEntry("End Of CheckForDynamicValues");
				return new DynamicValuesOfNounPVerb() { PageContent = newDoc.DocumentNode.Descendants("b").Select(x => x.InnerHtml).Any() ? newParagraph : string.Empty, SearchedBy = SearchedBy };
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "CheckForDynamicValues: Exception is thrown in Main method", ex, null);
				return new DynamicValuesOfNounPVerb();
			}
		}

		#endregion Noun Plus Block

		#region General Block

		private ProrietarySearchModel FetchHeaderAndHilightResult(LoaderLinkQueue loaderLinkQueue, string htmlContent, List<WebLinkMetaDataModel> metaDataList, out bool isFullSearchRequired)
		{
			try
			{
				var activityTypes = metaDataList.Where(x => !string.IsNullOrEmpty(x.ActivityName.Trim()))
																						.Select(x => new
																						{
																							ActivityType = x.ActivityName.Trim(),
																							IsFullSearchRequired = x.IsFullSearchRequired
																						}).OrderBy(p => p.ActivityType.Substring(0)).Distinct().ToList();

				StringBuilder htmlResult = new StringBuilder();
				bool isFullSearch = false;
				if (activityTypes.Any())
				{
					log.LogEntry("FetchHeaderAndHilighResult", htmlContent);
					var htmlDoc = new HtmlDocument();
					htmlDoc.LoadHtml(htmlContent);
					List<string> headerTags = new List<string>() { "h1", "h2", "h3" };
					bool isFound = false;

					////var isAnyUrlMatched = activityTypes.Any(c => Regex.IsMatch(Helper.RemoveSpecialCharacterswithSpace(loaderLinkQueue.SiteURL).ToLower(), string.Format(@"\b{0}\b", c.ActivityType.ToLower()), RegexOptions.IgnoreCase));
					////if (isAnyUrlMatched)
					////{
					////	isFound = isAnyUrlMatched;
					////	////var activityType = activityTypes.Where(c => Helper.RemoveSpecialCharacterswithSpace(loaderLinkQueue.SiteURL).ToLower().Contains(string.Format("\b{0}\b", c.ToLower()))).Select(x => x).FirstOrDefault();
					////	var activityType = activityTypes.Where(c => Regex.IsMatch(Helper.RemoveSpecialCharacterswithSpace(loaderLinkQueue.SiteURL).ToLower(), string.Format(@"\b{0}\b", c.ActivityType.ToLower()), RegexOptions.IgnoreCase)).Select(x => x).FirstOrDefault();
					////	isFullSearch = isFullSearch ? isFullSearch : activityType.IsFullSearchRequired;
					////	////htmlResult.Append(Helper.RemoveSpecialCharacterswithSpace(loaderLinkQueue.SiteURL).ToLower().Replace(activityType.ToLower(), "<b style=\"background:#7fffd4\">" + activityType + "</b>"));
					////	htmlResult.Append(Regex.Replace(Helper.RemoveSpecialCharacterswithSpace(loaderLinkQueue.SiteURL), activityType.ActivityType, "<b style=\"background:#7fffd4\">" + activityType.ActivityType + "</b>", RegexOptions.IgnoreCase));
					////}

					if ((loaderLinkQueue.PageType != (int)Helper.PageTypes.PDF && !isFound) || isFullSearch)
					{
						headerTags.ForEach(y =>
						{
							////if (isFound)
							////	return;

							var nodeH1 = htmlDoc.DocumentNode.SelectNodes("//" + y);
							if (nodeH1 != null)
							{
								foreach (var nod in nodeH1)
								{
									////var isAnyMatched = activityTypes.Any(c => nod.InnerText.ToLower().Contains(string.Format("\b{0}\b", c.ToLower())));
									var isAnyMatched = activityTypes.Any(c => Regex.IsMatch(nod.InnerText.ToLower(), string.Format(@"\b{0}\b", c.ActivityType.ToLower()), RegexOptions.IgnoreCase));

									if (isAnyMatched)
									{
										isFound = isAnyMatched;
										////var activityType = activityTypes.Where(c => nod.InnerText.ToLower().Contains(string.Format("\b{0}\b", c.ToLower()))).Select(x => x).FirstOrDefault();
										var activityType = activityTypes.Where(c => Regex.IsMatch(nod.InnerText.ToLower(), string.Format(@"\b{0}\b", c.ActivityType.ToLower()), RegexOptions.IgnoreCase)).Select(x => x).ToList();
										activityType.ForEach(x =>
										{
											isFullSearch = isFullSearch ? isFullSearch : x.IsFullSearchRequired;
											htmlResult.Append(Regex.Replace(nod.InnerText, x.ActivityType, "<b style=\"background:#7fffd4\">" + x.ActivityType + "</b>", RegexOptions.IgnoreCase));
										});
										////break;
									}
								}
							}
						});
					}

					if ((!isFound && loaderLinkQueue.PageType != (int)Helper.PageTypes.PDF) || isFullSearch)
					{
						string pageContent = Regex.Replace(htmlContent, "<[^>]+>", "");

						string firstFewWords = Regex.Match(pageContent.ReplaceHtmlContents(), @"(\w+\b.*?){" + Helper.AllowFirstFewWordsForActivityType + "}").ToString().ReplaceHtmlContents();
						var isAnyMatched = activityTypes.Any(c => Regex.IsMatch(firstFewWords.ToLower(), string.Format(@"\b{0}\b", c.ActivityType.ToLower()), RegexOptions.IgnoreCase));
						if (isAnyMatched)
						{
							isFound = isAnyMatched;
							////var activityType = activityTypes.Where(c => firstFewWords.ToLower().Contains(string.Format("\b{0}\b", c.ToLower()))).Select(x => x).FirstOrDefault();
							var activityType = activityTypes.Where(c => Regex.IsMatch(firstFewWords.ToLower(), string.Format(@"\b{0}\b", c.ActivityType.ToLower()), RegexOptions.IgnoreCase)).Select(x => x).ToList();
							activityType.ForEach(x =>
							{
								////htmlResult.Append(firstFewWords.Replace(activityType, "<b style=\"background:#7fffd4\">" + activityType + "</b>"));
								isFullSearch = isFullSearch ? isFullSearch : x.IsFullSearchRequired;
								firstFewWords = Regex.Replace(firstFewWords, x.ActivityType, "<b style=\"background:#7fffd4\">" + x.ActivityType + "</b>", RegexOptions.IgnoreCase);
							});
							htmlResult.Append(firstFewWords);
						}
					}
					else if (!isFound || isFullSearch)
					{
						string regex = @"\A(?:.*\n){" + Helper.AllowFirstFewLinesForActivityType + "}";
						////string newHtmlContent = Regex.Replace(htmlContent, @"\n\s+", "\n").Trim();
						string newHtmlContent = Regex.Replace(htmlContent, @"<p>", "\n").Replace("<br>", "\n").Replace("<html><body>", "").Replace("</body></html>", "").Replace("</p>", "").Trim();

						newHtmlContent = Regex.Replace(newHtmlContent, @"\n\s+", "\n").Trim();

						string firstFewLines = Regex.Match(newHtmlContent, regex).ToString();
						////var isAnyMatched = activityTypes.Any(c => firstFewLines.ToLower().Contains(string.Format("\b{0}\b", c.ToLower())));
						var isAnyMatched = activityTypes.Any(c => Regex.IsMatch(firstFewLines.ToLower(), string.Format(@"\b{0}\b", c.ActivityType.ToLower()), RegexOptions.IgnoreCase));

						if (isAnyMatched)
						{
							isFound = isAnyMatched;
							////var activityType = activityTypes.Where(c => firstFewLines.ToLower().Contains(string.Format("\b{0}\b", c.ToLower()))).Select(x => x).FirstOrDefault();
							var activityType = activityTypes.Where(c => Regex.IsMatch(firstFewLines.ToLower(), string.Format(@"\b{0}\b", c.ActivityType.ToLower()), RegexOptions.IgnoreCase)).Select(x => x).ToList();
							activityType.ForEach(x =>
							{
								isFullSearch = isFullSearch ? isFullSearch : x.IsFullSearchRequired;
								////htmlResult.Append(firstFewLines.Replace(activityType, "<b style=\"background:#7fffd4\">" + activityType + "</b>"));
								firstFewLines = Regex.Replace(firstFewLines, x.ActivityType, "<b style=\"background:#7fffd4\">" + x.ActivityType + "</b>", RegexOptions.IgnoreCase);
							});
							htmlResult.Append(firstFewLines);
						}
					}

					log.LogEntry("Output Result FetchHeaderAndHilighResult: ", htmlResult);
					log.LogEntry("End Of FetchHeaderAndHilighResult");
				}
				isFullSearchRequired = isFullSearch;
				if (!string.IsNullOrEmpty(htmlResult.ToString()))
				{
					return new ProrietarySearchModel()
					{
						SearchType = Helper.ScrappingTypes.BY_ACTIVITY_TYPE.ToString(),
						HtmlResult = htmlResult,
						ObjectList = metaDataList
					};
				}
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "FetchHeaderAndHilighResult: Exception is thrown in Main method", ex, null);
			}
			isFullSearchRequired = false;
			return null;
		}

		#endregion General Block

		#region Helper Methods

		private List<WebLinkMetaDataModel> MetaDataList(LoaderLinkQueue loaderLinkQueue)
		{
			return MetaData.GetMetaDataListForScrapingActvityType(loaderLinkQueue);
		}

		private List<WebLinkPhraseModel> PhraseList(LoaderLinkQueue loaderLinkQueue)
		{
			return MetaData.GetPhrasesListForScraping(loaderLinkQueue);
		}

		private List<WebLinkNounPVerbModel> NounPVerbList(LoaderLinkQueue loaderLinkQueue)
		{
			return MetaData.GetNounPVerbListForScraping(loaderLinkQueue);
		}

		private void InsertProrietoryTags(LoaderLinkQueue loaderLinkQueue, List<ProrietarySearchModel> prorietarySearchList)
		{
			try
			{
				log.LogSimple(LoggingLevel.Information, "InsertProrietoryTags: " + prorietarySearchList);
				List<ScrappedProprietoryTagsMapping> accutalProrietoryTagCountList = new List<ScrappedProprietoryTagsMapping>();

				prorietarySearchList.ForEach(x =>
				{
					if (Helper.ScrappingTypes.BY_ACTIVITY_TYPE.ToString() == x.SearchType)
					{
						var accutalProrietoryTagCount = GetActivityTypesCounts(x.HtmlResult, (List<WebLinkMetaDataModel>)x.ObjectList);
						accutalProrietoryTagCountList.AddRange(accutalProrietoryTagCount);
					}
					else if (Helper.ScrappingTypes.BY_PHRASE.ToString() == x.SearchType)
					{
						var accutalProrietoryTagCount = GetPhrasesCounts(x.HtmlResult, (List<WebLinkPhraseModel>)x.ObjectList);
						accutalProrietoryTagCountList.AddRange(accutalProrietoryTagCount);
					}
					else if (Helper.ScrappingTypes.BY_NOUN_VERB.ToString() == x.SearchType)
					{
						var accutalProrietoryTagCount = GetNounPVerbCounts(x.HtmlResult, (List<WebLinkNounPVerbModel>)x.ObjectList, x.SearchedBy);
						accutalProrietoryTagCountList.AddRange(accutalProrietoryTagCount);
					}
				});

				if (loaderLinkQueue.WebLinkTypeId == (int)Helper.WebSiteTypes.OfficialSector)
				{
					#region Maximum Value Code
					////var maxValue = accutalProrietoryTagCountList.Max(x => x.SearchValue);
					////var actualProprietaryTagHighestValue = accutalProrietoryTagCountList.Where(x => x.SearchValue == maxValue).Select(x => x).FirstOrDefault();
					#endregion
					var actualProprietaryTagHighestValue = accutalProrietoryTagCountList.Where(x => !x.IsFullSearchRequired).Select(x => x).FirstOrDefault();
					if (actualProprietaryTagHighestValue == null)
					{
						actualProprietaryTagHighestValue = accutalProrietoryTagCountList.Select(x => x).FirstOrDefault();
					}
					accutalProrietoryTagCountList = new List<ScrappedProprietoryTagsMapping>();
					accutalProrietoryTagCountList.Add(actualProprietaryTagHighestValue);
				}

				bool isSaved = ScrapperActivity.InsertTaggedResults(loaderLinkQueue, accutalProrietoryTagCountList);

				if (isSaved)
				{
					StandardTagsAndQuantification.StandardTags(loaderLinkQueue);
				}
				log.LogSimple(LoggingLevel.Information, "End of InsertProrietoryTags: " + prorietarySearchList);
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "InsertProrietoryTags: Exception is thrown in Main method", ex, null);
			}
		}
		////private List<MediaSectorLegislatorModel> GetListOfLegislator()
		////{
		////  return MetaData.GetAllLegislatorListForScrapperActivity();
		////}

		////private List<MediaSectorStateHeadModel> GetListOfStateHeads()
		////{
		////	return MetaData.GetStateHeadListForScrapperActivity();
		////}
		#endregion Helper Methods
	}
}