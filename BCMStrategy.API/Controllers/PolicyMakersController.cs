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
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace BCMStrategy.API.Controllers
{
  [Authentication]
  [RoutePrefix("api/PolicyMakers")]
  public class PolicyMakersController : BaseApiController
  {
    private static readonly EventLogger<PolicyMakersController> _log = new EventLogger<PolicyMakersController>();

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

    [Route("UpdatePolicyMaker")]
    [HttpPost]
    public async Task<IHttpActionResult> UpdatePolicyMaker(PolicyMakerModel policyMakerModel)
    {
      try
      {
        bool isSave = false;

        if (!ModelState.IsValid)
        {
          return Ok(FormatResult(false, ModelState));
        }

        isSave = await PolicyMakerRepository.UpdatePolicyMaker(policyMakerModel);

        ////return Ok(FormatResult(isSave, (isSave ? Resources.Resource.SuccessfulMessageForPolicyMakers : Resources.Resource.ErrorWhileSaving)));
        return Ok(FormatResult(isSave, (isSave ? (string.IsNullOrEmpty(policyMakerModel.PolicyMakerHashId) ? Resource.PolicyMakerAddedSuccess : Resource.PolicyMakerUpdatedSuccess) : Resources.Resource.ErrorWhileSaving)));
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, policyMakerModel);
        if (string.IsNullOrEmpty(policyMakerModel.PolicyMakerHashId))
          AuditLogs.Write<PolicyMakerModel, string>(AuditConstants.PolicyMaker, AuditType.InsertFailure, policyMakerModel, (string)null, Helper.GetInnerException(ex));
        else
          AuditLogs.Write<PolicyMakerModel, string>(AuditConstants.PolicyMaker, AuditType.UpdateFailure, policyMakerModel, (string)null, Helper.GetInnerException(ex));
        return BadRequest(ex.Message);
      }
    }

    [Route("DeletePolicyMaker")]
    [HttpGet]
    public async Task<IHttpActionResult> DeletePolicyMaker(string policyMakerHashId)
    {
      try
      {
        bool isSave = false;

        isSave = await PolicyMakerRepository.DeletePolicyMaker(policyMakerHashId);

        return Ok(FormatResult(isSave, (isSave ? Resources.Resource.PolicyMakersDeletedSuccessfully : Resources.Resource.ErrorWhileDeleting)));
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, policyMakerHashId);
        AuditLogs.Write(AuditConstants.PolicyMaker, AuditType.DeleteFailure, Helper.GetInnerException(ex));
        return BadRequest(ex.Message);
      }
    }

    [Route("GetAllPolicyMakerList")]
    [HttpGet]
    public async Task<IHttpActionResult> GetAllPolicyMakerList(string parametersJson)
    {
      var parameters = JsonConvert.DeserializeObject<GridParameters>(parametersJson);
      ApiOutput apiOutput = await PolicyMakerRepository.GetAllPolicyMakerList(parameters);

      var result = new { Data = apiOutput.Data, Total = apiOutput.TotalRecords };
      return Json(result);
    }

    /// <summary>
    /// Get dropdown Country list
    /// </summary>
    /// <returns>Country List</returns>
    [Route("GetDropdownDesignationList")]
    [HttpGet]
    public async Task<IHttpActionResult> GetDropdownDesignationList()
    {
      try
      {
        ApiOutput apiOutput = await PolicyMakerRepository.GetDropdownDesignationList();
        return Ok(apiOutput);
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex);
        return BadRequest(ex.Message);
      }
    }

    [Route("ImportPolicyMakersCSVRecord")]
    [HttpPost]
    public async Task<IHttpActionResult> ImportPolicyMakersCSVRecord(List<PolicyMakersCsvImportModel> polcyMakersModel)
    {
      ApiOutput apiOutput = new ApiOutput();

      Validate(polcyMakersModel);
      if (!ModelState.IsValid)
      {
        return Ok(FormatResult(false, ModelState));
      }

      bool isSave = false;
      isSave = await PolicyMakerRepository.ImportPolicyMakerRecords(polcyMakersModel);
      apiOutput.Data = isSave;
      apiOutput.TotalRecords = 0;
      apiOutput.ErrorMessage = isSave ? Resource.SuccessfullImprtMessage : Resource.ValidateImportError;
      return Ok(apiOutput);
    }

    [Route("ValidatePolicyMakersCSVRecord")]
    [HttpPost]
    public async Task<IHttpActionResult> ValidatePolicyMakersCSVRecord()
    {
      ApiOutput apiOutput = new ApiOutput();
      var httpRequest = HttpContext.Current.Request;
      string instTypeId = HttpContext.Current.Request.Params["InstitutionTypeHashId"];
      apiOutput.Data = new List<string>();
      apiOutput.TotalRecords = 0;
      bool isValid = true;
      string validationMsg = string.Empty;
      ////HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

      if (httpRequest.Files.Count > 0)
      {
        List<IList<string>> validateLines;

        validationMsg = ImportCSV(httpRequest, 4, out validateLines);

        if (!string.IsNullOrEmpty(validationMsg))
        {
          isValid = false;
        }
        else if (string.IsNullOrEmpty(instTypeId))
        {
          isValid = false;
          validationMsg = "InstType:" + string.Format(Resource.ValidateRequireDropDown, Resource.LblInstitutionTypes);
        }
        else
        {
          var listOfPolicyMakers = await ValidatePolicyMakersModel(validateLines, instTypeId.ToDecrypt());
          apiOutput.Data = listOfPolicyMakers;
          apiOutput.TotalRecords = listOfPolicyMakers.Count;
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

    private async Task<List<PolicyMakersCsvImportModel>> ValidatePolicyMakersModel(List<IList<string>> validateLines, string instTypeId)
    {
      List<PolicyMakersCsvImportModel> listOfPolicyMakers = new List<PolicyMakersCsvImportModel>();

      if (validateLines == null || !validateLines.Any())
        return listOfPolicyMakers;

      listOfPolicyMakers = validateLines.Select(line => new PolicyMakersCsvImportModel()
      {
        CountryName = !string.IsNullOrEmpty(line[0].Trim()) ? line[0].Trim() : null,
        DesignationName = !string.IsNullOrEmpty(line[1].Trim()) ? line[1].Trim() : null,
        PolicyFirstName = !string.IsNullOrEmpty(line[2].Trim()) ? line[2].Trim() : null,
        PolicyLastName = !string.IsNullOrEmpty(line[3].Trim()) ? line[3].Trim() : null,
        //PolicyName = (line[2].Trim() + " " ?? string.Empty) + (line[3].Trim() ?? string.Empty),
        DesignationMasterId = PolicyMakerRepository.GetDesignationIdByName((!string.IsNullOrEmpty(line[1].Trim()) ? line[1].Trim() : string.Empty)),
        InstitutionTypeMasterId = !string.IsNullOrEmpty(instTypeId) ? Convert.ToInt32(instTypeId) : 0,
        CountryMasterId = CountryRepository.GetCountryIdByName((!string.IsNullOrEmpty(line[0].Trim()) ? line[0].Trim() : string.Empty))
      }).ToList();

      int parts = listOfPolicyMakers.Count / 50;
      var splitedListOfPolicyMakers = CommonRepository.Split(listOfPolicyMakers, parts > 1 ? parts : 1);
      var handleValidation = new List<Task>();

      foreach (var chunkOfPolciyMakers in splitedListOfPolicyMakers)
      {
        handleValidation.Add(ValidatePolicyMakersModelState(chunkOfPolciyMakers, listOfPolicyMakers));
      }

      await Task.WhenAll(handleValidation);
      return listOfPolicyMakers;
    }

    private async Task ValidatePolicyMakersModelState(List<PolicyMakersCsvImportModel> chunkOfPolciyMakers, List<PolicyMakersCsvImportModel> listOfPolicyMakers)
    {
      await Task.Yield();
      foreach (var model in chunkOfPolciyMakers)
      {
        this.Validate<PolicyMakersCsvImportModel>(model);
        HandleModelStateForImportPolicyMakers(ModelState, model, listOfPolicyMakers);
        List<KeyValuePair<string, string>> errorModel = CommonUtilities.HandleModelStateForImport(ModelState);
        model.ErrorModel = errorModel;
        ModelState.Clear();
      }
    }

    private void HandleModelStateForImportPolicyMakers(System.Web.Http.ModelBinding.ModelStateDictionary ModelState, PolicyMakersCsvImportModel model, List<PolicyMakersCsvImportModel> listOfPolicyMakers)
    {
      var listOfDuplicate = listOfPolicyMakers.Count(x => x.CountryMasterHashId == model.CountryMasterHashId && x.DesignationMasterHashId == model.DesignationMasterHashId && x.PolicyLastName == model.PolicyLastName);

      if (listOfDuplicate > 1)
      {
        ModelState.AddModelError("PolicyName", Resource.ValidatePolicyNameDuplicate);
      }
    }

    /// <summary>
    /// Get Policy Maker By Hash Id
    /// </summary>
    /// <param name="policyMakerHashId"></param>
    /// <returns></returns>
    [Route("GetPolicyMakerByHashId")]
    [HttpGet]
    public async Task<IHttpActionResult> GetPolicyMakerByHashId(string policyMakerHashId)
    {
      try
      {
        PolicyMakerModel policyMakerModel = await PolicyMakerRepository.GetPolicyMakerByHashId(policyMakerHashId);
        return Ok(policyMakerModel);
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, policyMakerHashId);
        return BadRequest(ex.Message);
      }
    }
  }
}
