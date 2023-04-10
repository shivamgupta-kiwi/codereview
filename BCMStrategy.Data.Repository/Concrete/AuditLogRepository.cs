using BCMStrategy.Common.AuditLog;
using BCMStrategy.Common.Kendo;
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
  public class AuditLogRepository : IAuditLog
  {
    private static readonly EventLogger<AuditLogRepository> log = new EventLogger<AuditLogRepository>();

    public async Task<ApiOutput> GetDDAuditTableList()
    {
      ApiOutput apiOutput = new ApiOutput();
      List<DropdownMaster> auditTableList;

      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        IQueryable<DropdownMaster> query = db.audittables.Where(x => x.IsCustomerTable == false)
            .Select(x => new DropdownMaster()
            {
              Key = x.Id,
              Value = x.TableName
            }).OrderBy(x => x.Value);
        auditTableList = await query.ToListAsync();
      }
      apiOutput.Data = auditTableList;
      apiOutput.TotalRecords = 0;
      apiOutput.ErrorMessage = auditTableList.Any() ? string.Empty : string.Format(Resource.ValidateMessageNoDataFound, Resource.LblAuditTable);
      return apiOutput;
    }

    public async Task<ApiOutput> GetDDCustomerList()
    {
      ApiOutput apiOutput = new ApiOutput();
      List<DropdownMaster> customerTableList;

      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        IQueryable<DropdownMaster> query = db.user.Where(x => x.IsActive == true && !x.IsDeleted && x.UserType == "CUSTOMER")
            .Select(x => new DropdownMaster()
            {
              Key = x.Id,
              Value = x.FirstName + " " + x.LastName
            }).OrderBy(x => x.Value);
        customerTableList = await query.ToListAsync();
      }
      apiOutput.Data = customerTableList;
      apiOutput.TotalRecords = 0;
      apiOutput.ErrorMessage = customerTableList.Any() ? string.Empty : string.Format(Resource.ValidateMessageNoDataFound, Resource.LblCustomer);
      return apiOutput;
    }

    /// <summary>
    /// Get all Audit Log
    /// </summary>
    /// <param name="parametersJson">Grid Parameter to filter or sorting</param>
    /// <returns>return the list</returns>
    public async Task<ApiOutput> GetAllAuditLogList(GridParameters parametersJson, string auditPageHash)
    {
				ApiOutput apiOutput = new ApiOutput();
			try
			{
				List<AuditLogModel> auditLogList;
				int totalRecord = 0;
				int? auditPageId = !string.IsNullOrEmpty(auditPageHash) ? auditPageHash.ToDecrypt().ToInt32() : (int?)null;

				using (BCMStrategyEntities db = new BCMStrategyEntities())
				{

					IQueryable<AuditLogModel> query = (from a in db.auditlog.Where(x => x.AuditTableId == auditPageId)
																						 join y in db.audittables on a.AuditTableId equals y.Id
																						 where y.IsCustomerTable == false
																						 select new AuditLogModel
																						 {
																							 AuditLogMasterId = a.Id,
																							 AuditTable = a.audittables.TableName,
																							 AuditType = a.audittype.Status,
																							 BeforeValue = a.BeforeValue,
																							 AfterValue = a.AfterValue,
																							 AuditDescription = a.AuditDescription,
																							 IpAddress = a.IPAddress,
																							 Created = a.Created.Value.ToString(),
																							 UserName = a.CreatedBy
																						 });

					if (parametersJson.Sort == null || parametersJson.Sort.Count == 0)
					{
						query = query.OrderByDescending(x => x.AuditLogMasterId);
					}
					db.Database.Log = msg => System.Diagnostics.Debug.WriteLine(msg);
					auditLogList = await query.ModifyList(parametersJson, out totalRecord).ToListAsync();
				}
				apiOutput.Data = auditLogList;
				apiOutput.TotalRecords = totalRecord;
			}
			catch (Exception ex)
			{
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in the GetAllAuditLogList method", ex, null);
			}

			return apiOutput;
    }

    public async Task<ApiOutput> GetAllCustomerAuditLogList(GridParameters parametersJson, string customerHashId)
    {
      ApiOutput apiOutput = new ApiOutput();
      List<AuditLogModel> auditLogList;

      int totalRecord = 0;
      int? customerId = !string.IsNullOrEmpty(customerHashId) ? customerHashId.ToDecrypt().ToInt32() : 0;

      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        IQueryable<AuditLogModel> query = db.auditlog.Where(a => a.UserId == customerId)
            .Select(x => new AuditLogModel()
            {
              AuditLogMasterId = x.Id,
              AuditTable = x.audittables.TableName,
              AuditType = x.audittype.Status,
              AfterValue = x.AfterValue,
              AuditDescription = x.AuditDescription,
              IpAddress = x.IPAddress,
              Created = x.Created.Value.ToString(),
              UserName = x.CreatedBy
            });
        if (parametersJson.Sort == null || parametersJson.Sort.Count == 0)
        {
          query = query.OrderByDescending(x => x.AuditLogMasterId);
        }
        auditLogList = await query.ModifyList(parametersJson, out totalRecord).ToListAsync();
      }
      apiOutput.Data = auditLogList;
      apiOutput.TotalRecords = totalRecord;
      return apiOutput;
    }

    /// <summary>
    /// Use To Write The Audit
    /// </summary>
    /// <param name="auditFunctionality">audit Functionality</param>
    /// <param name="auditType">audit Type </param>
    /// <typeparam name="T">The first generic type parameter.</typeparam>
    /// <param name="beforeValue">before value</param>
    /// <param name="afterValue">after value</param>
    /// <param name="description">description object</param>
    /// <returns>Void Task</returns>
    public async Task WriteAudit<T>(string auditFunctionality, AuditType auditType, T beforeValue, T afterValue, string description, int? userId = 0)
    {
      try
      {
        string userName = string.Empty;

        using (BCMStrategyEntities db = new BCMStrategyEntities())
        {
          int modifiedUserId = 0;
          int tableId = db.audittables.Where(a => a.TableCode == auditFunctionality).Select(a => a.Id).FirstOrDefault();
          string strBeforeVal = beforeValue != null ? Helper.SerializeObjectTojson(beforeValue) : string.Empty;
          string strAfterVal = afterValue != null ? Helper.SerializeObjectTojson(afterValue) : string.Empty;

          try
          {
            var dict = strAfterVal.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
               .Select(part => part.Split(':'))
               .ToDictionary(split => split[0], split => split[1]);

            if (strBeforeVal == string.Empty && strAfterVal != string.Empty)
            {
              try
              {
                if (dict.ContainsKey("\"CreatedBy\""))
                {
                  string myCreatedByValue;
                  dict.TryGetValue("\"CreatedBy\"", out myCreatedByValue);

                  myCreatedByValue = myCreatedByValue.Replace("}", "").Replace("\"", "");

                  if (myCreatedByValue != string.Empty)
                  {
                    modifiedUserId = int.Parse(myCreatedByValue);
                  }

                  userName = db.user.Where(x => x.Id == modifiedUserId).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault();
                }
              }
              catch (Exception ex)
              {
                log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in the WriteAudit insert method", ex, null);
              }
            }
            else if (strBeforeVal != string.Empty && strAfterVal != string.Empty)
            {
              try
              {
                if (dict.ContainsKey("\"ModifiedBy\""))
                {
                  string myModifiedByValue;
                  dict.TryGetValue("\"ModifiedBy\"", out myModifiedByValue);

                  myModifiedByValue = myModifiedByValue.Replace("}", "").Replace("\"", "");

                  if (myModifiedByValue != string.Empty)
                  {
                    modifiedUserId = int.Parse(myModifiedByValue);
                  }

                  userName = db.user.Where(x => x.Id == modifiedUserId).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault();
                }
              }
              catch (Exception ex)
              {
                log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in the WriteAudit Update method", ex, null);
              }
            }
          }
          catch (Exception exp)
          {
            log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in the WriteAudit general method", exp, null);
          }

          if (userName == string.Empty && userId != null)
          {
            userName = db.user.Where(x => x.Id == userId).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault();
          }

          if (tableId != 0)
          {
            auditlog objAudit = new auditlog
            {
              AuditTableId = tableId,
              BeforeValue = strBeforeVal,
              AfterValue = strAfterVal,
              AuditDescription = description,
              AuditTypeId = (int)auditType,
              IPAddress = Helper.GetSystemIPAddress(),
              Created = Helper.GetCurrentDateTime(),
              UserId = userId == 0 ? null : userId,
              CreatedBy = userName
            };
            db.auditlog.Add(objAudit);
            await db.SaveChangesAsync();
          }
        }
      }
      catch (Exception ex)
      {
        log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in the WriteAudit main method", ex, null);
      }
    }

    /// <summary>
    /// Use To Write The Audit
    /// </summary>
    /// <param name="auditFunctionality">audit Functionality</param>
    /// <param name="auditType">audit Type </param>
    /// <typeparam name="T1">The first generic type parameter.</typeparam>
    /// <typeparam name="T2">The second generic type parameter.</typeparam>
    /// <param name="beforeValue">before value</param>
    /// <param name="afterValue">after value</param>
    /// <param name="description">description object</param>
    /// <returns>Void Task</returns>
    public async Task WriteAudit<T1, T2>(string auditFunctionality, AuditType auditType, T1 beforeValue, T2 afterValue, string description)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        int tableId = db.audittables.Where(a => a.TableCode == auditFunctionality).Select(a => a.Id).FirstOrDefault();
        string serializedBeforeValObj = string.Empty;
        string serializedAfterValObj = string.Empty;

        if (beforeValue != null)
          serializedBeforeValObj = Helper.SerializeObjectTojson(beforeValue);

        if (afterValue != null)
          serializedAfterValObj = Helper.SerializeObjectTojson(beforeValue);


        //// string strAfterVal = afterValue != null ? Helper.SerializeObjectTojson(afterValue) : string.Empty;

        if (tableId != 0)
        {
          auditlog objAudit = new auditlog
          {
            AuditTableId = tableId,
            BeforeValue = serializedBeforeValObj,
            AfterValue = serializedAfterValObj,
            AuditDescription = description,
            AuditTypeId = (int)auditType,
            IPAddress = Helper.GetSystemIPAddress(),
            Created = Helper.GetCurrentDateTime(),
          };
          db.auditlog.Add(objAudit);
          await db.SaveChangesAsync();
        }
      }
    }

		public void WriteAudit<T1, T2>(object applicationSetting, AuditType update, T1 serializeGlobalConfiguration, object glbConfiguration, T1 updateSuccessMsg)
		{
			throw new NotImplementedException();
		}
	}
}
