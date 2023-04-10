using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCMStrategy.DAL.Context;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Logger;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Data.Entity;

namespace BCMStrategy.Data.Repository.Concrete
{
	public class ScrapperActivityRepository : IScrapperActivityRepository
	{
		private static readonly EventLogger<ScrapperActivityRepository> log = new EventLogger<ScrapperActivityRepository>();

		#region Insertion of Lexicons Counts

		/// <summary>
		/// Insert the tagged paragraphs in the DAtabase
		/// </summary>
		/// <param name="loaderLinkQueue">List of DAta</param>
		/// <param name="htmlResult">Tagged Paragraphs.</param>
		/// <returns></returns>
		public bool AddLexiconResults(LoaderLinkQueue loaderLinkQueue, StringBuilder htmlResult, List<ScrappedContentMapping> accutalIssueCountList)
		{
			log.LogEntry("AddLexiconResults: ", loaderLinkQueue, htmlResult, accutalIssueCountList);
			bool isSave = false;
			try
			{
				using (BCMStrategyEntities db = new BCMStrategyEntities())
				{
					if (accutalIssueCountList.Count > 0)
					{
						DateTime currentTimeStamp = Helper.GetCurrentDateTime();

						scrapedcontents objScrapedcontents = new scrapedcontents()
						{
							ScanningLinkDetailId = loaderLinkQueue.Id,
							Content = ASCIIEncoding.ASCII.GetBytes(htmlResult.ToString()),
							Created = currentTimeStamp,
							CreatedBy = "ScrapperActivity",
							scrapedlexiconmapping = ConvertToScrappedLexiconsList(accutalIssueCountList)
						};
						db.scrapedcontents.Add(objScrapedcontents);

						isSave = db.SaveChanges() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
					}
					else
					{
						isSave = true;
					}
				}
				log.LogEntry("End Of AddLexiconResults");
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "AddLexiconResults: Exception is thrown in Main method", ex, null);
			}
			return isSave;
		}

		private List<scrapedlexiconmapping> ConvertToScrappedLexiconsList(List<ScrappedContentMapping> accutalIssueCountList)
		{
			try
			{
				DateTime currentTimeStamp = Helper.GetCurrentDateTime();
				List<scrapedlexiconmapping> listOfScrappedLexicon = accutalIssueCountList.Select(x => new scrapedlexiconmapping()
				{
					LexiconId = x.LexiconId,
					LexiconsTerms = x.LexiconTerms,
					IssuesCount = x.IssueCount,
					CreatedBy = "ScrapperActivity",
					Created = currentTimeStamp
				}).ToList();

				return listOfScrappedLexicon;
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "AddLexiconResults: Exception is thrown in Main method", ex, null);
				return new List<scrapedlexiconmapping>();
			}
		}

		#endregion Insertion of Lexicons Counts

		#region Insertion of Activity Type Count and values

		/// <summary>
		/// Insertion of Tagged results and counts
		/// </summary>
		/// <param name="loaderLinkQueue">Pass the processId and Process InstanceId</param>
		/// <param name="htmlResult">Html Content</param>
		/// <param name="accutalProrietoryTagCountList">List of Actual Proprietory Tags</param>
		/// <returns>return saved or not.</returns>
		public bool InsertTaggedResults(LoaderLinkQueue loaderLinkQueue, List<ScrappedProprietoryTagsMapping> accutalProrietoryTagCountList)
		{
			log.LogEntry("InsertTaggedResults: ", loaderLinkQueue, accutalProrietoryTagCountList);
			bool isSave = false;
			try
			{

				using (BCMStrategyEntities db = new BCMStrategyEntities())
				{
					if (accutalProrietoryTagCountList != null && accutalProrietoryTagCountList.Count > 0)
					{
						DateTime currentTimeStamp = Helper.GetCurrentDateTime();
						StringBuilder htmlResult = new StringBuilder();
						accutalProrietoryTagCountList.ForEach(x =>
						{
							htmlResult.Append(x.HtmlResult);
						});
						scrappedproprietorytags objScrapedProprietoryTag = new scrappedproprietorytags()
						{
							ScanningLinkDetailId = loaderLinkQueue.Id,
							Content = ASCIIEncoding.ASCII.GetBytes(htmlResult.ToString()),
							Created = currentTimeStamp,
							scrappedproprietorytagsmapping = ConvertToScrappedProprietoryTagsList(accutalProrietoryTagCountList)
						};
						db.scrappedproprietorytags.Add(objScrapedProprietoryTag);

						isSave = db.SaveChanges() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
					}
					else
					{
						isSave = true;
					}
				}
				log.LogEntry("End Of InsertTaggedResults");
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "InsertTaggedResults: Exception is thrown in Main method", ex, null);
			}
			return isSave;
		}

		private List<scrappedproprietorytagsmapping> ConvertToScrappedProprietoryTagsList(List<ScrappedProprietoryTagsMapping> accutalProrietoryTagCountList)
		{
			DateTime currentTimeStamp = Helper.GetCurrentDateTime();
			List<scrappedproprietorytagsmapping> listOfScrappedLexicon = accutalProrietoryTagCountList.Select(x => new scrappedproprietorytagsmapping()
			{
				SearchTypeId = x.SearchTypeId,
				SearchValue = x.SearchValue,
				MetaDataTypeId = x.MetaDataId,
				SearchCounts = x.SearchCount,
				Created = currentTimeStamp,
				SearchType = x.SearchType,
				ActivityTypeId = x.ActivityTypeId
			}).ToList();
			////var maxValue = accutalProrietoryTagCountList.Max(x => x.SearchValue);
			////var proprietaryTagMaxValue = accutalProrietoryTagCountList.Where(x => x.SearchValue == maxValue).FirstOrDefault();
			////List<scrappedproprietorytagsmapping> listOfScrappedLexicon = new List<scrappedproprietorytagsmapping>() {
			////	new scrappedproprietorytagsmapping()
			////	{
			////		SearchTypeId = proprietaryTagMaxValue.SearchTypeId,
			////		SearchValue = proprietaryTagMaxValue.SearchValue,
			////		MetaDataTypeId = proprietaryTagMaxValue.MetaDataId,
			////		SearchCounts = proprietaryTagMaxValue.SearchCount,
			////		Created = currentTimeStamp,
			////		SearchType = proprietaryTagMaxValue.SearchType,
			////		ActivityTypeId = proprietaryTagMaxValue.ActivityTypeId
			////	}
			////};

			return listOfScrappedLexicon;
		}

		/// <summary>
		/// Store Activity Type for Hardcode Values
		/// </summary>
		/// <param name="loaderLinkQueue">Weblink details</param>
		/// <returns>return saved or not</returns>
		public bool AddActivityTypeResultsForHardCoded(LoaderLinkQueue loaderLinkQueue, List<WebLinkMetaDataModel> metaDataList)
		{
			log.LogEntry("AddActivityTypeResults: ", loaderLinkQueue);
			bool isSave = false;
			try
			{
				using (BCMStrategyEntities db = new BCMStrategyEntities())
				{
					DateTime currentTimeStamp = Helper.GetCurrentDateTime();

					if (metaDataList.Count > 0)
					{
						scrappedproprietorytags objScrapedProprietoryTag = new scrappedproprietorytags()
						{
							ScanningLinkDetailId = loaderLinkQueue.Id,
							Content = ASCIIEncoding.ASCII.GetBytes("Hard coded"),
							Created = currentTimeStamp,
							scrappedproprietorytagsmapping = ConvertToScrappedProprietoryTagsListForHardCoded(db, loaderLinkQueue, metaDataList)
						};
						db.scrappedproprietorytags.Add(objScrapedProprietoryTag);
					}
          isSave = db.SaveChanges() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
				}
				log.LogEntry("End Of AddActivityTypeResults");
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "AddActivityTypeResults: Exception is thrown in Main method", ex, null);
			}
			return isSave;
		}

		private List<scrappedproprietorytagsmapping> ConvertToScrappedProprietoryTagsListForHardCoded(BCMStrategyEntities db, LoaderLinkQueue loaderLinkQueue, List<WebLinkMetaDataModel> metaDataList)
		{
			List<scrappedproprietorytagsmapping> listOfScrappedProprietoryTags = new List<scrappedproprietorytagsmapping>();
			log.LogEntry("ConvertToScrappedProprietoryTagsListForHardCoded: ", loaderLinkQueue);
			try
			{
				DateTime currentTimeStamp = Helper.GetCurrentDateTime();
				var hardCodedValues = db.weblinks.Where(x => x.Id == loaderLinkQueue.WebSiteId).FirstOrDefault();
				////bool isMetaDataValAvailable = hardCodedValues.weblinkactivitytype.Any() ? hardCodedValues.weblinkactivitytype.FirstOrDefault().metadatavalue.
				List<int> metaDataMasterIds = metaDataList.Select(a => a.MetaDataMasterId).Distinct().ToList();

				if (metaDataMasterIds.Any())
				{
					metaDataMasterIds.ForEach(metaDataId =>
					{
						scrappedproprietorytagsmapping scrappedProprietoryTags = new scrappedproprietorytagsmapping()
						{
							SearchTypeId = hardCodedValues.weblinkactivitytype.Any() ? hardCodedValues.weblinkactivitytype.FirstOrDefault().metadatavalue.ActivityTypeId : null,
							SearchValue = hardCodedValues.weblinkactivitytype.Any() ? hardCodedValues.weblinkactivitytype.FirstOrDefault().metadatavalue.ActivityValue : 0,
							MetaDataTypeId = metaDataId,
							SearchCounts = 1,
							SearchType = BCMStrategy.Data.Abstract.Helper.ScrappingTypes.HARDCODED.ToString(),
							Created = currentTimeStamp
						};
						listOfScrappedProprietoryTags.Add(scrappedProprietoryTags);
					});
				}
				log.LogEntry("End Of ConvertToScrappedProprietoryTagsListForHardCoded: ", loaderLinkQueue);
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "ConvertToScrappedProprietoryTagsListForHardCoded: Exception is thrown in Main method", ex, null);
			}
			return listOfScrappedProprietoryTags;
		}
		#endregion Insertion of Activity Type Count and values

		#region Standard Tags
		public bool AddStandardResults(LoaderLinkQueue loaderLinkQueue, WebLinkStandardTagsModel webLinkStandardList)
		{
			bool isSave = false;
			log.LogEntry("AddStandardResults: ", loaderLinkQueue);
			try
			{
				using (BCMStrategyEntities db = new BCMStrategyEntities())
				{
					DateTime currentTimeStamp = Helper.GetCurrentDateTime();

					scrappedstandardtags objScrapedStandardTag = new scrappedstandardtags()
					{
						////DateOfIssue = webLinkStandardList.DateOfIssue,
						DateOfIssue = loaderLinkQueue.PublishDate.HasValue ? loaderLinkQueue.PublishDate : webLinkStandardList.DateOfIssue,
						scrapperstandardtags_entitytypes = ConvertToScrappedStandardTags_EntityList(db, loaderLinkQueue, webLinkStandardList),
						ScanningLinkDetailsId = loaderLinkQueue.Id,
						Content = string.IsNullOrEmpty(webLinkStandardList.Content) ? ASCIIEncoding.ASCII.GetBytes("Hard coded") : ASCIIEncoding.ASCII.GetBytes(webLinkStandardList.Content),
						SearchType = string.IsNullOrEmpty(webLinkStandardList.Content) ? BCMStrategy.Data.Abstract.Helper.ScrappingTypes.HARDCODED.ToString() : BCMStrategy.Data.Abstract.Helper.ScrappingTypes.DYNAMIC.ToString(),
						Created = currentTimeStamp,
						scrappedstandardtags_sectors = ConvertToScrappedStandardTags_SectorsList(db, loaderLinkQueue, webLinkStandardList),
						scrappedstandardtag_policymakers = ConvertToScrappedStandardTags_PolicyMakersList(db, loaderLinkQueue, webLinkStandardList)
					};
					db.scrappedstandardtags.Add(objScrapedStandardTag);

          isSave = db.SaveChanges() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
				}
				log.LogEntry("End Of AddStandardResults");
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "AddStandardResults: Exception is thrown in Main method", ex, null);
			}
			return isSave;
		}

		private List<scrapperstandardtags_entitytypes> ConvertToScrappedStandardTags_EntityList(BCMStrategyEntities db, LoaderLinkQueue loaderLinkQueue, WebLinkStandardTagsModel webLinkStandardList)
		{
			List<scrapperstandardtags_entitytypes> listOfScrappedStandardTags_EntityTypes = new List<scrapperstandardtags_entitytypes>();
			log.LogEntry("ConvertToScrappedStandardTags_EntityList: ", loaderLinkQueue);
			try
			{
				if (webLinkStandardList.EntityTypeList != null && webLinkStandardList.EntityTypeList.Any())
				{
					webLinkStandardList.EntityTypeList.ForEach(entityTypes =>
					{
						scrapperstandardtags_entitytypes scrapperStandardTags_EntityTypes = new scrapperstandardtags_entitytypes()
						{
							CountryId = entityTypes.CountryId,
							EntityId = entityTypes.EntityId,
							EntityTypeId = entityTypes.EntityTypeId
						};
						listOfScrappedStandardTags_EntityTypes.Add(scrapperStandardTags_EntityTypes);
					});
				}
				log.LogEntry("End Of ConvertToScrappedStandardTags_EntityList: ", loaderLinkQueue);
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "ConvertToScrappedStandardTags_EntityList: Exception is thrown in Main method", ex, null);
			}
			return listOfScrappedStandardTags_EntityTypes;
		}

		private List<scrappedstandardtag_policymakers> ConvertToScrappedStandardTags_PolicyMakersList(BCMStrategyEntities db, LoaderLinkQueue loaderLinkQueue, WebLinkStandardTagsModel webLinkStandardList)
		{
			List<scrappedstandardtag_policymakers> listOfScrappedStandardTags_PolicyMakers = new List<scrappedstandardtag_policymakers>();
			log.LogEntry("ConvertToScrappedStandardTags_PolicyMakersList: ", loaderLinkQueue);
			try
			{
				if (webLinkStandardList.IndividualsList != null && webLinkStandardList.IndividualsList.Any())
				{
					webLinkStandardList.IndividualsList.ForEach(individual =>
					{
						scrappedstandardtag_policymakers scrappedStandardTags_PolicyMakers = new scrappedstandardtag_policymakers()
						{
							PolicyMaker_Name = individual.IndividualFullName
						};
						listOfScrappedStandardTags_PolicyMakers.Add(scrappedStandardTags_PolicyMakers);
					});
				}
				log.LogEntry("End Of ConvertToScrappedStandardTags_PolicyMakersList: ", loaderLinkQueue);
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "ConvertToScrappedStandardTags_PolicyMakersList: Exception is thrown in Main method", ex, null);
			}
			return listOfScrappedStandardTags_PolicyMakers;
		}

		private List<scrappedstandardtags_sectors> ConvertToScrappedStandardTags_SectorsList(BCMStrategyEntities db, LoaderLinkQueue loaderLinkQueue, WebLinkStandardTagsModel webLinkStandardList)
		{
			List<scrappedstandardtags_sectors> listOfScrappedStandardTags_Sectors = new List<scrappedstandardtags_sectors>();
			log.LogEntry("ConvertToScrappedStandardTags_SectorsList: ", loaderLinkQueue);
			try
			{
				if (webLinkStandardList.SectorsList != null && webLinkStandardList.SectorsList.Any())
				{
					webLinkStandardList.SectorsList.ForEach(sectors =>
					{
						scrappedstandardtags_sectors scrappedStandardTags_Secors = new scrappedstandardtags_sectors()
						{
							SectorId = sectors.SectorId
						};
						listOfScrappedStandardTags_Sectors.Add(scrappedStandardTags_Secors);
					});
				}
				log.LogEntry("End Of ConvertToScrappedStandardTags_SectorsList: ", loaderLinkQueue);
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "ConvertToScrappedStandardTags_SectorsList: Exception is thrown in Main method", ex, null);
			}
			return listOfScrappedStandardTags_Sectors;
		}
		#endregion

		#region Archival Process Methods
		public void UpdateScrappedContent(DateTime date)
		{
			log.LogEntry("UpdateScrappedContent: ", date);
			try
			{
				using (BCMStrategyEntities db = new BCMStrategyEntities())
				{

					var listOfScrapperContent = db.scrapedcontents.Where(e => date > e.Created).ToList();
					if (listOfScrapperContent != null)
					{
						listOfScrapperContent.ForEach(x =>
						{
							x.Content = ASCIIEncoding.ASCII.GetBytes(string.Empty);
						});
					}
          bool result = db.SaveChanges() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
					if (result)
					{
						UpdateScrappedProprietaryTagsContent(date);
					}
				}
				log.LogEntry("End Of UpdateScrappedContent");
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "UpdateScrappedContent: Exception is thrown in Main method", ex, null);
			}
		}

		private void UpdateScrappedProprietaryTagsContent(DateTime date)
		{
			log.LogEntry("UpdateScrappedProprietaryTagsContent: ", date);
			try
			{
				using (BCMStrategyEntities db = new BCMStrategyEntities())
				{
					var listOfScrapperProprietaryContent = db.scrappedproprietorytags.Where(x => x.Created.Value < date).ToList();
					if (listOfScrapperProprietaryContent != null)
					{
						listOfScrapperProprietaryContent.ForEach(x =>
						{
							x.Content = ASCIIEncoding.ASCII.GetBytes(string.Empty);
						});
					}
          bool result = db.SaveChanges() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
					if (result)
					{
						UpdateScrappedStandardTagsContent(date);
					}
				}
				log.LogEntry("End Of UpdateScrappedProprietaryTagsContent");
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "UpdateScrappedProprietaryTagsContent: Exception is thrown in Main method", ex, null);
			}
		}

		private void UpdateScrappedStandardTagsContent(DateTime date)
		{
			log.LogEntry("UpdateScrappedStandardTagsContent: ", date);
			try
			{
				using (BCMStrategyEntities db = new BCMStrategyEntities())
				{
					var listOfScrapperStandardTagsContent = db.scrappedstandardtags.Where(x => x.Created.Value < date).ToList();
					if (listOfScrapperStandardTagsContent != null)
					{
						listOfScrapperStandardTagsContent.ForEach(x =>
						{
							x.Content = ASCIIEncoding.ASCII.GetBytes(string.Empty);
						});
					}
					db.SaveChanges();
				}
				log.LogEntry("End Of UpdateScrappedStandardTagsContent");
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "UpdateScrappedStandardTagsContent: Exception is thrown in Main method", ex, null);
			}
		}

		public List<string> GetGuidParentSite()
		{
			log.LogEntry("GetGuidParentSite");
			List<string> listOfGuid = new List<string>();
			try
			{
				using (BCMStrategyEntities db = new BCMStrategyEntities())
				{
					var webSiteURL = db.weblinks.AsNoTracking().Where(x => x.IsActive && !x.IsDeleted).Select(x => new { SiteURL = x.WebLinkURL.ToLower() }).AsEnumerable();
					var webSiteURLs = webSiteURL.Union(db.weblinkrss.AsNoTracking().Where(x => x.weblinks.IsActive && !x.weblinks.IsDeleted).Select(x => new { SiteURL = x.RSSFeedURL.ToLower() }).AsEnumerable()).ToList();
          ////List<string> webSitURLList = webSiteURL.SelectMany(x => x.weblinkrss.Select(y => y.RSSFeedURL)).ToList();
          ////webSitURLList.Union(webSiteURL.Select(x => x.WebLinkURL).ToList());

					listOfGuid = (from lllog in db.loaderlinklog.AsNoTracking().AsEnumerable() // Continue in memory
												join wsUrls in webSiteURLs on lllog.SiteURL.ToLower() equals wsUrls.SiteURL
												select lllog.Guid).ToList();

				}
				log.LogEntry("End Of UpdateScrappedContent");
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "UpdateScrappedContent: Exception is thrown in Main method", ex, null);
			}
			return listOfGuid;
		}
		#endregion

		#region General Function for Lexicon Counts

		/// <summary>
		/// Extract Paragraph with Nest Lexicons
		/// </summary>
		/// <param name="htmlContent">Html Page Content</param>
		/// <param name="lexiconList">List of Lexicons</param>
		/// <returns></returns>
		public ScrappedContentMappingCounts FetchLexiconResult(string htmlContent, List<LexiconModel> lexiconList, out string updatedHtmlContent)
		{
			string htmlContentUpdate = string.Empty;
			ScrappedContentMappingCounts scrappedContentMappingCounts = new ScrappedContentMappingCounts();
			try
			{
				log.LogEntry("FetchLexiconResult", htmlContent, lexiconList);
				List<ScrappedContentMapping> lexiconContentCount = new List<ScrappedContentMapping>();
				var htmlDoc = new HtmlDocument();
				htmlDoc.LoadHtml(htmlContent);
				StringBuilder htmlResult = new StringBuilder();
				var node = htmlDoc.DocumentNode.SelectNodes("//b");
				htmlContentUpdate = htmlDoc.DocumentNode.InnerHtml;
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
						string updatedParagraph = CheckForNestedLexicons(paragraph, lexiconList, htmlContentUpdate, out htmlContentUpdate);
						var lexiconIssueCount = GetLexiconsIssueCounts(updatedParagraph, lexiconList);
						lexiconContentCount.AddRange(lexiconIssueCount);
						htmlResult.Append(updatedParagraph);
					}

					log.LogEntry("Output Result FetchLexiconResult: ", htmlResult);
					log.LogEntry("End Of FetchLexiconResult");
				}
				updatedHtmlContent = htmlContentUpdate;
				scrappedContentMappingCounts.HtmlContents = htmlResult;

				scrappedContentMappingCounts.ActaulLexiconCounts = lexiconContentCount
																														.GroupBy(l => new { l.LexiconId, l.LexiconTerms }).Distinct()
																														.SelectMany(lex => lex.Select(
																														lexi => new
																														{
																															LexiconTerms = lexi.LexiconTerms,
																															IssueCount = lex.Sum(c => c.IssueCount),
																															LexiconId = lexi.LexiconId,
																														}).Distinct().ToList()
																														.Select(lexicon => new ScrappedContentMapping()
																														{
																															LexiconTerms = lexicon.LexiconTerms,
																															IssueCount = lexicon.IssueCount,
																															LexiconId = lexicon.LexiconId
																														})).ToList();
				return scrappedContentMappingCounts;
			}
			catch (Exception ex)
			{
				updatedHtmlContent = htmlContentUpdate;
				scrappedContentMappingCounts.HtmlContents = new StringBuilder();
				log.LogError(LoggingLevel.Error, "BadRequest", "FetchLexiconResult: Exception is thrown in Main method", ex, null);
				return scrappedContentMappingCounts;
			}
		}

		/// <summary>
		/// Check for Nested Lexicon terms exist or not. If exist than tag those terms.
		/// </summary>
		/// <param name="paragraph">Paragraph of Html Page</param>
		/// <param name="lexiconList">List of Lexicons</param>
		/// <returns></returns>
		private string CheckForNestedLexicons(string paragraph, List<LexiconModel> lexiconList, string htmlContent, out string htmlContentUpdate)
		{
			string htmlContentOriginal = htmlContent;
			try
			{
				log.LogEntry("CheckForNestedLexicons", paragraph, lexiconList);
				var doc = new HtmlDocument();
				doc.LoadHtml(paragraph);
				string newParagraph = paragraph;
				var pTags = doc.DocumentNode.Descendants("b").Select(x => x.InnerHtml).Distinct().ToArray();
				foreach (string lexiconTagged in pTags.ToList())
				{
					newParagraph = CheckForNestedLexiconsCombinations(newParagraph, lexiconTagged, lexiconList);
          ////htmlContentOriginal = Regex.Replace(htmlContentOriginal, paragraph, newParagraph, RegexOptions.Compiled);
					htmlContentOriginal = htmlContentOriginal.Replace(paragraph, newParagraph);
					paragraph = newParagraph;
				}

				var newDoc = new HtmlDocument();
				newDoc.LoadHtml(newParagraph);
				log.LogEntry("Ouput of CheckForNestedLexicons: ", newParagraph);
				log.LogEntry("End Of CheckForNestedLexicons");
				htmlContentUpdate = htmlContentOriginal;
				return newDoc.DocumentNode.Descendants("b").Select(x => x.InnerHtml).Any() ? newParagraph : string.Empty;
			}
			catch (Exception ex)
			{
				htmlContentUpdate = htmlContentOriginal;
				log.LogError(LoggingLevel.Error, "BadRequest", "CheckForNestedLexicons: Exception is thrown in Main method", ex, null);
				return string.Empty;
			}
		}

		/// <summary>
		/// Check for Nested Combinations
		/// </summary>
		/// <param name="paragraph">String of contents</param>
		/// <param name="lexiconTagged">Lexicons</param>
		/// <param name="lexiconList">List of Lexicons</param>
		/// <returns>return highlighted Content</returns>
		private string CheckForNestedLexiconsCombinations(string paragraph, string lexiconTagged, List<LexiconModel> lexiconList)
		{
			string newParagraph = paragraph;
			try
			{
				List<LexiconModel> nextedLexicons = lexiconList.Where(x => x.LexiconIssue.ToLower() == lexiconTagged.ToLower()).Select(x => x).ToList();
				bool isAnyLexiconMatched = false;
				foreach (LexiconModel lexiconModel in nextedLexicons)
				{
					List<string> combinationIssueList = !lexiconModel.IsNested && string.IsNullOrEmpty(lexiconModel.CombinationValue) ? new List<string>() : lexiconModel.CombinationValue.Split(';').Select(x => x.Trim()).ToList();
					////if (combinationIssueList.Any() && combinationIssueList.All(c => paragraph.Contains(string.Format("\b{0}\b", c))))
					if (combinationIssueList.Any() && combinationIssueList.All(c => Regex.IsMatch(newParagraph, string.Format(@"\b{0}\b", c), RegexOptions.IgnoreCase)))
					{
						isAnyLexiconMatched = true;
						combinationIssueList.ForEach(nestedCombination =>
						{
							////newParagraph = string.IsNullOrEmpty(nestedCombination) ? newParagraph : newParagraph.Replace(nestedCombination, "<i style=\"background:#7fffd4\">" + nestedCombination + "</i>");
							newParagraph = string.IsNullOrEmpty(nestedCombination) ? newParagraph : Regex.Replace(newParagraph, string.Format(@"\b{0}\b", nestedCombination), string.Format("<i style=\"background:#7fffd4\">{0}</i>", nestedCombination), RegexOptions.IgnoreCase);
						});
					}

					if (!lexiconModel.IsNested)
					{
						isAnyLexiconMatched = true;
					}
				}
				if (!isAnyLexiconMatched && nextedLexicons.Any())
				{
					////newParagraph = paragraph.Replace("<b style=\"background:#7fffd4\">" + lexiconTagged + "</b>", lexiconTagged);
					newParagraph = Regex.Replace(paragraph, "<b style=\"background:#7fffd4\">" + lexiconTagged + "</b>", lexiconTagged, RegexOptions.IgnoreCase);
				}
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "CheckForNestedLexiconsCombinations: Exception is thrown in Main method", ex, null);
				return string.Empty;
			}
			return newParagraph;
		}

		/// <summary>
		/// Insert the lexicons Issue count in the database
		/// </summary>
		/// <param name="htmlResult">html Result</param>
		/// <returns></returns>
		public List<ScrappedContentMapping> GetLexiconsIssueCounts(string htmlResult, List<LexiconModel> lexiconList)
		{
			try
			{
				log.LogEntry("GetLexiconsIssueCounts: ", htmlResult);
				List<ScrappedContentMapping> accutalIssueCountList;

				var htmlDoc = new HtmlDocument();
				htmlDoc.LoadHtml(htmlResult);
				var issueCountList = htmlDoc.DocumentNode.Descendants("b").Select(x => x.InnerHtml.ToLower()).GroupBy(x => x).Select(x => new { Issues = x.Key, IssueCount = x.Count() }).ToList();

				var issueNestedLexiconList = htmlDoc.DocumentNode.Descendants("i").Select(x => x.InnerHtml).GroupBy(x => x).Select(x => new { Issues = x.Key, IssueCount = x.Count() }).ToList();

				#region Original Code

				////var accutalIssueCountList1 = (from lexicons in lexiconList.Where(x=>x.IsNested ==false)
				////                              join issues in issueCountList on lexicons.LexiconIssue equals issues.Issues
				////                              select new ScrappedContentMapping()
				////                              {
				////                                LexiconId = lexicons.LexiconeIssueMasterId,
				////                                IssueCount = issues.IssueCount,
				////                                LexiconTerms = lexicons.LexiconIssue
				////                              }).Distinct().ToList();

				////var accutalIssueCountList2 = (from lexicons in lexiconList.Where(x => x.IsNested == true)
				////                              from nestedLexi in issueNestedLexiconList
				////                              join issues in issueCountList on lexicons.LexiconIssue equals issues.Issues
				////                              where !string.IsNullOrEmpty(lexicons.CombinationValue) ? lexicons.CombinationValue.Split(';').Contains(nestedLexi.Issues) : true
				////                              select new ScrappedContentMapping()
				////                              {
				////                                LexiconId = lexicons.LexiconeIssueMasterId,
				////                                IssueCount = issues.IssueCount,
				////                                LexiconTerms = string.Format("{0} {1}", lexicons.LexiconIssue, lexicons.IsNested ? string.Format("({0})", lexicons.CombinationValue) : string.Empty)
				////                              }).Distinct().ToList();

				#endregion Original Code

				if (issueNestedLexiconList.Count > 0)
				{
					accutalIssueCountList = issueNestedLexiconList.SelectMany(nestedIssue => lexiconList.Join(issueCountList, lexicons => lexicons.LexiconIssue.ToLower(), issues => issues.Issues.ToLower(), (lexicons, issues) => new
					{
						LexiconeIssueMasterId = lexicons.LexiconeIssueMasterId,
						IssueCount = issues.IssueCount,
						CombinationValue = lexicons.CombinationValue,
						LexiconIssue = lexicons.LexiconIssue,
						IsNested = lexicons.IsNested
					}).Where(x => !string.IsNullOrEmpty(x.CombinationValue) ? x.CombinationValue.Split(';').Any(combi => combi.ToLower() == nestedIssue.Issues.ToLower()) : Helper.saveChangesSuccessful)).Distinct()
						.Select(x => new ScrappedContentMapping()
						{
							LexiconId = x.LexiconeIssueMasterId,
							IssueCount = x.IssueCount,
							LexiconTerms = string.Format("{0} {1}", x.LexiconIssue, x.IsNested ? string.Format("({0})", x.CombinationValue) : string.Empty)
						}).Distinct().ToList();
				}
				else
				{
					accutalIssueCountList = lexiconList.Join(issueCountList, lexicons => lexicons.LexiconIssue.ToLower(), issues => issues.Issues.ToLower(), (lexicons, issues) => new
					{
						LexiconeIssueMasterId = lexicons.LexiconeIssueMasterId,
						IssueCount = issues.IssueCount,
						CombinationValue = lexicons.CombinationValue,
						LexiconIssue = lexicons.LexiconIssue,
						IsNested = lexicons.IsNested
					}).Distinct().Where(x => !x.IsNested)
					 .Select(x => new ScrappedContentMapping()
					 {
						 LexiconId = x.LexiconeIssueMasterId,
						 IssueCount = x.IssueCount,
						 LexiconTerms = string.Format("{0} {1}", x.LexiconIssue, x.IsNested ? string.Format("({0})", x.CombinationValue) : string.Empty)
					 }).Distinct().ToList();
				}
				log.LogEntry("End Of GetLexiconsIssueCounts");
				return accutalIssueCountList;
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "GetLexiconsIssueCounts: Exception is thrown in Main method", ex, null);
				return new List<ScrappedContentMapping>();
			}
		}
		#endregion
	}
}