using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCMStrategy.Common.Kendo;
using BCMStrategy.DAL.Context;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Logger;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Resources;
using BCMStrategy.Common.AuditLog;
using BCMStrategy.Common.Unity;

namespace BCMStrategy.Data.Repository.Concrete
{
  public class LexiconRepository : ILexicon
  {
    /// <summary>
    /// The _audit repository
    /// </summary>
    private IAuditLog _auditRepository;

    /// <summary>
    /// Gets the audit repository.
    /// </summary>
    /// <value>
    /// The audit repository.
    /// </value>
    private IAuditLog AuditRepository
    {
      get
      {
        if (this._auditRepository == null)
        {
          this._auditRepository = UnityHelper.Resolve<IAuditLog>();
        }

        return this._auditRepository;
      }
    }


    /// <summary>
    /// Add and update of leicon term
    /// </summary>
    /// <param name="lexiconModel">lexicon Model with lexicon values</param>
    /// <returns>Is Saved or not</returns>
    public async Task<bool> UpdateLexicon(LexiconModel lexiconModel)
    {
      bool isSave = false;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        DateTime currentTimeStamp = Helper.GetCurrentDateTime();
        if (string.IsNullOrEmpty(lexiconModel.LexiconeIssueMasterHashId))
        {
          lexiconissues objLexicon = new lexiconissues()
          {
            LexiconTypeId = lexiconModel.LexiconeTypeMasterHashId.ToDecrypt().ToInt32(),
            LexiconIssue = lexiconModel.LexiconIssue,
            CombinationValue = lexiconModel.CombinationValue,
            IsNested = lexiconModel.IsNested,
            Created = currentTimeStamp,
            CreatedBy = UserAccessHelper.CurrentUserIdentity.ToString(),
            lexiconissuelinker = ConvertToLinkersList(lexiconModel.LexiconLinkers)
          };
          db.lexiconissues.Add(objLexicon);
          isSave = db.SaveChanges() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
          LexiconTermViewModel model = GetLexiconTermAuditModel(objLexicon);
          Task.Run(() => AuditRepository.WriteAudit<LexiconTermViewModel>(AuditConstants.LexiconTerm, AuditType.Insert, null, model, AuditConstants.InsertSuccessMsg));
        }
        else
        {
          int decryptLexiconId = lexiconModel.LexiconeIssueMasterHashId.ToDecrypt().ToInt32();

          var objLexicon = await db.lexiconissues.Where(x => x.Id == decryptLexiconId && !x.IsDeleted).FirstOrDefaultAsync();
          LexiconTermViewModel beforeModel = GetLexiconTermAuditModel(objLexicon);
          if (objLexicon != null)
          {
            objLexicon.LexiconTypeId = lexiconModel.LexiconeTypeMasterHashId.ToDecrypt().ToInt32();
            objLexicon.LexiconIssue = lexiconModel.LexiconIssue;
            objLexicon.IsNested = lexiconModel.IsNested;
            objLexicon.CombinationValue = lexiconModel.CombinationValue;
            objLexicon.Modified = currentTimeStamp;
            objLexicon.ModifiedBy = UserAccessHelper.CurrentUserIdentity.ToString();
          }
          string[] lexiconLinkersArray = lexiconModel.LexiconLinkers.Split(',');

          var deleteLexiconLink = objLexicon.lexiconissuelinker.Where(x => !lexiconLinkersArray.Any(y => x.Linkers == y));
          ////var deleteLexiconLink = objLexicon.lexiconissuelinker.Where(x => lexiconLinkersArray.Any(y => x.Linkers == y) == false);
          foreach (var lnk in deleteLexiconLink)
          {
            lnk.IsDeleted = true;
          }

          var newLinkerList = lexiconLinkersArray.Where(x => !objLexicon.lexiconissuelinker.Any(y => x == y.Linkers)).ToDBModel(objLexicon.Id);
          ////var newLinkerList = lexiconLinkersArray.Where(x => objLexicon.lexiconissuelinker.Any(y => x == y.Linkers) == false).ToDBModel(objLexicon.Id);

          db.lexiconissuelinker.AddRange(newLinkerList);
          isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
          LexiconTermViewModel afterModel = GetLexiconTermAuditModel(objLexicon);
          Task.Run(() => AuditRepository.WriteAudit<LexiconTermViewModel>(AuditConstants.LexiconTerm, AuditType.Update, beforeModel, afterModel, AuditConstants.UpdateSuccessMsg));
        }
      }
      return isSave;
    }

    private LexiconTermViewModel GetLexiconTermAuditModel(lexiconissues lexiconModel)
    {
      LexiconTermViewModel model = null;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        var lexconObj = db.lexiconissues.Where(a => a.Id == lexiconModel.Id).FirstOrDefault();
        model = new LexiconTermViewModel()
        {
          LexiconType = lexconObj.lexicontype != null ? lexconObj.lexicontype.Type : string.Empty,
          LexiconTerm = lexconObj.LexiconIssue,
          Nested = lexconObj.IsNested,
          Created = lexconObj.Created.HasValue ? Helper.GetCurrentFormatedDateTime() : string.Empty,
          Modified = lexconObj.Modified.HasValue ? lexconObj.Modified.ToFormatedDateTime() : string.Empty,
          CreatedBy = lexconObj.CreatedBy,
          ModifiedBy = lexconObj.ModifiedBy
        };
      }
      return model;
    }


    /// <summary>
    /// Get the Lexicon List
    /// </summary>
    /// <returns>Lexicon list </returns>
    public List<LexiconModel> GetLexiconListForScraping()
    {
      List<LexiconModel> lexiconeList;

      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        lexiconeList = db.lexiconissues
          .Where(x => !x.IsDeleted)
            .Select(x => new LexiconModel()
            {
              LexiconeIssueMasterId = x.Id,
              LexiconeTypeMasterId = x.LexiconTypeId,
              LexiconIssue = x.LexiconIssue,
              CombinationValue = x.CombinationValue,
              IsNested = x.IsNested
            }).ToList();
      }
      return lexiconeList;
    }

    private List<lexiconissuelinker> ConvertToLinkersList(string lexiconsLinks)
    {
      List<lexiconissuelinker> lexiconIssueLinkerList = lexiconsLinks.Split(',').Select(x => new lexiconissuelinker()
      {
        Linkers = x
      }).ToList();
      return lexiconIssueLinkerList;
    }

    public async Task<ApiOutput> GetAllLexiconList(GridParameters parametersJson)
    {
      ApiOutput apiOutput = new ApiOutput();

      List<LexiconModel> lexiconList;
      int totalRecord = 0;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        IQueryable<LexiconModel> query = db.lexiconissues
          .Where(x => !x.IsDeleted)
            .Select(x => new LexiconModel()
            {
              LexiconeIssueMasterId = x.Id,
              LexiconeTypeMasterId = x.LexiconTypeId,
              LexiconIssue = x.LexiconIssue,
              CombinationValue = x.CombinationValue,
              LexiconType = x.lexicontype.Type,
              IsNested = x.IsNested,
              Status = x.IsNested  ? Resources.Enums.Status.Yes.ToString() : Enums.Status.No.ToString(),
              Linker = x.lexiconissuelinker.Where(s => !s.IsDeleted).Select(y => y.Linkers).ToList()
            });

        if (parametersJson.Sort == null || parametersJson.Sort.Count == 0)
        {
          query = query.OrderByDescending(x => x.LexiconeIssueMasterId);
        }
        lexiconList = await query.ModifyList(parametersJson, out totalRecord).ToListAsync();
      }

      lexiconList.ForEach(a =>
      {
          a.TotalRecordCount = totalRecord;
      });

      apiOutput.Data = lexiconList;
      apiOutput.TotalRecords = totalRecord;
      return apiOutput;
    }

    /// <summary>
    /// Delete Lexicon term
    /// </summary>
    /// <param name="lexiconIssueMasterHashId">lexicon id to Delete</param>
    /// <returns>return successfull message</returns>
    public async Task<bool> DeleteLexicon(string lexiconIssueMasterHashId)
    {
      bool isSave = false;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {

        if (!string.IsNullOrEmpty(lexiconIssueMasterHashId))
        {
          int decryptLexiconIssueId = lexiconIssueMasterHashId.ToDecrypt().ToInt32();

          var objLexicon = await db.lexiconissues.Where(x => x.Id == decryptLexiconIssueId && !x.IsDeleted).FirstOrDefaultAsync();

          if (objLexicon != null)
          {
            objLexicon.IsDeleted = true;
          }

          var DeletelinkersList = db.lexiconissuelinker.Where(x => x.LexiconIssueId == decryptLexiconIssueId);
          foreach (var Dellnk in DeletelinkersList)
          {
            Dellnk.IsDeleted = true;
          }
          isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
          LexiconTermViewModel model = GetLexiconTermAuditModel(objLexicon);
          Task.Run(() => AuditRepository.WriteAudit<LexiconTermViewModel>(AuditConstants.LexiconTerm, AuditType.Delete, model, null, AuditConstants.DeleteSuccessMsg));		  
        }
      }
      return isSave;
    }

    public async Task<bool> ImportLexiconRecords(List<LexiconCsvImportModel> lexiconImportModelList)
    {
      bool isSave = false;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        var handleValidatedImportedUser = new List<Task>();

        handleValidatedImportedUser.Add(InsertNewImportedLexicon(db, lexiconImportModelList));

        await Task.WhenAll(handleValidatedImportedUser);

        isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
      }
      return isSave;
    }

    private async Task InsertNewImportedLexicon(BCMStrategyEntities db, List<LexiconCsvImportModel> lexiconImportModelList)
    {
      await Task.Yield();
      DateTime currentTimeStamp = Helper.GetCurrentDateTime();
      var listOfInsertedRecord = lexiconImportModelList.Select(x => new lexiconissues()
      {
        LexiconTypeId = x.LexiconTypeMasterHashId.ToDecrypt().ToInt32(),
        LexiconIssue = x.LexiconIssue,
        CombinationValue = x.CombinationValue,
        IsNested = x.IsNested,
        Created = currentTimeStamp,
        CreatedBy = UserAccessHelper.CurrentUserIdentity.ToString(),
        lexiconissuelinker = ConvertToLinkersList(x.LexiconLinkers)
      });
      db.lexiconissues.AddRange(listOfInsertedRecord);
    }

    public async Task<LexiconModel> GetLexiconByHashId(string lexiconHashId)
    {
      LexiconModel model = new LexiconModel();
      int lexiconDecryptId = lexiconHashId.ToDecrypt().ToInt32();
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        var objLexicon = db.lexiconissues.Where(a => a.Id == lexiconDecryptId).FirstOrDefault();
        if (objLexicon != null)
        {
          model.LexiconeIssueMasterId = objLexicon.Id;
          model.LexiconeTypeMasterId = objLexicon.LexiconTypeId;
          model.LexiconIssue = objLexicon.LexiconIssue;
          model.CombinationValue =  objLexicon.CombinationValue;
          model.LexiconType = objLexicon.lexicontype.Type;
          model.IsNested = objLexicon.IsNested;
          model.Status = objLexicon.IsNested ? Resources.Enums.Status.Yes.ToString() : Enums.Status.No.ToString();
          model.Linker = objLexicon.lexiconissuelinker.Where(s => !s.IsDeleted).Select(y => y.Linkers).ToList();
        }
      }
      return model;
    }
  }
}
