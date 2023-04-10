using BCMStrategy.Common.AuditLog;
using BCMStrategy.Common.Kendo;
using BCMStrategy.Common.Unity;
using BCMStrategy.DAL.Context;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Repository.Concrete
{
  public class InstitutionsRepository : IInstitutions
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
    /// Add and update of Institutions
    /// </summary>
    /// <param name="institutionModel">Institution Model with Institution values</param>
    /// <returns>Is Saved or not</returns>
    public async Task<bool> UpdateInstitutions(InstitutionModel institutionModel)
    {
      bool isSave = false;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        DateTime currentTimeStamp = Helper.GetCurrentDateTime();

        if (string.IsNullOrEmpty(institutionModel.InstitutionMasterHashId))
        {
          int institutionTypeId = institutionModel.InstitutionTypeHashId.ToDecrypt().ToInt32();
          string institutionTypeName = db.institutiontypes.Where(x => x.Id == institutionTypeId).Select(x => x.InstitutionType).FirstOrDefault();

          institution objInstitution = new institution()
          {
            CountryId = institutionModel.IsEuropeanUnion ? (int?)null : institutionTypeName == Helper.InstitutionTypes.Multilateral.ToString() ? (int?)null : institutionModel.CountryMasterHashId.ToDecrypt().ToInt32(),
            InstitutionTypeId = institutionTypeId,
            InstitutionName = institutionModel.InstitutionsName,
            IsEuropean = institutionModel.IsEuropeanUnion,
            Created = currentTimeStamp,
            CreatedBy = UserAccessHelper.CurrentUserIdentity.ToString(),
            EntityName = institutionModel.EntityName
          };

          db.institution.Add(objInstitution);
          isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
          InstitutionViewModel model = GetInstitutionAuditModel(objInstitution);
          Task.Run(() => AuditRepository.WriteAudit<InstitutionViewModel>(AuditConstants.Institution, AuditType.Insert, null, model, AuditConstants.InsertSuccessMsg));
        }
        else
        {
          int decryptInstitutionsId = institutionModel.InstitutionMasterHashId.ToDecrypt().ToInt32();
          int institutionTypeId = institutionModel.InstitutionTypeHashId.ToDecrypt().ToInt32();

          string institutionTypeName = db.institutiontypes.Where(x => x.Id == institutionTypeId).Select(x => x.InstitutionType).FirstOrDefault();
          var objInstitutions = await db.institution.Where(x => x.Id == decryptInstitutionsId && !x.IsDeleted).FirstOrDefaultAsync();

          InstitutionViewModel beforeUpdateModel = GetInstitutionAuditModel(objInstitutions);
          ////string institutionSerialized = Helper.SerializeObjectTojson(objInstitutions);
          if (objInstitutions != null)
          {
            objInstitutions.CountryId = institutionModel.IsEuropeanUnion ? (int?)null : institutionTypeName == Helper.InstitutionTypes.Multilateral.ToString() ? (int?)null : institutionModel.CountryMasterHashId.ToDecrypt().ToInt32();
            objInstitutions.InstitutionTypeId = institutionTypeId;
            objInstitutions.InstitutionName = institutionModel.InstitutionsName;

            objInstitutions.IsEuropean = institutionModel.IsEuropeanUnion;
            objInstitutions.Modified = currentTimeStamp;
            objInstitutions.ModifiedBy = UserAccessHelper.CurrentUserIdentity.ToString();
            objInstitutions.EntityName = institutionModel.EntityName;
          }

          isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
          InstitutionViewModel afterUpdateModel = GetInstitutionAuditModel(objInstitutions);
          Task.Run(() => AuditRepository.WriteAudit<InstitutionViewModel>(AuditConstants.Institution, AuditType.Update, beforeUpdateModel, afterUpdateModel, AuditConstants.UpdateSuccessMsg));
        }
      }
      return isSave;
    }

    /// <summary>
    /// Delete Institution
    /// </summary>
    /// <param name="institutionMasterHashId">Institution id to Delete</param>
    /// <returns>return successfull message</returns>
    public async Task<bool> DeleteInstitution(string institutionMasterHashId)
    {
      bool isSave = false;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        ////DateTime currentTimeStamp = Helper.GetCurrentDateTime();
        if (!string.IsNullOrEmpty(institutionMasterHashId))
        {
          int decryptInstitutionId = institutionMasterHashId.ToDecrypt().ToInt32();

          var objInstitution = await db.institution.Where(x => x.Id == decryptInstitutionId && !x.IsDeleted).FirstOrDefaultAsync();

          if (objInstitution != null)
          {
            objInstitution.IsDeleted = true;
          }
          isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;

          InstitutionViewModel model = GetInstitutionAuditModel(objInstitution);
          model.ModifiedBy = UserAccessHelper.CurrentUserIdentity.ToString();

          Task.Run(() => AuditRepository.WriteAudit<InstitutionViewModel>(AuditConstants.Institution, AuditType.Delete, model, model, AuditConstants.DeleteSuccessMsg, UserAccessHelper.CurrentUserIdentity));
        }
      }
      return isSave;
    }

    private InstitutionViewModel GetInstitutionAuditModel(institution institutionModel)
    {
      InstitutionViewModel model = null;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (institutionModel.Id != 0)
        {
          var institutionObj = db.institution.Where(a => a.Id == institutionModel.Id).FirstOrDefault();
          model = new InstitutionViewModel()
          {
            InstitutionType = institutionObj.institutiontypes != null ? institutionObj.institutiontypes.InstitutionType : string.Empty,
            Country = institutionObj.country != null ? institutionObj.country.Name : string.Empty,
            Institution = institutionModel.InstitutionName,
            EuropeanUnion = institutionModel.IsEuropean,
            EntityName = institutionModel.EntityName,
            Created = institutionModel.Created.HasValue ? Helper.GetCurrentFormatedDateTime() : string.Empty,
            Modified = institutionModel.Modified.HasValue ? institutionModel.Modified.ToFormatedDateTime() : string.Empty,
            CreatedBy = institutionModel.CreatedBy,
            ModifiedBy = institutionModel.ModifiedBy
          };
        }
        else
        {
          model = new InstitutionViewModel()
          {
            InstitutionType = institutionModel.institutiontypes != null ? institutionModel.institutiontypes.InstitutionType : string.Empty,
            Country = institutionModel.country != null ? institutionModel.country.Name : string.Empty,
            Institution = institutionModel.InstitutionName,
            EuropeanUnion = institutionModel.IsEuropean,
            EntityName = institutionModel.EntityName,
            Created = institutionModel.Created.HasValue ? Helper.GetCurrentFormatedDateTime() : string.Empty,
            Modified = institutionModel.Modified.HasValue ? institutionModel.Modified.ToFormatedDateTime() : string.Empty,
            CreatedBy = institutionModel.CreatedBy,
            ModifiedBy = institutionModel.ModifiedBy
          };
        }
      }
      return model;
    }

    /// <summary>
    /// Get all the list of Institution
    /// </summary>
    /// <param name="parametersJson">Grid Parameter to filter or sorting</param>
    /// <returns>return the list</returns>
    public async Task<ApiOutput> GetAllInstitutionsList(GridParameters parametersJson)
    {
      ApiOutput apiOutput = new ApiOutput();
      List<InstitutionModel> institutionList;
      int totalRecord = 0;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        IQueryable<InstitutionModel> query = db.institution
          .Where(x => !x.IsDeleted)
            .Select(x => new InstitutionModel()
            {
              InstitutionMasterId = x.Id,
              CountryMasterId = x.CountryId.HasValue ? x.CountryId.Value : 0,
              InstitutionTypeMasterId = x.InstitutionTypeId,
              CountryName = x.country.Name,
              EntityName = x.EntityName,
              //IsEuropeanUnion = x.IsEuropean,
              IsEuropeanUnionString = x.IsEuropean ? "Yes" : "No",
              InstitutionType = x.institutiontypes.InstitutionType,
              InstitutionsName = string.IsNullOrEmpty(x.InstitutionName) ? "" : x.InstitutionName
            });
        if (parametersJson.Sort == null || parametersJson.Sort.Count == 0)
        {
          query = query.OrderByDescending(x => x.InstitutionMasterId);
        }
        institutionList = await query.ModifyList(parametersJson, out totalRecord).ToListAsync();
      }
      apiOutput.Data = institutionList;
      apiOutput.TotalRecords = totalRecord;
      return apiOutput;
    }

    /// <summary>
    /// Import the data's of Institutions
    /// </summary>
    /// <param name="institutionImportModelList"></param>
    /// <returns></returns>
    public async Task<bool> ImportInstitutionRecords(List<InstitutionCsvImportModel> institutionImportModelList)
    {
      bool isSave = false;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        var handleValidatedImportedUser = new List<Task>();

        handleValidatedImportedUser.Add(InsertNewImportedInstitution(db, institutionImportModelList));

        await Task.WhenAll(handleValidatedImportedUser);

        isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
      }
      return isSave;
    }

    /// <summary>
    /// Insert Import Data
    /// </summary>
    /// <param name="db">db object</param>
    /// <param name="institutionImportModelList">Institution Model object</param>
    /// <returns></returns>
    private async Task InsertNewImportedInstitution(BCMStrategyEntities db, List<InstitutionCsvImportModel> institutionImportModelList)
    {
      await Task.Yield();
      DateTime currentTimeStamp = Helper.GetCurrentDateTime();

      ////int? tempCountryId = !string.IsNullOrEmpty(x.CountryMasterHashId) ? x.CountryMasterHashId.ToDecrypt().ToInt32() : (int?)null;

      var listOfInsertedRecord = institutionImportModelList.Select(x => new institution()
      {
        CountryId = !x.IsEuropeanUnion ? (!string.IsNullOrEmpty(x.CountryMasterHashId) ? x.CountryMasterHashId.ToDecrypt().ToInt32() : (int?)null) : (int?)null,
        InstitutionTypeId = x.InstitutionTypeMasterHashId.ToDecrypt().ToInt32(),
        InstitutionName = x.InstitutionName,
        IsEuropean = x.IsEuropeanUnion,
        Created = currentTimeStamp,
        CreatedBy = UserAccessHelper.CurrentUserIdentity.ToString(),
        EntityName = x.EntityName
      });
      db.institution.AddRange(listOfInsertedRecord);

      foreach (institution institution in listOfInsertedRecord)
      {
        InstitutionViewModel institutionModel = GetInstitutionAuditModel(institution);

        Task.Run(() => AuditRepository.WriteAudit<InstitutionViewModel>(AuditConstants.Institution, AuditType.Insert, null, institutionModel, AuditConstants.ImportInsertSuccessMsg));
      }

    }

    public string GetInstitutionName(int instTypeId, int countryId)
    {
      string instTypeName = string.Empty;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        instTypeName = db.institution.Where(x => x.InstitutionTypeId == instTypeId && x.CountryId == countryId).Select(x => x.InstitutionName).FirstOrDefault();
      }
      return instTypeName;
    }

    public async Task<InstitutionModel> GetInstitutionByHashId(string institutionHashId)
    {
      InstitutionModel model = new InstitutionModel();
      int institutionDecryptId = institutionHashId.ToDecrypt().ToInt32();
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        var objInstitution = db.institution.Where(a => a.Id == institutionDecryptId).FirstOrDefault();
        if (objInstitution != null)
        {
          model.InstitutionMasterId = objInstitution.Id;
          model.CountryMasterId = objInstitution.CountryId.HasValue ? objInstitution.CountryId.Value : 0;
          model.InstitutionTypeMasterId = objInstitution.InstitutionTypeId;
          model.CountryName = objInstitution.CountryId.HasValue ? objInstitution.country.Name : string.Empty;
          model.EntityName = objInstitution.EntityName;
          model.IsEuropeanUnionString = objInstitution.IsEuropean ? Resources.Enums.Status.Yes.ToString() : Resources.Enums.Status.No.ToString();
          model.InstitutionType = objInstitution.institutiontypes.InstitutionType;
          model.InstitutionsName = !string.IsNullOrEmpty(objInstitution.InstitutionName) ? objInstitution.InstitutionName : string.Empty ;
        }
      }
      return model;
    }
  }
}