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
using BCMStrategy.API.Filter;

namespace BCMStrategy.API.Controllers
{
	[Authentication]
	[RoutePrefix("api/PrivilegeApi")]
	public class PrivilegeApiController : BaseApiController
	{

		private static readonly EventLogger<PolicyMakersController> _log = new EventLogger<PolicyMakersController>();

		private IPrivilege _privilegeRepository;

		private IPrivilege PrivilegeRepository
		{
			get
			{
				if (_privilegeRepository == null)
				{
					_privilegeRepository = new PrivilegeRepository();
				}

				return _privilegeRepository;
			}
		}

		/// <summary>
		/// Get the List of Customer
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		[Route("GetAllCustomer")]
		[HttpGet]
		public async Task<IHttpActionResult> GetAllCustomer(string parametersJson)
		{
			try
			{
				var parameters = JsonConvert.DeserializeObject<GridParameters>(parametersJson);
				ApiOutput apiOutput = await PrivilegeRepository.GetAllCustomer(parameters);
				var result = new { Data = apiOutput.Data, Total = apiOutput.TotalRecords };
				return Json(result);
			}
			catch (Exception ex)
			{
				_log.LogError(LoggingLevel.Error, "BadRequest", "Exception occur while Getting All Customer DDL Data", ex);
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// Get Lexicon Term HashIds Based On LexiconType
		/// </summary>
		/// <param name="lexiconTypeHashId"></param>
		/// <returns></returns>
		[Route("GetLexiconTermHashIdsBasedOnLexiconType")]
		[HttpGet]
		public async Task<IHttpActionResult> GetLexiconTermHashIdsBasedOnLexiconType(string lexiconTypeHashId, string parameterMap)
		{
			try
			{
				var parameters = JsonConvert.DeserializeObject<GridParameters>(parameterMap);
				List<LexiconModel> lexiconList = await PrivilegeRepository.GetLexiconTermHashIdsBasedOnLexiconType(lexiconTypeHashId, parameters);
				return Ok(FormatResult(lexiconList, string.Empty));
			}
			catch (Exception ex)
			{
				_log.LogError(LoggingLevel.Error, "BadRequest", "Exception occur while Getting data of LexiconTermHashId", ex);
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// Update LexiconAccess Ids
		/// </summary>
		/// <param name="webLinkModel"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("UpdateLexiconAccessPrivilege")]
		public async Task<IHttpActionResult> UpdateLexiconAccessPrivilege(LexiconAccessManagementModel lexiconAccessManagementModel)
		{
			try
			{
				HandleModelStateForLexiconPrivilege(lexiconAccessManagementModel, ModelState);
				if (!ModelState.IsValid)
				{
					return Ok(FormatResult(false, ModelState));
				}

				bool isSave = await PrivilegeRepository.UpdateLexiconAccessPrivilege(lexiconAccessManagementModel);
				return Ok(FormatResult(isSave, (isSave ? (string.IsNullOrEmpty(lexiconAccessManagementModel.CustomerMasterHashId) ? Resources.Resource.LexiconPrivilegeAddedSuccess : Resources.Resource.LexiconPrivilegeUpdatedSuccess) : Resources.Resource.ErrorWhileSaving)));
			}
			catch (Exception ex)
			{
				_log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, lexiconAccessManagementModel);
				return BadRequest(ex.Message);
			}
		}


		public void HandleModelStateForLexiconPrivilege(LexiconAccessManagementModel lexiconAccessManagementModel, System.Web.Http.ModelBinding.ModelStateDictionary modelState)
		{

			if (!lexiconAccessManagementModel.SelectedCustomerHashIds.Any() && string.IsNullOrEmpty(lexiconAccessManagementModel.CustomerMasterHashId))
			{
				ModelState.AddModelError("lexiconAccessManagementModel.selectedCustomerHashIds", Resources.Resource.CustomerCustomValidation);
			}
			else if (!lexiconAccessManagementModel.SelectedLexiconHashIds.Any() && string.IsNullOrEmpty(lexiconAccessManagementModel.CustomerMasterHashId))
			{
				ModelState.AddModelError("lexiconAccessManagementModel.SelectedLexiconHashIds", Resources.Resource.LexiconIssueCustomValidation);
			}

		}

		/// <summary>
		/// Get the List Lexicon Access Customer
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		[Route("GetAllLexiconAccessCustomer")]
		[HttpGet]
		public async Task<IHttpActionResult> GetAllLexiconAccessCustomer(string parametersJson)
		{
			try
			{
				var parameters = JsonConvert.DeserializeObject<GridParameters>(parametersJson);
				ApiOutput apiOutput = await PrivilegeRepository.GetAllLexiconAccessCustomer(parameters);
				var result = new { Data = apiOutput.Data, Total = apiOutput.TotalRecords };
				return Json(result);
			}
			catch (Exception ex)
			{
				_log.LogError(LoggingLevel.Error, "BadRequest", "Exception occur GetAllLexiconAccessCustomer", ex);
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// Get Lexicon By Customer Hash Id
		/// </summary>
		/// <param name="webLinkHashId"></param>
		/// <returns></returns> 
		[Route("GetLexiconIdsBasedOnCustomer")]
		[HttpGet]
		public async Task<IHttpActionResult> GetLexiconIdsBasedOnCustomer(string customerHashId)
		{
			try
			{
				List<LexiconAccessManagementModel> list = await PrivilegeRepository.GetLexiconIdsBasedOnCustomer(customerHashId);
				return Ok(list);
			}
			catch (Exception ex)
			{
				_log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, customerHashId);
				return BadRequest(ex.Message);
			}
		}
	}
}