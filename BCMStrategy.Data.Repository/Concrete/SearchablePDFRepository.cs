using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using BCMStrategy.Common.Kendo;
using BCMStrategy.DAL.Context;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Resources;
using BCMStrategy.Common.AuditLog;
using BCMStrategy.Common.Unity;
using BCMStrategy.Logger;

namespace BCMStrategy.Data.Repository.Concrete
{
	public class SearchablePdfRepository : ISearchablePdf
	{
    private static readonly EventLogger<SearchablePdfRepository> log = new EventLogger<SearchablePdfRepository>();

		/// <summary>
		/// The _audit repository
		/// </summary>
		private IAuditLog _auditRepository;

		/// <summary>
		/// Gets the audit repository.
		/// </summary>
		/// <value>
		/// The audit repository.
		/// </value>
		private IAuditLog AuditRepository
		{
			get
			{
				if (this._auditRepository == null)
				{
					this._auditRepository = UnityHelper.Resolve<IAuditLog>();
				}

				return this._auditRepository;
			}
		}

		/// <summary>
		/// Get the list of PDf Based on Lexicon terms and Date
		/// </summary>
		/// <param name="searchablePDFParameters">Properties to search for pdf</param>
		/// <returns></returns>
		public async Task<ApiOutput> GetListOfPDFBasedOnLexicon(SearchablePdfParameters searchablePDFParameters, GridParameters parametersJson)
		{
			List<SearchablePdfModel> list;
			ApiOutput apiOutput = new ApiOutput();
			int totalRecord = 0;
			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{
				IQueryable<SearchablePdfModelRaw> query1 = RetrieveQuery(db, searchablePDFParameters);
				if (!string.IsNullOrEmpty(searchablePDFParameters.ActionType))
				{
					query1 = query1.Where(x => x.ScrappedProprietoryTagsMapping.metadatatypes.MetaData == searchablePDFParameters.ActionType);
				}
				var query = query1.Select(x => new
				{
					Created = x.Created,
					PDFURL = x.PDFURL,
					WebSiteURL = x.WebSiteURL,
					ProprietaryTags = x.ScrappedProprietoryTagsMapping.metadatatypes.MetaData
				}).Distinct();
				if (parametersJson.Sort == null || parametersJson.Sort.Count == 0)
				{
					query = query.OrderByDescending(x => x.Created);
				}
				var list1 = await query.ModifyList(parametersJson, out totalRecord).ToListAsync();

				list = list1.Select(x => new SearchablePdfModel()
				{
					CreatedDate = x.Created,
					PDFURL = x.PDFURL,
					WebSiteURL = x.WebSiteURL,
					ProprietaryTags = x.ProprietaryTags
				}).ToList();
			}
			if (!string.IsNullOrEmpty(searchablePDFParameters.ActionType))
			{
				int currentUserId = UserAccessHelper.CurrentUserIdentity.ToString().ToInt32();
				AuditRepository.WriteAudit<SearchablePdfParameters>(CustomerAuditConstants.DashboardSearchLexicon, AuditType.ResultView, null, searchablePDFParameters, string.Format(Resource.FilteredOutAuditMessage, searchablePDFParameters.FromDate.Replace("-", "/"), searchablePDFParameters.ActionType, searchablePDFParameters.Lexicon), currentUserId);
			}
			apiOutput.Data = list;
			apiOutput.TotalRecords = totalRecord;

			return apiOutput;
		}

		public List<SearchablePdfLineChartModel> GetLineChartDataBasedOnLexicon(SearchablePdfParametersForDrillDown searchablePDFParameters)
		{

			List<SearchablePdfLineChartModel> list = new List<SearchablePdfLineChartModel>();

			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{
				try
				{
					if (!string.IsNullOrEmpty(searchablePDFParameters.MonthDate))
					{
						////DateTime dateExtract = DateTime.ParseExact(searchablePDFParametersForDrillDown.MonthDate, "MMM-yy", null);
						////SearchablePDFParameters searchablePDFParameters = new SearchablePDFParameters()
						////{
						////	FromDate = dateExtract.ToString("MM-dd-yyyy"),
						////	ToDate = dateExtract.AddMonths(1).AddDays(-1).ToString("MM-dd-yyyy"),
						////	Lexicon = searchablePDFParametersForDrillDown.Lexicon
						////};

						SearchablePdfParameters searchPDFDataLexiconParameters = new SearchablePdfParameters()
						{
							FromDate = DateTime.ParseExact(searchablePDFParameters.FromDate, "dd-MMM-yy", null).ToString("MM-dd-yyyy"),
							ToDate = DateTime.ParseExact(searchablePDFParameters.ToDate, "dd-MMM-yy", null).ToString("MM-dd-yyyy"),
							Lexicon = searchablePDFParameters.Lexicon
						};

						var lineChartDataQuery = RetrieveQuery(db, searchPDFDataLexiconParameters).ToList().Distinct();
						var lineChartData = (from x in lineChartDataQuery
																 select new
																 {
																	 ScrappedContentMappingId = x.ScrappedContentMappingId,
																	 Value = x.ScrappedProprietoryTagsMapping.SearchValue.HasValue ? x.ScrappedProprietoryTagsMapping.SearchValue.Value : 0,
																	 MetaDataTypeId = x.ScrappedProprietoryTagsMapping.MetaDataTypeId,
																	 MetaDataType = x.ScrappedProprietoryTagsMapping.metadatatypes.MetaData,
																	 CreateDate = x.CreatedString
																 }).Distinct().ToList();

						if (lineChartData.Any())
						{
							var metadatatypesIdList = lineChartData.Select(a => a.MetaDataType).Distinct().ToList();
							List<string> createDateList = lineChartData.Select(x => x.CreateDate).Distinct().ToList();
							createDateList.ForEach(a =>
							{
								SearchablePdfLineChartModel searchablePDFLineChartModel = new SearchablePdfLineChartModel();
								searchablePDFLineChartModel.CreatedString = a;
								metadatatypesIdList.ForEach(x =>
								{
									SearchablePdfLineChartProprietaryModel proprietaryModel = new SearchablePdfLineChartProprietaryModel();
									proprietaryModel.ProprietaryTagType = x;
									proprietaryModel.ProprietaryTagValue = lineChartData.Where(l => l.CreateDate == a && l.MetaDataType == x).Select(v => v.Value).Sum();
									searchablePDFLineChartModel.ProprietaryModel.Add(proprietaryModel);
								});
								list.Add(searchablePDFLineChartModel);
							});
						}
						int currentUserId = UserAccessHelper.CurrentUserIdentity.ToString().ToInt32();
						AuditRepository.WriteAudit<SearchablePdfParameters>(CustomerAuditConstants.DashboardSearchLexicon, AuditType.ResultView, null, searchPDFDataLexiconParameters, string.Format(Resource.LineChartAuditMessage, searchablePDFParameters.MonthDate, searchPDFDataLexiconParameters.Lexicon), currentUserId);

					}
				}
				catch (Exception ex)
				{
          log.LogError(LoggingLevel.Error, "BadRequest", "SearchablePDFRepository: Exception is thrown in GetLineChartDataBasedOnLexicon method", ex, null);
				}
			}

			return list;
		}


		public List<SearchablePdfLineChartModel> GetLineMonthWiseChartDataBasedOnLexicon(SearchablePdfParameters searchablePDFParameters)
		{

			List<SearchablePdfLineChartModel> list = new List<SearchablePdfLineChartModel>();

			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{
				db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
				var lineChartDataQuery = RetrieveQuery(db, searchablePDFParameters).ToList().Distinct();

				var lineChartData = (from x in lineChartDataQuery
														 select new
														 {
															 ScrappedContentMappingId = x.ScrappedContentMappingId,
															 Value = x.ScrappedProprietoryTagsMapping.SearchValue.HasValue ? x.ScrappedProprietoryTagsMapping.SearchValue.Value : 0,
															 MetaDataTypeId = x.ScrappedProprietoryTagsMapping.MetaDataTypeId,
															 MetaDataType = x.ScrappedProprietoryTagsMapping.metadatatypes.MetaData,
															 CreateDate = x.CreatedMonthString,
															 FromDate = new DateTime(x.Created.Value.Year,x.Created.Value.Month,1),
															 ToDate = new DateTime(x.Created.Value.Year, x.Created.Value.Month, DateTime.DaysInMonth(x.Created.Value.Year, x.Created.Value.Month)),
														 }).Distinct().ToList();

				if (lineChartData.Any())
				{
					var metadatatypesIdList = lineChartData.Select(a => a.MetaDataType).Distinct().ToList();
					List<string> createDateList = lineChartData.Select(x => x.CreateDate).Distinct().ToList();



					DateTime fromDateT = Convert.ToDateTime(searchablePDFParameters.FromDate);
					DateTime? fromDate = new DateTime(fromDateT.Year, fromDateT.Month, fromDateT.Day);
					DateTime toDateT = Convert.ToDateTime(searchablePDFParameters.ToDate);
					DateTime? toDate = new DateTime(toDateT.Year, toDateT.Month, toDateT.Day);

					createDateList.ForEach(a =>
					{
						var getDateRangeByMonth = lineChartData.Where(x => x.CreateDate == a).Select(x =>  x).FirstOrDefault();
						SearchablePdfLineChartModel searchablePDFLineChartModel = new SearchablePdfLineChartModel();
						searchablePDFLineChartModel.CreatedString = a;

						if (getDateRangeByMonth.FromDate > fromDate && getDateRangeByMonth.ToDate < toDate)
						{
							searchablePDFLineChartModel.FromDate = getDateRangeByMonth.FromDate.ToString("dd-MMM-yy");
							searchablePDFLineChartModel.ToDate = getDateRangeByMonth.ToDate.ToString("dd-MMM-yy");
						}
            else if (getDateRangeByMonth.FromDate <= fromDate && getDateRangeByMonth.ToDate >= toDate)
            {
              searchablePDFLineChartModel.FromDate = new DateTime(getDateRangeByMonth.ToDate.Year, getDateRangeByMonth.ToDate.Month, fromDate.Value.Day).ToString("dd-MMM-yy");
							searchablePDFLineChartModel.ToDate = new DateTime(getDateRangeByMonth.ToDate.Year, getDateRangeByMonth.ToDate.Month, toDate.Value.Day).ToString("dd-MMM-yy");
            }
						else if (getDateRangeByMonth.ToDate >= toDate)
						{
							searchablePDFLineChartModel.FromDate = getDateRangeByMonth.FromDate.ToString("dd-MMM-yy");
							searchablePDFLineChartModel.ToDate = new DateTime(getDateRangeByMonth.ToDate.Year, getDateRangeByMonth.ToDate.Month, toDate.Value.Day).ToString("dd-MMM-yy");
						}
						else
						{
							searchablePDFLineChartModel.FromDate = new DateTime(getDateRangeByMonth.FromDate.Year, getDateRangeByMonth.FromDate.Month, fromDate.Value.Day).ToString("dd-MMM-yy");
							searchablePDFLineChartModel.ToDate = getDateRangeByMonth.ToDate.ToString("dd-MMM-yy");
						}

						metadatatypesIdList.ForEach(x =>
						{
							SearchablePdfLineChartProprietaryModel proprietaryModel = new SearchablePdfLineChartProprietaryModel();
							proprietaryModel.ProprietaryTagType = x;
							proprietaryModel.ProprietaryTagValue = lineChartData.Where(l => l.CreateDate == a && l.MetaDataType == x).Select(v => v.Value).Sum();
							searchablePDFLineChartModel.ProprietaryModel.Add(proprietaryModel);
						});
						list.Add(searchablePDFLineChartModel);
					});
				}

				int currentUserId = UserAccessHelper.CurrentUserIdentity.ToString().ToInt32();
				DateTime fromDateTemp = Convert.ToDateTime(searchablePDFParameters.FromDate);
				DateTime toDateTemp = Convert.ToDateTime(searchablePDFParameters.ToDate);
				AuditRepository.WriteAudit<SearchablePdfParameters>(CustomerAuditConstants.DashboardSearchLexicon, AuditType.ResultView, null, searchablePDFParameters, "Period From: " + fromDateTemp.ToString("MM/dd/yyyy") + " to " + toDateTemp.ToString("MM/dd/yyyy") + " Lexicon Term: " + searchablePDFParameters.Lexicon, currentUserId);

			}
			return list;
		}

		/// <summary>
		/// Get All Lexicons
		/// </summary>
		/// <param name="searchTerm"></param>
		/// <returns></returns>
		public async Task<List<DropdownMaster>> GetAllLexicons(string searchTerm)
		{
			int user = UserAccessHelper.CurrentUserIdentity.ToString().ToInt32();
			////int processId = processIdGeneric;
			List<int?> lexiconAccessList = new List<int?>();


			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{
				string userType = (from e in db.user
													 where e.Id == user
													 select e.UserType).FirstOrDefault();

				if (userType == Enums.UserType.CUSTOMER.ToString())
				{
					lexiconAccessList = db.lexiconprivilege.Where(e => e.UserId == user && e.IsDeleted != 1).Select(e => e.LexiconIssueId).ToList();
				}
				db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);

				List<DropdownMaster> dropDownList = db.lexiconissues
																										.Where(a => (userType == Enums.UserType.ADMIN.ToString() || lexiconAccessList.Contains(a.Id)) &&
																																((a.LexiconIssue).Contains(searchTerm.Trim()) || (a.LexiconIssue + " " + a.CombinationValue).Contains(searchTerm.Trim())))
																										.Select(a => new DropdownMaster()
																										{
																											Value = a.IsNested ? a.LexiconIssue + " " + a.CombinationValue : a.LexiconIssue
																										}).Distinct().OrderBy(x => x.Value).ToList();

        ////List<DropdownMaster> dropDownListLinker = db.lexiconissuelinker
        ////																				.Where(a => (userType == Enums.UserType.ADMIN.ToString() || lexiconAccessList.Contains(a.LexiconIssueId)) &&
        ////																										(a.Linkers).Contains(searchTerm))
        ////																				.Select(a => new DropdownMaster()
        ////																				{
        ////																					Value = a.Linkers
        ////																				}).Distinct().ToList();

        ////dropDownList.AddRange(dropDownListLinker);
				return dropDownList;
			}
		}

		private IQueryable<SearchablePdfModelRaw> RetrieveQuery(BCMStrategyEntities db, SearchablePdfParameters searchablePDFParameters)
		{
			List<int?> lexiconAccessList = new List<int?>();

      ////DateTime? fromDate = DateTime.ParseExact(searchablePDFParameters.FromDate, "MM/dd/yyyy", null);
			DateTime fromDateTemp = Convert.ToDateTime(searchablePDFParameters.FromDate);
			DateTime? fromDate = new DateTime(fromDateTemp.Year, fromDateTemp.Month, fromDateTemp.Day);
			DateTime toDateTemp = Convert.ToDateTime(searchablePDFParameters.ToDate);
			DateTime? toDate = new DateTime(toDateTemp.Year, toDateTemp.Month, toDateTemp.Day).AddHours(23).AddMinutes(59).AddSeconds(59);


			int user = UserAccessHelper.CurrentUserIdentity.ToString().ToInt32();
			////int processId = processIdGeneric;

			string userType = (from e in db.user
												 where e.Id == user
												 select e.UserType).FirstOrDefault();

			if (userType == Enums.UserType.CUSTOMER.ToString())
			{
				////lexiconAccessList = db.lexiconprivilege.Where(e => e.UserId == user && e.IsDeleted != 1 && (e.lexiconissues.LexiconIssue == searchablePDFParameters.Lexicon || e.lexiconissues.lexiconissuelinker.Any(y => y.Linkers == searchablePDFParameters.Lexicon))).Select(e => e.LexiconIssueId).ToList();
				lexiconAccessList = db.lexiconprivilege.Where(e => e.UserId == user && e.IsDeleted != 1 && (e.lexiconissues.LexiconIssue + " " + e.lexiconissues.CombinationValue == searchablePDFParameters.Lexicon || e.lexiconissues.LexiconIssue == searchablePDFParameters.Lexicon)).Select(e => e.LexiconIssueId).ToList();
			}

			//// Get the lexicons Id list to search

			var scrappedContent = (from sc in db.scrapedcontents
														 join scl in db.scrapedlexiconmapping on sc.Id equals scl.ScrapedContentid
														 where sc.Created >= fromDate.Value && sc.Created <= toDate.Value
														 select new
														 {
															 scanninglinkdetails = sc.scanninglinkdetails,
															 CreatedDate = sc.Created,
															 LexiconId = scl.LexiconId,
															 ScrappedContentMappingId = scl.Id,
															 IsProprietaryExist = sc.scanninglinkdetails.scrappedproprietorytags.Any(),
															 IsStandardTagsExist = sc.scanninglinkdetails.scrappedstandardtags.Any(),
															 IsDocumentExist = sc.scanninglinkdetails.documentstorage.Any()
														 }).AsQueryable();


			var LexiconIssues = db.lexiconissues
													.Where(x => !x.IsDeleted && x.lexiconissuelinker.Any(y => !y.IsDeleted) &&
																			(userType == Enums.UserType.ADMIN.ToString() || lexiconAccessList.Contains(x.Id)) &&
																			(x.LexiconIssue + " " + x.CombinationValue == searchablePDFParameters.Lexicon || x.LexiconIssue == searchablePDFParameters.Lexicon)
																)
													.Select(x => new { Id = x.Id })
													.Distinct()
													.AsQueryable();

			IQueryable<SearchablePdfModelRaw> query = (from sc in scrappedContent
																								 join lexi in LexiconIssues on sc.LexiconId equals lexi.Id
																								 join spt in db.scrappedproprietorytags on sc.scanninglinkdetails.Id equals spt.ScanningLinkDetailId
																								 join sptm in db.scrappedproprietorytagsmapping on spt.Id equals sptm.ScrappedProprietoryTagId
																								 where sc.IsProprietaryExist && sc.IsStandardTagsExist && sc.IsDocumentExist
																								 select new SearchablePdfModelRaw()
																								 {
																									 ScrappedContentMappingId = sc.ScrappedContentMappingId,
																									 WebSiteURL = sc.scanninglinkdetails.WebSiteURL,
																									 PDFURL = userType == Enums.UserType.CUSTOMER.ToString() ? string.Empty : sc.scanninglinkdetails.documentstorage.FirstOrDefault().URL,
																									 Created = sc.CreatedDate,
																									 ScrappedProprietoryTagsMapping = sptm,
																									 ScanningLinkDetailId = sc.scanninglinkdetails.Id
																								 });

			return query;

		}
	}
}