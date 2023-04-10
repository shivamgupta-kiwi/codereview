using BCMStrategy.API.Filter;
using BCMStrategy.Common.Kendo;
using BCMStrategy.Common.Unity;
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
    [RoutePrefix("api/LexiconType")]
    public class LexiconTypeController : BaseApiController
    {
        private static readonly EventLogger<LexiconTypeController> _log = new EventLogger<LexiconTypeController>();

        private ILexiconType _lexiconTypeRepository;

        private ILexiconType LexiconTypeRepository
        {
            get
            {
                if (_lexiconTypeRepository == null)
                {
                    _lexiconTypeRepository = UnityHelper.Resolve<ILexiconType>();
                }
                return _lexiconTypeRepository;
            }
        }


        /// <summary>
        /// Get Drop down list of Lexicon Type
        /// </summary>
        /// <returns>Return lexicon type list</returns>
        [Route("GetDropdownLexiconTypeList")]
        [HttpGet]
        public async Task<IHttpActionResult> GetDropdownLexiconTypeList()
        {
            try
            {
                ApiOutput apiOutput = await LexiconTypeRepository.GetDDLLexiconTypeList();
                return Ok(apiOutput);
            }
            catch (Exception ex)
            {
                _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex);
                return BadRequest(ex.Message);
            }
        }


        [Route("GetAllLexiconTypeList")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAllLexiconTypeList(string parametersJson)
        {
            var parameters = JsonConvert.DeserializeObject<GridParameters>(parametersJson);
            ApiOutput apiOutput = await LexiconTypeRepository.GetAllLexiconTypeList(parameters);
            var result = new { Data = apiOutput.Data, Total = apiOutput.TotalRecords };
            return Json(result);
        }


    }
}
