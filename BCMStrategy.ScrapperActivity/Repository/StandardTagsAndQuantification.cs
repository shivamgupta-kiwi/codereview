using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Data.Repository.Concrete;
using BCMStrategy.Logger;
using BCMStrategy.ScrapperActivity.Abstract;
using HtmlAgilityPack;

namespace BCMStrategy.ScrapperActivity.Repository
{
	public class StandardTagsAndQuantification : IStandardTagsAndQuantification
	{
		private static readonly EventLogger<StandardTagsAndQuantification> log = new EventLogger<StandardTagsAndQuantification>();

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

		#endregion General Variables


		/// <summary>
		/// Finding the Standard tags
		/// </summary>
		/// <param name="loaderLinkQueue"></param>
		public void StandardTags(LoaderLinkQueue loaderLinkQueue)
		{
			try
			{
				log.LogEntry("StandardTags: ", loaderLinkQueue);
				WebLinkStandardTagsModel webLinkStandardList = StandardTagList(loaderLinkQueue);
				if (webLinkStandardList != null && loaderLinkQueue.WebLinkTypeId == (int)Helper.WebSiteTypes.OfficialSector)
				{
					StandardTagsOfficialSectors(loaderLinkQueue, webLinkStandardList);
				}
				else
				{
					StandardTagsMediaSectors(loaderLinkQueue, webLinkStandardList);
				}
				////else
				////{
				////  searchParameters.FreeSearch = "guidId:\"" + loaderLinkQueue.GUID + "\"";
				////  PageDetailsView pageDetailsView = GlobalConfiguration.Get(searchParameters);
				////  if (pageDetailsView.Products.Count > 0)
				////  {
				////    var pageSource = pageDetailsView.Products.Select(x => x).FirstOrDefault().PageSource[0];
				////    webLinkStandardList.DateOfIssue = ExtractDate(pageSource);
				////  }
				////  bool result = AddStandardResultsForHardCoded(loaderLinkQueue, webLinkStandardList);
				////}
				log.LogEntry("End of StandardTags");
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "StandardTags: Exception is thrown in Main method", ex, null);
			}
		}

		#region Standard Tags

		#region Official Sectors Standard Tags

		/// <summary>
		/// Standard tags for Official Sectors
		/// </summary>
		/// <param name="loaderLinkQueue">Web Page details</param>
		/// <param name="webLinkStandardList">Standard Detail of Web Pages</param>
		private void StandardTagsOfficialSectors(LoaderLinkQueue loaderLinkQueue, WebLinkStandardTagsModel webLinkStandardList)
		{
			SolrSearchParameters searchParameters = new SolrSearchParameters();

			List<string> standardTagsTypeArray = webLinkStandardList.IndividualsList.Select(x => string.Format("{0} {1}", x.IndividualFirstName, x.IndividualLastName)).ToList();
			standardTagsTypeArray.AddRange(webLinkStandardList.IndividualsList.Select(x => string.Format("{0} {1}", x.Designation, x.IndividualLastName)).ToList());
			standardTagsTypeArray.AddRange(webLinkStandardList.SectorsList.Select(x => x.SectorName).ToList());
			if (standardTagsTypeArray.Any())
			{
				searchParameters.FreeSearch = "guidId:\"" + loaderLinkQueue.GUID + "\" AND pageSource:(" + "\"" + string.Join("\" OR \"", standardTagsTypeArray) + "\"" + ")";
				PageDetailsView pageDetailsView = GlobalConfiguration.Get(searchParameters);

				if (pageDetailsView.Highlight.Count > 0)
				{
					var highlightedResult = pageDetailsView.Highlight.Select(x => x).FirstOrDefault().Value.Values.FirstOrDefault().FirstOrDefault();
					StringBuilder htmlResult = FetchHighlightedResult(highlightedResult);
					//// webLinkStandardList.DateOfIssue = ExtractDate(highlightedResult);
					webLinkStandardList.DateOfIssue = Helper.GetCurrentDateTime();
					AddStandardResults(loaderLinkQueue, htmlResult, webLinkStandardList);
				}
				else
				{
					searchParameters.FreeSearch = "guidId:\"" + loaderLinkQueue.GUID + "\"";
					pageDetailsView = GlobalConfiguration.Get(searchParameters);
					if (pageDetailsView.Products.Count > 0)
					{
						/////var pageSource = pageDetailsView.Products.Select(x => x).FirstOrDefault().PageSource[0];
						//// webLinkStandardList.DateOfIssue = ExtractDate(pageSource);
						webLinkStandardList.DateOfIssue = Helper.GetCurrentDateTime();
					}
					AddStandardResults(loaderLinkQueue, new StringBuilder(), webLinkStandardList);
				}
			}
			else
			{
				searchParameters.FreeSearch = "guidId:\"" + loaderLinkQueue.GUID + "\"";
				PageDetailsView pageDetailsView = GlobalConfiguration.Get(searchParameters);
				if (pageDetailsView.Products.Count > 0)
				{
	
					//// webLinkStandardList.DateOfIssue = ExtractDate(pageSource);
					webLinkStandardList.DateOfIssue = Helper.GetCurrentDateTime();
				}
				AddStandardResults(loaderLinkQueue, new StringBuilder(), webLinkStandardList);
			}
		}

		#endregion Official Sectors Standard Tags

		#region Media Sectors Standard Tags

		/// <summary>
		/// Standards tags for Media Sectors
		/// </summary>
		/// <param name="loaderLinkQueue">Web page Details</param>
		private void StandardTagsMediaSectors(LoaderLinkQueue loaderLinkQueue, WebLinkStandardTagsModel webLinkStandardList)
		{
			SolrSearchParameters searchParameters = new SolrSearchParameters();
			WebLinkStandardTagsMasterModel webLinkStandardTagsMasterModel = new WebLinkStandardTagsMasterModel()
			{
				PolicyMakersList = GetListOfIndividuals(),
				InstitutionsList = GetListOfInstitutions()
			};

			if (webLinkStandardTagsMasterModel.PolicyMakersList.Any() || webLinkStandardTagsMasterModel.InstitutionsList.Any())
			{
				List<string> individualsArray = webLinkStandardTagsMasterModel.PolicyMakersList.Where(x => !string.IsNullOrEmpty(x.IndividualLastName)).Select(x => x.IndividualFirstNLastName).ToList();
				individualsArray.AddRange(webLinkStandardTagsMasterModel.PolicyMakersList.Where(x => !string.IsNullOrEmpty(x.IndividualLastName)).Select(x => x.IndividualDesignationNLName).ToList());

				individualsArray.AddRange(webLinkStandardTagsMasterModel.InstitutionsList.Where(x => !string.IsNullOrEmpty(x.InstitutionsName)).Select(x => x.InstitutionsName).ToList());
				individualsArray.AddRange(webLinkStandardTagsMasterModel.InstitutionsList.Where(x => !string.IsNullOrEmpty(x.InstitutionsAbbreviation)).Select(x => x.InstitutionsAbbreviation).ToList());

				individualsArray.AddRange(webLinkStandardList.SectorsList.Select(x => x.SectorName).ToList());

				searchParameters.FreeSearch = "guidId:\"" + loaderLinkQueue.GUID + "\" AND pageSource:(" + "\"" + string.Join("\" OR \"", individualsArray) + "\"" + ")";
				PageDetailsView pageDetailsView = GlobalConfiguration.Get(searchParameters);

				if (pageDetailsView.Highlight.Any())
				{
					var highlightedResult = pageDetailsView.Highlight.Select(x => x).FirstOrDefault().Value.Values.FirstOrDefault().FirstOrDefault();
					StringBuilder htmlResult = FetchHighlightedResult(highlightedResult);
					//// webLinkStandardList.DateOfIssue = ExtractDate(highlightedResult);
					webLinkStandardList.DateOfIssue = Helper.GetCurrentDateTime();
					AddStandardResults(loaderLinkQueue, htmlResult, webLinkStandardList, webLinkStandardTagsMasterModel);
				}
			}
			////  else
			////  {
			////    StandardTagsWithStandAlone(loaderLinkQueue, webLinkStandardList);
			////  }
			////}
			////else
			////{
			////  StandardTagsWithStandAlone(loaderLinkQueue, webLinkStandardList);
			////}
		}

		////private void StandardTagsWithStandAlone(LoaderLinkQueue loaderLinkQueue, WebLinkStandardTagsModel webLinkStandardList)
		////{
		////  SolrSearchParameters searchParameters = new SolrSearchParameters();

		////  WebLinkStandardTagsMasterModel webLinkStandardTagsMasterModel = new WebLinkStandardTagsMasterModel()
		////  {
		////    ////LegislatorLists = GetListOfLegislator(),
		////    StateHeadList = GetListOfStateHeads()
		////  };

		////  List<string> stateHeadFNLArray = webLinkStandardTagsMasterModel.StateHeadList.Where(x => !string.IsNullOrEmpty(x.StateHeadLastName)).Select(x => x.StateHeadFirstNLastName).ToList();
		////  List<string> stateHeadDNLArray = webLinkStandardTagsMasterModel.StateHeadList.Where(x => !string.IsNullOrEmpty(x.StateHeadLastName)).Select(x => x.StateHeadDesignationNLName).ToList();

		////  List<string> searchInSolList = stateHeadFNLArray;
		////  searchInSolList.AddRange(stateHeadFNLArray);

		////  if (searchInSolList.Any())
		////  {
		////    searchParameters.FreeSearch = "guidId:\"" + loaderLinkQueue.GUID + "\" AND pageSource:(" + "\"" + string.Join("\" OR \"", searchInSolList) + "\"" + ")";
		////    PageDetailsView pageDetailsView = GlobalConfiguration.Get(searchParameters);

		////    if (pageDetailsView.Highlight.Any())
		////    {
		////      var highlightedResult = pageDetailsView.Highlight.Select(x => x).FirstOrDefault().Value.Values.FirstOrDefault().FirstOrDefault();
		////      StringBuilder htmlResult = FetchHighlightedResult(highlightedResult);
		////      webLinkStandardList.DateOfIssue = ExtractDate(highlightedResult);
		////      AddStandardResults(loaderLinkQueue, htmlResult, webLinkStandardList, webLinkStandardTagsMasterModel);
		////    }
		////  }
		////}

		#endregion Media Sectors Standard Tags

		/// <summary>
		/// Adding the Standard tags that are found in the Html or HardCoded
		/// </summary>
		/// <param name="loaderLinkQueue">Web link Details</param>
		/// <param name="htmlResult">Content of Html</param>
		/// <param name="webLinkStandardList">Detail of Web links</param>
		private void AddStandardResults(LoaderLinkQueue loaderLinkQueue, StringBuilder htmlResult, WebLinkStandardTagsModel webLinkStandardList, WebLinkStandardTagsMasterModel webLinkStandardTagsMasterModel = null)
		{
			log.LogEntry("AddStandardResults: ", loaderLinkQueue, htmlResult);
			WebLinkStandardTagsModel accutalProrietoryTagCountList;

			try
			{
				if (loaderLinkQueue.WebLinkTypeId == (int)Helper.WebSiteTypes.OfficialSector)
				{
					accutalProrietoryTagCountList = AddStandardTagsOfficialSectorResults(htmlResult, webLinkStandardList);
				}
				else
				{
					accutalProrietoryTagCountList = AddStandardTagsMediaSectorResults(htmlResult, webLinkStandardList, webLinkStandardTagsMasterModel);
				}
				ScrapperActivity.AddStandardResults(loaderLinkQueue, accutalProrietoryTagCountList);
				log.LogEntry("End Of AddStandardResults");
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "AddStandardResults: Exception is thrown in Main method", ex, null);
			}
			///return isSave;
		}

		private WebLinkStandardTagsModel AddStandardTagsOfficialSectorResults(StringBuilder htmlResult, WebLinkStandardTagsModel webLinkStandardList)
		{
			WebLinkStandardTagsModel accutalProrietoryTagCountList;
			if (!string.IsNullOrEmpty(htmlResult.ToString()))
			{
				var htmlDoc = new HtmlDocument();
				htmlDoc.LoadHtml(htmlResult.ToString());
				var standardList = htmlDoc.DocumentNode.Descendants("b").Select(x => x.InnerHtml.ToLower()).GroupBy(x => x).Select(x => new { Standard = x.Key, StandardCount = x.Count() }).ToList();

				var listOfPolicyMakersFound = webLinkStandardList.IndividualsList.Where(x => standardList.Any(y => x.IndividualFirstNLastName.ToLower() == y.Standard.ToLower() || x.IndividualDesignationNLName.ToLower() == y.Standard.ToLower())).ToList();

				var listOfSectorsFound = webLinkStandardList.SectorsList.Where(x => standardList.Any(y => x.SectorName == y.Standard)).ToList();

				accutalProrietoryTagCountList = new WebLinkStandardTagsModel()
				{
					DateOfIssue = webLinkStandardList.DateOfIssue,
					EntityTypeList = webLinkStandardList.EntityTypeList,
					////CountryId = webLinkStandardList.CountryId,
					////EntityId = webLinkStandardList.EntityId,
					////EntityTypeId = webLinkStandardList.EntityTypeId,
					IndividualsList = listOfPolicyMakersFound,
					Content = htmlResult.ToString(),
					SectorsList = listOfSectorsFound
				};
			}
			else
			{
				accutalProrietoryTagCountList = new WebLinkStandardTagsModel()
				{
					DateOfIssue = webLinkStandardList.DateOfIssue,
					EntityTypeList = webLinkStandardList.EntityTypeList
					////CountryId = webLinkStandardList.CountryId,
					////EntityId = webLinkStandardList.EntityId,
					////EntityTypeId = webLinkStandardList.EntityTypeId
				};
			}
			return accutalProrietoryTagCountList;
		}

		private WebLinkStandardTagsModel AddStandardTagsMediaSectorResults(StringBuilder htmlResult, WebLinkStandardTagsModel webLinkStandardList, WebLinkStandardTagsMasterModel webLinkStandardTagsMasterModel)
		{
			WebLinkStandardTagsModel accutalProrietoryTagCountList = new WebLinkStandardTagsModel();
			if (!string.IsNullOrEmpty(htmlResult.ToString()))
			{
				var htmlDoc = new HtmlDocument();

				htmlDoc.LoadHtml(htmlResult.ToString());

				var standardList = htmlDoc.DocumentNode.Descendants("b").Select(x => x.InnerHtml.ToLower()).GroupBy(x => x).Where(x => x.Key.ToLower() != "").Select(x => new { Standard = x.Key.ToLower(), StandardCount = x.Count() }).ToList();

				var listOfPolicyMakers = webLinkStandardTagsMasterModel.PolicyMakersList != null ?
																											webLinkStandardTagsMasterModel.PolicyMakersList
																																	.Where(x => standardList.Any(y => x.IndividualFirstNLastName.ToLower() == y.Standard ||
																																																		x.IndividualDesignationNLName.ToLower() == y.Standard)).ToList() :
																											webLinkStandardTagsMasterModel.PolicyMakersList;

				var listOfInstitutions = webLinkStandardTagsMasterModel.InstitutionsList != null ?
																												webLinkStandardTagsMasterModel.InstitutionsList.Where(x => standardList.Any(y => x.InstitutionsName.ToLower() == y.Standard ||
																																																																				 x.InstitutionsAbbreviation.ToLower() == y.Standard)).ToList() :
																												webLinkStandardTagsMasterModel.InstitutionsList;

				var listOfSectors = webLinkStandardList.SectorsList.Any() ? webLinkStandardList.SectorsList.Where(x => standardList.Any(y => x.SectorName.ToLower() == y.Standard)).ToList() : webLinkStandardList.SectorsList;

				var listOfStateHeadFound = webLinkStandardTagsMasterModel.StateHeadList != null ? webLinkStandardTagsMasterModel.StateHeadList.Where(x => standardList.Any(y => x.StateHeadDesignationNLName.ToLower() == y.Standard || x.StateHeadFirstNLastName.ToLower() == y.Standard)).ToList() : webLinkStandardTagsMasterModel.StateHeadList;

				accutalProrietoryTagCountList = new WebLinkStandardTagsModel()
				{
					DateOfIssue = webLinkStandardList.DateOfIssue,
					Content = htmlResult.ToString(),
					IndividualsList = listOfStateHeadFound != null && listOfStateHeadFound.Any() ? ConvertToScrappedStandardTagsHeadStateList(listOfStateHeadFound) : ConvertToScrappedStandardTagsIndividualList(listOfPolicyMakers),
					EntityTypeList = listOfInstitutions != null && listOfInstitutions.Any() ? ConvertToScrappedStandardTagsEntityList(listOfInstitutions) : ConvertToScrappedStandardTagsEntityList(listOfPolicyMakers),
					SectorsList = listOfSectors
				};
			}
			return accutalProrietoryTagCountList;
		}

		private List<WebLinkIndividualsModel> ConvertToScrappedStandardTagsHeadStateList(List<MediaSectorStateHeadModel> headStateDataList)
		{
			List<WebLinkIndividualsModel> listOfScrappedHeadStateTags = new List<WebLinkIndividualsModel>();
			log.LogEntry("ConvertToScrappedStandardTagsHeadStateList: ", headStateDataList);
			try
			{
				if (headStateDataList != null && headStateDataList.Any())
				{
					headStateDataList.ForEach(headState =>
					{
						WebLinkIndividualsModel scrappedHeadStateTags = new WebLinkIndividualsModel()
						{
							IndividualId = headState.StateHeadId,
							IndividualFirstName = headState.StateHeadFirstName,
							IndividualLastName = headState.StateHeadLastName,
							CountryId = headState.CountryId,
							Designation = headState.Designation
						};
						listOfScrappedHeadStateTags.Add(scrappedHeadStateTags);
					});
				}
				log.LogEntry("End Of ConvertToScrappedStandardTagsHeadStateList: ", headStateDataList);
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "ConvertToScrappedStandardTagsHeadStateList: Exception is thrown in Main method", ex, null);
			}
			return listOfScrappedHeadStateTags;
		}

		private List<WebLinkIndividualsModel> ConvertToScrappedStandardTagsIndividualList(List<MediaSectorIndividualsModel> individualDataList)
		{
			List<WebLinkIndividualsModel> listOfScrappedHeadStateTags = new List<WebLinkIndividualsModel>();
			log.LogEntry("ConvertToScrappedStandardTagsIndividualList: ", individualDataList);
			try
			{
				if (individualDataList != null && individualDataList.Any())
				{
					individualDataList.ForEach(individual =>
					{
						WebLinkIndividualsModel scrappedHeadStateTags = new WebLinkIndividualsModel()
						{
							IndividualFirstName = individual.IndividualFirstName,
							IndividualLastName = individual.IndividualLastName,
							CountryId = individual.CountryId,
							Designation = individual.Designation,
							IndividualId = individual.IndividualId,
							InstiutionTypeId = individual.InstiutionTypeId
						};
						listOfScrappedHeadStateTags.Add(scrappedHeadStateTags);
					});
				}
				log.LogEntry("End Of ConvertToScrappedStandardTagsHeadStateList: ", individualDataList);
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "ConvertToScrappedStandardTagsHeadStateList: Exception is thrown in Main method", ex, null);
			}
			return listOfScrappedHeadStateTags;
		}

		private List<WebLinkEntityModel> ConvertToScrappedStandardTagsEntityList(List<MediaSectorIndividualsModel> individualDataList)
		{
			List<WebLinkEntityModel> listOfScrappedEntityTags = new List<WebLinkEntityModel>();
			log.LogEntry("ConvertToScrappedStandardTagsEntityList: ", individualDataList);
			try
			{
				if (individualDataList != null && individualDataList.Any())
				{
					individualDataList.ForEach(individual =>
					{
						if (individual.InstitutionId != null && individual.InstitutionId.Any())
						{
							individual.InstitutionId.ForEach(entity =>
							{
								WebLinkEntityModel scrappedHeadStateTags = new WebLinkEntityModel()
								{
									CountryId = individual.CountryId,
									EntityId = !string.IsNullOrEmpty(entity) ? entity.ToInt32() : (int?)null,
									EntityTypeId = individual.InstiutionTypeId
								};
								listOfScrappedEntityTags.Add(scrappedHeadStateTags);
							});
						}
						else
						{
							WebLinkEntityModel scrappedHeadStateTags = new WebLinkEntityModel()
							{
								CountryId = individual.CountryId,
								EntityId = (int?)null,
								EntityTypeId = individual.InstiutionTypeId
							};
							listOfScrappedEntityTags.Add(scrappedHeadStateTags);
						}

					});
				}
				log.LogEntry("End Of ConvertToScrappedStandardTagsHeadStateList: ", individualDataList);
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "ConvertToScrappedStandardTagsEntityList: Exception is thrown in Main method", ex, null);
			}
			return listOfScrappedEntityTags;
		}

		private List<WebLinkEntityModel> ConvertToScrappedStandardTagsEntityList(List<MediaSectorInstitutionsModel> institutionsDataList)
		{
			List<WebLinkEntityModel> listOfScrappedEntityTags = new List<WebLinkEntityModel>();
			log.LogEntry("ConvertToScrappedStandardTagsEntityList: ", institutionsDataList);
			try
			{
				if (institutionsDataList != null && institutionsDataList.Any())
				{
					institutionsDataList.ForEach(individual =>
					{
						WebLinkEntityModel scrappedHeadStateTags = new WebLinkEntityModel()
						{
							CountryId = individual.CountryId,
							EntityId = individual.InstitutionsId > 0 ? individual.InstitutionsId : (int?)null,
							EntityTypeId = individual.InstiutionTypeId
						};
						listOfScrappedEntityTags.Add(scrappedHeadStateTags);
					});
				}
				log.LogEntry("End Of ConvertToScrappedStandardTagsHeadStateList: ", institutionsDataList);
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "ConvertToScrappedStandardTagsEntityList: Exception is thrown in Main method", ex, null);
			}
			return listOfScrappedEntityTags;
		}

		/// <summary>
		/// Insert the Standard Details of Hardcoded
		/// </summary>
		/// <param name="loaderLinkQueue">Web link details</param>
		/// <param name="webLinkStandardList">Details of Weblinks</param>
		/// <returns></returns>
		////private bool AddStandardResultsForHardCoded(LoaderLinkQueue loaderLinkQueue, WebLinkStandardTagsModel webLinkStandardList)
		////{
		////  bool isSave = false;
		////  log.LogEntry("AddActivityTypeResultsForHardCoded: ", loaderLinkQueue);
		////  try
		////  {
		////    isSave = ScrapperActivity.AddStandardResults(loaderLinkQueue, webLinkStandardList);
		////    log.LogEntry("End Of AddStandardResultsForHardCoded");
		////  }
		////  catch (Exception ex)
		////  {
		////    log.LogError(LoggingLevel.Error, "BadRequest", "AddStandardResultsForHardCoded: Exception is thrown in Main method", ex, null);
		////  }
		////  return isSave;
		////}

		#endregion Standard Tags

		#region General Block
		/// <summary>
		/// Extract ActivityTypeResult From html content
		/// </summary>
		/// <param name="htmlContent">Page Content</param>
		/// <returns>Fetch Paragraph from content and store in StringBuilder</returns>
		public StringBuilder FetchHighlightedResult(string htmlContent)
		{
			try
			{
				StringBuilder htmlResult = new StringBuilder();
				log.LogEntry("FetchActivityTypesResult", htmlContent);
				var htmlDoc = new HtmlDocument();
				htmlDoc.LoadHtml(htmlContent);

				var nodeB = htmlDoc.DocumentNode.SelectNodes("//b");
				if (nodeB != null)
				{
					List<string> paraCollections = new List<string>();
					foreach (var nod in nodeB)
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
						htmlResult.Append(paragraph);
					}
					log.LogEntry("Output Result FetchActivityTypesResult: ", htmlResult);
					log.LogEntry("End Of FetchActivityTypesResult");
				}
				return htmlResult;
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "FetchActivityTypesResult: Exception is thrown in Main method", ex, null);
				return new StringBuilder();
			}
		}
		#endregion General Block

		#region Helper Methods


		private WebLinkStandardTagsModel StandardTagList(LoaderLinkQueue loaderLinkQueue)
		{
			return MetaData.StandardTagList(loaderLinkQueue);
		}

		private List<MediaSectorIndividualsModel> GetListOfIndividuals()
		{
			return MetaData.GetAllPolicyMakerListForScrapperActivity();
		}

		private List<MediaSectorInstitutionsModel> GetListOfInstitutions()
		{
			return MetaData.GetAllInstitutionsListForScrapperActivity();
		}
		#endregion Helper Methods
	}
}