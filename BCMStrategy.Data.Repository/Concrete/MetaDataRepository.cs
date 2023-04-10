using System.Collections.Generic;
using System.Linq;
using BCMStrategy.DAL.Context;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;

namespace BCMStrategy.Data.Repository.Concrete
{
	public class MetaDataRepository : IMetaData
	{
		/// <summary>
		/// Get the List of Meta Data Values
		/// </summary>
		/// <returns></returns>
		public List<WebLinkMetaDataModel> GetMetaDataListForScrapingActvityType(LoaderLinkQueue loaderLinkQueue)
		{
			List<WebLinkMetaDataModel> metaDataList = new List<WebLinkMetaDataModel>();
			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{
				int[] metaDataTypes = db.weblinkproprietarytags.Where(x => x.WeblinkId == loaderLinkQueue.WebSiteId && x.weblinks.IsActive && !x.weblinks.IsDeleted).Select(x => x.MetaDataTypeId).ToArray();

				////metaDataList = db.metadatavalue
				////  .Where(x => x.metadatatypes.websitetypes.TypeId == loaderLinkQueue.WebLinkTypeId && x.metadatatypes.IsDeleted == false
				////                   && metaDataTypes.Contains(x.MetaDataTypeId)
				////  )
				////  .Select(x => new WebLinkMetaDataModel()
				////    {
				////      ActivityTypeMasterId = x.ActivityTypeId.HasValue ? x.ActivityTypeId.Value : (int?)null,
				////      ActivityName = x.ActivityTypeId.HasValue ? x.activitytype.ActivityName : string.Empty,
				////      ActivityValue = x.ActivityValue,
				////      MetaDataMasterId = x.MetaDataTypeId,
				////      MetaDataName = x.metadatatypes.MetaData
				////    }).ToList();
				if (metaDataTypes.Any())
				{
					var attachedActivityTypeWithWebLink = db.weblinkactivitytype.Where(x => x.WebLinkId == loaderLinkQueue.WebSiteId && x.weblinks.IsActive && !x.weblinks.IsDeleted && !x.metadatavalue.activitytype.IsDeleted 
																&& metaDataTypes.Contains(x.metadatavalue.MetaDataTypeId)).ToList();

					var metaDataValueList = db.metadatavalue
						 .Where(x => x.metadatatypes.websitetypes.TypeId == loaderLinkQueue.WebLinkTypeId &&
												 !x.metadatatypes.IsDeleted && 
												 !x.activitytype.IsDeleted && 
												 metaDataTypes.Contains(x.MetaDataTypeId)).ToList();

					metaDataList = metaDataValueList.Where(x =>
																			(attachedActivityTypeWithWebLink == null ||
																			 attachedActivityTypeWithWebLink.Count == 0 ||
																			 (attachedActivityTypeWithWebLink.Count > 0 &&
																				attachedActivityTypeWithWebLink.Select(y => y.MetaDataTypeId).Contains(x.Id))))
						 .Select(x => new WebLinkMetaDataModel()
						 {
							 ActivityTypeMasterId = x.ActivityTypeId.HasValue ? x.ActivityTypeId.Value : (int?)null,
							 ActivityName = x.ActivityTypeId.HasValue ? x.activitytype.ActivityName : string.Empty,
							 ActivityValue = x.ActivityValue,
							 MetaDataMasterId = x.MetaDataTypeId,
							 MetaDataName = x.metadatatypes.MetaData,
							 IsActivityTypeAssignedWebLink = attachedActivityTypeWithWebLink.Any(),
							 IsFullSearchRequired = x.activitytype.IsFullSearchRequired
						 }).Distinct().ToList();

					var relatedActivities = metaDataValueList.Where(x =>
																		 (attachedActivityTypeWithWebLink == null ||
																			attachedActivityTypeWithWebLink.Count == 0 ||
																			(attachedActivityTypeWithWebLink.Count > 0 &&
																			 attachedActivityTypeWithWebLink.Select(y => y.MetaDataTypeId).Contains(x.Id))) && x.activitytype.relatedactivitytypes.Any())
						 .SelectMany(x => x.activitytype.relatedactivitytypes)
						 .Select(x => new WebLinkMetaDataModel()
						 {
							 ActivityTypeMasterId = x.ActivityTypeId.HasValue ? x.ActivityTypeId.Value : (int?)null,
							 ActivityName = x.RelatedActvity,
							 ActivityValue = x.activitytype.metadatavalue.FirstOrDefault().ActivityValue,
							 MetaDataMasterId = x.activitytype.metadatavalue.FirstOrDefault().MetaDataTypeId,
							 MetaDataName = x.activitytype.metadatavalue.FirstOrDefault().metadatatypes.MetaData,
							 IsActivityTypeAssignedWebLink = attachedActivityTypeWithWebLink.Any(),
							 IsFullSearchRequired = x.activitytype.IsFullSearchRequired
						 }).Distinct().ToList();

					metaDataList.AddRange(relatedActivities);
				}
			}
			return metaDataList;
		}

		/// <summary>
		/// Get the List of Phrases Values
		/// </summary>
		/// <returns></returns>
		public List<WebLinkPhraseModel> GetPhrasesListForScraping(LoaderLinkQueue loaderLinkQueue)
		{
			List<WebLinkPhraseModel> metaDataPhraseList;
			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{
				int[] metaDataTypes = db.weblinkproprietarytags.Where(x => x.weblinks.IsActive && !x.weblinks.IsDeleted && x.WeblinkId == loaderLinkQueue.WebSiteId).Select(x => x.MetaDataTypeId).ToArray();

				////  metaDataPhraseList = db.metadatavalue.Join(db.metadataphrases, metaDataValue => metaDataValue.MetaDataTypeId, metaDataPhrase => metaDataPhrase.MetaDataTypeId, (metaDataValue, metaDataPhrase) => new
				////  {
				////    metaDataValue,
				////    metaDataPhrase
				////  }).Where(x => x.metaDataValue.metadatatypes.websitetypes.TypeId == loaderLinkQueue.WebLinkTypeId
				////               && metaDataTypes.Contains(x.metaDataValue.MetaDataTypeId))
				////                      .Select(x => new PhraseModel()
				////                      {
				////                        MetaDataValue = x.metaDataValue.ActivityValue,
				////                        MetaDataMasterId = x.metaDataPhrase.MetaDataTypeId,
				////                        MetaDataName = x.metaDataValue.metadatatypes.MetaData,
				////                        Phrase = x.metaDataPhrase.Phrases,
				////                        PhraseMasterId = x.metaDataPhrase.Id
				////                      }).ToList();
				////}

				metaDataPhraseList = db.metadataphrases.Where(x => !string.IsNullOrEmpty(x.Phrases) &&
																													 x.metadatatypes.websitetypes.TypeId == loaderLinkQueue.WebLinkTypeId &&
																													 !x.metadatatypes.IsDeleted &&
																													 !x.IsDeleted
										 && metaDataTypes.Contains(x.MetaDataTypeId))
														 .Select(x => new WebLinkPhraseModel()
														 {
															 MetaDataValue = x.metadatatypes.IsActivityTypeExist.HasValue && !x.metadatatypes.IsActivityTypeExist.Value ? x.metadatatypes.Value.HasValue ? x.metadatatypes.Value.Value : 0 : x.ActivityTypeId.HasValue && x.ActivityTypeId.Value > 0 ? x.activitytype.metadatavalue.FirstOrDefault().ActivityValue : 0,
															 MetaDataMasterId = x.MetaDataTypeId,
															 MetaDataName = x.metadatatypes.MetaData,
															 Phrase = x.Phrases,
															 PhraseMasterId = x.Id
														 }).ToList();
			}
			return metaDataPhraseList;
		}

		/// <summary>
		/// Get the List of Noun plus Verb Values
		/// </summary>
		/// <returns></returns>
		public List<WebLinkNounPVerbModel> GetNounPVerbListForScraping(LoaderLinkQueue loaderLinkQueue)
		{
			List<WebLinkNounPVerbModel> metaDataNounPVerbList;
			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{
				int[] metaDataTypes = db.weblinkproprietarytags.Where(x => x.weblinks.IsActive && !x.weblinks.IsDeleted && x.WeblinkId == loaderLinkQueue.WebSiteId).Select(x => x.MetaDataTypeId).ToArray();

				metaDataNounPVerbList = db.metadatanounplusverb.Where(x => !x.IsDeleted && x.metadatatypes.websitetypes.TypeId == loaderLinkQueue.WebLinkTypeId && 
          !x.metadatatypes.IsDeleted
										&& metaDataTypes.Contains(x.MetaDataTypeId))
														.Select(x => new WebLinkNounPVerbModel
														{
															MetaDataValue = x.metadatatypes.IsActivityTypeExist.HasValue && !x.metadatatypes.IsActivityTypeExist.Value ? x.metadatatypes.Value.HasValue ? x.metadatatypes.Value.Value : 0 : x.ActivityTypeId.HasValue && x.ActivityTypeId.Value > 0 ? x.activitytype.metadatavalue.FirstOrDefault().ActivityValue : 0,
															MetaDataMasterId = x.MetaDataTypeId,
															MetaDataName = x.metadatatypes.MetaData,
															Verb = x.Verb,
															Noun = x.Noun,
															NounPVerbMasterId = x.Id,
															IsHardCode = x.IsHardCoded,
															ActivityTypeId = x.ActivityTypeId
														}).ToList();
			}
			return metaDataNounPVerbList;
		}

		/// <summary>
		/// Get the List of Dynamic Tables based on Verb and Noun
		/// </summary>
		/// <param name="nounPVerbModel">List of Noun Plus Verb</param>
		/// <returns></returns>
		public List<DynamicNounPVerbResultModel> GetListOfDynmicTable(WebLinkNounPVerbModel nounPVerbModel)
		{
			List<DynamicNounPVerbResultModel> metaDataNounPVerbList = new List<DynamicNounPVerbResultModel>();
			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{
				var listOfDynamicTable = db.dynamictablesfieldsfornounplusverb.Where(x => x.dynamictablesfornounplusverb.DynamicType == nounPVerbModel.Noun).ToList();
				if (listOfDynamicTable != null)
				{
					foreach (var tables in listOfDynamicTable)
					{
						string restOfArray = string.Format("CONCAT_WS(' ',{0})", tables.Fields.Replace(";", ","));

						string query = string.Format("SELECT Distinct {0} As Name FROM {1}", restOfArray, tables.dynamictablesfornounplusverb.TableName);
						if (!string.IsNullOrEmpty(tables.ReferenceColumnId) && !string.IsNullOrEmpty(tables.ReferenceTable))
						{
							query += string.Format(" Inner Join {0} On {1} = {2}", tables.ReferenceTable, tables.PrimaryId, tables.ReferenceColumnId);
						}

						query += string.Format(" Where {0}.IsDeleted=false", tables.dynamictablesfornounplusverb.TableName);

						List<DynamicNounPVerbResultModel> metaDataNounPVerb = db.Database.SqlQuery<DynamicNounPVerbResultModel>(query, "FirstName").Select(x => new DynamicNounPVerbResultModel() { Name = x.Name }).ToList();
						metaDataNounPVerbList.AddRange(metaDataNounPVerb.Where(x => !string.IsNullOrEmpty(x.Name)));
					}
				}
			}
			return metaDataNounPVerbList;
		}

		/// <summary>
		/// Get the list of Standard Tags List
		/// </summary>
		/// <param name="loaderLinkQueue">Weblink Details</param>
		/// <returns>list of Standard Tags List</returns>
		public WebLinkStandardTagsModel StandardTagList(LoaderLinkQueue loaderLinkQueue)
		{
			WebLinkStandardTagsModel standardTagsList;
			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{
				//// db.Database.Log = msg => System.Diagnostics.Debug.WriteLine(msg);
				var standardTagsListTemp = db.weblinks.Where(x => x.Id == loaderLinkQueue.WebSiteId && !x.IsDeleted).Select(x => new
				{
					EntityTypeList = x,
					IndividualsList = x.weblinkpolicymaker,
					SectorsList = x.weblinksector
				}).FirstOrDefault();

				standardTagsList = new WebLinkStandardTagsModel()
				{
					EntityTypeList = new List<WebLinkEntityModel>()
					{
						new WebLinkEntityModel() {
							CountryId = standardTagsListTemp.EntityTypeList.CountryId.HasValue ? standardTagsListTemp.EntityTypeList.CountryId.Value : (int?)null,
							CountryName = standardTagsListTemp.EntityTypeList.CountryId.HasValue ? standardTagsListTemp.EntityTypeList.country.Name : string.Empty,
							EntityId = standardTagsListTemp.EntityTypeList.EntityId.HasValue ? standardTagsListTemp.EntityTypeList.EntityId.Value : (int?)null,
							EntityName = standardTagsListTemp.EntityTypeList.EntityId.HasValue ? standardTagsListTemp.EntityTypeList.institution.InstitutionName : string.Empty,
							EntityTypeId = standardTagsListTemp.EntityTypeList.InstitutionTypeId.HasValue ? standardTagsListTemp.EntityTypeList.InstitutionTypeId.Value : (int?)null,
							EntityTypeName = standardTagsListTemp.EntityTypeList.InstitutionTypeId.HasValue ? standardTagsListTemp.EntityTypeList.institutiontypes.InstitutionType : string.Empty
						}
					},
					IndividualsList = standardTagsListTemp.IndividualsList.Select(y => new WebLinkIndividualsModel()
					{
						IndividualId = y.PolicyMakerId,
						IndividualFirstName = y.policymakers.PolicyFirstName,
						IndividualLastName = y.policymakers.PolicyLastName,
						Designation = y.policymakers.designation.Designation1
					}).ToList(),
					SectorsList = standardTagsListTemp.SectorsList.Any() ? standardTagsListTemp.SectorsList.Select(y => new WebLinkSectorsModel()
					{
						SectorId = y.SectorId,
						SectorName = y.sector.SectorName
					}).ToList() : db.sector.Select(y => new WebLinkSectorsModel()
					{
						SectorId = y.Id,
						SectorName = y.SectorName
					}).ToList()
				};
			}
			return standardTagsList;
		}

		/////// <summary>
		/////// Get all the lagislative list for scrapper activity
		/////// </summary>
		/////// <returns>List of Lagislative</returns>
		////public List<MediaSectorLegislatorModel> GetAllLegislatorListForScrapperActivity()
		////{
		////  List<MediaSectorLegislatorModel> legislatorList = new List<MediaSectorLegislatorModel>();
		////  using (BCMStrategyEntities db = new BCMStrategyEntities())
		////  {
		////    legislatorList = db.individuallegislators.Where(a => a.IsDeleted != true)
		////        .Select(x => new MediaSectorLegislatorModel()
		////        {
		////          LegislatorId = x.Id,
		////          LegislatorFirstName = x.FirstName,
		////          LegislatorLastName = x.LastName,
		////          CountryId = x.CountryId.HasValue ? x.CountryId.Value : x.CountryId,
		////          Designation = x.
		////        }).ToList();
		////  }
		////  return legislatorList;
		////}

		/// <summary>
		/// Get the list of PolicyMakers to find Standard tags in Media Sectors
		/// </summary>
		/// <returns>List of Individuals</returns>
		public List<MediaSectorIndividualsModel> GetAllPolicyMakerListForScrapperActivity()
		{
			List<MediaSectorIndividualsModel> policyMakerList;

			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{
				policyMakerList = db.policymakers
					.Where(x => !x.IsDeleted)
						.Select(x => new MediaSectorIndividualsModel()
						{
							IndividualId = x.Id,
							CountryId = x.CountryId.HasValue ? x.CountryId.Value : x.CountryId,
							InstiutionTypeId = x.InstitutionId,
							IndividualFirstName = x.PolicyFirstName ?? string.Empty,
							IndividualLastName = x.PolicyLastName ?? string.Empty,
							Designation = x.designation.Designation1,
							InstitutionId = x.institutiontypes.institution.Where(y => y.InstitutionTypeId == x.InstitutionId && y.CountryId == x.CountryId).Select(y => y.Id.ToString()).ToList()
						}).ToList();

				List<MediaSectorIndividualsModel> stateHeadList = db.statehead
																				.Where(x => !x.IsDeleted)
																					.Select(x => new MediaSectorIndividualsModel()
																					{
																						IndividualId = x.Id,
																						CountryId = x.CountryId,
																						IndividualFirstName = x.FirstName ?? string.Empty,
																						IndividualLastName = x.LastName ?? string.Empty,
																						Designation = x.designation.Designation1
																					}).ToList();

				List<MediaSectorIndividualsModel> LagistlaterList = db.individuallegislatorsdesignation
																			 .Where(x => !x.individuallegislators.IsDeleted)
																				 .Select(x => new MediaSectorIndividualsModel()
																				 {
																					 IndividualId = x.individuallegislators.Id,
																					 CountryId = x.individuallegislators.CountryId,
																					 IndividualFirstName = x.individuallegislators.FirstName ?? string.Empty,
																					 IndividualLastName = x.individuallegislators.LastName ?? string.Empty,
																					 Designation = x.designation.Designation1
																				 }).ToList();

				policyMakerList.AddRange(stateHeadList);
				policyMakerList.AddRange(LagistlaterList);
			}
			return policyMakerList;
		}

		/// <summary>
		/// Get the list of Media Sectors Institutions
		/// </summary>
		/// <returns>List of all Institutions</returns>
		public List<MediaSectorInstitutionsModel> GetAllInstitutionsListForScrapperActivity()
		{
			List<MediaSectorInstitutionsModel> institutionsList;

			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{
				institutionsList = db.institution
					.Where(x => !x.IsDeleted)
						.Select(x => new MediaSectorInstitutionsModel()
						{
							InstitutionsId = x.Id,
							CountryId = x.CountryId.HasValue ? x.CountryId.Value : x.CountryId,
							InstiutionTypeId = x.InstitutionTypeId,
							InstitutionsName = x.InstitutionName ?? string.Empty,
							InstitutionsAbbreviation = x.EntityName ?? string.Empty
						}).ToList();
			}
			return institutionsList;
		}

		/// <summary>
		/// Get the list of State Heads for ScapperActivity
		/// </summary>
		/// <returns>List </returns>
		public List<MediaSectorStateHeadModel> GetStateHeadListForScrapperActivity()
		{
			List<MediaSectorStateHeadModel> stateHeadList;
			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{
				stateHeadList = db.statehead
					.Where(x => !x.IsDeleted)
						.Select(x => new MediaSectorStateHeadModel()
						{
							StateHeadId = x.Id,
							CountryId = x.CountryId,
							StateHeadFirstName = x.FirstName ?? string.Empty,
							StateHeadLastName = x.LastName ?? string.Empty,
							Designation = x.designation.Designation1
						}).ToList();
			}
			return stateHeadList;
		}
	}
}