using BCMStrategy.API.Filter;
using BCMStrategy.Common.Unity;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Logger;
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
  [RoutePrefix("api/WebsiteTypes")]
  public class WebsiteTypesController : BaseApiController
  {
    private static readonly EventLogger<WebsiteTypesController> _log = new EventLogger<WebsiteTypesController>();

    private IWebsiteTypes _WebsiteTypesRepository;

    private IWebsiteTypes WebsiteTypesRepository
    {
      get
      {
        if (_WebsiteTypesRepository == null)
        {
          _WebsiteTypesRepository = UnityHelper.Resolve<IWebsiteTypes>();
        }
        return _WebsiteTypesRepository;
      }
    }

    /// <summary>
    /// Get Drop down list of website Type
    /// </summary>
    /// <returns>Return website types list</returns>
    [Route("GetDropdownWebsiteTypesList")]
    [HttpGet]
    public async Task<IHttpActionResult> GetDropdownWebsiteTypesList()
    {
      try
      {
        ApiOutput apiOutput = await WebsiteTypesRepository.GetDDLWebsiteTypesList();
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
