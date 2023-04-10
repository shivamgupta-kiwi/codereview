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
using BCMStrategy.Common.Unity;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Data.Repository.Concrete;
using BCMStrategy.Logger;
using BCMStrategy.Resources;
using Newtonsoft.Json;
using BCMStrategy.API.AuditLog;
using BCMStrategy.Common.AuditLog;
using BCMStrategy.Data.Abstract;

namespace BCMStrategy.API.Controllers
{
  [Authentication]
  [RoutePrefix("api/InstitutionTypes")]
  public class InstitutionTypesApiController : BaseApiController
  {
    private static readonly EventLogger<InstitutionTypesApiController> _log = new EventLogger<InstitutionTypesApiController>();

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

    private IInstitutionTypes _institutionTypeRepository;

    private IInstitutionTypes InstitutionTypesRepository
    {
      get
      {
        if (_institutionTypeRepository == null)
        {
          _institutionTypeRepository = UnityHelper.Resolve<IInstitutionTypes>();
        }
        return _institutionTypeRepository;
      }
    }

    [Route("UpdateInstitutionTypes")]
    [HttpPost]
    public async Task<IHttpActionResult> UpdateInstitutionTypes(InstitutionTypesModel institutionTypesModel)
    {
      try
      {
        bool isSave = false;
        if (!ModelState.IsValid)
        {
          return Ok(FormatResult(false, ModelState));
        }

        isSave = await InstitutionTypesRepository.UpdateInstitutionTypes(institutionTypesModel);

        return Ok(FormatResult(isSave, (isSave ? (string.IsNullOrEmpty(institutionTypesModel.InstitutionTypesHashId) ? Resources.Resource.InstitutionTypeAddedSuccess : Resources.Resource.InstitutionTypeUpdatedSuccess) : Resources.Resource.ErrorWhileSaving)));
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, institutionTypesModel);
        if (string.IsNullOrEmpty(institutionTypesModel.InstitutionTypesHashId))
          AuditLogs.Write<InstitutionTypesModel, string>(AuditConstants.InstitutionType, AuditType.InsertFailure, institutionTypesModel, (string)null, Helper.GetInnerException(ex));
        else
          AuditLogs.Write<InstitutionTypesModel, string>(AuditConstants.InstitutionType, AuditType.UpdateFailure, institutionTypesModel, (string)null, Helper.GetInnerException(ex));
        return BadRequest(ex.Message);
      }
    }

    [Route("DeleteInstitutionTypes")]
    [HttpGet]
    public async Task<IHttpActionResult> DeleteInstitutionTypes(string institutionTypesHashId)
    {
      try
      {
        ApiOutput apiOutput = await InstitutionTypesRepository.DeleteInstitutionTypes(institutionTypesHashId);
        return Ok(apiOutput);
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, institutionTypesHashId);
        AuditLogs.Write(AuditConstants.InstitutionType, AuditType.DeleteFailure, Helper.GetInnerException(ex));
        return BadRequest(ex.Message);
      }
    }

    [Route("GetAllInstitutionTypesList")]
    [HttpGet]
    public async Task<IHttpActionResult> GetAllInstitutionTypesList(string parametersJson)
    {
      var parameters = JsonConvert.DeserializeObject<GridParameters>(parametersJson);
      ApiOutput apiOutput = await InstitutionTypesRepository.GetAllInstitutionTypesList(parameters);
      var result = new { Data = apiOutput.Data, Total = apiOutput.TotalRecords };
      return Json(result);
    }

    /// <summary>
    /// Retrieve Drop down list of Institution Types
    /// </summary>
    /// <returns>Return Institution type list</returns>
    [Route("GetDropdownInstitutionTypeList")]
    [HttpGet]
    public async Task<IHttpActionResult> GetDropdownInstitutionTypeList()
    {
      try
      {
        ApiOutput apiOutput = await InstitutionTypesRepository.GetDDLInstitutionTpeList();
        return Ok(apiOutput);
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex);
        return BadRequest(ex.Message);
      }
    }

    [Route("ImportCSVRecord")]
    [HttpPost]
    public async Task<IHttpActionResult> ImportCSVRecord(List<InstitutionTypesImportModel> institutionTypesBatchModel)
    {
      ApiOutput apiOutput = new ApiOutput();

      Validate(institutionTypesBatchModel);

      if (!ModelState.IsValid)
      {
        return Ok(FormatResult(false, ModelState));
      }

      bool isSave = false;
      isSave = await InstitutionTypesRepository.ImportInstitutionTypeRecord(institutionTypesBatchModel);
      apiOutput.Data = isSave;
      apiOutput.TotalRecords = 0;
      apiOutput.ErrorMessage = isSave ? Resources.Resource.SuccessfullImprtMessage : Resources.Resource.ValidateImportError;
      return Ok(apiOutput);
    }

    [Route("ValidateCSVRecord")]
    [HttpPost]
    public async Task<IHttpActionResult> ValidateCSVRecord()
    {
      ApiOutput apiOutput = new ApiOutput();
      var httpRequest = HttpContext.Current.Request;

      apiOutput.Data = new List<string>();
      apiOutput.TotalRecords = 0;
      bool isValid = true;
      string validationMsg = string.Empty;
      ////HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);
      
      if (httpRequest.Files.Count > 0)
      {
        var postedFile = httpRequest.Files[0];

        string MaximumFileUploadCSV = await CommonRepository.GetCSVFileSize();

        if (postedFile != null && postedFile.ContentLength > 0)
        {
          int MaxContentLength = 1024 * 1024 * Convert.ToInt32(MaximumFileUploadCSV);
          IList<string> AllowedFileExtensions = new List<string>();
          AllowedFileExtensions.Add(".csv");

          var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
          var extension = ext.ToLower();
          if (!AllowedFileExtensions.Contains(extension))
          {
            isValid = false;
            validationMsg = Resources.Resource.ValidateCSVFileType;
          }
          else if (postedFile.ContentLength > MaxContentLength)
          {
            isValid = false;
            validationMsg = string.Format(Resources.Resource.ValidateFileSize, MaximumFileUploadCSV);
          }
          else
          {
            using (var reader = new StreamReader(postedFile.InputStream))
            {
              var data = CsvParser.ParseHeadAndTail(reader, ',', '"');

              var header = data.Item1;
              var lines = data.Item2;
              var validateLines = lines.ToList();

              if (header == null || header.Count() != 1 || validateLines.Count == 0 || validateLines.Any(x => x.Count() != 1))
              {
                isValid = false;
                validationMsg = Resources.Resource.ValidateImportCSV;
              }
              else
              {
                var listOfInstitutionTypes = await ValidateInstitutionTypesModel(validateLines);
                apiOutput.Data = listOfInstitutionTypes;
                apiOutput.TotalRecords = listOfInstitutionTypes.Count;
              }
            }
          }
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

    private async Task<List<InstitutionTypesImportModel>> ValidateInstitutionTypesModel(List<IList<string>> validateLines)
    {
      List<InstitutionTypesImportModel> listOfInstitutionTypes = new List<InstitutionTypesImportModel>();

      if (validateLines == null || !validateLines.Any())
        return listOfInstitutionTypes;

      listOfInstitutionTypes = validateLines.Select(line => new InstitutionTypesImportModel()
      {
        InstitutionTypesName = !string.IsNullOrEmpty(line[0].Trim()) ? line[0].Trim() : null
      }).ToList();

      int parts = listOfInstitutionTypes.Count / 50;
      var splitedListOfInstTypes = Split(listOfInstitutionTypes, parts > 1 ? parts : 1);
      var handleValidation = new List<Task>();

      foreach (var chunkOfInstTypes in splitedListOfInstTypes)
      {
        handleValidation.Add(ValidateInstitutionTypesModelState(chunkOfInstTypes, listOfInstitutionTypes));
      }

      await Task.WhenAll(handleValidation);
      return listOfInstitutionTypes;
    }

    private async Task ValidateInstitutionTypesModelState(List<InstitutionTypesImportModel> chunkOfInstitutionTypes, List<InstitutionTypesImportModel> listOfInstitutionTypes)
    {
      await Task.Yield();
      foreach (var model in chunkOfInstitutionTypes)
      {
        this.Validate<InstitutionTypesImportModel>(model);
        HandleModelStateForImportInstitutionTypes(ModelState, model, listOfInstitutionTypes);
        List<KeyValuePair<string, string>> errorModel = CommonUtilities.HandleModelStateForImport(ModelState);
        model.ErrorModel = errorModel;
        ModelState.Clear();
      }
    }

    private void HandleModelStateForImportInstitutionTypes(System.Web.Http.ModelBinding.ModelStateDictionary ModelState, InstitutionTypesImportModel model, List<InstitutionTypesImportModel> listOfInstitutionTypes)
    {
      var listOfDuplicate = listOfInstitutionTypes.Count(x => x.InstitutionTypesName == model.InstitutionTypesName);
      if (listOfDuplicate > 1)
      {
        ModelState.AddModelError("InstitutionTypesName", Resource.ValidateInstitutionTypesDuplicate);
      }
    }

    public static List<List<T>> Split<T>(List<T> source, int parts)
    {
      return source
          .Select((x, i) => new { Index = i, Value = x })
          .GroupBy(x => x.Index / parts)
          .Select(x => x.Select(v => v.Value).ToList())
          .ToList();
    }

    /// <summary>
    /// Get Institution Type By Hash Id for edit
    /// </summary>
    /// <param name="institutionTypeHashId"></param>
    /// <returns></returns>
    [Route("GetInstitutionTypeByHashId")]
    [HttpGet]
    public async Task<IHttpActionResult> GetInstitutionTypeByHashId(string institutionTypeHashId)
    {
      try
      {
        InstitutionTypesModel institutionTypesModel = await InstitutionTypesRepository.GetInstitutionTypeByHashId(institutionTypeHashId);
        return Ok(institutionTypesModel);
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, institutionTypeHashId);
        return BadRequest(ex.Message);
      }
    }
  }
}