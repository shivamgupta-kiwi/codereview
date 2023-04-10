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
  public class ActivityTypeRepository : IActivityType
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
    /// Add and update of activity type
    /// </summary>
    /// <param name="activityTypeModel">activitytype Model with activitytype values</param>
    /// <returns>Is Saved or not</returns>
    public async Task<bool> UpdateActivityType(ActivityTypeModel activityTypeModel)
    {
      bool isSave = false;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        DateTime currentTimeStamp = Helper.GetCurrentDateTime();

        if (string.IsNullOrEmpty(activityTypeModel.ActivityTypeMasterHashId))
        {
          activitytype objActivityType = new activitytype()
          {
            ActivityName = activityTypeModel.ActivityName,
            Created = currentTimeStamp,
            CreatedBy = UserAccessHelper.CurrentUserIdentity.ToString(),
            metadatavalue = MetadataValueList(activityTypeModel),
            relatedactivitytypes = ConvertToRelatedActivityTypeList(activityTypeModel.RelatedActvityType),
						ColorCode =  activityTypeModel.ColorCode
          };
          db.activitytype.Add(objActivityType);
          isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
          ActivityTypeAuditViewModel model = GetActivityTypeAuditModel(objActivityType);
          Task.Run(() => AuditRepository.WriteAudit<ActivityTypeAuditViewModel>(AuditConstants.ActivityType, AuditType.Insert, null, model, AuditConstants.InsertSuccessMsg));
        }
        else
        {
          int decryptActivityTypeId = activityTypeModel.ActivityTypeMasterHashId.ToDecrypt().ToInt32();

          var objActivityType = await db.activitytype.Where(x => x.Id == decryptActivityTypeId && !x.IsDeleted).FirstOrDefaultAsync();
          ActivityTypeAuditViewModel beforeModel = GetActivityTypeAuditModel(objActivityType);
          if (objActivityType != null)
          {
						objActivityType.ColorCode = activityTypeModel.ColorCode;
            objActivityType.ActivityName = activityTypeModel.ActivityName;
            objActivityType.Modified = currentTimeStamp;
            objActivityType.ModifiedBy = UserAccessHelper.CurrentUserIdentity.ToString();
          }
          var ObjMetadataValue = await db.metadatavalue.Where(s => s.ActivityTypeId == decryptActivityTypeId).FirstOrDefaultAsync();
          if (ObjMetadataValue != null)
          {
            ObjMetadataValue.MetaDataTypeId = activityTypeModel.MetadataTypeMasterHashId.ToDecrypt().ToInt32();
            ObjMetadataValue.ActivityValue = Convert.ToDecimal(activityTypeModel.ActivityValue);
          }

          string[] relatedActivityTypeArray = activityTypeModel.RelatedActvityType.Split(',');

          var deleterelatedActivityType = objActivityType.relatedactivitytypes.Where(x => !relatedActivityTypeArray.Any(y => x.RelatedActvity == y));
          ////var deleteLexiconLink = objLexicon.lexiconissuelinker.Where(x => lexiconLinkersArray.Any(y => x.Linkers == y) == false);
          db.relatedactivitytypes.RemoveRange(deleterelatedActivityType);

          if (!string.IsNullOrEmpty(activityTypeModel.RelatedActvityType))
          {
            var newRelatedActivityTypeList = relatedActivityTypeArray.Where(x => !objActivityType.relatedactivitytypes.Any(y => x == y.RelatedActvity)).ToDBModelForActivityType(objActivityType.Id);
            ////var newLinkerList = lexiconLinkersArray.Where(x => objLexicon.lexiconissuelinker.Any(y => x == y.Linkers) == false).ToDBModel(objLexicon.Id);
            db.relatedactivitytypes.AddRange(newRelatedActivityTypeList);
          }

          isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
          ActivityTypeAuditViewModel afterModel = GetActivityTypeAuditModel(objActivityType);
          Task.Run(() => AuditRepository.WriteAudit<ActivityTypeAuditViewModel>(AuditConstants.ActivityType, AuditType.Update, beforeModel, afterModel, AuditConstants.UpdateSuccessMsg));
        }

      }
      return isSave;
    }

    private ActivityTypeAuditViewModel GetActivityTypeAuditModel(activitytype activitytypeModel)
    {
      ActivityTypeAuditViewModel model = null;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        string tempActionType = string.Empty;
        string tempWebSiteType = string.Empty;
        string actionType = tempActionType + " (" + tempWebSiteType + ")";

        var activityTypeObj = db.activitytype.Where(a => a.Id == activitytypeModel.Id).FirstOrDefault();
        if (activityTypeObj.metadatavalue.Any())
        {
          tempActionType = activityTypeObj.metadatavalue.FirstOrDefault().metadatatypes != null ? activityTypeObj.metadatavalue.FirstOrDefault().metadatatypes.MetaData : string.Empty;
          tempWebSiteType = activityTypeObj.metadatavalue.FirstOrDefault().metadatatypes != null ? activityTypeObj.metadatavalue.FirstOrDefault().metadatatypes.websitetypes.TypeName : string.Empty;
          actionType = tempActionType + " (" + tempWebSiteType + ")";
        }


        model = new ActivityTypeAuditViewModel()
        {
          ActionType = actionType,
          ActivityType = activityTypeObj.ActivityName,
          ActivityValue = activityTypeObj.metadatavalue.Any() ? activityTypeObj.metadatavalue.FirstOrDefault().ActivityValue.ToString() : string.Empty,
          Created = activityTypeObj.Created.HasValue ? Helper.GetCurrentFormatedDateTime() : string.Empty,
          Modified = activityTypeObj.Modified.HasValue ? activityTypeObj.Modified.ToFormatedDateTime() : string.Empty,
          CreatedBy = activityTypeObj.CreatedBy,
          ModifiedBy = activityTypeObj.ModifiedBy,
        };
      }
      return model;
    }


    private List<metadatavalue> MetadataValueList(ActivityTypeModel activityTypeModel)
    {
      List<metadatavalue> metaDataValuesList = new List<metadatavalue>();
      metadatavalue metaDataValue = new metadatavalue()
      {
        MetaDataTypeId = activityTypeModel.MetadataTypeMasterHashId.ToDecrypt().ToInt32(),
        ActivityValue = Convert.ToDecimal(activityTypeModel.ActivityValue)
      };
      metaDataValuesList.Add(metaDataValue);
      return metaDataValuesList;
    }
    /// <summary>
    /// Delete activity type
    /// </summary>
    /// <param name="activityTypeMasterHashId">activity type id to Delete</param>
    /// <returns>return successfull message</returns>
    public async Task<ApiOutput> DeleteActivityType(string activityTypeMasterHashId)
    {
      ApiOutput apiOutput = new ApiOutput();
      bool isSave = false;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {

        if (!string.IsNullOrEmpty(activityTypeMasterHashId))
        {
          int decrypAtactivityTypeId = activityTypeMasterHashId.ToDecrypt().ToInt32();

          var objActivityType = await db.activitytype.Where(x => x.Id == decrypAtactivityTypeId && !x.IsDeleted).FirstOrDefaultAsync();

          if (objActivityType != null)
          {

            if (!objActivityType.metadatanounplusverb.Any())
            {
              objActivityType.IsDeleted = true;
              isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
              apiOutput.Data = isSave;
              apiOutput.ErrorMessage = isSave ? Resources.Resource.ActivityTypeDeletedSuccessfully : Resources.Resource.ErrorWhileSaving;
            }
            else
            {
              isSave = false;
              apiOutput.Data = isSave;
              apiOutput.ErrorMessage = Resources.Resource.ValidationActivityTypeForDelete;
            }

          }
          ////isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
          ActivityTypeAuditViewModel model = GetActivityTypeAuditModel(objActivityType);
          Task.Run(() => AuditRepository.WriteAudit<ActivityTypeAuditViewModel>(AuditConstants.ActivityType, AuditType.Delete, model, null, AuditConstants.DeleteSuccessMsg));

        }
      }
      return apiOutput;
    }

    /// <summary>
    /// Get all the list Activity type
    /// </summary>
    /// <param name="parametersJson">Grid Parameter to filter or sorting</param>
    /// <returns>return the list</returns>
    public async Task<ApiOutput> GetAllActivityTypeList(GridParameters parametersJson)
    {
      ApiOutput apiOutput = new ApiOutput();
      List<ActivityTypeModel> activityTypeList;
      int totalRecord = 0;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        IQueryable<ActivityTypeModel> query = db.activitytype
          .Where(x => !x.IsDeleted)
            .Select(x => new ActivityTypeModel()
            {
              ActivityTypeMasterId = x.Id,
              WebsiteType = x.metadatavalue.Where(a => a.ActivityTypeId == x.Id).Select(z => z.metadatatypes.websitetypes.TypeName).FirstOrDefault(),
              MetaData = x.metadatavalue.Where(a => a.ActivityTypeId == x.Id && a.MetaDataTypeId == a.metadatatypes.Id).Select(b => b.metadatatypes.MetaData).FirstOrDefault(),
              MetadataTypeMasterId = x.metadatavalue.Where(s => s.ActivityTypeId == x.Id).Select(y => y.MetaDataTypeId).FirstOrDefault(),
              ActivityName = x.ActivityName,
              ActivityValue = x.metadatavalue.Where(m => m.ActivityTypeId == x.Id).Select(c => c.ActivityValue).FirstOrDefault().ToString(),
              RelatedActvityTypeList = x.relatedactivitytypes.Where(r => r.ActivityTypeId == x.Id).OrderBy(g => g.Id).Select(s => s.RelatedActvity).ToList(),
							ColorCode = x.ColorCode
            });
        if (parametersJson.Sort == null || parametersJson.Sort.Count == 0)
        {
          query = query.OrderByDescending(x => x.ActivityTypeMasterId);
        }
        activityTypeList = await query.ModifyList(parametersJson, out totalRecord).ToListAsync();
      }
      apiOutput.Data = activityTypeList;
      apiOutput.TotalRecords = totalRecord;
      return apiOutput;
    }

    public async Task<ApiOutput> GetDDActivityTypeList(string actionTypeMasterHashId)
    {
      ApiOutput apiOutput = new ApiOutput();
      List<DropdownMaster> activityTypeList;
      int decrypActionTypeId = actionTypeMasterHashId != null ? actionTypeMasterHashId.ToDecrypt().ToInt32() : 0;

      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        IQueryable<DropdownMaster> query = db.metadatavalue
            .Where(s => s.MetaDataTypeId == decrypActionTypeId && !s.activitytype.IsDeleted).Select(x => new DropdownMaster()
            {
              Key = x.activitytype.Id,
              Value = x.activitytype.ActivityName
            }).OrderBy(x => x.Value);
        activityTypeList = await query.ToListAsync();
      }
      apiOutput.Data = activityTypeList;
      apiOutput.TotalRecords = 0;
      apiOutput.ErrorMessage = activityTypeList.Any() ? string.Empty : string.Format(Resource.ValidateMessageNoDataFound, Resource.LblActivityType);
      return apiOutput;
    }

    public async Task<ActivityTypeModel> GetActivityTypeByHashId(string activityTypeHashId)
    {
      ActivityTypeModel model = new ActivityTypeModel();
      int activityTypeDecryptId = activityTypeHashId.ToDecrypt().ToInt32();
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        var objActivityType = db.activitytype.Where(a => a.Id == activityTypeDecryptId).FirstOrDefault();
        if (objActivityType != null)
        {
          model.ActivityTypeMasterId = objActivityType.Id;
          model.WebsiteType = objActivityType.metadatavalue.Where(a => a.ActivityTypeId == objActivityType.Id).Select(z => z.metadatatypes.websitetypes.TypeName).FirstOrDefault();
          model.MetaData = objActivityType.metadatavalue.Where(a => a.ActivityTypeId == objActivityType.Id && a.MetaDataTypeId == a.metadatatypes.Id).Select(b => b.metadatatypes.MetaData).FirstOrDefault();
          model.MetadataTypeMasterId = objActivityType.metadatavalue.Where(s => s.ActivityTypeId == objActivityType.Id).Select(y => y.MetaDataTypeId).FirstOrDefault();
          model.ActivityName = objActivityType.ActivityName;
          model.ActivityValue = objActivityType.metadatavalue.Where(m => m.ActivityTypeId == objActivityType.Id).Select(c => c.ActivityValue).FirstOrDefault().ToString();
          model.RelatedActvityTypeList = objActivityType.relatedactivitytypes.Where(r => r.ActivityTypeId == objActivityType.Id).OrderBy(g => g.Id).Select(s => s.RelatedActvity).ToList();
					model.ColorCode = objActivityType.ColorCode;
        }
      }
      return model;
    }

    private List<relatedactivitytypes> ConvertToRelatedActivityTypeList(string relatedActivityType)
    {
      List<relatedactivitytypes> relatedactivitytypesList = new List<relatedactivitytypes>();
      if (!string.IsNullOrEmpty(relatedActivityType))
      {
        relatedactivitytypesList = relatedActivityType.Split(',').Select(x => new relatedactivitytypes()
        {
          RelatedActvity = x
        }).ToList();
      }
      return relatedactivitytypesList;
    }
  }
}
