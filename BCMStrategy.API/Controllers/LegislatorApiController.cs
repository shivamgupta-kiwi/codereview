using BCMStrategy.Common.Kendo;
using BCMStrategy.Common.Unity;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Common.Unity;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Logger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using BCMStrategy.API.AuditLog;
using BCMStrategy.Common.AuditLog;
using BCMStrategy.Data.Abstract;
using BCMStrategy.API.Filter;

namespace BCMStrategy.API.Controllers
{
    [Authentication]
    [RoutePrefix("api/Legislator")]
    public class LegislatorApiController : BaseApiController
    {
        /// <summary>
        /// The log
        /// </summary>
        private static readonly EventLogger<LegislatorApiController> log = new EventLogger<LegislatorApiController>();

        /// <summary>
        /// The Legislator repository
        /// </summary>
        private ILegislator _legislator;

        private ILegislator LegislatorRepository
        {
            get
            {
                if (_legislator == null)
                {
                    _legislator = UnityHelper.Resolve<ILegislator>();
                }

                return _legislator;
            }
        }


        /// <summary>
        /// Get the List of all the Legislator Page DDL
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllLegislatorManagementDDL")]
        public async Task<IHttpActionResult> GetAllLegislatorManagementDDL()
        {
            try
            {
                LegislatorViewModel result = await LegislatorRepository.GetAllLegislatorPageDDL();
                return Ok(result);
            }
            catch (Exception ex)
            {
                log.LogError(LoggingLevel.Error, "BadRequest", "Exception occur while Getting WebLink page DDL", ex);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get the List Legislator
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [Route("GetAllLegislators")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAllLegislators(string parametersJson)
        {
            try
            {
                var parameters = JsonConvert.DeserializeObject<GridParameters>(parametersJson);
                ApiOutput apiOutput = await LegislatorRepository.GetAllLegislatorList(parameters);
                var result = new { Data = apiOutput.Data, Total = apiOutput.TotalRecords };
                return Json(result);
            }
            catch (Exception ex)
            {
                log.LogError(LoggingLevel.Error, "BadRequest", "Exception occur while Getting webLinks", ex);
                return BadRequest(ex.Message);
            }
        }

        [Route("UpdateLegislator")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateLegislator(LegislatorViewModel model)
        {
            try
            {
                bool isSave = false;
                if (!ModelState.IsValid)
                {
                    return Ok(FormatResult(false, ModelState));
                }

                isSave = await LegislatorRepository.UpdateLegislator(model);

                return Ok(FormatResult(isSave, (isSave ? (string.IsNullOrEmpty(model.LegislatorHashId) ? Resources.Resource.IndividualLegislatorAddedSuccess : Resources.Resource.IndividualLegislatorUpdatedSuccess) : Resources.Resource.ErrorWhileSaving)));
            }
            catch (Exception ex)
            {
                log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, model);
                if (string.IsNullOrEmpty(model.LegislatorHashId))
                  AuditLogs.Write<LegislatorViewModel, string>(AuditConstants.IndividualLegislator, AuditType.InsertFailure, model, (string)null, Helper.GetInnerException(ex));
                else
                  AuditLogs.Write<LegislatorViewModel, string>(AuditConstants.IndividualLegislator, AuditType.UpdateFailure, model, (string)null, Helper.GetInnerException(ex));
                return BadRequest(ex.Message);
            }
        }


        [Route("DeleteLegislator")]
        [HttpGet]
        public async Task<IHttpActionResult> DeleteLegislator(string legislatorHashId)
        {
            try
            {
                bool isSave = false;

                isSave = await LegislatorRepository.DeleteLegislator(legislatorHashId);

                return Ok(FormatResult(isSave, (isSave ? Resources.Resource.LegislatorDeletedSuccessfully : Resources.Resource.ErrorWhileDeleting)));
            }
            catch (Exception ex)
            {
                log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, legislatorHashId);
                AuditLogs.Write(AuditConstants.IndividualLegislator, AuditType.DeleteFailure, Helper.GetInnerException(ex));
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get Legislator By Hash Id
        /// </summary>
        /// <param name="webLinkHashId"></param>
        /// <returns></returns>
        [Route("GetLegislatorByHashId")]
        [HttpGet]
        public async Task<IHttpActionResult> GetLegislatorByHashId(string legislatorHashId)
        {
            try
            {
                LegislatorViewModel model = await LegislatorRepository.GetLegislatorBasedOnHashId(legislatorHashId);
                return Ok(model);
            }
            catch (Exception ex)
            {
                log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, legislatorHashId);
                return BadRequest(ex.Message);
            }
        }
    }
}