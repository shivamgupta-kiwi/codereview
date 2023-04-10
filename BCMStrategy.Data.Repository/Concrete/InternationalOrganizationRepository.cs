using BCMStrategy.Common.AuditLog;
using BCMStrategy.Common.Kendo;
using BCMStrategy.Common.Unity;
using BCMStrategy.DAL.Context;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Logger;
using BCMStrategy.Resources;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Repository.Concrete
{
  public class InternationalOrganizationRepository : IInternationalOrganization
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
    /// Add/Update International Organizations
    /// </summary>
    /// <param name="internationalOrganization">Model Of Organizations</param>
    /// <returns>return saved or not</returns>
    public async Task<bool> UpdateInternationalOrganization(InternationalOrganizationModel internationalOrganization)
    {
      bool isSave = false;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        DateTime currentTimeStamp = Helper.GetCurrentDateTime();

        if (string.IsNullOrEmpty(internationalOrganization.InternaltionalOrganizationMasterHashId))
        {
          internationalorganizations objInternationalOrganizations = new internationalorganizations()
          {
            DesignationId = string.IsNullOrEmpty(internationalOrganization.DesignationMasterHashId) ? (int?)null : internationalOrganization.DesignationMasterHashId.ToDecrypt().ToInt32(),
            OrganizationName = internationalOrganization.OrganizationName,
            LeaderFirstName = internationalOrganization.LeaderFirstName,
            LeaderLastName = internationalOrganization.LeaderLastName,
            EntityName = internationalOrganization.EntityName,
            IsMultiLateral = internationalOrganization.IsMultiLateral,
            Created = currentTimeStamp,
            CreatedBy = UserAccessHelper.CurrentUserIdentity.ToString()
          };
          db.internationalorganizations.Add(objInternationalOrganizations);
          isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
          InternalOrgViewModel model = GetInternalOrganizationAuditModel(objInternationalOrganizations);
          Task.Run(() => AuditRepository.WriteAudit<InternalOrgViewModel>(AuditConstants.InternalOrganization, AuditType.Insert, null, model, AuditConstants.InsertSuccessMsg));
        }
        else
        {
          int decryptInternationalOrgId = internationalOrganization.InternaltionalOrganizationMasterHashId.ToDecrypt().ToInt32();

          var objInternationalOrganizations = await db.internationalorganizations.Where(x => x.Id == decryptInternationalOrgId && !x.IsDeleted).FirstOrDefaultAsync();
          InternalOrgViewModel beforemodel = GetInternalOrganizationAuditModel(objInternationalOrganizations);

          if (objInternationalOrganizations != null)
          {
            objInternationalOrganizations.DesignationId = string.IsNullOrEmpty(internationalOrganization.DesignationMasterHashId) ? (int?)null : internationalOrganization.DesignationMasterHashId.ToDecrypt().ToInt32();
            objInternationalOrganizations.OrganizationName = internationalOrganization.OrganizationName;
            objInternationalOrganizations.LeaderFirstName = internationalOrganization.LeaderFirstName;
            objInternationalOrganizations.LeaderLastName = internationalOrganization.LeaderLastName;
            objInternationalOrganizations.EntityName = internationalOrganization.EntityName;
            objInternationalOrganizations.IsMultiLateral = internationalOrganization.IsMultiLateral;
            objInternationalOrganizations.Modified = currentTimeStamp;
            objInternationalOrganizations.ModifiedBy = UserAccessHelper.CurrentUserIdentity.ToString();
          }
          isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
          InternalOrgViewModel aftermodel = GetInternalOrganizationAuditModel(objInternationalOrganizations);
          Task.Run(() => AuditRepository.WriteAudit<InternalOrgViewModel>(AuditConstants.InternalOrganization, AuditType.Update, beforemodel, aftermodel, AuditConstants.UpdateSuccessMsg));
        }
      }
      return isSave;
    }

    /// <summary>
    /// Delete the Organization from DB
    /// </summary>
    /// <param name="internationalOrgMasterHashId">Pass the International Organization id</param>
    /// <returns>saved result</returns>
    public async Task<bool> DeleteInternationalOrganization(string internationalOrgMasterHashId)
    {
      bool isSave = false;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        
        if (!string.IsNullOrEmpty(internationalOrgMasterHashId))
        {
          int decryptInternationalOrgId = internationalOrgMasterHashId.ToDecrypt().ToInt32();

          var objInternationalOrganization = await db.internationalorganizations.Where(x => x.Id == decryptInternationalOrgId && !x.IsDeleted).FirstOrDefaultAsync();

          if (objInternationalOrganization != null)
          {
            objInternationalOrganization.IsDeleted = true;
          }
          isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
          InternalOrgViewModel model = GetInternalOrganizationAuditModel(objInternationalOrganization);
          Task.Run(() => AuditRepository.WriteAudit<InternalOrgViewModel>(AuditConstants.InternalOrganization, AuditType.Delete, model, null, AuditConstants.DeleteSuccessMsg, UserAccessHelper.CurrentUserIdentity));
        }
      }
      return isSave;
    }

    private InternalOrgViewModel GetInternalOrganizationAuditModel(internationalorganizations internationalorganizationsModel)
    {
      InternalOrgViewModel model = null;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (internationalorganizationsModel.Id > 0)
        {
          var internationalorganizationsObj = db.internationalorganizations.Where(a => a.Id == internationalorganizationsModel.Id).FirstOrDefault();
          model = new InternalOrgViewModel()
          {
            OrganizationName = internationalorganizationsObj.OrganizationName,
            Designation = internationalorganizationsObj.designation != null ? internationalorganizationsObj.designation.Designation1 : string.Empty,
            FirstName = internationalorganizationsObj.LeaderFirstName,
            LastName = internationalorganizationsObj.LeaderLastName,
            EntityName = internationalorganizationsObj.EntityName,
            Created = internationalorganizationsObj.Created.HasValue ? Helper.GetCurrentFormatedDateTime() : string.Empty,
            Modified = internationalorganizationsObj.Modified.HasValue ? internationalorganizationsObj.Modified.ToFormatedDateTime() : string.Empty,
            CreatedBy = internationalorganizationsObj.CreatedBy,
            ModifiedBy = internationalorganizationsObj.ModifiedBy
          };
        }
        else
        {
          model = new InternalOrgViewModel()
          {
            OrganizationName = internationalorganizationsModel.OrganizationName,
            Designation = internationalorganizationsModel.designation != null ? internationalorganizationsModel.designation.Designation1 : internationalorganizationsModel.DesignationId.ToString(),
            FirstName = internationalorganizationsModel.LeaderFirstName,
            LastName = internationalorganizationsModel.LeaderLastName,
            EntityName = internationalorganizationsModel.EntityName,
            Created = internationalorganizationsModel.Created.HasValue ? Helper.GetCurrentFormatedDateTime() : string.Empty,
            Modified = internationalorganizationsModel.Modified.HasValue ? internationalorganizationsModel.Modified.ToFormatedDateTime() : string.Empty,
            CreatedBy = internationalorganizationsModel.CreatedBy,
            ModifiedBy = internationalorganizationsModel.ModifiedBy
          };
        }
      }
      return model;
    }

    /// <summary>
    /// Get the list of List of International Organizations
    /// </summary>
    /// <param name="parametersJson">Grid Parameters</param>
    /// <returns>return the list of Organizations</returns>
    public async Task<ApiOutput> GetAllInternationalOrganizationList(GridParameters parametersJson)
    {
      ApiOutput apiOutput = new ApiOutput();
      List<InternationalOrganizationModel> internationalOrganizationList;
      int totalRecord = 0;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        IQueryable<InternationalOrganizationModel> query = db.internationalorganizations
          .Where(x => !x.IsDeleted)
            .Select(x => new InternationalOrganizationModel()
            {
              InternaltionalOrganizationMasterId = x.Id,
              DesignationMasterId = x.DesignationId ?? 0,
              OrganizationName = x.OrganizationName,
              DesignationName = x.DesignationId != null ? x.designation.Designation1 : string.Empty,
              EntityName = x.EntityName,
              LeaderName = (x.LeaderFirstName + " " ?? string.Empty) + (x.LeaderLastName ?? string.Empty),
              LeaderFirstName = x.LeaderFirstName ?? string.Empty,
              LeaderLastName = x.LeaderLastName ?? string.Empty,
              IsMultiLateral = x.IsMultiLateral,
              Status = x.IsMultiLateral ? Enums.Status.Yes.ToString() : Enums.Status.No.ToString(),
            });
        if (parametersJson.Sort == null || parametersJson.Sort.Count == 0)
        {
          query = query.OrderByDescending(x => x.InternaltionalOrganizationMasterId);
        }
        internationalOrganizationList = await query.ModifyList(parametersJson, out totalRecord).ToListAsync();
      }
      apiOutput.Data = internationalOrganizationList;
      apiOutput.TotalRecords = totalRecord;
      return apiOutput;
    }

    /// <summary>
    /// Import Bulk insert from csv Import
    /// </summary>
    /// <param name="internationalOrganizationImportModelList">International Organization Import List</param>
    /// <returns>Saved or not</returns>
    public async Task<bool> ImportInternationOrganizationRecord(List<InternationalOrganizationImportModel> internationalOrganizationImportModelList)
    {
      bool isSave = false;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        var handleValidatedImportedUser = new List<Task>();

        handleValidatedImportedUser.Add(InsertNewInternationalOrganization(db, internationalOrganizationImportModelList));

        await Task.WhenAll(handleValidatedImportedUser);

        isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
      }
      return isSave;
    }

    /// <summary>
    /// International Organization Import
    /// </summary>
    /// <param name="db">db</param>
    /// <param name="internationalOrganizationImportModelList">International Organization Import List</param>
    /// <returns></returns>
    private async Task InsertNewInternationalOrganization(BCMStrategyEntities db, List<InternationalOrganizationImportModel> internationalOrganizationImportModelList)
    {
      await Task.Yield();
      DateTime currentTimeStamp = Helper.GetCurrentDateTime();

      var listOfInsertedRecord = internationalOrganizationImportModelList.Select(x => new internationalorganizations()
      {
        OrganizationName = x.OrganizationName,
        LeaderFirstName = x.LeaderFirstName,
        LeaderLastName = x.LeaderLastName,
        EntityName = x.EntityName,
        DesignationId = string.IsNullOrEmpty(x.DesignationMasterHashId) ? (int?)null : x.DesignationMasterHashId.ToDecrypt().ToInt32(),
        IsMultiLateral = x.IsMultiLateral,
        Created = currentTimeStamp,
        CreatedBy = UserAccessHelper.CurrentUserIdentity.ToString()
      });
      db.internationalorganizations.AddRange(listOfInsertedRecord);

      foreach (internationalorganizations internationalOrganization in listOfInsertedRecord)
      {
        InternalOrgViewModel internationOrganizationModel = GetInternalOrganizationAuditModel(internationalOrganization);

        Task.Run(() => AuditRepository.WriteAudit<InternalOrgViewModel>(AuditConstants.InternalOrganization, AuditType.Insert, null, internationOrganizationModel, AuditConstants.ImportInsertSuccessMsg));
      }
    }

    public async Task<InternationalOrganizationModel> GetInternationalOrgByHashId(string internationalOrgHashId)
    {
      InternationalOrganizationModel model = new InternationalOrganizationModel();
      int internationalOrgDecryptId = internationalOrgHashId.ToDecrypt().ToInt32();
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        var objInternationalOrganizations = db.internationalorganizations.Where(a => a.Id == internationalOrgDecryptId).FirstOrDefault();
        if (objInternationalOrganizations != null)
        {
          model.InternaltionalOrganizationMasterId = objInternationalOrganizations.Id;
          model.DesignationMasterId = objInternationalOrganizations.DesignationId ?? 0;
          model.OrganizationName = objInternationalOrganizations.OrganizationName;
          model.DesignationName = objInternationalOrganizations.DesignationId != null ? objInternationalOrganizations.designation.Designation1 : string.Empty;
          model.EntityName = objInternationalOrganizations.EntityName;
          model.LeaderName = (objInternationalOrganizations.LeaderFirstName + " " ?? string.Empty) + (objInternationalOrganizations.LeaderLastName ?? string.Empty);
          model.LeaderFirstName = objInternationalOrganizations.LeaderFirstName ?? string.Empty;
          model.LeaderLastName = objInternationalOrganizations.LeaderLastName ?? string.Empty;
          model.IsMultiLateral = objInternationalOrganizations.IsMultiLateral;
          model.Status = objInternationalOrganizations.IsMultiLateral ? Enums.Status.Yes.ToString() : Enums.Status.No.ToString();
        }
      }
      return model;
    }
  }
}