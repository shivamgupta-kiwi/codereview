using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using BCMStrategy.Common.Kendo;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Data.Repository.Concrete;
using BCMStrategy.Logger;
using Newtonsoft.Json;
using BCMStrategy.API.Filter;
using BCMStrategy.Resources;

namespace BCMStrategy.API.Controllers
{
	[Authentication]
	[RoutePrefix("api/SearchablePDF")]
	public class SearchablePdfController : BaseApiController
	{
		private static readonly EventLogger<SearchablePdfController> _log = new EventLogger<SearchablePdfController>();

		private ISearchablePdf _searchablePDFRepository;

		private ISearchablePdf SearchablePDFRepository
		{
			get
			{
				if (_searchablePDFRepository == null)
				{
					_searchablePDFRepository = new SearchablePdfRepository();
				}

				return _searchablePDFRepository;
			}
		}

		[HttpGet]
		[Route("GetSearchablePDfDatabasedOnLexicon")]
		public async Task<IHttpActionResult> GetSearchablePDfDatabasedOnLexicon(string FromDate, string ToDate, string Lexicon, string MetaDataType, string parametersJson, bool IsMonthlyClicked = false)
		{
			try
			{
        ApiOutput apiOutput;
				string fromDate = string.Empty;
				string toDate = string.Empty;
				if (IsMonthlyClicked)
				{
          ////DateTime dateExtract = DateTime.ParseExact(FromDate, "MMM-yy", null);
          ////fromDate = dateExtract.ToString("MM-dd-yyyy");
          ////toDate = dateExtract.AddMonths(1).AddDays(-1).ToString("MM-dd-yyyy");
					fromDate = DateTime.ParseExact(FromDate, "dd-MMM-yy", null).ToString("MM-dd-yyyy");
					toDate = DateTime.ParseExact(ToDate, "dd-MMM-yy", null).ToString("MM-dd-yyyy");
				}
				else {
					DateTime dateExtract = DateTime.ParseExact(FromDate, "dd-MMM-yy", null);
					fromDate = dateExtract.ToString("MM-dd-yyyy");
					toDate = dateExtract.ToString("MM-dd-yyyy");
				}
				SearchablePdfParameters searchablePDFParameters = new SearchablePdfParameters()
				{
					FromDate = fromDate,
					ToDate = toDate,
					Lexicon = Lexicon,
					ActionType = MetaDataType
				};

				var parameters = JsonConvert.DeserializeObject<GridParameters>(parametersJson);
				apiOutput = await SearchablePDFRepository.GetListOfPDFBasedOnLexicon(searchablePDFParameters, parameters);
				var result = new { Data = apiOutput.Data, Total = apiOutput.TotalRecords };
				return Json(result);
			}
			catch (Exception ex)
			{
				_log.LogError(LoggingLevel.Error, "BadRequest", "Exception occur while Getting DashBoard data", ex);
				return BadRequest(ex.Message);
			}
		}

		private void HandleModelState(System.Web.Http.ModelBinding.ModelStateDictionary ModelState, SearchablePdfParameters searchablePDFParameters)
		{
			if (!string.IsNullOrEmpty(searchablePDFParameters.FromDate) && !string.IsNullOrEmpty(searchablePDFParameters.ToDate))
			{
				DateTime fromDate = Convert.ToDateTime(searchablePDFParameters.FromDate);
				DateTime toDate = Convert.ToDateTime(searchablePDFParameters.ToDate);
				if (fromDate > toDate)
				{
					ModelState.AddModelError("FromDate", Resource.ValidateCompareDate);
				}
			}
		}

		[HttpPost]
		[Route("GetSearchablePDfMonthWiseChartDatabasedOnLexicon")]
		public async Task<IHttpActionResult> GetSearchablePDfMonthWiseChartDatabasedOnLexicon(SearchablePdfParameters searchablePDFParameters)
		{
			try
			{
        ////APIOutput apiOutput = new APIOutput();
				HandleModelState(ModelState, searchablePDFParameters);
				if (!ModelState.IsValid)
				{
					return Ok(FormatResult(false, ModelState));
				}
				List<SearchablePdfLineChartModel> result = SearchablePDFRepository.GetLineMonthWiseChartDataBasedOnLexicon(searchablePDFParameters);
				return Ok(result);
			}
			catch (Exception ex)
			{
				_log.LogError(LoggingLevel.Error, "BadRequest", "Exception occur while Getting Searchable PDF data", ex);
				return BadRequest(ex.Message);
			}
		}

		[HttpPost]
		[Route("GetSearchablePDfChartDatabasedOnLexicon")]
		public async Task<IHttpActionResult> GetSearchablePDfChartDatabasedOnLexicon(SearchablePdfParametersForDrillDown searchablePDFParametersForDrillDown)
		{
			try
			{
				List<SearchablePdfLineChartModel> result = SearchablePDFRepository.GetLineChartDataBasedOnLexicon(searchablePDFParametersForDrillDown);
				return Ok(result);
			}
			catch (Exception ex)
			{
				_log.LogError(LoggingLevel.Error, "BadRequest", "Exception occur while Getting Searchable PDF data", ex);
				return BadRequest(ex.Message);
			}
		}

		[HttpGet]
		[Route("GetAllLexicons")]
		public async Task<IHttpActionResult> GetAllLexicons(string searchTerm)
		{
			try
			{
				List<DropdownMaster> result = await SearchablePDFRepository.GetAllLexicons(searchTerm);
				return Ok(result);
			}
			catch (Exception ex)
			{
				_log.LogError(LoggingLevel.Error, "BadRequest", "Exception occur while Getting All Lexicon DDL Data", ex);
				return BadRequest(ex.Message);
			}
		}


	}
}
