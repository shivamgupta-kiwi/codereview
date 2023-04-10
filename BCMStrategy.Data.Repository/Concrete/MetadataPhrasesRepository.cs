using BCMStrategy.Common.AuditLog;
using BCMStrategy.Common.Kendo;
using BCMStrategy.Common.Unity;
using BCMStrategy.DAL.Context;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Logger;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Repository.Concrete
{
  public class MetadataPhrasesRepository : IMetadataPhrases
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
    /// Add and update of metadata phrases
    /// </summary>
    /// <param name="metadataPhrasesModel">MetadataPhrases Model with MetadataPhrases values</param>
    /// <returns>Is Saved or not</returns>
    public async Task<bool> UpdateMetadataPhrases(MetadataPhrasesModel metadataPhrasesModel)
    {
      bool isSave = false;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        DateTime currentTimeStamp = Helper.GetCurrentDateTime();
				int? nullval = null;
				if (string.IsNullOrEmpty(metadataPhrasesModel.MetadataPhrasesMasterHashId))
        {
          metadataphrases objMetadataPhrases = new metadataphrases()
          {
            MetaDataTypeId = metadataPhrasesModel.MetadataTypeMasterHashId.ToDecrypt().ToInt32(),
						ActivityTypeId = metadataPhrasesModel.ActivityTypeMasterHashId != "" ? metadataPhrasesModel.ActivityTypeMasterHashId.ToDecrypt().ToInt32() : nullval,
						Phrases = metadataPhrasesModel.MetadataPhrases,
            Created = currentTimeStamp,
            CreatedBy = UserAccessHelper.CurrentUserIdentity.ToString(),
          };
          db.metadataphrases.Add(objMetadataPhrases);
          isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
          PhrasesAuditViewModel model = GetPhrasesAuditModel(objMetadataPhrases);
          Task.Run(() => AuditRepository.WriteAudit<PhrasesAuditViewModel>(AuditConstants.Phrases, AuditType.Insert, null, model, AuditConstants.InsertSuccessMsg));
        }
        else
        {
          int decryptMetadataPhrasesId = metadataPhrasesModel.MetadataPhrasesMasterHashId.ToDecrypt().ToInt32();

          var objMetadataPhrases = await db.metadataphrases.Where(x => x.Id == decryptMetadataPhrasesId && !x.IsDeleted).FirstOrDefaultAsync();
          PhrasesAuditViewModel beforeModel = GetPhrasesAuditModel(objMetadataPhrases);
          if (objMetadataPhrases != null)
          {
            objMetadataPhrases.MetaDataTypeId = metadataPhrasesModel.MetadataTypeMasterHashId.ToDecrypt().ToInt32();
						objMetadataPhrases.ActivityTypeId = metadataPhrasesModel.ActivityTypeMasterHashId != "" ? metadataPhrasesModel.ActivityTypeMasterHashId.ToDecrypt().ToInt32() : nullval;
            objMetadataPhrases.Phrases = metadataPhrasesModel.MetadataPhrases;
            objMetadataPhrases.Modified = currentTimeStamp;
            objMetadataPhrases.ModifiedBy = UserAccessHelper.CurrentUserIdentity.ToString();
          }
          isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
          PhrasesAuditViewModel afterModel = GetPhrasesAuditModel(objMetadataPhrases);
          Task.Run(() => AuditRepository.WriteAudit<PhrasesAuditViewModel>(AuditConstants.Phrases, AuditType.Update, beforeModel, afterModel, AuditConstants.UpdateSuccessMsg));
        }

      }
      return isSave;
    }
    /// <summary>
    /// Delete metadata Phrases
    /// </summary>
    /// <param name="metadataPhrasesMasterHashId">metadataPhrases id to Delete</param>
    /// <returns>return successfull message</returns>
    public async Task<bool> DeleteMetadataPhrases(string metadataPhrasesMasterHashId)
    {
      bool isSave = false;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (!string.IsNullOrEmpty(metadataPhrasesMasterHashId))
        {
          int decrypMetadataPhrasesId = metadataPhrasesMasterHashId.ToDecrypt().ToInt32();

          var objMetadataPhrases = await db.metadataphrases.Where(x => x.Id == decrypMetadataPhrasesId && !x.IsDeleted).FirstOrDefaultAsync();

          if (objMetadataPhrases != null)
          {
            objMetadataPhrases.IsDeleted = true;
          }
          isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
          PhrasesAuditViewModel model = GetPhrasesAuditModel(objMetadataPhrases);
          Task.Run(() => AuditRepository.WriteAudit<PhrasesAuditViewModel>(AuditConstants.Phrases, AuditType.Delete, model, null, AuditConstants.DeleteSuccessMsg));
        }
      }
      return isSave;
    }


    private PhrasesAuditViewModel GetPhrasesAuditModel(metadataphrases phrasesModel)
    {
      PhrasesAuditViewModel model = null;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        var phrasesObj = db.metadataphrases.Where(a => a.Id == phrasesModel.Id).FirstOrDefault();
        model = new PhrasesAuditViewModel()
        {
          Phrases = phrasesObj.Phrases,
          ActionType = phrasesObj.metadatatypes != null ? phrasesObj.metadatatypes.MetaData : string.Empty,
					ActivityType = phrasesObj.activitytype != null ? phrasesObj.activitytype.ActivityName : string.Empty,
					Created = phrasesObj.Created.HasValue ? Helper.GetCurrentFormatedDateTime() : string.Empty,
          Modified = phrasesObj.Modified.HasValue ? phrasesObj.Modified.ToFormatedDateTime() : string.Empty,
          CreatedBy = phrasesObj.CreatedBy,
          ModifiedBy = phrasesObj.ModifiedBy
        };
      }
      return model;
    }

    /// <summary>
    /// Get all the list Metadata Phrases
    /// </summary>
    /// <param name="parametersJson">Grid Parameter to filter or sorting</param>
    /// <returns>return the list</returns>
    public async Task<ApiOutput> GetAllMetadataPhrasesList(GridParameters parametersJson)
    {
      ApiOutput apiOutput = new ApiOutput();
      List<MetadataPhrasesModel> metadataPhrasesList;
      int totalRecord = 0;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        IQueryable<MetadataPhrasesModel> query = db.metadataphrases
            .Where(x => !x.IsDeleted)
            .Select(x => new MetadataPhrasesModel()
            {
              MetadataPhrasesMasterId = x.Id,
              MetadataTypeMasterId = x.MetaDataTypeId,
							ActivityTypeMasterId = x.ActivityTypeId != null ? x.ActivityTypeId.Value : 0,
              ActivityType = x.activitytype.ActivityName,
              ActivityValue = x.ActivityTypeId.HasValue ? x.activitytype.metadatavalue.Where(s => s.ActivityTypeId == x.ActivityTypeId).Select(s => s.ActivityValue.ToString()).FirstOrDefault() : x.metadatatypes.Value.ToString(),
							MetadataPhrases = x.Phrases,
              MetaData = x.metadatatypes.MetaData,
              WebsiteType = x.metadatatypes.websitetypes.TypeName
            });
        if (parametersJson.Sort == null || parametersJson.Sort.Count == 0)
        {
          query = query.OrderByDescending(x => x.MetadataPhrasesMasterId);
        }
        metadataPhrasesList = await query.ModifyList(parametersJson, out totalRecord).ToListAsync();
      }
      apiOutput.Data = metadataPhrasesList;
      apiOutput.TotalRecords = totalRecord;
      return apiOutput;
    }

    public async Task<MetadataPhrasesModel> GetMetadataPhrasesByHashId(string phrasesHashId)
    {
      MetadataPhrasesModel model = new MetadataPhrasesModel();
      int metadataPhrasesDecryptId = phrasesHashId.ToDecrypt().ToInt32();
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        var objMetadataPhrases = db.metadataphrases.Where(a => a.Id == metadataPhrasesDecryptId).FirstOrDefault();
        if (objMetadataPhrases != null)
        {
          model.MetadataPhrasesMasterId = objMetadataPhrases.Id;
          model.MetadataTypeMasterId = objMetadataPhrases.MetaDataTypeId;
          model.ActivityTypeMasterId = objMetadataPhrases.ActivityTypeId != null ? objMetadataPhrases.ActivityTypeId.Value : 0;
          model.MetadataPhrases = objMetadataPhrases.Phrases;
          model.ActivityType = objMetadataPhrases.ActivityTypeId.HasValue ? objMetadataPhrases.activitytype.ActivityName : string.Empty;
          model.ActivityValue = objMetadataPhrases.ActivityTypeId.HasValue ? objMetadataPhrases.activitytype.metadatavalue.Where(s => s.ActivityTypeId == objMetadataPhrases.ActivityTypeId).Select(s => s.ActivityValue.ToString()).FirstOrDefault() : objMetadataPhrases.metadatatypes.Value.ToString();
          model.MetaData = objMetadataPhrases.metadatatypes.MetaData;
          model.WebsiteType = objMetadataPhrases.metadatatypes.websitetypes.TypeName;
        }
      }
      return model;
    }

  }
}
