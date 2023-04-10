using BCMStrategy.API.AuditLog;
using BCMStrategy.API.Filter;
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
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;


namespace BCMStrategy.API.Controllers
{
  [Authentication]
  [RoutePrefix("api/InternationalOrganization")]
  public class InternationalOrganizationApiController : BaseApiController
  {
    private static readonly EventLogger<InternationalOrganizationApiController> _log = new EventLogger<InternationalOrganizationApiController>();

    private IInternationalOrganization _internationalOrganizationRepository;

    private IInternationalOrganization InternationalOrganizationRepository
    {
      get
      {
        if (_internationalOrganizationRepository == null)
        {
          _internationalOrganizationRepository = new InternationalOrganizationRepository();
        }

        return _internationalOrganizationRepository;
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

    [Route("UpdateInternationalOrganization")]
    [HttpPost]
    public async Task<IHttpActionResult> UpdateInternationalOrganization(InternationalOrganizationModel internationalOrganization)
    {
      try
      {
        bool isSave = false;
        if (!ModelState.IsValid)
        {
          return Ok(FormatResult(false, ModelState));
        }

        isSave = await InternationalOrganizationRepository.UpdateInternationalOrganization(internationalOrganization);

        return Ok(FormatResult(isSave, (isSave ? (string.IsNullOrEmpty(internationalOrganization.InternaltionalOrganizationMasterHashId) ? Resource.OrganizationAddedSuccess : Resource.OrganizationUpdatedSuccess) : Resources.Resource.ErrorWhileSaving)));
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, internationalOrganization);
        if (string.IsNullOrEmpty(internationalOrganization.InternaltionalOrganizationMasterHashId))
          AuditLogs.Write<InternationalOrganizationModel, string>(AuditConstants.InternalOrganization, AuditType.InsertFailure, internationalOrganization, (string)null, Helper.GetInnerException(ex));
        else
          AuditLogs.Write<InternationalOrganizationModel, string>(AuditConstants.InternalOrganization, AuditType.UpdateFailure, internationalOrganization, (string)null, Helper.GetInnerException(ex));
        return BadRequest(ex.Message);
      }
    }

    [Route("DeleteInternationalOrganization")]
    [HttpGet]
    public async Task<IHttpActionResult> DeleteInternationalOrganization(string internationalOrgMasterHashId)
    {
      try
      {
        bool isSave = false;
        isSave = await InternationalOrganizationRepository.DeleteInternationalOrganization(internationalOrgMasterHashId);

        return Ok(FormatResult(isSave, (isSave ? Resources.Resource.OrganizationDeletedSuccessfully : Resources.Resource.ErrorWhileDeleting)));
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, internationalOrgMasterHashId);
        AuditLogs.Write(AuditConstants.InternalOrganization, AuditType.DeleteFailure, Helper.GetInnerException(ex));
        return BadRequest(ex.Message);
      }
    }

    [Route("GetAllInternationalOrganizationList")]
    [HttpGet]
    public async Task<IHttpActionResult> GetAllInternationalOrganizationList(string parametersJson)
    {
      var parameters = JsonConvert.DeserializeObject<GridParameters>(parametersJson);
      ApiOutput apiOutput = await InternationalOrganizationRepository.GetAllInternationalOrganizationList(parameters);
      var result = new { Data = apiOutput.Data, Total = apiOutput.TotalRecords };
      return Json(result);
    }

    private async Task<List<InternationalOrganizationImportModel>> ValidateInternationalOrganizationModel(List<IList<string>> validateLines)
    {
      List<InternationalOrganizationImportModel> listOfInternationalOrg = new List<InternationalOrganizationImportModel>();

      if (validateLines == null || !validateLines.Any())
        return listOfInternationalOrg;

      ////string[] checkVal = { Resources.Enums.Status.Yes.ToString() ,
      ////                  Resources.Enums.Status.No.ToString()
      ////                  };

      ////string abc = Resources.Enums.Status.Yes.ToString();/*  Enums.Status.Yes.ToString();*/


      listOfInternationalOrg = validateLines.Select(line => new InternationalOrganizationImportModel()
      {
        OrganizationName = !string.IsNullOrEmpty(line[0].Trim()) ? line[0].Trim() : null,
        DesignationName = !string.IsNullOrEmpty(line[1].Trim()) ? line[1].Trim() : null,
        LeaderFirstName = !string.IsNullOrEmpty(line[2].Trim()) ? line[2].Trim() : null,
        LeaderLastName = !string.IsNullOrEmpty(line[3].Trim()) ? line[3].Trim() : null,
        LeaderName = (line[2].Trim() + " " ?? string.Empty) + (line[3].Trim() ?? string.Empty),
        EntityName = !string.IsNullOrEmpty(line[4].Trim()) ? line[4].Trim() : null,
        DesignationMasterId = PolicyMakerRepository.GetDesignationIdByName((!string.IsNullOrEmpty(line[1].Trim()) ? line[1].Trim() : string.Empty)),
        IsMultiLateral = string.IsNullOrEmpty(line[5].Trim()) ? Helper.saveChangesNotSuccessful : line[5].Trim().ToString().ToLower() == "yes" ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful,
        IsMultiLateralStr = !string.IsNullOrEmpty(line[5].Trim()) ? line[5].Trim().ToString().ToLower() : null
      }).ToList();

      int parts = listOfInternationalOrg.Count / 50;
      var splitedListOfInternationalOrg = CommonRepository.Split(listOfInternationalOrg, parts > 1 ? parts : 1);
      var handleValidation = new List<Task>();

      foreach (var chunkOfInternationalOrg in splitedListOfInternationalOrg)
      {
        handleValidation.Add(ValidateHeadStateModelState(chunkOfInternationalOrg, listOfInternationalOrg));
      }

      await Task.WhenAll(handleValidation);
      return listOfInternationalOrg;
    }

    private async Task ValidateHeadStateModelState(List<InternationalOrganizationImportModel> chunkOfInternationalOrg, List<InternationalOrganizationImportModel> listOfInternationalOrg)
    {
      await Task.Yield();
      foreach (var model in chunkOfInternationalOrg)
      {
        this.Validate<InternationalOrganizationImportModel>(model);
        HandleModelStateForImportInternationalOrg(ModelState, model, listOfInternationalOrg);
        List<KeyValuePair<string, string>> errorModel = CommonUtilities.HandleModelStateForImport(ModelState);
        model.ErrorModel = errorModel;
        ModelState.Clear();
      }
    }

    private void HandleModelStateForImportInternationalOrg(System.Web.Http.ModelBinding.ModelStateDictionary ModelState, InternationalOrganizationImportModel model, List<InternationalOrganizationImportModel> listOfInternationalOrg)
    {
      if (!string.IsNullOrEmpty(model.OrganizationName))
      {
        var listOfDuplicate = listOfInternationalOrg.Count(x => x.OrganizationName == model.OrganizationName && x.DesignationName == model.DesignationName && x.LeaderFirstName == model.LeaderFirstName && x.LeaderLastName == model.LeaderLastName && x.EntityName == model.EntityName);
        if (listOfDuplicate > 1)
        {
          ModelState.AddModelError("OrganizationName", Resource.ValidateInternationalOrgDuplicate);
        }
      }
      if (!string.IsNullOrEmpty(model.IsMultiLateralStr) && !model.IsMultiLateralStr.Contains(Resources.Enums.Status.Yes.ToString().ToLower()) && !model.IsMultiLateralStr.Contains(Resources.Enums.Status.No.ToString().ToLower()))
      {
        ModelState.AddModelError("ValidationYesNo", Resource.ValidationYesNo);
      }
    }

    [Route("ValidateInternationalOrgCSVRecord")]
    [HttpPost]
    public async Task<IHttpActionResult> ValidateInternationalOrgCSVRecord()
    {
      ApiOutput apiOutput = new ApiOutput();
      var httpRequest = HttpContext.Current.Request;

      apiOutput.Data = new List<string>();
      apiOutput.TotalRecords = 0;
      bool isValid = true;
      string validationMsg = string.Empty;

      if (httpRequest.Files.Count > 0)
      {
        List<IList<string>> validateLines;

        validationMsg = ImportCSV(httpRequest, 6, out validateLines);

        if (string.IsNullOrEmpty(validationMsg))
        {
          var listOfInternationalOrganization = await ValidateInternationalOrganizationModel(validateLines);
          apiOutput.Data = listOfInternationalOrganization;
          apiOutput.TotalRecords = listOfInternationalOrganization.Count;
        }
        else
        {
          isValid = false;
        }
      }
      else
      {
        isValid = false;
        validationMsg = Resource.ValidateToSelectFile;
      }

      apiOutput.ErrorMessage = isValid ? string.Empty : validationMsg;
      return Ok(apiOutput);
    }

    [Route("ImportInternationalOrgCSVRecord")]
    [HttpPost]
    public async Task<IHttpActionResult> ImportInternationalOrgCSVRecord(List<InternationalOrganizationImportModel> internationalOrganizationImportModel)
    {
      ApiOutput apiOutput = new ApiOutput();

      Validate(internationalOrganizationImportModel);

      if (!ModelState.IsValid)
      {
        return Ok(FormatResult(false, ModelState));
      }

      bool isSave = false;
      isSave = await InternationalOrganizationRepository.ImportInternationOrganizationRecord(internationalOrganizationImportModel);
      apiOutput.Data = isSave;
      apiOutput.TotalRecords = 0;
      apiOutput.ErrorMessage = isSave ? Resource.SuccessfullImprtMessage : Resource.ValidateImportError;
      return Ok(apiOutput);
    }

    /// <summary>
    /// Get International Organization data By Hash Id
    /// </summary>
    /// <param name="internationalOrgHashId"></param>
    /// <returns></returns>
    [Route("GetInternationalOrgByHashId")]
    [HttpGet]
    public async Task<IHttpActionResult> GetInternationalOrgByHashId(string internationalOrgHashId)
    {
      try
      {
        InternationalOrganizationModel internationalOrganizationModel = await InternationalOrganizationRepository.GetInternationalOrgByHashId(internationalOrgHashId);
        return Ok(internationalOrganizationModel);
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, internationalOrgHashId);
        return BadRequest(ex.Message);
      }
    }

  }
}