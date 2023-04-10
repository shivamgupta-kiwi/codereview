using BCMStrategy.API.AuditLog;
using BCMStrategy.API.Filter;
using BCMStrategy.API.Helpers;
using BCMStrategy.Common.AuditLog;
using BCMStrategy.Common.Kendo;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Data.Repository.Concrete;
using BCMStrategy.Logger;
using BCMStrategy.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace BCMStrategy.API.Controllers
{
  [Authentication]
  [RoutePrefix("api/Institution")]
  public class InstitutionApiController : BaseApiController
  {
    private static readonly EventLogger<InstitutionApiController> _log = new EventLogger<InstitutionApiController>();

    private IInstitutions _institutionRepository;

    private IInstitutions InstitutionRepository
    {
      get
      {
        if (_institutionRepository == null)
        {
          _institutionRepository = new InstitutionsRepository();
        }

        return _institutionRepository;
      }
    }

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

    private IInstitutionTypes _institutionTypesRepository;

    private IInstitutionTypes InstitutionTypesRepository
    {
      get
      {
        if (_institutionTypesRepository == null)
        {
          _institutionTypesRepository = new InstitutionTypesRepository();
        }

        return _institutionTypesRepository;
      }
    }

    [Route("UpdateInstitutions")]
    [HttpPost]
    public async Task<IHttpActionResult> UpdateInstitutions(InstitutionModel institutionModel)
    {
      try
      {
        bool isSave = false;
        if (!ModelState.IsValid)
        {
          return Ok(FormatResult(false, ModelState));
        }

        isSave = await InstitutionRepository.UpdateInstitutions(institutionModel);

        return Ok(FormatResult(isSave, (isSave ? (string.IsNullOrEmpty(institutionModel.InstitutionMasterHashId) ? Resources.Resource.InstitutionAddedSuccess : Resources.Resource.InstitutionUpdatedSuccess) : Resources.Resource.ErrorWhileSaving)));
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, institutionModel);
        if (string.IsNullOrEmpty(institutionModel.InstitutionMasterHashId))
          AuditLogs.Write<InstitutionModel, string>(AuditConstants.Institution, AuditType.InsertFailure, institutionModel, (string)null, Helper.GetInnerException(ex));
        else
          AuditLogs.Write<InstitutionModel, string>(AuditConstants.Institution, AuditType.UpdateFailure, institutionModel, (string)null, Helper.GetInnerException(ex));
        return BadRequest(ex.Message);
      }
    }

    [Route("DeleteInstitution")]
    [HttpGet]
    public async Task<IHttpActionResult> DeleteInstitution(string institutionMasterHashId)
    {
      try
      {
        bool isSave = false;
        isSave = await InstitutionRepository.DeleteInstitution(institutionMasterHashId);

        return Ok(FormatResult(isSave, (isSave ? Resources.Resource.InstitutionDeletedSuccessfully : Resources.Resource.ErrorWhileDeleting)));
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, institutionMasterHashId);
        AuditLogs.Write(AuditConstants.Institution, AuditType.DeleteFailure, Helper.GetInnerException(ex));
        return BadRequest(ex.Message);
      }
    }

    [Route("GetAllInstitutionsList")]
    [HttpGet]
    public async Task<IHttpActionResult> GetAllInstitutionsList(string parametersJson)
    {
      var parameters = JsonConvert.DeserializeObject<GridParameters>(parametersJson);
      ApiOutput apiOutput = await InstitutionRepository.GetAllInstitutionsList(parameters);
      var result = new { Data = apiOutput.Data, Total = apiOutput.TotalRecords };
      return Json(result);
    }

    [Route("ValidateInstitutionCSVRecord")]
    [HttpPost]
    public async Task<IHttpActionResult> ValidateInstitutionCSVRecord()
    {
      ApiOutput apiOutput = new ApiOutput();
      var httpRequest = HttpContext.Current.Request;
      string instTypeId = HttpContext.Current.Request.Params["InstitutionTypeHashId"];
      apiOutput.Data = new List<string>();
      apiOutput.TotalRecords = 0;
      bool isValid = true;
      string validationMsg = string.Empty;

      List<IList<string>> validateLines;
      validationMsg = ImportCSV(httpRequest, 4, out validateLines);

      if (string.IsNullOrEmpty(validationMsg))
      {
        if (string.IsNullOrEmpty(instTypeId))
        {
          isValid = false;
          validationMsg = "InstType:" + string.Format(Resource.ValidateRequireDropDown, Resource.LblInstitutionTypes);
        }
        else
        {
          var listOfInstitutions = await ValidateInstitutionModel(validateLines, instTypeId.ToDecrypt());
          apiOutput.Data = listOfInstitutions;
          apiOutput.TotalRecords = listOfInstitutions.Count;
        }
      }
      else
      {
        isValid = false;
      }

      apiOutput.ErrorMessage = isValid ? string.Empty : validationMsg;
      return Ok(apiOutput);
    }

    [Route("ImportInstitutionsCSVRecord")]
    [HttpPost]
    public async Task<IHttpActionResult> ImportInstitutionsCSVRecord(List<InstitutionCsvImportModel> institutionsModel)
    {
      ApiOutput apiOutput = new ApiOutput();

      Validate(institutionsModel);
      if (!ModelState.IsValid)
      {
        return Ok(FormatResult(false, ModelState));
      }

      bool isSave = false;
      isSave = await InstitutionRepository.ImportInstitutionRecords(institutionsModel);
      apiOutput.Data = isSave;
      apiOutput.TotalRecords = 0;
      apiOutput.ErrorMessage = isSave ? Resource.SuccessfullImprtMessage : Resource.ValidateImportError;
      return Ok(apiOutput);
    }

    public List<KeyValuePair<string, string>> ErrorModel { get; set; }

    private async Task<List<InstitutionCsvImportModel>> ValidateInstitutionModel(List<IList<string>> validateLines, string instTypeId)
    {
      List<InstitutionCsvImportModel> listOfInstiutions = new List<InstitutionCsvImportModel>();

      if (validateLines == null || !validateLines.Any())
        return listOfInstiutions;

      listOfInstiutions = validateLines.Select(line => new InstitutionCsvImportModel()
      {
        CountryName = !string.IsNullOrEmpty(line[0].Trim()) ? line[0].Trim() : null,
        InstitutionName = !string.IsNullOrEmpty(line[1].Trim()) ? line[1].Trim() : null,
        IsEuropeanUnion = !string.IsNullOrEmpty(line[2].Trim()) && line[2].Trim().ToLower() == "yes" ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful,
        InstitutionTypeMasterId = !string.IsNullOrEmpty(instTypeId) ? Convert.ToInt32(instTypeId) : 0,
        CountryMasterId = CountryRepository.GetCountryIdByName((!string.IsNullOrEmpty(line[0].Trim()) ? line[0].Trim() : string.Empty)),
        EntityName = !string.IsNullOrEmpty(line[3].Trim()) ? line[3].Trim() : null,
      }).ToList();

      int parts = listOfInstiutions.Count / 50;
      var splitedListOfInstitutions = CommonRepository.Split(listOfInstiutions, parts > 1 ? parts : 1);
      var handleValidation = new List<Task>();

      foreach (var chunkOfInstitutions in splitedListOfInstitutions)
      {
        handleValidation.Add(ValidateInstitutionsModelState(chunkOfInstitutions, listOfInstiutions));
      }

      await Task.WhenAll(handleValidation);
      return listOfInstiutions;
    }

    private async Task ValidateInstitutionsModelState(List<InstitutionCsvImportModel> chunkOfInstitutions, List<InstitutionCsvImportModel> listOfInstiutions)
    {
      await Task.Yield();
      foreach (var model in chunkOfInstitutions)
      {
        this.Validate<InstitutionCsvImportModel>(model);
        HandleModelStateForImportInstitutions(ModelState, model, listOfInstiutions);
        List<KeyValuePair<string, string>> errorModel = CommonUtilities.HandleModelStateForImport(ModelState);
        model.ErrorModel = errorModel;
        ModelState.Clear();
      }
    }

    private void HandleModelStateForImportInstitutions(System.Web.Http.ModelBinding.ModelStateDictionary ModelState, InstitutionCsvImportModel model, List<InstitutionCsvImportModel> listOfInstiutions)
    {
      if (!string.IsNullOrEmpty(model.CountryMasterHashId) && !string.IsNullOrEmpty(model.InstitutionName))
      {
        var listOfDuplicate = listOfInstiutions.Count(x => x.CountryMasterHashId == model.CountryMasterHashId && x.InstitutionName == model.InstitutionName);
        if (listOfDuplicate > 1)
        {
          ModelState.AddModelError("InstitutionName", Resource.ValidateInstitutionsDuplicate);
        }
      }

      var institutionTypeModel = InstitutionTypesRepository.GetInstitutionTypeByHashId(model.InstitutionTypeMasterHashId);

      if (!model.IsEuropeanUnion && institutionTypeModel.Result.InstitutionTypesName != Helper.InstitutionTypes.Multilateral.ToString() && string.IsNullOrEmpty(model.CountryMasterHashId))
      {
        ModelState.AddModelError("CountryName", string.Format(Resource.ValidateRequiredField, "Country"));
      }
    }

    /// <summary>
    /// Get Institution By Hash Id
    /// </summary>
    /// <param name="institutionHashId"></param>
    /// <returns></returns>
    [Route("GetInstitutionByHashId")]
    [HttpGet]
    public async Task<IHttpActionResult> GetInstitutionByHashId(string institutionHashId)
    {
      try
      {
        InstitutionModel institutionModel = await InstitutionRepository.GetInstitutionByHashId(institutionHashId);
        return Ok(institutionModel);
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, institutionHashId);
        return BadRequest(ex.Message);
      }
    }
  }
}