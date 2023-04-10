using BCMStrategy.DAL.Context;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using BCMStrategy.Common.Kendo;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Resources;
using BCMStrategy.Common.AuditLog;
using BCMStrategy.Common.Unity;

namespace BCMStrategy.Data.Repository.Concrete
{
  public class InstitutionTypesRepository : IInstitutionTypes
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
    /// Insert or Update Institution Type
    /// </summary>
    /// <returns>Institution Type </returns>
    public async Task<bool> UpdateInstitutionTypes(InstitutionTypesModel institutionTypesModel)
    {
      bool isSave = false;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        DateTime currentTimeStamp = Helper.GetCurrentDateTime();

        if (string.IsNullOrEmpty(institutionTypesModel.InstitutionTypesHashId))
        {
          institutiontypes objInstitutionTypes = new institutiontypes()
          {
            InstitutionType = institutionTypesModel.InstitutionTypesName,
            Created = currentTimeStamp,
            CreatedBy = UserAccessHelper.CurrentUserIdentity.ToString()
          };
          db.institutiontypes.Add(objInstitutionTypes);
          isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
          InstitutionTypeAuditViewModel model = GetInstitutionTypeAuditModel(objInstitutionTypes);
          Task.Run(() => AuditRepository.WriteAudit<InstitutionTypeAuditViewModel>(AuditConstants.InstitutionType, AuditType.Insert, null, model, AuditConstants.InsertSuccessMsg));
        }
        else
        {
          int decryptInstitutionTypesId = institutionTypesModel.InstitutionTypesHashId.ToDecrypt().ToInt32();

          var objInstitutionTypes = await db.institutiontypes.Where(x => x.Id == decryptInstitutionTypesId).FirstOrDefaultAsync();
          InstitutionTypeAuditViewModel beforeUpdateModel = GetInstitutionTypeAuditModel(objInstitutionTypes);
          ////string serializeInstitutionType = Helper.SerializeObjectTojson(objInstitutionTypes);
          if (objInstitutionTypes != null)
          {

            objInstitutionTypes.InstitutionType = institutionTypesModel.InstitutionTypesName;
            objInstitutionTypes.Modified = currentTimeStamp;
            objInstitutionTypes.ModifiedBy = UserAccessHelper.CurrentUserIdentity.ToString();
          }
          
          isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
          InstitutionTypeAuditViewModel afterUpdateModel = GetInstitutionTypeAuditModel(objInstitutionTypes);
          Task.Run(() => AuditRepository.WriteAudit<InstitutionTypeAuditViewModel>(AuditConstants.InstitutionType, AuditType.Update, beforeUpdateModel, afterUpdateModel, AuditConstants.UpdateSuccessMsg));
        }
        
      }
      return isSave;
    }
    /// <summary>
    /// Delete the Institution Type
    /// </summary>
    /// <returns>Institution Type </returns>
    public async Task<ApiOutput> DeleteInstitutionTypes(string institutionTypesHashId)
    {
      ApiOutput apiOutput = new ApiOutput();
      bool isSave = false;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (!string.IsNullOrEmpty(institutionTypesHashId))
        {
          int decryptInstitutionTypesId = institutionTypesHashId.ToDecrypt().ToInt32();

          var objInstitutionTypes = await db.institutiontypes.Where(x => x.Id == decryptInstitutionTypesId && !x.IsDeleted).FirstOrDefaultAsync();

          if (objInstitutionTypes != null)
          {
            if (!objInstitutionTypes.institution.Any(x => !x.IsDeleted) && !objInstitutionTypes.policymakers.Any(x => !x.IsDeleted))
            {
              objInstitutionTypes.IsDeleted = true;
              isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
              apiOutput.Data = isSave;
              apiOutput.ErrorMessage = isSave ? Resources.Resource.InstitutionTypesDeletedSuccessfully : Resources.Resource.ErrorWhileSaving;
              InstitutionTypeAuditViewModel model = GetInstitutionTypeAuditModel(objInstitutionTypes);
              Task.Run(() => AuditRepository.WriteAudit<InstitutionTypeAuditViewModel>(AuditConstants.InstitutionType, AuditType.Delete, model, null, AuditConstants.DeleteSuccessMsg, UserAccessHelper.CurrentUserIdentity));
            }
            else {
              isSave = false;
              apiOutput.Data = isSave;
              apiOutput.ErrorMessage = Resources.Resource.ValidationInstitutionTypeForDelete;
            }
          }
        }
      }
      return apiOutput;
    }


    private InstitutionTypeAuditViewModel GetInstitutionTypeAuditModel(institutiontypes institutionTypeModel)
    {
      InstitutionTypeAuditViewModel model = null;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (institutionTypeModel.Id > 0)
        {
          var institutionTypeObj = db.institutiontypes.Where(a => a.Id == institutionTypeModel.Id).FirstOrDefault();
          model = new InstitutionTypeAuditViewModel()
          {
            InstitutionType = institutionTypeObj.institution != null ? institutionTypeObj.InstitutionType : string.Empty,
            Created = institutionTypeObj.Created.HasValue ? Helper.GetCurrentFormatedDateTime() : string.Empty,
            Modified = institutionTypeObj.Modified.HasValue ? institutionTypeObj.Modified.ToFormatedDateTime() : string.Empty,
            CreatedBy = institutionTypeObj.CreatedBy,
            ModifiedBy = institutionTypeObj.ModifiedBy
          };
        }
        else
        {
          model = new InstitutionTypeAuditViewModel()
          {
            InstitutionType = institutionTypeModel.InstitutionType,
            Created = institutionTypeModel.Created.HasValue ? Helper.GetCurrentFormatedDateTime() : string.Empty,
            Modified = institutionTypeModel.Modified.HasValue ? institutionTypeModel.Modified.ToFormatedDateTime() : string.Empty,
            CreatedBy = institutionTypeModel.CreatedBy,
            ModifiedBy = institutionTypeModel.ModifiedBy
          };
        }
      }
      return model;
    }

    /// <summary>
    /// Get the Institution Type list
    /// </summary>
    /// <returns>Institution Type list </returns>
    public async Task<ApiOutput> GetAllInstitutionTypesList(GridParameters parameters)
    {
      ApiOutput apiOutput = new ApiOutput();
      List<InstitutionTypesModel> institutionTypesList;
      int totalRecord = 0;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        IQueryable<InstitutionTypesModel> query = db.institutiontypes
            .Where(x => !x.IsDeleted)
            .Select(x => new InstitutionTypesModel()
            {
              InstitutionTypesId = x.Id,
              InstitutionTypesName = x.InstitutionType
            });
        if (parameters.Sort == null || parameters.Sort.Count == 0)
        {
          query = query.OrderByDescending(x => x.InstitutionTypesId);
        }
        institutionTypesList = await query.ModifyList(parameters, out totalRecord).ToListAsync();
      }
      apiOutput.Data = institutionTypesList;
      apiOutput.TotalRecords = totalRecord;
      return apiOutput;
    }
    public async Task<ApiOutput> GetDDLInstitutionTpeList()
    {
      ApiOutput apiOutput = new ApiOutput();
      List<DropdownMaster> institutionTypeList;

      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        IQueryable<DropdownMaster> query = db.institutiontypes
            .Where(x => !x.IsDeleted)
            .Select(x => new DropdownMaster()
            {
              Key = x.Id,
              Value = x.InstitutionType
            }).OrderBy(x => x.Value);
        institutionTypeList = await query.ToListAsync();
      }
      apiOutput.Data = institutionTypeList;
      apiOutput.TotalRecords = 0;
      apiOutput.ErrorMessage = institutionTypeList.Any() ? string.Empty : string.Format(Resource.ValidateMessageNoDataFound, Resource.LblCountry);
      return apiOutput;
    }
    /// <summary>
    /// Import all the records to Import
    /// </summary>
    /// <param name="institutionTypesImportModelList">List of Institution Types</param>
    /// <returns>return saved or not status</returns>
    public async Task<bool> ImportInstitutionTypeRecord(List<InstitutionTypesImportModel> institutionTypesImportModel)
    {
      bool isSave = false;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        var handleValidatedImportedUser = new List<Task>();

        handleValidatedImportedUser.Add(InsertNewImportedInstitutionType(db, institutionTypesImportModel));

        await Task.WhenAll(handleValidatedImportedUser);

        isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
      }
      return isSave;
    }

    /// <summary>
    /// Import Institution Types
    /// </summary>
    /// <param name="db"></param>
    /// <param name="institutionTypesImportModelList"></param>
    /// <returns></returns>
    private async Task InsertNewImportedInstitutionType(BCMStrategyEntities db, List<InstitutionTypesImportModel> institutionTypeImportModelList)
    {
      await Task.Yield();
      DateTime currentTimeStamp = Helper.GetCurrentDateTime();

      var listOfInsertedRecord = institutionTypeImportModelList.Select(x => new institutiontypes()
      {
        InstitutionType = x.InstitutionTypesName,
        Created = currentTimeStamp,
        CreatedBy = UserAccessHelper.CurrentUserIdentity.ToString()
      });

      db.institutiontypes.AddRange(listOfInsertedRecord);

      foreach (institutiontypes institutionViewModel in listOfInsertedRecord)
      {
        InstitutionTypeAuditViewModel model = GetInstitutionTypeAuditModel(institutionViewModel);

        Task.Run(() => AuditRepository.WriteAudit<InstitutionTypeAuditViewModel>(AuditConstants.InstitutionType, AuditType.Insert, null, model, AuditConstants.ImportInsertSuccessMsg));
      }
    }

    public async Task<InstitutionTypesModel> GetInstitutionTypeByHashId(string institutionTypeHashId)
    {
      InstitutionTypesModel model = new InstitutionTypesModel();
      int institutionTypeDecryptId = institutionTypeHashId.ToDecrypt().ToInt32();
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        var objInstitutionTypes = db.institutiontypes.Where(a => a.Id == institutionTypeDecryptId).FirstOrDefault();
        if (objInstitutionTypes != null)
        {
          model.InstitutionTypesId = objInstitutionTypes.Id;
          model.InstitutionTypesName = objInstitutionTypes.InstitutionType;
        }
      }
      return model;
    }
  }
}
