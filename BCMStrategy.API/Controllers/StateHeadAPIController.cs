using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using BCMStrategy.API.Filter;
using BCMStrategy.API.Helpers;
using BCMStrategy.Common.Kendo;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Data.Repository.Concrete;
using BCMStrategy.Logger;
using BCMStrategy.Resources;
using Newtonsoft.Json;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Common.AuditLog;
using BCMStrategy.API.AuditLog;

namespace BCMStrategy.API.Controllers
{
  [Authentication]
  [RoutePrefix("api/StateHead")]
  public class StateHeadApiController : BaseApiController
  {
    private static readonly EventLogger<StateHeadApiController> _log = new EventLogger<StateHeadApiController>();

    private ICommonRepository _commonRepository;

    private ICommonRepository CommonRepository
    {
      get
      {
        if (_commonRepository == null)
        {
          _commonRepository = new CommonRepository();
        }

        return _commonRepository;
      }
    }

    private ICountryMaster _countryRepository;

    private ICountryMaster CountryRepository
    {
      get
      {
        if (_countryRepository == null)
        {
          _countryRepository = new CountryMasterRepository();
        }

        return _countryRepository;
      }
    }

    private IHeadStateRepository _headStateRepository;

    private IHeadStateRepository HeadStateRepository
    {
      get
      {
        if (_headStateRepository == null)
        {
          _headStateRepository = new HeadStateRepository();
        }

        return _headStateRepository;
      }
    }
    private IPolicyMaker _policyMakerRepository;

    private IPolicyMaker PolicyMakerRepository
    {
      get
      {
        if (_policyMakerRepository == null)
        {
          _policyMakerRepository = new PolicyMakerRepository();
        }

        return _policyMakerRepository;
      }
    }
    [Route("ImportCSVRecord")]
    [HttpPost]
    public async Task<IHttpActionResult> ImportCSVRecord(List<HeadStateImportModel> headStateBatchModel)
    {
      ApiOutput apiOutput = new ApiOutput();

      Validate(headStateBatchModel);

      if (!ModelState.IsValid)
      {
        return Ok(FormatResult(false, ModelState));
      }

      bool isSave = false;
      isSave = await HeadStateRepository.ImportHeadStateRecord(headStateBatchModel);
      apiOutput.Data = isSave;
      apiOutput.TotalRecords = 0;
      apiOutput.ErrorMessage = isSave ? Resource.SuccessfullImprtMessage : Resource.ValidateImportError;
      return Ok(apiOutput);
    }

    [Route("ValidateCSVRecord")]
    [HttpPost]
    public async Task<IHttpActionResult> ValidateCSVRecord()
    {
      ApiOutput apiStateHeadOutput = new ApiOutput();
      var httpStateHeadRequest = HttpContext.Current.Request;

      apiStateHeadOutput.Data = new List<string>();
      apiStateHeadOutput.TotalRecords = 0;

      bool isValidStateHead = true;
      string validationMsg = string.Empty;

      if (httpStateHeadRequest.Files.Count > 0)
      {
        List<IList<string>> validateLines;
        validationMsg = ImportCSV(httpStateHeadRequest, 4, out validateLines);

        if (string.IsNullOrEmpty(validationMsg))
        {
          var listOfHeadState = await ValidateHeadStateModel(validateLines);
          apiStateHeadOutput.Data = listOfHeadState;
          apiStateHeadOutput.TotalRecords = listOfHeadState.Count;
        }
        else
        {
          isValidStateHead = false;
        }
      }
      else
      {
        isValidStateHead = false;
        validationMsg = Resource.ValidateToSelectFile;
      }

      apiStateHeadOutput.ErrorMessage = isValidStateHead ? string.Empty : validationMsg;
      return Ok(apiStateHeadOutput);
    }

    [Route("GetAllStateHeadList")]
    [HttpGet]
    public async Task<IHttpActionResult> GetAllStateHeadList(string parametersJson)
    {
      var parameters = JsonConvert.DeserializeObject<GridParameters>(parametersJson);
      ApiOutput apiOutput = await HeadStateRepository.GetStateHeadList(parameters);
      var result = new { Data = apiOutput.Data, Total = apiOutput.TotalRecords };
      return Json(result);
    }

    [Route("UpdateStateHead")]
    [HttpPost]
    public async Task<IHttpActionResult> UpdateStateHead(StateHeadModel stateHeadModel)
    {
      try
      {
        bool isSave = false;

        if (!ModelState.IsValid)
        {
          return Ok(FormatResult(false, ModelState));
        }

        isSave = await HeadStateRepository.UpdateStateHead(stateHeadModel);

        return Ok(FormatResult(isSave, (isSave ? (string.IsNullOrEmpty(stateHeadModel.StateHeadMasterHashId) ? Resources.Resource.HeadStateAddedSuccess : Resources.Resource.HeadStateUpdatedSuccess) : Resources.Resource.ErrorWhileSaving)));
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, stateHeadModel);
        if (string.IsNullOrEmpty(stateHeadModel.StateHeadMasterHashId))
          AuditLogs.Write<StateHeadModel, string>(AuditConstants.HeadOfStateAndGovrnment, AuditType.InsertFailure, stateHeadModel, (string)null, Helper.GetInnerException(ex));
        else
          AuditLogs.Write<StateHeadModel, string>(AuditConstants.HeadOfStateAndGovrnment, AuditType.UpdateFailure, stateHeadModel, (string)null, Helper.GetInnerException(ex));
        return BadRequest(ex.Message);
      }
    }

    [Route("DeleteStateHead")]
    [HttpGet]
    public async Task<IHttpActionResult> DeleteStateHead(string stateHeadMasterHashId)
    {
      try
      {
        bool isSave = false;

        isSave = await HeadStateRepository.DeleteStateHead(stateHeadMasterHashId);

        return Ok(FormatResult(isSave, (isSave ? Resources.Resource.StateHeadDeletedSuccessfully : Resources.Resource.ErrorWhileDeleting)));
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, stateHeadMasterHashId);
        AuditLogs.Write(AuditConstants.HeadOfStateAndGovrnment, AuditType.DeleteFailure, (string)null, (string)null, Helper.GetInnerException(ex));
        return BadRequest(ex.Message);
      }
    }

    private async Task<List<HeadStateImportModel>> ValidateHeadStateModel(List<IList<string>> validateLines)
    {
      List<HeadStateImportModel> listOfHeadState = new List<HeadStateImportModel>();

      if (validateLines == null || !validateLines.Any())
        return listOfHeadState;

      listOfHeadState = validateLines.Select(line => new HeadStateImportModel()
      {
        CountryName = !string.IsNullOrEmpty(line[0].Trim()) ? line[0].Trim() : null,
        DesignationName = !string.IsNullOrEmpty(line[1].Trim()) ? line[1].Trim() : null,
        FirstName = !string.IsNullOrEmpty(line[2].Trim()) ? line[2].Trim() : null,
        LastName = !string.IsNullOrEmpty(line[3].Trim()) ? line[3].Trim() : null,
        HeadStateName = (line[2].Trim() + " " ?? string.Empty) + (line[3].Trim() ?? string.Empty),
        DesignationMasterId = PolicyMakerRepository.GetDesignationIdByName((!string.IsNullOrEmpty(line[1].Trim()) ? line[1].Trim() : string.Empty)),
        CountryMasterId = CountryRepository.GetCountryIdByName((!string.IsNullOrEmpty(line[0].Trim()) ? line[0].Trim() : string.Empty))
      }).ToList();

      int parts = listOfHeadState.Count / 50;
      var splitedListOfHeadState = CommonRepository.Split(listOfHeadState, parts > 1 ? parts : 1);
      var handleValidation = new List<Task>();

      foreach (var chunkOfHeadState in splitedListOfHeadState)
      {
        handleValidation.Add(ValidateHeadStateModelState(chunkOfHeadState, listOfHeadState));
      }

      await Task.WhenAll(handleValidation);
      return listOfHeadState;
    }

    private async Task ValidateHeadStateModelState(List<HeadStateImportModel> chunkOfHeadState, List<HeadStateImportModel> listOfHeadState)
    {
      await Task.Yield();
      foreach (var model in chunkOfHeadState)
      {
        this.Validate<HeadStateImportModel>(model);
        HandleModelStateForImportHeadState(ModelState, model, listOfHeadState);
        List<KeyValuePair<string, string>> errorModel = CommonUtilities.HandleModelStateForImport(ModelState);
        model.ErrorModel = errorModel;
        ModelState.Clear();
      }
    }

    private void HandleModelStateForImportHeadState(System.Web.Http.ModelBinding.ModelStateDictionary ModelState, HeadStateImportModel model, List<HeadStateImportModel> listOfHeadState)
    {
      var listOfDuplicate = listOfHeadState.Count(x => x.CountryMasterHashId == model.CountryMasterHashId && x.DesignationMasterHashId == model.DesignationMasterHashId && x.FirstName == model.FirstName && x.LastName == model.LastName);
      if (listOfDuplicate > 1)
      {
        ModelState.AddModelError("HeadStateName", Resource.ValidateHeadStateDuplicate);
      }
    }

    /// <summary>
    /// Get Metadata noun plus verb By Hash Id
    /// </summary>
    /// <param name="stateHeadHashId"></param>
    /// <returns></returns>
    [Route("GetStateHeadByHashId")]
    [HttpGet]
    public async Task<IHttpActionResult> GetStateHeadByHashId(string stateHeadHashId)
    {
      try
      {
        StateHeadModel stateHeadModel = await HeadStateRepository.GetStateHeadByHashId(stateHeadHashId);
        return Ok(stateHeadModel);
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, stateHeadHashId);
        return BadRequest(ex.Message);
      }
    }
  }
}