using BCMStrategy.API.AuditLog;
using BCMStrategy.API.Filter;
using BCMStrategy.Common.AuditLog;
using BCMStrategy.Common.Kendo;
using BCMStrategy.Data.Abstract;
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
  [RoutePrefix("api/MetadataNounplusVerb")]
  public class MetadataNounplusVerbController : BaseApiController
  {
    private static readonly EventLogger<MetadataNounplusVerbController> _log = new EventLogger<MetadataNounplusVerbController>();
    

    private IMetadataNounplusVerb _metadataNounplusVerbRepository;

    private IMetadataNounplusVerb MetadataNounplusVerbRepository
    {
      get
      {
        if (_metadataNounplusVerbRepository == null)
        {
          _metadataNounplusVerbRepository = new MetadataNounplusVerbRepository();
        }

        return _metadataNounplusVerbRepository;
      }
    }

    /// <summary>
    /// Get Drop down list of Metadata Dynamic table Noun plus Verb
    /// </summary>
    /// <returns>Return meta data DynamicTableNounplusVerb list</returns>
    [Route("GetDropdownMetadataDynamicNounplusVerbList")]
    [HttpGet]
    public async Task<IHttpActionResult> GetDropdownMetadataDynamicNounplusVerbList()
    {
      try
      {
        ApiOutput apiOutput = await MetadataNounplusVerbRepository.GetDropdownMetadataDynamicNounplusVerbList();
        return Ok(apiOutput);
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex);
        return BadRequest(ex.Message);
      }
    }

    [Route("UpdateMetadataNounplusVerb")]
    [HttpPost]
    public async Task<IHttpActionResult> UpdateMetadataNounplusVerb(MetadataNounplusVerbModel metadataNounplusVerbModel)
    {
      try
      {
        bool isSave = false;
        
        if (!metadataNounplusVerbModel.IsHardCoded)
        {
          ModelState.Remove("MetadataNounplusVerbModel.Noun");
        }
        else
        {
          ModelState.Remove("MetadataNounplusVerbModel.MetadataDynamicNounplusVerb");
        }
        if (!ModelState.IsValid)
        {
          return Ok(FormatResult(false, ModelState));
        }

        isSave = await MetadataNounplusVerbRepository.UpdateMetadataNounplusVerb(metadataNounplusVerbModel);

        return Ok(FormatResult(isSave, (isSave ? (string.IsNullOrEmpty(metadataNounplusVerbModel.MetadataNounplusVerbMasterHashId) ? Resources.Resource.MetadataNounplusVerbAddedSuccess : Resources.Resource.MetadataNounplusVerbUpdatedSuccess) : Resources.Resource.ErrorWhileSaving)));
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, metadataNounplusVerbModel);

        if (string.IsNullOrEmpty(metadataNounplusVerbModel.MetadataNounplusVerbMasterHashId))
        {
          AuditLogs.Write<MetadataNounplusVerbModel, string>(AuditConstants.ActionNounPlusVerb, AuditType.InsertFailure, metadataNounplusVerbModel, (string)null, Helper.GetInnerException(ex));
        }
        else
        { 
          AuditLogs.Write<MetadataNounplusVerbModel, string>(AuditConstants.ActionNounPlusVerb, AuditType.UpdateFailure, metadataNounplusVerbModel, (string)null, Helper.GetInnerException(ex));
        }
        return BadRequest(ex.Message);
      }
    }

    [Route("DeleteMetadataNounplusVerb")]
    [HttpGet]
    public async Task<IHttpActionResult> DeleteMetadataNounplusVerb(string metadataNounplusVerbMasterHashId)
    {
      try
      {
        bool isSave = false;
        
        isSave = await MetadataNounplusVerbRepository.DeleteMetadataNounplusVerb(metadataNounplusVerbMasterHashId);

        return Ok(FormatResult(isSave, (isSave ? Resources.Resource.MetadataNounplusVerbDeletedSuccessfully : Resources.Resource.ErrorWhileDeleting)));
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, metadataNounplusVerbMasterHashId);
        AuditLogs.Write(AuditConstants.ActionNounPlusVerb, AuditType.DeleteFailure, Helper.GetInnerException(ex));
        return BadRequest(ex.Message);
      }
    }

    [Route("GetAllMetadataNounplusVerbList")]
    [HttpGet]
    public async Task<IHttpActionResult> GetAllMetadataNounplusVerbList(string parametersJson)
    {
      var parameters = JsonConvert.DeserializeObject<GridParameters>(parametersJson);
      ApiOutput apiOutput = await MetadataNounplusVerbRepository.GetAllMetadataNounplusVerbList(parameters);
      var result = new { Data = apiOutput.Data, Total = apiOutput.TotalRecords };
      return Json(result);
    }

    /// <summary>
    /// Get Metadata noun plus verb By Hash Id
    /// </summary>
    /// <param name="nounVerbHashId"></param>
    /// <returns></returns>
    [Route("GetMetadataNounPlusVerbByHashId")]
    [HttpGet]
    public async Task<IHttpActionResult> GetMetadataNounPlusVerbByHashId(string nounVerbHashId)
    {
      try
      {
        MetadataNounplusVerbModel metadataNounplusVerbModel = await MetadataNounplusVerbRepository.GetMetadataNounPlusVerbByHashId(nounVerbHashId);
        return Ok(metadataNounplusVerbModel);
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, nounVerbHashId);
        return BadRequest(ex.Message);
      }
    }

  }
}
