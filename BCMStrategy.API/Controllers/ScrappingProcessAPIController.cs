using BCMStrategy.API.Filter;
using BCMStrategy.Common.Kendo;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Data.Repository.Concrete;
using BCMStrategy.Logger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BCMStrategy.API.Controllers
{
  [Authentication]
	[RoutePrefix("api/ScrappingProcess")]
	public class ScrappingProcessApiController : BaseApiController
	{
    ////static readonly EventLogger<ScrappingProcessAPIController> _log = new EventLogger<ScrappingProcessAPIController>();

		private IScrappingProcess _scrappingProcessRepository;

		private IScrappingProcess ScrappingProcessRepository
		{
			get
			{
				if (_scrappingProcessRepository == null)
				{
					_scrappingProcessRepository = new ScrappingProcessRepository();
				}

				return _scrappingProcessRepository;
			}
		}

		[Route("GetAllScrappedSummaryList")]
		[HttpGet]
		public async Task<IHttpActionResult> GetAllScrappedSummaryList(int webSiteType, string processId = "")
		{
			ApiOutput apiOutput = await ScrappingProcessRepository.GetAllScrappedURLSummaryList(webSiteType, processId);
			var result = new { Data = apiOutput.Data, Total = apiOutput.TotalRecords };
			return Json(result);
		}

		[Route("GetAllScrappedList")]
		[HttpGet]
		public async Task<IHttpActionResult> GetAllScrappedList(string webSiteHashId, int webSiteType, string processEventId = "")
		{
			ApiOutput apiOutput = await ScrappingProcessRepository.GetAllScrappedURLList(webSiteHashId, webSiteType, processEventId);
			var result = new { Data = apiOutput.Data, Total = apiOutput.TotalRecords };
			return Json(result);
		}
	}
}
