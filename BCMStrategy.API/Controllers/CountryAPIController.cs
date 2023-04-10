using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using BCMStrategy.Common.Unity;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Logger;

namespace BCMStrategy.API.Controllers
{
  [RoutePrefix("api/Country")]
  public class CountryApiController : BaseApiController
  {
    private static readonly EventLogger<CountryApiController> _log = new EventLogger<CountryApiController>();
    private ICountryMaster _countryMasterRepository;

    private ICountryMaster CountryRepository
    {
      get
      {
        if (_countryMasterRepository == null)
        {
          _countryMasterRepository = UnityHelper.Resolve<ICountryMaster>();
        }

        return _countryMasterRepository;
      }
    }

    /// <summary>
    /// Get dropdown Country list
    /// </summary>
    /// <returns>Country List</returns>
    [Route("GetDropdownCountryList")]
    [HttpGet]
    public async Task<IHttpActionResult> GetDropdownCouncilNameList()
    {
      try
      {
        ApiOutput apiOutput = await CountryRepository.GetDDLCountryList();
        return Ok(apiOutput);
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex);
        return BadRequest(ex.Message);
      }
    }
  }
}
