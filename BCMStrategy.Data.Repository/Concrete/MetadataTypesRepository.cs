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
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Repository.Concrete
{
  public class MetadataTypesRepository : IMetadataTypes
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
    public async Task<ApiOutput> GetDropdownMetadataTypesList()
    {
      ApiOutput apiOutput = new ApiOutput();
      List<DropdownMaster> metadaraTypesList;

      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        IQueryable<DropdownMaster> query = db.metadatatypes.Where(s => !s.IsDeleted)
            .Select(x => new DropdownMaster()
            {
              Key = x.Id,
              Value = x.MetaData + " (" + x.websitetypes.TypeName + ")"
            }).OrderBy(x => x.Value);
        metadaraTypesList = await query.ToListAsync();
      }
      apiOutput.Data = metadaraTypesList;
      apiOutput.TotalRecords = 0;
      apiOutput.ErrorMessage = metadaraTypesList.Any() ? string.Empty : string.Format(Resource.ValidateMessageNoDataFound, Resource.LblMetadataTypes);
      return apiOutput;
    }

    /// <summary>
    /// Get the Metadata Type list
    /// </summary>
    /// <returns>metadata Type list </returns>
    public async Task<ApiOutput> GetAllMetadataTypesList(GridParameters parametersJson)
    {
      ApiOutput apiOutput = new ApiOutput();
      List<MetadataTypesModel> metadataTypesList;
      int totalRecord = 0;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        IQueryable<MetadataTypesModel> query = db.metadatatypes
            .Where(x => !x.IsDeleted)
            .Select(x => new MetadataTypesModel()
            {
              MetadataTypesMasterId = x.Id,
              WebsiteTypeMasterId = x.WebSiteTypeId,
              WebsiteType = x.websitetypes.TypeName,
              MetaData = x.MetaData,
              MetaDataValue = x.Value.ToString() == "" || x.Value == null ? Resources.Enums.Status.NA.ToString() : x.Value.ToString(),
              IsActivityTypeExist = x.IsActivityTypeExist,
              Status = x.IsActivityTypeExist == true ? Resources.Enums.Status.Yes.ToString() : Enums.Status.No.ToString()
            });
        if (parametersJson.Sort == null || parametersJson.Sort.Count == 0)
        {
          query = query.OrderByDescending(x => x.MetadataTypesMasterId);
        }
        metadataTypesList = await query.ModifyList(parametersJson, out totalRecord).ToListAsync();
      }
      apiOutput.Data = metadataTypesList;
      apiOutput.TotalRecords = totalRecord;
      return apiOutput;
    }


    /// <summary>
    /// Add and update of Metadata types
    /// </summary>
    /// <param name="MetadataTypesModel">Metadata types Model with metadata types values</param>
    /// <returns>Is Saved or not</returns>
    public async Task<bool> UpdateMetadataTypes(MetadataTypesModel metadataTypesModel)
    {
      bool isSave = false;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        DateTime currentTimeStamp = Helper.GetCurrentDateTime();
        decimal? nullval = null;
        if (string.IsNullOrEmpty(metadataTypesModel.MetadataTypesMasterHashId))
        {
          metadatatypes objMetadataTypes = new metadatatypes()
          {
            WebSiteTypeId = metadataTypesModel.WebsiteTypeHashId.ToDecrypt().ToInt32(),
            MetaData = metadataTypesModel.MetaData,
            Value = metadataTypesModel.MetaDataValue != "" ? Convert.ToDecimal(metadataTypesModel.MetaDataValue) : nullval,
            IsActivityTypeExist = metadataTypesModel.IsActivityTypeExist,
            Created = currentTimeStamp,
            CreatedBy = UserAccessHelper.CurrentUserIdentity.ToString()
          };
          db.metadatatypes.Add(objMetadataTypes);
          isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
          ActionTypeAuditViewModel model = GetActionTypeAuditModel(objMetadataTypes);
          Task.Run(() => AuditRepository.WriteAudit<ActionTypeAuditViewModel>(AuditConstants.ActionType, AuditType.Insert, null, model, AuditConstants.InsertSuccessMsg));
        }
        else
        {

          int decryptMetadataTypesId = metadataTypesModel.MetadataTypesMasterHashId.ToDecrypt().ToInt32();

          var objMetadataTypes = await db.metadatatypes.Where(x => x.Id == decryptMetadataTypesId && !x.IsDeleted).FirstOrDefaultAsync();
          ActionTypeAuditViewModel beforeModel = GetActionTypeAuditModel(objMetadataTypes);
          if (objMetadataTypes != null)
          {
            objMetadataTypes.WebSiteTypeId = metadataTypesModel.WebsiteTypeHashId.ToDecrypt().ToInt32();
            objMetadataTypes.MetaData = metadataTypesModel.MetaData;
            objMetadataTypes.Value = metadataTypesModel.MetaDataValue != "" ? Convert.ToDecimal(metadataTypesModel.MetaDataValue) : nullval;
            objMetadataTypes.IsActivityTypeExist = metadataTypesModel.IsActivityTypeExist;
            objMetadataTypes.Modified = currentTimeStamp;
            objMetadataTypes.ModifiedBy = UserAccessHelper.CurrentUserIdentity.ToString();
          }
          isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
          ActionTypeAuditViewModel afterModel = GetActionTypeAuditModel(objMetadataTypes);
          Task.Run(() => AuditRepository.WriteAudit<ActionTypeAuditViewModel>(AuditConstants.ActionType, AuditType.Insert, beforeModel, afterModel, AuditConstants.UpdateSuccessMsg));
        }

      }
      return isSave;
    }

    /// <summary>
    /// Delete metadata Types
    /// </summary>
    /// <param name="metadataTypesMasterHashId">metadata types id to Delete</param>
    /// <returns>return successfull message</returns>
    public async Task<ApiOutput> DeleteMetadataTypes(string metadataTypesMasterHashId)
    {
      ApiOutput apiOutput = new ApiOutput();
      bool isSave = false;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {

        if (!string.IsNullOrEmpty(metadataTypesMasterHashId))
        {
          int decryptMetadataTypesId = metadataTypesMasterHashId.ToDecrypt().ToInt32();

          var objMetadataTypes = await db.metadatatypes.Where(x => x.Id == decryptMetadataTypesId && !x.IsDeleted).FirstOrDefaultAsync();

          if (objMetadataTypes != null)
          {
            if (!objMetadataTypes.metadatavalue.Any() && !objMetadataTypes.metadataphrases.Any() && !objMetadataTypes.weblinkproprietarytags.Any() && !objMetadataTypes.scrappedproprietorytagsmapping.Any() && !objMetadataTypes.metadatanounplusverb.Any())
            {
              objMetadataTypes.IsDeleted = true;
              isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
              apiOutput.Data = isSave;
              apiOutput.ErrorMessage = isSave ? Resources.Resource.MetadataTypesDeletedSuccessfully : Resources.Resource.ErrorWhileSaving;
              ActionTypeAuditViewModel model = GetActionTypeAuditModel(objMetadataTypes);
              Task.Run(() => AuditRepository.WriteAudit<ActionTypeAuditViewModel>(AuditConstants.ActionType, AuditType.Delete, model, null, AuditConstants.DeleteSuccessMsg));
            }
            else
            {
              isSave = false;
              apiOutput.Data = isSave;
              apiOutput.ErrorMessage = Resources.Resource.ValidationMetadataTypeForDelete;
            }

          }
        }
      }
      return apiOutput;
    }


    private ActionTypeAuditViewModel GetActionTypeAuditModel(metadatatypes metadatatypesModel)
    {
      ActionTypeAuditViewModel model = null;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        var metadatatypesObj = db.metadatatypes.Where(a => a.Id == metadatatypesModel.Id).FirstOrDefault();
        model = new ActionTypeAuditViewModel()
        {
          WebsiteType = metadatatypesObj.websitetypes != null ? metadatatypesObj.websitetypes.TypeName : string.Empty,
          ActionType = metadatatypesObj.MetaData,
          ActionTypeValue = metadatatypesObj.Value.HasValue ? metadatatypesObj.Value.ToString() : string.Empty,
          GranularActivityTypeExists = metadatatypesObj.IsActivityTypeExist.HasValue ? (bool)metadatatypesObj.IsActivityTypeExist : (bool?)null,
          Created = metadatatypesObj.Created.HasValue ? Helper.GetCurrentFormatedDateTime() : string.Empty,
          Modified = metadatatypesObj.Modified.HasValue ? metadatatypesObj.Modified.ToFormatedDateTime() : string.Empty,
          CreatedBy = metadatatypesObj.CreatedBy,
          ModifiedBy = metadatatypesObj.ModifiedBy
        };
      }
      return model;
    }

    /// <summary>
    /// Import the data's of metadataTypes
    /// </summary>
    /// <param name="ImportMetadataTypesRecords"></param>
    /// <returns></returns>
    public async Task<bool> ImportMetadataTypesRecords(List<MetadataTypesCsvImportModel> metadataTypesImportModelList)
    {
      bool isSave = false;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        var handleValidatedImportedUser = new List<Task>();

        handleValidatedImportedUser.Add(InsertNewImportedMetadataTypes(db, metadataTypesImportModelList));

        await Task.WhenAll(handleValidatedImportedUser);

        isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
      }
      return isSave;
    }

    /// <summary>
    /// Insert Import Data
    /// </summary>
    /// <param name="db">db object</param>
    /// <param name="metadataTypesImportModelList">Institution Model object</param>
    /// <returns></returns>
    private async Task InsertNewImportedMetadataTypes(BCMStrategyEntities db, List<MetadataTypesCsvImportModel> metadataTypesImportModelList)
    {
      await Task.Yield();
      DateTime currentTimeStamp = Helper.GetCurrentDateTime();

      var listOfInsertedRecord = metadataTypesImportModelList.Select(x => new metadatatypes()
      {
        WebSiteTypeId = x.WebsiteTypeHashId.ToDecrypt().ToInt32(),
        MetaData = x.MetaData,
        Value = Convert.ToDecimal(x.MetaDataValue),
        IsActivityTypeExist = x.IsActivityTypeExist,
        Created = currentTimeStamp,
        CreatedBy = UserAccessHelper.CurrentUserIdentity.ToString()
      });
      db.metadatatypes.AddRange(listOfInsertedRecord);
    }

    /// <summary>
    /// get metadata type data based on hash id
    /// </summary>
    /// <param name="metadataTypeHashId"></param>
    /// <returns></returns>
    public async Task<MetadataTypesModel> GetMetadataTypeByHashId(string metadataTypeHashId)
    {
      MetadataTypesModel model = new MetadataTypesModel();
      int metadataTypeDecryptId = metadataTypeHashId.ToDecrypt().ToInt32();
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        var objMetadataTypes = db.metadatatypes.Where(a => a.Id == metadataTypeDecryptId).FirstOrDefault();
        if (objMetadataTypes != null)
        {
          model.MetadataTypesMasterId = objMetadataTypes.Id;
          model.WebsiteTypeMasterId = objMetadataTypes.WebSiteTypeId;
          model.WebsiteType = objMetadataTypes.websitetypes.TypeName;
          model.MetaData = objMetadataTypes.MetaData;
          model.MetaDataValue = objMetadataTypes.Value.ToString() == "" || objMetadataTypes.Value == null ? Resources.Enums.Status.NA.ToString() : objMetadataTypes.Value.ToString();
          model.IsActivityTypeExist = objMetadataTypes.IsActivityTypeExist;
          model.Status = objMetadataTypes.IsActivityTypeExist == true ? Resources.Enums.Status.Yes.ToString() : Enums.Status.No.ToString();
        }
      }
      return model;
    }
  }
}
