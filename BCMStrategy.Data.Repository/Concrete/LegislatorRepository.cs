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
using System.Text;
using System.Threading.Tasks;
using DALContext = BCMStrategy.DAL.Context;

namespace BCMStrategy.Data.Repository.Concrete
{
    public class LegislatorRepository : ILegislator
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
        /// Get All Legislator DDL
        /// </summary>
        /// <returns></returns>
       public async Task<LegislatorViewModel> GetAllLegislatorPageDDL(bool isEdit = false)
        {
            LegislatorViewModel model = new LegislatorViewModel();
            using (DALContext.BCMStrategyEntities db = new DALContext.BCMStrategyEntities())
            {
                if (!isEdit)
                {
                    model.EntityDDL = await db.euentities.Select(a => new DropdownMaster()
                    {
                        Key = a.Id,
                        Value = a.EntityName
                    }).ToListAsync();

                    model.SectorDDL = await db.sector.Select(a => new DropdownMaster()
                    {
                        Key = a.Id,
                        Value = a.SectorName
                    }).ToListAsync();
                }

                model.DesignationDDL = await db.designation.Select(a => new DropdownMaster()
                {
                    Key = a.Id,
                    Value = a.Designation1
                }).ToListAsync();

                model.CommiteeDDL = await db.committee.Select(a => new DropdownMaster()
                {
                    Key = a.Id,
                    Value = a.CommitteeName
                }).ToListAsync();
            }
            return model;
        }

        /// <summary>
       ///  Used to get All Legislator
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
       public async Task<ApiOutput> GetAllLegislatorList(GridParameters parametersJson)
       {
           ApiOutput apiOutput = new ApiOutput();
           List<LegislatorViewModel> list;
           int totalRecord = 0;
           using (BCMStrategyEntities db = new BCMStrategyEntities())
           {
               IQueryable<LegislatorViewModel> query = db.individuallegislators.Where(a => a.IsDeleted != Helper.saveChangesSuccessful)
                   .Select(x => new LegislatorViewModel()
                   {
                       LegislatorMasterId = x.Id,
                       FirstName = x.FirstName,
                       LastName = x.LastName,
                       Entity = x.euentities != null ? x.euentities.EntityName : string.Empty,
                       Sector = x.sector != null ? x.sector.SectorName : string.Empty,
                       Country = x.country != null ? x.country.Name : string.Empty,
                       CreatedDate = x.Created
                   });
               if (parametersJson.Sort == null || parametersJson.Sort.Count == 0)
               {
                   query = query.OrderByDescending(x => x.CreatedDate);
               }
               list = await query.ModifyList(parametersJson, out totalRecord).ToListAsync();
           }
           apiOutput.Data = list;
           apiOutput.TotalRecords = totalRecord;
           return apiOutput;
       }

        /// <summary>
       /// Update Legislator
        /// </summary>
        /// <param name="webLinkViewModel"></param>
        /// <returns></returns>
       public async Task<bool> UpdateLegislator(LegislatorViewModel lagislatorViewModel)
       {
           using (DALContext.BCMStrategyEntities db = new DALContext.BCMStrategyEntities())
           {
               #region Get List Of Ids
               List<int> designationIds = lagislatorViewModel.DesignationHashIds.Any() ? lagislatorViewModel.DesignationHashIds.Select(a => a.ToDecrypt().ToInt32()).ToList() : new List<int>();
               List<int> commiteeIds = lagislatorViewModel.CommiteeHashIds.Any() ? lagislatorViewModel.CommiteeHashIds.Select(a => a.ToDecrypt().ToInt32()).ToList() : new List<int>();
               #endregion

               if (!string.IsNullOrEmpty(lagislatorViewModel.LegislatorHashId))
               {
                   #region Update
                   int legislatorId = lagislatorViewModel.LegislatorHashId.ToDecrypt().ToInt32();
                   individuallegislators dbLegislators = db.individuallegislators.Where(a => a.Id == legislatorId).FirstOrDefault();
                   LegislatorAuditViewModel beforeModel = GetLegislatorAuditModel(dbLegislators);
                   dbLegislators.FirstName = lagislatorViewModel.FirstName;
                   dbLegislators.LastName = lagislatorViewModel.LastName;
                   dbLegislators.SectorId = !string.IsNullOrEmpty(lagislatorViewModel.SectorHashId) ? lagislatorViewModel.SectorHashId.ToDecrypt().ToInt32() : (int?)null;
                   dbLegislators.CountryId = !string.IsNullOrEmpty(lagislatorViewModel.CountryHashId) ? lagislatorViewModel.CountryHashId.ToDecrypt().ToInt32() : (int?)null;
                   dbLegislators.EntityId = !string.IsNullOrEmpty(lagislatorViewModel.EntityHashId) ? lagislatorViewModel.EntityHashId.ToDecrypt().ToInt32() : (int?)null;
                   dbLegislators.Modified = DateTime.Now;
                   dbLegislators.ModifiedBy = UserAccessHelper.CurrentUserIdentity.ToString();
                   #region Designation
                   List<int> dbDesignationIds = dbLegislators.individuallegislatorsdesignation.Select(a => a.DesignationId).ToList();
                   List<int> dbDesignationIdsToRemove = dbDesignationIds.Where(a => !designationIds.Contains(a)).ToList();
                   List<int> newDesignationIdsToAdd = designationIds.Where(a => !dbDesignationIds.Contains(a)).ToList();
                   List<individuallegislatorsdesignation> designationToRemove = dbLegislators.individuallegislatorsdesignation.Where(a => dbDesignationIdsToRemove.Contains(a.DesignationId)).ToList();
                   db.individuallegislatorsdesignation.RemoveRange(designationToRemove);

                   List<individuallegislatorsdesignation> webLinkDesignationToAdd = newDesignationIdsToAdd.Select(id => new individuallegislatorsdesignation()
                   {
                       DesignationId = id,
                       IndividualLegislatorsId = legislatorId
                   }).ToList();

                   db.individuallegislatorsdesignation.AddRange(webLinkDesignationToAdd);
                   #endregion Designation

                   #region Commitee
                   List<int> dbCommiteeIds = dbLegislators.individuallegislatorscommittee.Select(a => a.CommitteeId).ToList();
                   List<int> dbCommiteeIdsToRemove = dbCommiteeIds.Where(a => !commiteeIds.Contains(a)).ToList();
                   List<int> newCommiteeIdsToAdd = commiteeIds.Where(a => !dbCommiteeIds.Contains(a)).ToList();
                   List<individuallegislatorscommittee> commiteeToRemove = dbLegislators.individuallegislatorscommittee.Where(a => dbCommiteeIdsToRemove.Contains(a.CommitteeId)).ToList();
                   db.individuallegislatorscommittee.RemoveRange(commiteeToRemove);

                   List<individuallegislatorscommittee> commiteeToAdd = newCommiteeIdsToAdd.Select(id => new individuallegislatorscommittee()
                   {
                       CommitteeId = id,
                       IndividualLegislatorsId = legislatorId
                   }).ToList();

                   db.individuallegislatorscommittee.AddRange(commiteeToAdd);
                   #endregion Commitee
                   
                   db.SaveChanges();
                   LegislatorAuditViewModel afterModel = GetLegislatorAuditModel(dbLegislators);
                   Task.Run(() => AuditRepository.WriteAudit<LegislatorAuditViewModel>(AuditConstants.IndividualLegislator, AuditType.Update, beforeModel, afterModel, AuditConstants.UpdateSuccessMsg));
                   #endregion
               }
               else
               {
                   #region ADD
                   individuallegislators lagislatorObj = new individuallegislators()
                             {
                                 FirstName = lagislatorViewModel.FirstName,
                                 LastName = lagislatorViewModel.LastName,
                                 SectorId = !string.IsNullOrEmpty(lagislatorViewModel.SectorHashId) ? lagislatorViewModel.SectorHashId.ToDecrypt().ToInt32() : (int?)null,
                                 CountryId = !string.IsNullOrEmpty(lagislatorViewModel.CountryHashId) ? lagislatorViewModel.CountryHashId.ToDecrypt().ToInt32() : (int?)null,
                                 EntityId = !string.IsNullOrEmpty(lagislatorViewModel.EntityHashId) ? lagislatorViewModel.EntityHashId.ToDecrypt().ToInt32() : (int?)null,
                                 Created = DateTime.Now,
                                 CreatedBy = UserAccessHelper.CurrentUserIdentity.ToString(),
                                 individuallegislatorsdesignation = designationIds.Select(id => new individuallegislatorsdesignation()
                                 {
                                     DesignationId = id
                                 }).ToList(),
                                 individuallegislatorscommittee = commiteeIds.Select(id => new individuallegislatorscommittee()
                                 {
                                     CommitteeId = id
                                 }).ToList(),
                                 IsDeleted=false
                             };

                   db.individuallegislators.Add(lagislatorObj);
                   await db.SaveChangesAsync();
                   LegislatorAuditViewModel model = GetLegislatorAuditModel(lagislatorObj);
                   Task.Run(() => AuditRepository.WriteAudit<LegislatorAuditViewModel>(AuditConstants.IndividualLegislator, AuditType.Insert, null, model, AuditConstants.InsertSuccessMsg));
                   #endregion
               }
           }
           return true;
       }


        /// <summary>
       /// Delete Legislator
        /// </summary>
        /// <param name="webLinkHashId"></param>
        /// <returns></returns>
       public async Task<bool> DeleteLegislator(string legislatorHashId)
       {
           bool isSave = false;
           using (BCMStrategyEntities db = new BCMStrategyEntities())
           {
               if (!string.IsNullOrEmpty(legislatorHashId))
               {
                   int decryptLegislatorId = legislatorHashId.ToDecrypt().ToInt32();

                   var objLegislator = await db.individuallegislators.Where(x => x.Id == decryptLegislatorId && x.IsDeleted != Helper.saveChangesNotSuccessful).FirstOrDefaultAsync();

                   if (objLegislator != null)
                   {
                       objLegislator.IsDeleted = true;
                       isSave = await db.SaveChangesAsync() > 0;
                   }
                   LegislatorAuditViewModel model = GetLegislatorAuditModel(objLegislator);
                   Task.Run(() => AuditRepository.WriteAudit<LegislatorAuditViewModel>(AuditConstants.IndividualLegislator, AuditType.Delete, model, null, AuditConstants.DeleteSuccessMsg));		  
               }

           }
           return isSave;
       }

       private LegislatorAuditViewModel GetLegislatorAuditModel(individuallegislators individuallegislatorsModel)
       {
         LegislatorAuditViewModel model = null;
         using (BCMStrategyEntities db = new BCMStrategyEntities())
         {
           var legislatorObj = db.individuallegislators.Where(a => a.Id == individuallegislatorsModel.Id).FirstOrDefault();
           model = new LegislatorAuditViewModel()
           {
             FirstName = legislatorObj.FirstName,
             LastName = legislatorObj.LastName,
             EntityName = legislatorObj.euentities != null ? legislatorObj.euentities.EntityName : string.Empty,
             Sector = legislatorObj.sector != null ? legislatorObj.sector.SectorName : string.Empty,
             Country = legislatorObj.country != null ? legislatorObj.country.Name : string.Empty,
             Created = legislatorObj.Created.HasValue ? Helper.GetCurrentFormatedDateTime() : string.Empty,
             Modified = legislatorObj.Modified.HasValue ? legislatorObj.Modified.ToFormatedDateTime() : string.Empty,
             CreatedBy = legislatorObj.CreatedBy,
             ModifiedBy = legislatorObj.ModifiedBy
           };
         }
         return model;
       }


        /// <summary>
       /// Get Legislator Based on Hash Id
        /// </summary>
        /// <param name="webLinkHashId"></param>
        /// <returns></returns>
       public async Task<LegislatorViewModel> GetLegislatorBasedOnHashId(string legislatorHashId)
       {
           LegislatorViewModel model = new LegislatorViewModel();
           int legislatorId = legislatorHashId.ToDecrypt().ToInt32();
           using (DALContext.BCMStrategyEntities db = new DALContext.BCMStrategyEntities())
           {
               var legislatorsObj = db.individuallegislators.Where(a => a.Id == legislatorId).FirstOrDefault();
               var legislatorDestinationIds = legislatorsObj.individuallegislatorsdesignation.Select(a => a.DesignationId).ToList();
               var legislatorCommiteeIds = legislatorsObj.individuallegislatorscommittee.Select(a => a.CommitteeId).ToList();
               if (!string.IsNullOrEmpty(legislatorHashId))
               {
                   model.LegislatorMasterId = legislatorsObj.Id;
                   model.FirstName = legislatorsObj.FirstName;
                   model.LastName = legislatorsObj.LastName;
                   model.EntityId = legislatorsObj.EntityId.HasValue ? (int)legislatorsObj.EntityId : 0;
                   model.SectorId = legislatorsObj.SectorId.HasValue ? (int)legislatorsObj.SectorId : 0;
                   model.CountryId = legislatorsObj.country != null ? (int)legislatorsObj.CountryId : 0;
                   LegislatorViewModel DDLModel = await GetAllLegislatorPageDDL(true);
                   DDLModel.DesignationDDL.Where(a => legislatorDestinationIds.Contains((int)a.Key)).ToList().ForEach(b =>
                   {
                       b.data = true;
                   });

                   DDLModel.CommiteeDDL.Where(a => legislatorCommiteeIds.Contains((int)a.Key)).ToList().ForEach(b =>
                   {
                       b.data = true;
                   });

                   model.DesignationDDL = DDLModel.DesignationDDL;
                   model.CommiteeDDL = DDLModel.CommiteeDDL;
               }
           }

           return model;
       }
    }
}
