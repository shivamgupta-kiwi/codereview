using BCMStrategy.Common.AuditLog;
using BCMStrategy.Common.Kendo;
using BCMStrategy.Data.Abstract.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.Abstract
{
  public interface IAuditLog
  {
    /// <summary>
    /// Get DD Audit Table List
    /// </summary>
    /// <returns></returns>
    Task<ApiOutput> GetDDAuditTableList();

    /// <summary>
    /// Get DD Customer Table List
    /// </summary>
    /// <returns></returns>
    Task<ApiOutput> GetDDCustomerList();

    /// <summary>
    /// Get All Audit Log List
    /// </summary>
    /// <param name="parametersJson"></param>
    /// <returns></returns>
    Task<ApiOutput> GetAllAuditLogList(GridParameters parametersJson, string auditPageHash);

    /// <summary>
    /// Get All Customer Audit Log List
    /// </summary>
    /// <param name="parametersJson">parametersJson</param>
    /// <param name="customerHashId">customerHashId of the Customer</param>
    /// <returns></returns>
    Task<ApiOutput> GetAllCustomerAuditLogList(GridParameters parametersJson, string customerHashId);

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
    Task WriteAudit<T>(string auditFunctionality, AuditType auditType, T beforeValue, T afterValue, string description, int ? userId = 0);

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
    Task WriteAudit<T1, T2>(string auditFunctionality, AuditType auditType, T1 beforeValue, T2 afterValue, string description);

		void WriteAudit<T1, T2>(object applicationSetting, AuditType update, T1 serializeGlobalConfiguration, object glbConfiguration, T1 updateSuccessMsg);
	}
}
