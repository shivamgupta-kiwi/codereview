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
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Repository.Concrete
{
  public class PolicyMakerRepository : IPolicyMaker
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
    /// Add and update of Policy Makers
    /// </summary>
    /// <param name="policyMakerModel">Policy Model with Institution values</param>
    /// <returns>Is Saved or not</returns>
    public async Task<bool> UpdatePolicyMaker(PolicyMakerModel policyMakerModel)
    {
      bool isSave = false;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        DateTime currentTimeStamp = Helper.GetCurrentDateTime();
        int? nuvall = null;
        if (string.IsNullOrEmpty(policyMakerModel.PolicyMakerHashId))
        {
          policymakers objPolicyMakers = new policymakers()
          {
            CountryId = policyMakerModel.CountryMasterHashId.ToDecrypt().ToInt32() != 0 ? policyMakerModel.CountryMasterHashId.ToDecrypt().ToInt32() : nuvall,
            InstitutionId = policyMakerModel.InstitutionTypeHashId.ToDecrypt().ToInt32(),
            ////PolicyFirstName = policyMakerModel.PolicyMakerName,
            PolicyFirstName = policyMakerModel.PolicyFirstName,
            PolicyLastName = policyMakerModel.PolicyLastName,
            DesignationId = policyMakerModel.DesignationHashId.ToDecrypt().ToInt32(),
            Created = currentTimeStamp,
            CreatedBy = UserAccessHelper.CurrentUserIdentity.ToString()
          };
          db.policymakers.Add(objPolicyMakers);
          isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
          PolicyMakerAuditModel model = GetPolicyMakerAuditModel(objPolicyMakers);
          Task.Run(() => AuditRepository.WriteAudit<PolicyMakerAuditModel>(AuditConstants.PolicyMaker, AuditType.Insert, null, model, AuditConstants.InsertSuccessMsg));
        }
        else
        {
          int policyMakerId = policyMakerModel.PolicyMakerHashId.ToDecrypt().ToInt32();

          var objPolicyMakers = await db.policymakers.Where(x => x.Id == policyMakerId && !x.IsDeleted).FirstOrDefaultAsync();
          PolicyMakerAuditModel beforeModel = GetPolicyMakerAuditModel(objPolicyMakers);

          if (objPolicyMakers != null)
          {
            objPolicyMakers.CountryId = policyMakerModel.CountryMasterHashId.ToDecrypt().ToInt32() != 0 ? policyMakerModel.CountryMasterHashId.ToDecrypt().ToInt32() : nuvall;
            objPolicyMakers.InstitutionId = policyMakerModel.InstitutionTypeHashId.ToDecrypt().ToInt32();
            ////objPolicyMakers.PolicyFirstName = policyMakerModel.PolicyMakerName;
            objPolicyMakers.PolicyFirstName = policyMakerModel.PolicyFirstName;
            objPolicyMakers.PolicyLastName = policyMakerModel.PolicyLastName;
            objPolicyMakers.DesignationId = policyMakerModel.DesignationHashId.ToDecrypt().ToInt32();

            objPolicyMakers.Modified = currentTimeStamp;
            objPolicyMakers.ModifiedBy = UserAccessHelper.CurrentUserIdentity.ToString();
          }
          isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
          PolicyMakerAuditModel afterModel = GetPolicyMakerAuditModel(objPolicyMakers);
          Task.Run(() => AuditRepository.WriteAudit<PolicyMakerAuditModel>(AuditConstants.PolicyMaker, AuditType.Update, beforeModel, afterModel, AuditConstants.UpdateSuccessMsg));
        }
      }
      return isSave;
    }

    private PolicyMakerAuditModel GetPolicyMakerAuditModel(policymakers policyMakerModel)
    {
      PolicyMakerAuditModel model = null;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {

        if (policyMakerModel.Id > 0)
        {
          var policymakersObj = db.policymakers.Where(a => a.Id == policyMakerModel.Id).FirstOrDefault();
          model = new PolicyMakerAuditModel()
          {
            InstitutionType = policymakersObj.institutiontypes != null ? policymakersObj.institutiontypes.InstitutionType : string.Empty,
            CountryName = policymakersObj.country != null ? policymakersObj.country.Name : string.Empty,
            DesignationName = policymakersObj.designation != null ? policymakersObj.designation.Designation1 : string.Empty,
            PolicyFirstName = policymakersObj.PolicyFirstName,
            PolicyLastName = policymakersObj.PolicyLastName,
            Created = policymakersObj.Created.HasValue ? Helper.GetCurrentFormatedDateTime() : string.Empty,
            Modified = policymakersObj.Modified.HasValue ? policymakersObj.Modified.ToFormatedDateTime() : string.Empty,
            CreatedBy = policymakersObj.CreatedBy,
            ModifiedBy = policymakersObj.ModifiedBy
          };
        }
        else
        {
          model = new PolicyMakerAuditModel()
          {
            InstitutionType = policyMakerModel.institutiontypes != null ? policyMakerModel.institutiontypes.InstitutionType : policyMakerModel.InstitutionId.ToString(),
            CountryName = policyMakerModel.country != null ? policyMakerModel.country.Name : policyMakerModel.CountryId.ToString(),
            DesignationName = policyMakerModel.designation != null ? policyMakerModel.designation.Designation1 : string.Empty,
            PolicyFirstName = policyMakerModel.PolicyFirstName,
            PolicyLastName = policyMakerModel.PolicyLastName,
            Created = policyMakerModel.Created.HasValue ? Helper.GetCurrentFormatedDateTime() : string.Empty,
            Modified = policyMakerModel.Modified.HasValue ? policyMakerModel.Modified.ToFormatedDateTime() : string.Empty,
            CreatedBy = policyMakerModel.CreatedBy,
            ModifiedBy = policyMakerModel.ModifiedBy
          };
        }
      }
      return model;
    }

    /// <summary>
    /// Delete Policy Makers
    /// </summary>
    /// <param name="policyMakerHashId">Policy Maker id to Delete</param>
    /// <returns>return successful message</returns>
    public async Task<bool> DeletePolicyMaker(string policyMakerHashId)
    {
      bool isSave = false;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (!string.IsNullOrEmpty(policyMakerHashId))
        {
          int decryptPolicyId = policyMakerHashId.ToDecrypt().ToInt32();

          var objInstitution = await db.policymakers.Where(x => x.Id == decryptPolicyId && !x.IsDeleted).FirstOrDefaultAsync();

          if (objInstitution != null)
          {
            objInstitution.IsDeleted = true;
          }
          isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
          PolicyMakerAuditModel model = GetPolicyMakerAuditModel(objInstitution);
          Task.Run(() => AuditRepository.WriteAudit<PolicyMakerAuditModel>(AuditConstants.PolicyMaker, AuditType.Delete, model, null, AuditConstants.DeleteSuccessMsg, UserAccessHelper.CurrentUserIdentity));
        }
      }
      return isSave;
    }

    /// <summary>
    /// Get all the list of Policy Makers
    /// </summary>
    /// <param name="parametersJson">Grid Parameter to filter or sorting</param>
    /// <returns>return the list</returns>
    public async Task<ApiOutput> GetAllPolicyMakerList(GridParameters parametersJson)
    {
      ApiOutput apiOutput = new ApiOutput();
      List<PolicyMakerModel> policyMakerList;

      int totalRecord = 0;

      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        IQueryable<PolicyMakerModel> query = db.policymakers
          .Where(x => !x.IsDeleted)
            .Select(x => new PolicyMakerModel()
            {
              PolicyMakerId = x.Id,
              CountryMasterId = x.CountryId.HasValue ? x.CountryId.Value : x.CountryId,
              InstitutionTypeMasterId = x.InstitutionId,
              CountryName = x.country.Name,
              InstitutionType = x.institutiontypes.InstitutionType,
              PolicyFirstName = x.PolicyFirstName ?? string.Empty,
              PolicyLastName = x.PolicyLastName ?? string.Empty,
              PolicyMakerName = (x.PolicyFirstName + " " ?? string.Empty) + (x.PolicyLastName ?? string.Empty),
              DesignationMasterId = x.DesignationId != null ? x.DesignationId.Value : 0,
              DesignationName = x.designation.Designation1,
            });
        if (parametersJson.Sort == null || parametersJson.Sort.Count == 0)
        {
          query = query.OrderByDescending(x => x.PolicyMakerId);
        }
        policyMakerList = await query.ModifyList(parametersJson, out totalRecord).ToListAsync();
      }
      apiOutput.Data = policyMakerList;
      apiOutput.TotalRecords = totalRecord;
      return apiOutput;
    }

    /// <summary>
    /// Get All the Designation List
    /// </summary>
    /// <returns></returns>
    public async Task<ApiOutput> GetDropdownDesignationList()
    {
      ApiOutput apiOutput = new ApiOutput();
      List<DropdownMaster> designationDropdownList;

      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        IQueryable<DropdownMaster> query = db.designation
            .Select(x => new DropdownMaster()
            {
              Key = x.Id,
              Value = x.Designation1
            }).OrderBy(x => x.Value);
        designationDropdownList = await query.ToListAsync();
      }

      apiOutput.Data = designationDropdownList;
      apiOutput.TotalRecords = 0;
      apiOutput.ErrorMessage = designationDropdownList.Any() ? string.Empty : string.Format(Resource.ValidateMessageNoDataFound, Resource.LblDesignationType);
      return apiOutput;
    }

    /// <summary>
    /// Import the data's of Policy Makers
    /// </summary>
    /// <param name="policyMakersImportModelList"></param>
    /// <returns></returns>
    public async Task<bool> ImportPolicyMakerRecords(List<PolicyMakersCsvImportModel> policyMakersImportModelList)
    {
      bool isSave = false;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        var handleValidatedImportedUser = new List<Task>();

        handleValidatedImportedUser.Add(InsertNewImportedPolicyMaker(db, policyMakersImportModelList));

        await Task.WhenAll(handleValidatedImportedUser);

        isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
      }
      return isSave;
    }

    /// <summary>
    /// Insert Import Data
    /// </summary>
    /// <param name="db">db object</param>
    /// <param name="policyMakersImportModelList">Policy Maker Model object</param>
    /// <returns></returns>
    private async Task InsertNewImportedPolicyMaker(BCMStrategyEntities db, List<PolicyMakersCsvImportModel> policyMakersImportModelList)
    {
      await Task.Yield();
      DateTime currentTimeStamp = Helper.GetCurrentDateTime();
      int? nullval = null;

      var listOfInsertedRecord = policyMakersImportModelList.Select(x => new policymakers()
      {
        CountryId = x.CountryMasterHashId.ToDecrypt().ToInt32() != 0 ? x.CountryMasterHashId.ToDecrypt().ToInt32() : nullval,
        ////InstitutionTypeId = x.InstitutionTypeMasterHashId.ToString().ToInt32(),
        DesignationId = x.DesignationMasterHashId.ToDecrypt().ToInt32(),
        InstitutionId = x.InstitutionTypeMasterHashId.ToDecrypt().ToInt32(),
        PolicyFirstName = x.PolicyFirstName,
        PolicyLastName = x.PolicyLastName,
        Created = currentTimeStamp,
        CreatedBy = UserAccessHelper.CurrentUserIdentity.ToString()
      });
      db.policymakers.AddRange(listOfInsertedRecord);

      foreach (policymakers policyMakers in listOfInsertedRecord)
      {
        PolicyMakerAuditModel policyMakerModel = GetPolicyMakerAuditModel(policyMakers);

        Task.Run(() => AuditRepository.WriteAudit<PolicyMakerAuditModel>(AuditConstants.PolicyMaker, AuditType.Insert, null, policyMakerModel, AuditConstants.ImportInsertSuccessMsg));
      }
    }

    public int GetDesignationIdByName(string designationName)
    {
      int designationId = 0;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (!string.IsNullOrEmpty(designationName))
        {
          designationId = db.designation.Where(x => x.Designation1 == designationName).Select(x => x.Id).FirstOrDefault();
        }
      }
      return designationId;
    }

    public async Task<PolicyMakerModel> GetPolicyMakerByHashId(string policyMakerHashId)
    {
      PolicyMakerModel model = new PolicyMakerModel();
      int policyMakerDecryptId = policyMakerHashId.ToDecrypt().ToInt32();
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        var objPolicyMaker = db.policymakers.Where(a => a.Id == policyMakerDecryptId).FirstOrDefault();
        if (objPolicyMaker != null)
        {
          model.PolicyMakerId = objPolicyMaker.Id;
          model.CountryMasterId = objPolicyMaker.CountryId.HasValue ? objPolicyMaker.CountryId.Value : objPolicyMaker.CountryId;
          model.InstitutionTypeMasterId = objPolicyMaker.InstitutionId;
          model.CountryName = objPolicyMaker.CountryId.HasValue ? objPolicyMaker.country.Name : string.Empty;
          model.InstitutionType = objPolicyMaker.institutiontypes.InstitutionType;
          model.PolicyFirstName = objPolicyMaker.PolicyFirstName ?? string.Empty;
          model.PolicyLastName = objPolicyMaker.PolicyLastName ?? string.Empty;
          model.PolicyMakerName = (objPolicyMaker.PolicyFirstName + " " ?? string.Empty) + (objPolicyMaker.PolicyLastName ?? string.Empty);
          model.DesignationMasterId = objPolicyMaker.DesignationId != null ? objPolicyMaker.DesignationId.Value : 0;
          model.DesignationName = objPolicyMaker.designation.Designation1;
        }
      }
      return model;
    }
  }
}