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
using System.Web.Http;

namespace BCMStrategy.API.Controllers
{
  [Authentication]
  [RoutePrefix("api/Lexicon")]
  public class LexiconController : BaseApiController
  {
    private static readonly EventLogger<LexiconController> _log = new EventLogger<LexiconController>();

    private ILexicon _lexiconRepository;

    private ILexicon LexiconRepository
    {
      get
      {
        if (_lexiconRepository == null)
        {
          _lexiconRepository = new LexiconRepository();
        }

        return _lexiconRepository;
      }
    }

    ////private ICommonRepository _commonRepository;

    ////private ICommonRepository CommonRepository
    ////{
    ////  get
    ////  {
    ////    if (_commonRepository == null)
    ////    {
    ////      _commonRepository = new CommonRepository();
    ////    }

    ////    return _commonRepository;
    ////  }
    ////}

    [Route("UpdateLexicon")]
    [HttpPost]
    public async Task<IHttpActionResult> UpdateLexicon(LexiconModel lexiconModel)
    {
      try
      {
        bool isSave = false;
        if (!lexiconModel.IsNested)
        {
          ModelState.Remove("LexiconModel.CombinationValue");
        }

        if (!ModelState.IsValid)
        {
          return Ok(FormatResult(false, ModelState));
        }

        isSave = await LexiconRepository.UpdateLexicon(lexiconModel);

        return Ok(FormatResult(isSave, (isSave ? (string.IsNullOrEmpty(lexiconModel.LexiconeIssueMasterHashId) ? Resources.Resource.LexiconAddedSuccess : Resources.Resource.LexiconUpdatedSuccess) : Resources.Resource.ErrorWhileSaving)));
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, lexiconModel);
        if (string.IsNullOrEmpty(lexiconModel.LexiconeIssueMasterHashId))
          AuditLogs.Write<LexiconModel, string>(AuditConstants.LexiconTerm, AuditType.InsertFailure, lexiconModel, (string)null, Helper.GetInnerException(ex));
        else
          AuditLogs.Write<LexiconModel, string>(AuditConstants.LexiconTerm, AuditType.UpdateFailure, lexiconModel, (string)null, Helper.GetInnerException(ex));
        return BadRequest(ex.Message);
      }
    }

    [Route("GetAllLexiconList")]
    [HttpGet]
    public async Task<IHttpActionResult> GetAllLexiconList(string parametersJson)
    {
      var parameters = JsonConvert.DeserializeObject<GridParameters>(parametersJson);
      ApiOutput apiOutput = await LexiconRepository.GetAllLexiconList(parameters);
      var result = new { Data = apiOutput.Data, Total = apiOutput.TotalRecords };
      return Json(result);
    }

    [Route("DeleteLexicon")]
    [HttpGet]
    public async Task<IHttpActionResult> DeleteLexicon(string lexiconIssueMasterHashId)
    {
      try
      {
        bool isSave = false;
        
        isSave = await LexiconRepository.DeleteLexicon(lexiconIssueMasterHashId);

        return Ok(FormatResult(isSave, (isSave ? Resources.Resource.LexiconDeletedSuccessfully : Resources.Resource.ErrorWhileDeleting)));
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, lexiconIssueMasterHashId);
        AuditLogs.Write(AuditConstants.LexiconTerm, AuditType.DeleteFailure, Helper.GetInnerException(ex));
        return BadRequest(ex.Message);
      }
    }

    ////private async Task<List<LexiconCsvImportModel>> ValidateLexiconModel(List<IList<string>> validateLines, string lexiconTypeId)
    ////{
    ////  List<LexiconCsvImportModel> listOfLexicon = new List<LexiconCsvImportModel>();

    ////  if (validateLines == null || !validateLines.Any())
    ////    return listOfLexicon;

    ////  listOfLexicon = validateLines.Select(line => new LexiconCsvImportModel()
    ////  {
    ////    LexiconIssue = !string.IsNullOrEmpty(line[0].Trim()) ? line[0].Trim() : null,
    ////    IsNested = !string.IsNullOrEmpty(line[1].Trim()) ? line[1].Trim().ToString().ToLower() == "yes" ? true : line[1].Trim().ToString().ToLower() == "no" : false,
    ////    CombinationValue = !string.IsNullOrEmpty(line[2].Trim()) ? line[2].Trim() : null,
    ////    LexiconLinkers = !string.IsNullOrEmpty(line[3].Trim()) ? line[3].Trim() : null,
    ////    LexiconTypeMasterId = !string.IsNullOrEmpty(lexiconTypeId) ? Convert.ToInt32(lexiconTypeId) : 0,
    ////  }).ToList();

    ////  int parts = listOfLexicon.Count / 50;
    ////  var splitedListOfLexicon = CommonRepository.Split(listOfLexicon, parts > 1 ? parts : 1);
    ////  var handleValidation = new List<Task>();

    ////  foreach (var chunkOfLexicon in splitedListOfLexicon)
    ////  {
    ////    handleValidation.Add(ValidateLexiconModelState(chunkOfLexicon, listOfLexicon));
    ////  }

    ////  await Task.WhenAll(handleValidation);
    ////  return listOfLexicon;
    ////}

    ////private async Task ValidateLexiconModelState(List<LexiconCsvImportModel> chunkOfLexicon, List<LexiconCsvImportModel> listOfLexicon)
    ////{
    ////  await Task.Yield();
    ////  foreach (var model in chunkOfLexicon)
    ////  {

    ////    this.Validate<LexiconCsvImportModel>(model);

    ////    HandleModelStateForImportLexicon(ModelState, model, listOfLexicon);
    ////    List<KeyValuePair<string, string>> errorModel = CommonUtilities.HandleModelStateForImport(ModelState);

    ////    model.ErrorModel = errorModel;

    ////    ModelState.Clear();
    ////  }
    ////}

    ////private void HandleModelStateForImportLexicon(System.Web.Http.ModelBinding.ModelStateDictionary ModelState, LexiconCsvImportModel model, List<LexiconCsvImportModel> listOfLexicon)
    ////{
    ////  ////var listOfDuplicate = listOfLexicon.Where(x => x.LexiconIssue == model.LexiconIssue && x.CombinationValue == model.CombinationValue).Count();
    ////  var listOfDuplicate = listOfLexicon.Count(x => x.LexiconIssue == model.LexiconIssue && x.CombinationValue == model.CombinationValue);
    ////  if (listOfDuplicate > 1)
    ////  {
    ////    ModelState.AddModelError("LexiconDuplicate", Resource.ValidateLexiconDuplicate);
    ////  }
    ////  if (!model.IsNested)
    ////  {
    ////    ModelState.Remove("CombinationValue");
    ////    ModelState.Remove("LexiconCSVImportModel.CombinationValue");
    ////    if (!string.IsNullOrEmpty(model.CombinationValue))
    ////    {
    ////      ModelState.AddModelError("LexiconCombination", Resource.ValidateCombinationVal);
    ////    }

    ////  }
    ////}

    /// <summary>
    /// Get Lexicon By Hash Id
    /// </summary>
    /// <param name="policyMakerHashId"></param>
    /// <returns></returns>
    [Route("GetLexiconByHashId")]
    [HttpGet]
    public async Task<IHttpActionResult> GetLexiconByHashId(string lexiconHashId)
    {
      try
      {
        LexiconModel lexiconModel = await LexiconRepository.GetLexiconByHashId(lexiconHashId);
        return Ok(lexiconModel);
      }
      catch (Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, lexiconHashId);
        return BadRequest(ex.Message);
      }
    }
  }
}

