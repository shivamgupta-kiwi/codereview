using BCMStrategy.Common.Kendo;
using BCMStrategy.Common.Unity;
using BCMStrategy.DAL.Context;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Resources;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using BCMStrategy.Common.AuditLog;

namespace BCMStrategy.Data.Repository.Concrete
{
	public class MetadataNounplusVerbRepository : IMetadataNounplusVerb
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

		public async Task<ApiOutput> GetDropdownMetadataDynamicNounplusVerbList()
		{
			ApiOutput apiOutput = new ApiOutput();
			List<DropdownMaster> DynamicNounplusverbList;

			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{
				IQueryable<DropdownMaster> query = db.dynamictablesfornounplusverb
						.Select(x => new DropdownMaster()
						{
							Key = x.Id,
							Value = x.DynamicType
						}).OrderBy(x => x.Value);
				DynamicNounplusverbList = await query.ToListAsync();
			}
			apiOutput.Data = DynamicNounplusverbList;
			apiOutput.TotalRecords = 0;
			apiOutput.ErrorMessage = DynamicNounplusverbList.Any() ? string.Empty : string.Format(Resource.ValidateMessageNoDataFound, Resource.LblMetadataNoun);
			return apiOutput;
		}

		public async Task<bool> UpdateMetadataNounplusVerb(MetadataNounplusVerbModel metadataNounplusVerbModel)
		{
			bool isSave = false;
			int? nullval = null;
			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{
				DateTime currentTimeStamp = Helper.GetCurrentDateTime();

				if (string.IsNullOrEmpty(metadataNounplusVerbModel.MetadataNounplusVerbMasterHashId))
				{
					metadatanounplusverb objMetadataNounplusVerb = new metadatanounplusverb()
					{
						MetaDataTypeId = metadataNounplusVerbModel.MetadataTypeMasterHashId.ToDecrypt().ToInt32(),
						Noun = !metadataNounplusVerbModel.IsHardCoded ? metadataNounplusVerbModel.MetadataDynamicNounplusVerb : metadataNounplusVerbModel.Noun,
						Verb = metadataNounplusVerbModel.Verb,
						IsHardCoded = metadataNounplusVerbModel.IsHardCoded,
						ActivityTypeId = metadataNounplusVerbModel.ActivityTypeMasterHashId != "" ? metadataNounplusVerbModel.ActivityTypeMasterHashId.ToDecrypt().ToInt32() : nullval,
						Created = currentTimeStamp,
						CreatedBy = UserAccessHelper.CurrentUserIdentity.ToString()
					};
					db.metadatanounplusverb.Add(objMetadataNounplusVerb);
					isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
					ActionNounPlusVerbAuditViewModel model = GetActionNounPlusVerbAuditViewModel(objMetadataNounplusVerb);
					Task.Run(() => AuditRepository.WriteAudit<ActionNounPlusVerbAuditViewModel>(AuditConstants.ActionNounPlusVerb, AuditType.Insert, null, model, AuditConstants.InsertSuccessMsg));
				}
				else
				{
					int decryptMetadataNounplusVerbId = metadataNounplusVerbModel.MetadataNounplusVerbMasterHashId.ToDecrypt().ToInt32();

					var objMetadataNounplusVerb = await db.metadatanounplusverb.Where(x => x.Id == decryptMetadataNounplusVerbId && !x.IsDeleted).FirstOrDefaultAsync();
					ActionNounPlusVerbAuditViewModel beforeModel = GetActionNounPlusVerbAuditViewModel(objMetadataNounplusVerb);
          ////int nullval = null;
					if (objMetadataNounplusVerb != null)
					{
						objMetadataNounplusVerb.MetaDataTypeId = metadataNounplusVerbModel.MetadataTypeMasterHashId.ToDecrypt().ToInt32();
						objMetadataNounplusVerb.Noun = !metadataNounplusVerbModel.IsHardCoded ? metadataNounplusVerbModel.MetadataDynamicNounplusVerb : metadataNounplusVerbModel.Noun;
						objMetadataNounplusVerb.Verb = metadataNounplusVerbModel.Verb;
						objMetadataNounplusVerb.IsHardCoded = metadataNounplusVerbModel.IsHardCoded;
						objMetadataNounplusVerb.ActivityTypeId = metadataNounplusVerbModel.ActivityTypeMasterHashId != "" ? metadataNounplusVerbModel.ActivityTypeMasterHashId.ToDecrypt().ToInt32() : nullval;
						objMetadataNounplusVerb.Modified = currentTimeStamp;
						objMetadataNounplusVerb.ModifiedBy = UserAccessHelper.CurrentUserIdentity.ToString();
					}
					isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
					ActionNounPlusVerbAuditViewModel afterModel = GetActionNounPlusVerbAuditViewModel(objMetadataNounplusVerb);
					Task.Run(() => AuditRepository.WriteAudit<ActionNounPlusVerbAuditViewModel>(AuditConstants.ActionNounPlusVerb, AuditType.Update, beforeModel, afterModel, AuditConstants.UpdateSuccessMsg));

				}
			}
			return isSave;
		}

		/// <summary>
		/// Delete metadataNounplusVerb
		/// </summary>
		/// <param name="metadataNounplusVerbMasterHashId">NounplusVerb id to Delete</param>
		/// <returns>return successfull message</returns>
		public async Task<bool> DeleteMetadataNounplusVerb(string metadataNounplusVerbMasterHashId)
		{
			bool isSave = false;
			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{
				if (!string.IsNullOrEmpty(metadataNounplusVerbMasterHashId))
				{
					int decrypMetadataNounplusVerbId = metadataNounplusVerbMasterHashId.ToDecrypt().ToInt32();

					var objMetadataNounplusVerb = await db.metadatanounplusverb.Where(x => x.Id == decrypMetadataNounplusVerbId && !x.IsDeleted).FirstOrDefaultAsync();

					if (objMetadataNounplusVerb != null)
					{
						objMetadataNounplusVerb.IsDeleted = true;
					}

					isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
					ActionNounPlusVerbAuditViewModel model = GetActionNounPlusVerbAuditViewModel(objMetadataNounplusVerb);
					Task.Run(() => AuditRepository.WriteAudit<ActionNounPlusVerbAuditViewModel>(AuditConstants.ActionNounPlusVerb, AuditType.Delete, model, null, AuditConstants.DeleteSuccessMsg));
				}
			}
			return isSave;
		}

		private ActionNounPlusVerbAuditViewModel GetActionNounPlusVerbAuditViewModel(metadatanounplusverb metadatanounplusverbModel)
		{
			ActionNounPlusVerbAuditViewModel model = null;
			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{
				var nounPhraseObj = db.metadatanounplusverb.Where(a => a.Id == metadatanounplusverbModel.Id).FirstOrDefault();
				model = new ActionNounPlusVerbAuditViewModel()
				{
					ActionType = nounPhraseObj.metadatatypes != null ? nounPhraseObj.metadatatypes.MetaData : string.Empty,
					HardCoded = nounPhraseObj.IsHardCoded,
					Noun = nounPhraseObj.Noun,
					Verb = nounPhraseObj.Verb,
					Created = nounPhraseObj.Created.HasValue ? Helper.GetCurrentFormatedDateTime() : string.Empty,
					Modified = nounPhraseObj.Modified.HasValue ? nounPhraseObj.Modified.ToFormatedDateTime() : string.Empty,
					CreatedBy = nounPhraseObj.CreatedBy,
					ModifiedBy = nounPhraseObj.ModifiedBy
				};
			}
			return model;
		}

		/// <summary>
		/// Get all the list Metadata noun + verb
		/// </summary>
		/// <param name="parametersJson">Grid Parameter to filter or sorting</param>
		/// <returns>return the list</returns>
		public async Task<ApiOutput> GetAllMetadataNounplusVerbList(GridParameters parametersJson)
		{
			ApiOutput apiOutput = new ApiOutput();
			List<MetadataNounplusVerbModel> metadataNounplusVerbList;
			int totalRecord = 0;
			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{
				IQueryable<MetadataNounplusVerbModel> query = db.metadatanounplusverb
						.Where(x => !x.IsDeleted)
						.Select(x => new MetadataNounplusVerbModel()
						{
							MetadataNounplusVerbMasterId = x.Id,
							WebsiteType = x.metadatatypes.websitetypes.TypeName,
							MetaData = x.metadatatypes.MetaData,
							MetadataTypeMasterId = x.MetaDataTypeId,
							Noun = x.Noun,
							Verb = x.Verb,
							IsHardCoded = x.IsHardCoded,
							ActivityTypeMasterId = x.ActivityTypeId != null ? x.ActivityTypeId.Value : 0,
							ActivityType = x.activitytype.ActivityName,
							ActivityValue = x.ActivityTypeId.HasValue ? x.activitytype.metadatavalue.Where(s => s.ActivityTypeId == x.ActivityTypeId).Select(s => s.ActivityValue.ToString()).FirstOrDefault() : x.metadatatypes.Value.ToString(),
							Status = x.IsHardCoded ? Resources.Enums.Status.Yes.ToString() : Enums.Status.No.ToString()
						});
				if (parametersJson.Sort == null || parametersJson.Sort.Count == 0)
				{
					query = query.OrderByDescending(x => x.MetadataNounplusVerbMasterId);
				}
				metadataNounplusVerbList = await query.ModifyList(parametersJson, out totalRecord).ToListAsync();
			}
			apiOutput.Data = metadataNounplusVerbList;
			apiOutput.TotalRecords = totalRecord;
			return apiOutput;
		}

		public async Task<MetadataNounplusVerbModel> GetMetadataNounPlusVerbByHashId(string nounVerbHashId)
		{
			MetadataNounplusVerbModel model = new MetadataNounplusVerbModel();
			int metadataNounPlusverbDecryptId = nounVerbHashId.ToDecrypt().ToInt32();
			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{
				var objMetadataNounPlusVerb = db.metadatanounplusverb.Where(a => a.Id == metadataNounPlusverbDecryptId).FirstOrDefault();
				if (objMetadataNounPlusVerb != null)
				{
					model.MetadataNounplusVerbMasterId = objMetadataNounPlusVerb.Id;
					model.WebsiteType = objMetadataNounPlusVerb.metadatatypes.websitetypes.TypeName;
					model.MetaData = objMetadataNounPlusVerb.metadatatypes.MetaData;
					model.MetadataTypeMasterId = objMetadataNounPlusVerb.MetaDataTypeId;
					model.Noun = objMetadataNounPlusVerb.Noun;
					model.Verb = objMetadataNounPlusVerb.Verb;
					model.IsHardCoded = objMetadataNounPlusVerb.IsHardCoded;
					model.ActivityTypeMasterId = objMetadataNounPlusVerb.ActivityTypeId != null ? objMetadataNounPlusVerb.ActivityTypeId.Value : 0;
					model.ActivityType = objMetadataNounPlusVerb.ActivityTypeId.HasValue ? objMetadataNounPlusVerb.activitytype.ActivityName : string.Empty;
					model.ActivityValue = objMetadataNounPlusVerb.ActivityTypeId.HasValue ? objMetadataNounPlusVerb.activitytype.metadatavalue.Where(s => s.ActivityTypeId == objMetadataNounPlusVerb.ActivityTypeId).Select(s => s.ActivityValue.ToString()).FirstOrDefault() : objMetadataNounPlusVerb.metadatatypes.Value.ToString();
					model.Status = objMetadataNounPlusVerb.IsHardCoded ? Resources.Enums.Status.Yes.ToString() : Enums.Status.No.ToString();
				}
			}
			return model;
		}
	}
}