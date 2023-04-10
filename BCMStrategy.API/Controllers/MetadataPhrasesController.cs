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
using BCMStrategy.API.AuditLog;
using BCMStrategy.Common.AuditLog;
using BCMStrategy.Data.Abstract;

namespace BCMStrategy.API.Controllers
{
  [Authentication]
  [RoutePrefix("api/MetadataPhrases")]
  public class MetadataPhrasesController : BaseApiController
  {
    private static readonly EventLogger<MetadataPhrasesController> _log = new EventLogger<MetadataPhrasesController>();


    private IMetadataPhrases _metadataPhrasesRepository;

    private IMetadataPhrases MetadataPhrasesRepository
    {
      get
      {
        if (_metadataPhrasesRepository == null)
        {
          _metadataPhrasesRepository = new MetadataPhrasesRepository();
        }

        return _metadataPhrasesRepository;
      }
    }

    [Route("UpdateMetadataPhrases")]
    [HttpPost]
    public async Task<IHttpActionResult> UpdateMetadataPhrases(MetadataPhrasesModel metadataPhrasesModel)
    {
      try
      {
        bool isSave = false;
        
        if (!ModelState.IsValid)
        {
          return Ok(FormatResult(false, ModelState));
        }

        isSave = await MetadataPhrasesRepository.UpdateMetadataPhrases(metadataPhrasesModel);

        return Ok(FormatResult(isSave, (isSave ? (string.IsNullOrEmpty(metadataPhrasesModel.MetadataPhrasesMasterHashId) ? Resources.Resource.MetadataPhrasesAddedSuccess : Resources.Resource.MetadataPhrasesUpdatedSuccess) : Resources.Resource.ErrorWhileSaving)));
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, metadataPhrasesModel);
        if (string.IsNullOrEmpty(metadataPhrasesModel.MetadataPhrasesMasterHashId))
          AuditLogs.Write<MetadataPhrasesModel, string>(AuditConstants.Phrases, AuditType.InsertFailure, metadataPhrasesModel, (string)null, Helper.GetInnerException(ex));
        else
          AuditLogs.Write<MetadataPhrasesModel, string>(AuditConstants.Phrases, AuditType.UpdateFailure, metadataPhrasesModel, (string)null, Helper.GetInnerException(ex));
        return BadRequest(ex.Message);
      }
    }

    [Route("DeleteMetadataPhrases")]
    [HttpGet]
    public async Task<IHttpActionResult> DeleteMetadataPhrases(string metadataPhrasesMasterHashId)
    {
      try
      {
        
        bool isSave = false;

        isSave = await MetadataPhrasesRepository.DeleteMetadataPhrases(metadataPhrasesMasterHashId);

        return Ok(FormatResult(isSave, (isSave ? Resources.Resource.MetadataPhrasesDeletedSuccessfully : Resources.Resource.ErrorWhileDeleting)));
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, metadataPhrasesMasterHashId);
        AuditLogs.Write(AuditConstants.Phrases, AuditType.DeleteFailure, Helper.GetInnerException(ex));
        return BadRequest(ex.Message);
      }
    }

    [Route("GetAllMetadataPhrasesList")]
    [HttpGet]
    public async Task<IHttpActionResult> GetAllMetadataPhrasesList(string parametersJson)
    {
      var parameters = JsonConvert.DeserializeObject<GridParameters>(parametersJson);
      ApiOutput apiOutput = await MetadataPhrasesRepository.GetAllMetadataPhrasesList(parameters);
      var result = new { Data = apiOutput.Data, Total = apiOutput.TotalRecords };
      return Json(result);
    }

    /// <summary>
    /// Get Metadata Phrases By Hash Id
    /// </summary>
    /// <param name="phrasesHashId"></param>
    /// <returns></returns>
    [Route("GetMetadataPhrasesByHashId")]
    [HttpGet]
    public async Task<IHttpActionResult> GetMetadataPhrasesByHashId(string phrasesHashId)
    {
      try
      {
        MetadataPhrasesModel metadataPhrasesModel = await MetadataPhrasesRepository.GetMetadataPhrasesByHashId(phrasesHashId);
        return Ok(metadataPhrasesModel);
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, phrasesHashId);
        return BadRequest(ex.Message);
      }
    }
  }
}
