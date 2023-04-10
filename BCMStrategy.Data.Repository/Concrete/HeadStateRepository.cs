using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using BCMStrategy.Common.Kendo;
using BCMStrategy.DAL.Context;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Logger;
using BCMStrategy.Common.AuditLog;
using BCMStrategy.Common.Unity;

namespace BCMStrategy.Data.Repository.Concrete
{
  public class HeadStateRepository : IHeadStateRepository
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
    /// Import all the records to Import
    /// </summary>
    /// <param name="headStateImportModel">List of HeadState</param>
    /// <returns>return saved or not status</returns>
    public async Task<bool> ImportHeadStateRecord(List<HeadStateImportModel> headStateImportModel)
    {
      bool isSave = false;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        var handleValidatedImportedUser = new List<Task>();

        handleValidatedImportedUser.Add(InsertNewImportedHeadState(db, headStateImportModel));

        await Task.WhenAll(handleValidatedImportedUser);

        isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
      }
      return isSave;
    }

    /// <summary>
    /// Import State Heads
    /// </summary>
    /// <param name="db"></param>
    /// <param name="headStateImportModelList"></param>
    /// <returns></returns>
    private async Task InsertNewImportedHeadState(BCMStrategyEntities db, List<HeadStateImportModel> headStateImportModelList)
    {
      await Task.Yield();
      DateTime currentTimeStamp = Helper.GetCurrentDateTime();

      var listOfInsertedRecord = headStateImportModelList.Select(x => new statehead()
      {
        CountryId = x.CountryMasterHashId.ToDecrypt().ToInt32(),
        DesignationId = x.DesignationMasterHashId.ToDecrypt().ToInt32(),
        FirstName = x.FirstName,
        LastName = x.LastName,
        Created = currentTimeStamp,
        CreatedBy = UserAccessHelper.CurrentUserIdentity.ToString()
      });
      db.statehead.AddRange(listOfInsertedRecord);

      foreach (statehead stateHead in listOfInsertedRecord)
      {
        HeadofStateAndGovernmentAuditViewModel stateheadModel = GetHeadStateAuditModel(stateHead);

        Task.Run(() => AuditRepository.WriteAudit<HeadofStateAndGovernmentAuditViewModel>(AuditConstants.HeadOfStateAndGovrnment, AuditType.Insert, null, stateheadModel, AuditConstants.ImportInsertSuccessMsg));
      }
    }

    /// <summary>
    /// Get the State Head list
    /// </summary>
    /// <returns>State Head list </returns>
    public async Task<ApiOutput> GetStateHeadList(GridParameters parameters)
    {
      ApiOutput apiOutput = new ApiOutput();
      List<StateHeadModel> stateHeadList;
      int totalRecord = 0;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        IQueryable<StateHeadModel> query = db.statehead
          .Where(x => !x.IsDeleted)
            .Select(x => new StateHeadModel()
            {
              StateHeadMasterId = x.Id,
              CountryMasterId = x.CountryId,
              FirstName = x.FirstName ?? string.Empty,
              LastName = x.LastName ?? string.Empty,
              StateHeadName = (x.FirstName + " " ?? string.Empty) + (x.LastName ?? string.Empty),
              DesignationMasterId = x.DesignationId ?? 0,
              DesignationName = x.designation.Designation1,
              CountryName = x.country.Name
            });
        if (parameters.Sort == null || parameters.Sort.Count == 0)
        {
          query = query.OrderByDescending(x => x.StateHeadMasterId);
        }
        stateHeadList = await query.ModifyList(parameters, out totalRecord).ToListAsync();
      }
      apiOutput.Data = stateHeadList;
      apiOutput.TotalRecords = totalRecord;
      return apiOutput;
    }

    /// <summary>
    /// Add and update of StateHead
    /// </summary>
    /// <param name="stateHeadModel">Values for State Head</param>
    /// <returns>Is Saved or not</returns>
    public async Task<bool> UpdateStateHead(StateHeadModel stateHeadModel)
    {
      bool isSave = false;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        DateTime currentTimeStamp = Helper.GetCurrentDateTime();

        if (string.IsNullOrEmpty(stateHeadModel.StateHeadMasterHashId))
        {
          statehead objStateHead = new statehead()
          {
            FirstName = stateHeadModel.FirstName,
            LastName = stateHeadModel.LastName,
            DesignationId = stateHeadModel.DesignationHashId.ToDecrypt().ToInt32(),
            CountryId = stateHeadModel.CountryMasterHashId.ToDecrypt().ToInt32(),
            Created = currentTimeStamp,
            CreatedBy = UserAccessHelper.CurrentUserIdentity.ToString()
          };
          db.statehead.Add(objStateHead);
          isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
          HeadofStateAndGovernmentAuditViewModel model = GetHeadStateAuditModel(objStateHead);
          Task.Run(() => AuditRepository.WriteAudit<HeadofStateAndGovernmentAuditViewModel>(AuditConstants.HeadOfStateAndGovrnment, AuditType.Insert, null, model, AuditConstants.InsertSuccessMsg));
        }
        else
        {
          int decryptStateHeadId = stateHeadModel.StateHeadMasterHashId.ToDecrypt().ToInt32();

          var objStateHead = await db.statehead.Where(x => x.Id == decryptStateHeadId && !x.IsDeleted).FirstOrDefaultAsync();
          HeadofStateAndGovernmentAuditViewModel beforeModel = GetHeadStateAuditModel(objStateHead);


          if (objStateHead != null)
          {
            objStateHead.CountryId = stateHeadModel.CountryMasterHashId.ToDecrypt().ToInt32();
            objStateHead.FirstName = stateHeadModel.FirstName;
            objStateHead.LastName = stateHeadModel.LastName;
            objStateHead.DesignationId = stateHeadModel.DesignationHashId.ToDecrypt().ToInt32();
            objStateHead.Modified = currentTimeStamp;
            objStateHead.ModifiedBy = UserAccessHelper.CurrentUserIdentity.ToString();
          }
          isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
          HeadofStateAndGovernmentAuditViewModel afterModel = GetHeadStateAuditModel(objStateHead);
          Task.Run(() => AuditRepository.WriteAudit<HeadofStateAndGovernmentAuditViewModel>(AuditConstants.HeadOfStateAndGovrnment, AuditType.Update, beforeModel, afterModel, AuditConstants.UpdateSuccessMsg));
        }

      }
      return isSave;
    }

    /// <summary>
    /// Delete State Head
    /// </summary>
    /// <param name="stateHeadMasterHashId">State Head Master </param>
    /// <returns>Deleted Records</returns>
    public async Task<bool> DeleteStateHead(string stateHeadMasterHashId)
    {
      bool isSave = false;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {

        if (!string.IsNullOrEmpty(stateHeadMasterHashId))
        {
          int decryptStateHeadId = stateHeadMasterHashId.ToDecrypt().ToInt32();

          var objStateHead = await db.statehead.Where(x => x.Id == decryptStateHeadId && !x.IsDeleted).FirstOrDefaultAsync();

          if (objStateHead != null)
          {
            objStateHead.IsDeleted = true;
          }
          isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
          HeadofStateAndGovernmentAuditViewModel model = GetHeadStateAuditModel(objStateHead);
          Task.Run(() => AuditRepository.WriteAudit<HeadofStateAndGovernmentAuditViewModel>(AuditConstants.HeadOfStateAndGovrnment, AuditType.Delete, model, null, AuditConstants.DeleteSuccessMsg, UserAccessHelper.CurrentUserIdentity));
        }
      }
      return isSave;
    }

    private HeadofStateAndGovernmentAuditViewModel GetHeadStateAuditModel(statehead stateheadModel)
    {
      HeadofStateAndGovernmentAuditViewModel model = null;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (stateheadModel.Id > 0)
        {
          var headStateObj = db.statehead.Where(a => a.Id == stateheadModel.Id).FirstOrDefault();
          model = new HeadofStateAndGovernmentAuditViewModel()
          {
            Country = headStateObj.country != null ? headStateObj.country.Name : string.Empty,
            Designation = headStateObj.designation != null ? headStateObj.designation.Designation1 : string.Empty,
            FirstName = headStateObj.FirstName,
            LastName = headStateObj.LastName,
            Created = headStateObj.Created.HasValue ? Helper.GetCurrentFormatedDateTime() : string.Empty,
            Modified = headStateObj.Modified.HasValue ? headStateObj.Modified.ToFormatedDateTime() : string.Empty,
            CreatedBy = headStateObj.CreatedBy,
            ModifiedBy = headStateObj.ModifiedBy
          };
        }
        else
        {
          model = new HeadofStateAndGovernmentAuditViewModel()
          {
            Country = stateheadModel.country != null ? stateheadModel.country.Name : stateheadModel.CountryId.ToString(),
            Designation = stateheadModel.designation != null ? stateheadModel.designation.Designation1 : stateheadModel.DesignationId.ToString(),
            FirstName = stateheadModel.FirstName,
            LastName = stateheadModel.LastName,
            Created = stateheadModel.Created.HasValue ? Helper.GetCurrentFormatedDateTime() : string.Empty,
            Modified = stateheadModel.Modified.HasValue ? stateheadModel.Modified.ToFormatedDateTime() : string.Empty,
            CreatedBy = stateheadModel.CreatedBy,
            ModifiedBy = stateheadModel.ModifiedBy
          };
        }
      }
      return model;
    }

    public async Task<StateHeadModel> GetStateHeadByHashId(string stateHeadHashId)
    {
      StateHeadModel model = new StateHeadModel();
      int stateHeadDecryptId = stateHeadHashId.ToDecrypt().ToInt32();
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        var objStateHead = db.statehead.Where(a => a.Id == stateHeadDecryptId).FirstOrDefault();
        if (objStateHead != null)
        {
          model.StateHeadMasterId = objStateHead.Id;
          model.CountryMasterId = objStateHead.CountryId;
          model.FirstName = objStateHead.FirstName ?? string.Empty;
          model.LastName = objStateHead.LastName ?? string.Empty;
          model.StateHeadName = (objStateHead.FirstName + " " ?? string.Empty) + (objStateHead.LastName ?? string.Empty);
          model.DesignationMasterId = objStateHead.DesignationId ?? 0;
          model.DesignationName = objStateHead.designation.Designation1;
          model.CountryName = objStateHead.country.Name;
        }
      }
      return model;
    }

  }
}