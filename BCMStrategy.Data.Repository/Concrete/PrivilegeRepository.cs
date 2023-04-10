using BCMStrategy.Common.Kendo;
using BCMStrategy.DAL.Context;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCMStrategy.Data.Abstract;
using System.Data.SqlClient;
using BCMStrategy.Common.AuditLog;
using BCMStrategy.Common.Unity;

namespace BCMStrategy.Data.Repository.Concrete
{
  public class PrivilegeRepository : IPrivilege
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
    /// Get All Customer
    /// </summary>
    /// <param name="searchTerm"></param>
    /// <returns></returns>
    public async Task<ApiOutput> GetAllCustomer(GridParameters parameters)
    {
      ApiOutput apiOutput = new ApiOutput();
      List<LexiconAccessManagementModel> list;
      int totalRecord = 0;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        IQueryable<LexiconAccessManagementModel> query = db.user.Where(x => x.UserType == "CUSTOMER" && !x.lexiconprivilege.Any() && !x.IsDeleted)
            .Select(x => new LexiconAccessManagementModel()
            {
              CustomerMasterId = x.Id,
              CustomerFirstName = x.FirstName,
              CustomerMiddleName = x.MiddleName,
              CustomerLastName = x.LastName,
              Designation = x.Title,
            });
        if (parameters.Sort == null || parameters.Sort.Count == 0)
        {
          query = query.OrderByDescending(x => x.CustomerMasterId);
        }
        list = await query.ModifyList(parameters, out totalRecord).ToListAsync();
      }
      apiOutput.Data = list;
      apiOutput.TotalRecords = totalRecord;
      return apiOutput;
    }

    public async Task<List<LexiconModel>> GetLexiconTermHashIdsBasedOnLexiconType(string lexiconTypeHashId, GridParameters gridParameter)
    {
      List<LexiconModel> lexiconTermList;
      int lexiconTypeId = lexiconTypeHashId.ToDecrypt().ToInt32();
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        var lexiconIssueFilter = gridParameter.Filter.Filters.Where(a => a.Field == "LexiconIssue").Select(a => a.Value).FirstOrDefault();
        var lexiconIssueCombinationFilter = gridParameter.Filter.Filters.Where(a => a.Field == "CombinationValue").Select(a => a.Value).FirstOrDefault();
        string tempCombinationFilterVal = lexiconIssueFilter == null ? string.Empty : lexiconIssueFilter.ToString();
        string templexiconIssueCombinationFilterVal = lexiconIssueCombinationFilter == null ? string.Empty : lexiconIssueCombinationFilter.ToString();
        lexiconTermList = await db.lexiconissues
          .Where(x => !x.IsDeleted && x.LexiconTypeId == lexiconTypeId &&
              (string.IsNullOrEmpty(tempCombinationFilterVal) || x.LexiconIssue.Contains(tempCombinationFilterVal)) &&
              (string.IsNullOrEmpty(templexiconIssueCombinationFilterVal) || x.CombinationValue.Contains(templexiconIssueCombinationFilterVal))
              ).Select(x => new LexiconModel()
          {
            LexiconeIssueMasterId = x.Id,
            LexiconeTypeMasterId = x.LexiconTypeId
          }).ToListAsync();
      }

      return lexiconTermList;
    }

    /// <summary>
    /// Update Web Link
    /// </summary>
    /// <param name="webLinkViewModel"></param>
    /// <returns></returns>
    public async Task<bool> UpdateLexiconAccessPrivilege(LexiconAccessManagementModel lexiconAccessManagementModel)
    {
      List<int> customerIds = lexiconAccessManagementModel.SelectedCustomerHashIds.Any() ? lexiconAccessManagementModel.SelectedCustomerHashIds.Select(a => a.ToDecrypt().ToInt32()).ToList() : new List<int>();
      List<int> lexiconIssueIds = lexiconAccessManagementModel.SelectedLexiconHashIds.Any() ? lexiconAccessManagementModel.SelectedLexiconHashIds.Select(a => a.ToDecrypt().ToInt32()).ToList() : new List<int>();

      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        ////db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        db.Configuration.AutoDetectChangesEnabled = false;
        if (customerIds.Any() && lexiconIssueIds.Any() && string.IsNullOrEmpty(lexiconAccessManagementModel.CustomerMasterHashId))
        {
          var insertStr = GetLexiconPrivilegeInsertQuery(customerIds, lexiconIssueIds, false);

          db.Database.ExecuteSqlCommand(insertStr);
          db.SaveChanges();

          LexiconTermViewModel model = new LexiconTermViewModel();
          model.LexiconTerm = string.Join(",", lexiconIssueIds.ToArray());
          model.CreatedBy = UserAccessHelper.CurrentUserIdentity.ToString();

          Task.Run(() => AuditRepository.WriteAudit<LexiconTermViewModel>(AuditConstants.LexiconSubscription, AuditType.Insert, null, model, AuditConstants.InsertSuccessMsg, UserAccessHelper.CurrentUserIdentity));
        }
        else if (!string.IsNullOrEmpty(lexiconAccessManagementModel.CustomerMasterHashId))
        {
          LexiconTermViewModel insertedModel = new LexiconTermViewModel();
          LexiconTermViewModel updatedModel = new LexiconTermViewModel();

          int customerId = lexiconAccessManagementModel.CustomerMasterHashId.ToDecrypt().ToInt32();
          var dbLexiconIssueList = await db.lexiconprivilege.Where(a => a.UserId == customerId && a.IsDeleted != 1).Select(a => a.LexiconIssueId).ToListAsync();

          updatedModel.LexiconTerm = string.Join(",", dbLexiconIssueList.ToArray());

          List<int> listToRemove = dbLexiconIssueList.Where(a => !lexiconIssueIds.Contains((int)a)).Select(a => (int)a).ToList();
          List<lexiconprivilege> listToAdd = lexiconIssueIds.Where(a => !dbLexiconIssueList.Contains(a)).Select(a => new lexiconprivilege()
          {
            LexiconIssueId = a,
            UserId = customerId,
            Modified = DateTime.Now,
          }).ToList();

          if (listToRemove.Any())
          {
            var deleteQuery = GetLexiconPrivilegeDeleteQuery(listToRemove, customerId);
            db.Database.ExecuteSqlCommand(deleteQuery);

            insertedModel.LexiconTerm = string.Join(",", listToRemove.ToArray());
          }
          if (listToAdd.Any())
          {
            var insertStr = GetLexiconPrivilegeInsertQuery(listToAdd.Select(a => (int)a.UserId).ToList(), listToAdd.Select(a => (int)a.LexiconIssueId).ToList(), true);
            db.Database.ExecuteSqlCommand(insertStr);

            insertedModel.LexiconTerm = string.Join(",", listToAdd.Select(a => (int)a.LexiconIssueId).ToArray());
          }
          db.SaveChanges();

          insertedModel.ModifiedBy = UserAccessHelper.CurrentUserIdentity.ToString();
          Task.Run(() => AuditRepository.WriteAudit<LexiconTermViewModel>(AuditConstants.LexiconSubscription, AuditType.Update, updatedModel, insertedModel, AuditConstants.UpdateSuccessMsg, UserAccessHelper.CurrentUserIdentity));
        }
      }
      return true;
    }


    public string GetLexiconPrivilegeInsertQuery(List<int> customerIds, List<int> lexiconIssueIds, bool isEdit)
    {
      var customerIdStr = string.Join(",", customerIds);
      var lexiconIssueIdStr = string.Join(",", lexiconIssueIds);
      string insertQuery = string.Empty;

      if (isEdit)
      {
        insertQuery = string.Format("INSERT INTO lexiconprivilege(UserId,LexiconIssueId,Modified) (SELECT b.id,a.id,NOW() FROM ((SELECT id FROM lexiconissues WHERE id IN ({0})) AS A JOIN (SELECT id FROM USER WHERE id IN ({1})) B))", lexiconIssueIdStr, customerIdStr);
      }
      else
      {
        insertQuery = string.Format("INSERT INTO lexiconprivilege(UserId,LexiconIssueId,Created,Modified) (SELECT b.id,a.id,NOW(),NOW() FROM ((SELECT id FROM lexiconissues WHERE id IN ({0})) AS A JOIN (SELECT id FROM USER WHERE id IN ({1})) B))", lexiconIssueIdStr, customerIdStr);
      }
      return insertQuery;
    }

    public string GetLexiconPrivilegeDeleteQuery(List<int> Ids, int customerId)
    {
      var lexiconPrivilegeIds = string.Join(",", Ids);
      string deleteQuery = string.Format("Delete from lexiconprivilege where lexiconissueid in({0}) and userid={1}", lexiconPrivilegeIds, customerId);
      return deleteQuery;
    }


    /// <summary>
    /// Get All Web Links
    /// </summary>
    /// <param name="searchTerm"></param>
    /// <returns></returns>
    public async Task<ApiOutput> GetAllLexiconAccessCustomer(GridParameters parametersJson)
    {
      ApiOutput apiOutput = new ApiOutput();
      List<LexiconAccessManagementModel> list;
      int totalRecord = 0;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        IQueryable<LexiconAccessManagementModel> query = db.lexiconprivilege.Where(a => a.IsDeleted != 1 && !a.user.IsDeleted)
            .Select(x => new LexiconAccessManagementModel()
            {
              CustomerMasterId = (int)x.UserId,
              CustomerFirstName = x.user.FirstName,
              CustomerMiddleName = x.user.MiddleName,
              CustomerLastName = x.user.LastName,
              CompanyName = x.user.CompanyName,
              Designation = x.user.Title,
            }).Distinct();

        if (parametersJson.Sort == null || parametersJson.Sort.Count == 0)
        {
          query = query.OrderBy(x => x.CustomerFirstName);
        }
        list = await query.ModifyList(parametersJson, out totalRecord).ToListAsync();

        var userIds = list.Select(a => a.CustomerMasterId).ToList();
        var lexiconPrivileges = db.lexiconprivilege.Where(a => userIds.Contains((int)a.UserId));

        list.ForEach(obj =>
        {
          var tempList = lexiconPrivileges.Where(a => (int)a.UserId == obj.CustomerMasterId).Select(a => a.lexiconissues.lexicontype.Type).Distinct().ToList();
          obj.AccessLexiconTypesString = string.Join(",", tempList);
        });

      }
      apiOutput.Data = list;
      apiOutput.TotalRecords = totalRecord;
      return apiOutput;


    }


    /// <summary>
    /// Get LexiconIds Based On HashId
    /// </summary>
    /// <param name="webLinkHashId"></param>
    /// <returns></returns>
    public async Task<List<LexiconAccessManagementModel>> GetLexiconIdsBasedOnCustomer(string customerHashId)
    {
      int customerId = customerHashId.ToDecrypt().ToInt32();

      List<LexiconAccessManagementModel> list = null;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        db.Configuration.AutoDetectChangesEnabled = false;
        list = db.lexiconprivilege.Where(a => a.UserId == customerId).Select(dbObj => new LexiconAccessManagementModel()
        {
          LexiconeIssueId = (int)dbObj.LexiconIssueId,
          LexiconTypeId = dbObj.lexiconissues.LexiconTypeId
        }).ToList();
      }
      return list;
    }


  }
}
