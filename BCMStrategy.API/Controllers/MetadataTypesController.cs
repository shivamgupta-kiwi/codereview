using BCMStrategy.API.Filter;
using BCMStrategy.Common.Kendo;
using BCMStrategy.Common.Unity;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Data.Repository.Concrete;
using BCMStrategy.Logger;
using BCMStrategy.Resources;
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
  [RoutePrefix("api/MetadataTypes")]
  public class MetadataTypesController : BaseApiController
  {
    private static readonly EventLogger<MetadataTypesController> _log = new EventLogger<MetadataTypesController>();

    private IMetadataTypes _metadataTypesRepository;

    private IMetadataTypes MetadataTypesRepository
    {
      get
      {
        if (_metadataTypesRepository == null)
        {
          _metadataTypesRepository = UnityHelper.Resolve<IMetadataTypes>();
        }

        return _metadataTypesRepository;
      }
    }

    /// <summary>
    /// Get Drop down list of Metadata Type
    /// </summary>
    /// <returns>Return meta data types list</returns>
    [Route("GetDropdownMetadataTypesList")]
    [HttpGet]
    public async Task<IHttpActionResult> GetDropdownMetadataTypesList()
    {
      try
      {
        ApiOutput apiOutput = await MetadataTypesRepository.GetDropdownMetadataTypesList();
        return Ok(apiOutput);
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex);
        return BadRequest(ex.Message);
      }
    }

    [Route("GetAllMetadataTypesList")]
    [HttpGet]
    public async Task<IHttpActionResult> GetAllMetadataTypesList(string parametersJson)
    {
      var parameters = JsonConvert.DeserializeObject<GridParameters>(parametersJson);
      ApiOutput apiOutput = await MetadataTypesRepository.GetAllMetadataTypesList(parameters);
      var result = new { Data = apiOutput.Data, Total = apiOutput.TotalRecords };
      return Json(result);
    }

    [Route("UpdateMetadataTypes")]
    [HttpPost]
    public async Task<IHttpActionResult> UpdateMetadataTypes(MetadataTypesModel metadataTypesModel)
    {
      try
      {
        bool isSave = false;

        if (metadataTypesModel.IsActivityTypeExist == true)
        {
          ModelState.Remove("MetadataTypesModel.MetaDataValue");
        }

        if (!ModelState.IsValid)
        {
          return Ok(FormatResult(false, ModelState));
        }

        isSave = await MetadataTypesRepository.UpdateMetadataTypes(metadataTypesModel);

        return Ok(FormatResult(isSave, (isSave ? (string.IsNullOrEmpty(metadataTypesModel.MetadataTypesMasterHashId) ? Resources.Resource.MetadataTypsAddedSuccess : Resources.Resource.MetadataTypesUpdatedSuccess) : Resources.Resource.ErrorWhileSaving)));
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, metadataTypesModel);
        if (string.IsNullOrEmpty(metadataTypesModel.MetadataTypesMasterHashId))
          AuditLogs.Write<MetadataTypesModel, string>(AuditConstants.ActionType, AuditType.InsertFailure, metadataTypesModel, (string)null, Helper.GetInnerException(ex));
        else
          AuditLogs.Write<MetadataTypesModel, string>(AuditConstants.ActionType, AuditType.UpdateFailure, metadataTypesModel, (string)null, Helper.GetInnerException(ex));
        return BadRequest(ex.Message);
      }
    }

    [Route("DeleteMetadataTypes")]
    [HttpGet]
    public async Task<IHttpActionResult> DeleteMetadataTypes(string metadataTypesMasterHashId)
    {
      try
      {

        ApiOutput apiOutput = await MetadataTypesRepository.DeleteMetadataTypes(metadataTypesMasterHashId);
        return Ok(apiOutput);
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, metadataTypesMasterHashId);
        AuditLogs.Write(AuditConstants.ActionType, AuditType.DeleteFailure, Helper.GetInnerException(ex));
        return BadRequest(ex.Message);
      }
    }

    [Route("ImportMetadataTypesCSVRecord")]
    [HttpPost]
    public async Task<IHttpActionResult> ImportMetadataTypesCSVRecord(List<MetadataTypesCsvImportModel> metadataTypesModel)
    {
      ApiOutput apiOutput = new ApiOutput();

      Validate(metadataTypesModel);
      if (!ModelState.IsValid)
      {
        return Ok(FormatResult(false, ModelState));
      }

      bool isSave = false;
      isSave = await MetadataTypesRepository.ImportMetadataTypesRecords(metadataTypesModel);
      apiOutput.Data = isSave;
      apiOutput.TotalRecords = 0;
      apiOutput.ErrorMessage = isSave ? Resource.SuccessfullImprtMessage : Resource.ValidateImportError;
      return Ok(apiOutput);
    }

    /// <summary>
    /// Get Metadata Type By Hash Id
    /// </summary>
    /// <param name="metadataTypeHashId"></param>
    /// <returns></returns>
    [Route("GetMetadataTypeByHashId")]
    [HttpGet]
    public async Task<IHttpActionResult> GetMetadataTypeByHashId(string metadataTypeHashId)
    {
      try
      {
        MetadataTypesModel metadataTypesModel = await MetadataTypesRepository.GetMetadataTypeByHashId(metadataTypeHashId);
        return Ok(metadataTypesModel);
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, metadataTypeHashId);
        return BadRequest(ex.Message);
      }
    }
  }
}
