using BCMStrategy.DAL.Context;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Resources;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCMStrategy.Data.Abstract;
using System.Configuration;

namespace BCMStrategy.Data.Repository.Concrete
{
  public class ScrappingProcessRepository : IScrappingProcess
  {
    /// <summary>
    /// Get all list of scrapping data
    /// </summary>
    /// <param name="parametersJson">Grid Parameter to filter or sorting</param>
    /// <returns>return the list</returns>
    public async Task<ApiOutput> GetAllScrappedURLList(string webSiteHashId, int webSiteType, string processEventId)
    {
      ApiOutput apiOutput = new ApiOutput();
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        var processId = 0;////= (int)(db.loaderlinkqueue.Where(w => w.weblinks.WebSiteTypeId == webSiteType).Max(s => s.ProcessId));
        var idFromConfig = ConfigurationManager.AppSettings["ScrappingProcessId"].ToInt32();
        var scrappingProcessId = processEventId == "0" ? idFromConfig : processEventId.ToDecrypt().ToInt32();

        if (scrappingProcessId != 0)
        {
          processId = (db.loaderlinkqueue.Where(a => a.ProcessId <= scrappingProcessId && a.weblinks.WebSiteTypeId == webSiteType).Max(s => s.ProcessId));
        }
        else
        {
          processId = (db.loaderlinkqueue.Where(w => w.weblinks.WebSiteTypeId == webSiteType).Max(s => s.ProcessId));
        }

        ////var processDate = (DateTime)db.loaderlinkqueue.Where(x => x.ProcessId == processId && x.weblinks.WebSiteTypeId == webSiteType).Max(x => x.Created);

        int webSiteId = webSiteHashId.ToDecrypt().ToInt32();

        List<ScrappingProcessModel> scrappingProcessModel = (from data in db.scanninglinkqueue
                                                             where data.ReadTaken == 1 && data.ProcessId == processId && data.WebSiteId == webSiteId
                                                             && data.weblinks.WebSiteTypeId == webSiteType
                                                             select new ScrappingProcessModel
                                                             {
                                                               GUID = data.Guid,
                                                               ScrappingId = data.Id,
                                                               ProcessId = (int)data.ProcessId,
                                                               WebSiteId = data.WebSiteId,
																															 WebsiteURL = data.weblinks.WebLinkURL.Replace(" ","%20"),
                                                               ChildURLList = data.scanninglinkdetails.Select(scanDetail => new ScrappingData()
                                                               {
                                                                 Content = scanDetail.scrapedcontents.Select(x => x.Content).FirstOrDefault(),
                                                                 ScrappedId = scanDetail.Id,
                                                                 ScrappedWebsiteURL = scanDetail.WebSiteURL,
                                                               }).ToList()
                                                             }).ToList();

        foreach (var data in scrappingProcessModel)
        {
          if (data.ChildURLList.Count > 0)
          {
						LexiconTagList(data.ChildURLList);
            ProprietoryTagList(data.ChildURLList);
            StandardTagData(data.ChildURLList);
            GetDocumentURL(data.ChildURLList);
          }
        }

        ConsolidateModel finalModel = new ConsolidateModel();
        finalModel.scrappingModelList = scrappingProcessModel.ToList();
        apiOutput.Data = finalModel;
        apiOutput.TotalRecords = finalModel.scrappingModelList.Count;
        apiOutput.ErrorMessage = scrappingProcessModel.Any() ? string.Empty : string.Format(Resource.ValidateMessageNoDataFound, Resource.LblScrappingProcess);
        return apiOutput;
      }

    }

		private void LexiconTagList(List<ScrappingData> scrappingDataList)
		{
			foreach (ScrappingData scrappingData in scrappingDataList)
			{
				List<LexiconTagData> LexiconList = new List<LexiconTagData>();
				int scrappedDetailId = scrappingData.ScrappedId;
				if (scrappedDetailId > 0)
				{
					using (BCMStrategyEntities db = new BCMStrategyEntities())
					{

						LexiconList = (from data in db.scrapedlexiconmapping
													 where data.scrapedcontents.ScanningLinkDetailId == scrappedDetailId && !data.lexiconissues.IsDeleted
													 select new LexiconTagData
													 {
														 SearchCount = data.IssuesCount.HasValue ? data.IssuesCount.Value : 0,
														 LexiconTerm = data.lexiconissues.LexiconIssue,
														 CombinationValues = data.lexiconissues.CombinationValue
													 }).ToList();

						LexiconList = LexiconList
															.GroupBy(x => new { x.LexiconTerm, x.CombinationValues })
															.Select(x => new LexiconTagData
															{
																SearchCount = x.Sum(c => c.SearchCount),
																LexiconTerm = string.IsNullOrEmpty(x.First().CombinationValues) ? string.Format("{0}", x.First().LexiconTerm) : string.Format("{0} ({1})", x.First().LexiconTerm, x.First().CombinationValues)
															}).Distinct().ToList();
					}
				}
				scrappingData.LexiconTagList = LexiconList;
			}
		}

    private void ProprietoryTagList(List<ScrappingData> scrappingDataList)
    {
      foreach (ScrappingData scrappingData in scrappingDataList)
      {
        List<ProprietoryTagData> ProprietoryList = new List<ProprietoryTagData>();
        int scrappedDetailId = scrappingData.ScrappedId;
        if (scrappedDetailId > 0)
        {
          using (BCMStrategyEntities db = new BCMStrategyEntities())
          {

            ProprietoryList = (from data in db.scrappedproprietorytagsmapping
                               where data.scrappedproprietorytags.ScanningLinkDetailId == scrappedDetailId && !data.metadatatypes.IsDeleted
                               select new ProprietoryTagData
                               {
                                 ProprietoryTagDataId = data.Id,
                                 MetadatatypeId = data.MetaDataTypeId.HasValue ? data.MetaDataTypeId.Value : 0,
                                 SearchCount = data.SearchCounts.HasValue ? data.SearchCounts.Value : 0,
                                 SearchTypeId = data.SearchTypeId.HasValue ? data.SearchTypeId.Value : 0,
																 ActivityTypeId = data.ActivityTypeId.HasValue ? data.ActivityTypeId.Value : 0,
                                 SearchValue = data.SearchValue.HasValue ? data.SearchValue.Value : 0,
                                 SearchByType = data.SearchType.Replace("_", " "),
                                 MetadataTypeName = data.metadatatypes.MetaData,
                               }).ToList();

            ProprietoryList = ProprietoryList
                              .GroupBy(x => new { x.MetadataTypeName, x.SearchType, x.SearchValue })
                              .Select(x => new ProprietoryTagData
                              {
                                //// ProprietoryTagDataId = x.First().ProprietoryTagDataId,
																ActivityTypeId = x.First().ActivityTypeId,
                                MetadatatypeId = x.First().MetadatatypeId,
                                SearchCount = x.Sum(c => c.SearchCount),
                                SearchValue = x.First().SearchValue,
                                SearchTypeId = x.First().SearchTypeId,
                                SearchByType = x.First().SearchByType,
                                MetadataTypeName = x.First().MetadataTypeName,
                              }).Distinct().ToList();
          }
        }
        scrappingData.ProprietoryTagList = ProprietoryList;
      }
    }

    private void StandardTagData(List<ScrappingData> scrappingDataList)
    {
      foreach (ScrappingData scrappingData in scrappingDataList)
      {
        StandardTagData StandardTagData = new StandardTagData();
        int scrappedDetailId = scrappingData.ScrappedId;
        if (scrappedDetailId > 0)
        {
          using (BCMStrategyEntities db = new BCMStrategyEntities())
          {
            var tempStandardTagData = (from data in db.scrappedstandardtags
                                       where data.ScanningLinkDetailsId == scrappedDetailId
                                       select new
                                       {
                                         ////CountryName = data.CountryId.HasValue ? data.country.Name : string.Empty,
                                         ////EntityName = data.EntityId.HasValue ? data.institution.InstitutionName : string.Empty,
                                         ////EntityTypeName = data.EntityTypeId.HasValue ? data.institutiontypes.InstitutionType : string.Empty,
                                         EntityTypes = data.scrapperstandardtags_entitytypes,
                                         SearcyType = data.SearchType,
                                         Sectors = data.scrappedstandardtags_sectors,
                                         DateOfIssue = data.DateOfIssue,
                                         Individuals = data.scrappedstandardtag_policymakers
                                       }).FirstOrDefault();

            if (tempStandardTagData != null)
            {
              StandardTagData = new StandardTagData()
              {
                CountryName = string.Join(", ", tempStandardTagData.EntityTypes.Where(s => s.CountryId.HasValue).Select(x => x.country.Name).Distinct().ToList()),
                EntityName = string.Join(", ", tempStandardTagData.EntityTypes.Where(s => s.EntityId.HasValue).Select(x => x.institution.InstitutionName).Distinct().ToList()),
                EntityTypeName = string.Join(", ", tempStandardTagData.EntityTypes.Where(s => s.EntityTypeId.HasValue).Select(x => x.institutiontypes.InstitutionType).Distinct().ToList()),
                SearchType = tempStandardTagData.SearcyType,
								Individual = string.Join(", ", tempStandardTagData.Individuals.Select(x => x.PolicyMaker_Name).Distinct().ToList()),
								Sectors = string.Join(", ", tempStandardTagData.Sectors.Select(x => x.sector.SectorName).Distinct().ToList()),
                DateOfIssue = Convert.ToDateTime(tempStandardTagData.DateOfIssue) == DateTime.MinValue ? string.Empty : Convert.ToDateTime(tempStandardTagData.DateOfIssue).ToString("MMMM dd, yyyy")
              };
            }

          }
        }
        scrappingData.StandardTagData = StandardTagData;
      }
    }

		public async Task<ApiOutput> GetAllScrappedURLSummaryList(int webSiteType, string processEventId)
    {
      ApiOutput apiOutput = new ApiOutput();

      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        int processId = 0;
        int previousProcessId = 0;
        var idFromConfig = ConfigurationManager.AppSettings["ScrappingProcessId"].ToInt32();
        var scrappingProcessId = processEventId == "0" ? idFromConfig : processEventId.ToDecrypt().ToInt32();

        if (scrappingProcessId != 0)
        {
          processId = (db.loaderlinkqueue.Where(a => a.ProcessId <= scrappingProcessId && a.weblinks.WebSiteTypeId == webSiteType).Max(s => s.ProcessId));
          ////previousProcessId = (int)(db.loaderlinkqueue.Where(x => x.ProcessId < processId).Max(s => s.ProcessId)); ////old code
          previousProcessId = (db.loaderlinkqueue.Where(x => x.ProcessId < processId && x.weblinks.WebSiteTypeId == webSiteType).Max(s => s.ProcessId));
        }
        else
        {
          processId = (db.loaderlinkqueue.Where(w => w.weblinks.WebSiteTypeId == webSiteType).Max(s => s.ProcessId));
          ////previousProcessId = (int)(db.loaderlinkqueue.Where(x => x.ProcessId < processId).Max(s => s.ProcessId)); ////old code
          previousProcessId = (db.loaderlinkqueue.Where(x => x.ProcessId < processId && x.weblinks.WebSiteTypeId == webSiteType).Max(s => s.ProcessId));
        }

        var processDate = db.loaderlinkqueue.Where(x => x.ProcessId == processId).Max(x => x.Created).Value;
        processDate = CommonUtilities.ToESTTimezone(processDate);
        var previousProcessDate = db.loaderlinkqueue.Where(x => x.ProcessId == previousProcessId).Max(x => x.Created).Value;
        previousProcessDate = CommonUtilities.ToESTTimezone(previousProcessDate);
        List<ScrappingProcessSummaryModel> scrappingProcessModel = (from data in db.scanninglinkqueue
                                                                    where data.ReadTaken == 1 && data.ProcessId == processId
                                                                    && data.weblinks.WebSiteTypeId == webSiteType
                                                                    select new ScrappingProcessSummaryModel()
                                                                    {
                                                                      GUID = data.Guid,
                                                                      ScrappingId = data.Id,
                                                                      ProcessId = (int)data.ProcessId,
                                                                      WebSiteId = data.WebSiteId,
                                                                      WebsiteURL = data.weblinks.WebLinkURL,
                                                                      _ScanDate = processDate,
                                                                      PreviousProcessDate = previousProcessDate,
                                                                      ChildURLAvaliable = data.scanninglinkdetails.Any(),
                                                                      ChildURLList = data.scanninglinkdetails.Select(scanDetail => new ScrappingSummaryData()
                                                                      {
                                                                        LexiconCount = scanDetail.scrapedcontents.Count,
                                                                        ProprietaryTagsCount = scanDetail.scrappedproprietorytags.Count,
                                                                        StandardTagsCount = scanDetail.scrappedstandardtags.Count
                                                                      }).ToList()
                                                                    }).ToList();

        int[] webSiteIds = scrappingProcessModel.Select(x => x.WebSiteId).Distinct().ToArray();

        List<ScrappingProcessSummaryModel> unprocessedSites = (from data in db.loaderlinkqueue
                                                               where data.ProcessId == processId && !webSiteIds.Contains((int)data.WebSiteId)
                                                               select new ScrappingProcessSummaryModel()
                                                               {
                                                                 WebSiteId = (int)data.WebSiteId,
                                                                 WebsiteURL = data.weblinks.WebLinkURL,
                                                                 _ScanDate = processDate,
                                                                 PreviousProcessDate = previousProcessDate,
                                                                 ChildURLAvaliable = false
                                                               }).ToList();

        scrappingProcessModel.AddRange(unprocessedSites);
        var temp = scrappingProcessModel.Select(x => new
        {
          GUID = x.GUID,
          ScrappingId = x.ScrappingId,
          ProcessId = x.ProcessId,
          WebSiteId = x.WebSiteId,
          WebsiteURL = x.WebsiteURL,
          ChildURLAvaliable = x.ChildURLAvaliable,
          LastScanDate = x.ScannedDate,
          PreviousScanDate = x.PreviousProcessDateFormated,
          IsLexiconAvaialble = x.ChildURLList != null ? x.ChildURLList.Where(y => y.LexiconCount != 0).Select(y => y.LexiconCount).Any() : Helper.saveChangesNotSuccessful,
          IsProprietaryTagsAvaialble = x.ChildURLList != null ? x.ChildURLList.Where(y => y.ProprietaryTagsCount != 0).Select(y => y.ProprietaryTagsCount).Any() : Helper.saveChangesNotSuccessful,
          IsStandardTagsAvaialble = x.ChildURLList != null ? x.ChildURLList.Where(y => y.StandardTagsCount != 0).Select(y => y.StandardTagsCount).Any() : Helper.saveChangesNotSuccessful,
        }).OrderByDescending(x => x.ChildURLAvaliable).ThenByDescending(x => x.IsLexiconAvaialble).ThenByDescending(x => x.IsProprietaryTagsAvaialble).ThenByDescending(x => x.IsStandardTagsAvaialble);

        var ActualResult = temp.Select((x, index) => new SummaryModel()
        {
          SrNo = index + 1,
          WebSiteId = x.WebSiteId,
          WebSiteURL = x.WebsiteURL,
          ChildURLAvaliable = x.ChildURLAvaliable,
          IsLexiconAvaialble = x.IsLexiconAvaialble,
          IsProprietaryTagsAvaialble = x.IsProprietaryTagsAvaialble,
          IsStandardTagsAvaialble = x.IsStandardTagsAvaialble,
          Scan1 = x.ChildURLAvaliable ? "Yes" : "No",
          Scan2 = x.ChildURLAvaliable ? x.IsLexiconAvaialble ? "Yes" : "No" : "Not Applicable",
          ProprietoryTag = x.IsLexiconAvaialble ? x.IsProprietaryTagsAvaialble ? "Yes" : "No" : "Not Applicable",
          StandardTag = x.IsLexiconAvaialble && x.IsProprietaryTagsAvaialble ? x.IsStandardTagsAvaialble ? "Yes" : "No" : "Not Applicable",
          LastScanDate = x.LastScanDate,
          PreviousScanDate = x.PreviousScanDate
        });
        apiOutput.Data = ActualResult;
        apiOutput.TotalRecords = ActualResult.Count();

      }

      return apiOutput;

    }

    private void GetDocumentURL(List<ScrappingData> scrappingDataList)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        foreach (var childUrl in scrappingDataList)
        {
          if (childUrl.ProprietoryTagList.Count > 0 && childUrl.StandardTagData != null)
          {
            childUrl.DocumentURL = db.documentstorage.Where(w => w.ScanningLinkDetailid == childUrl.ScrappedId).Select(s => s.URL).FirstOrDefault();
          }
        }
      }
    }
  }
}
