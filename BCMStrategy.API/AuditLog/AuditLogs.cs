using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Common.Unity;
using BCMStrategy.Common.AuditLog;

namespace BCMStrategy.API.AuditLog
{
  /// <summary>
  /// Audit Log
  /// </summary>
  public static class AuditLogs
  {
    /// <summary>
    /// The _auditRepository
    /// </summary>
    private static IAuditLog _auditRepository;

    /// <summary>
    /// Gets the AuditLog repository.
    /// </summary>
    /// <value>
    /// The AuditLog repository.
    /// </value>
    private static IAuditLog AuditRepository
    {
      get
      {
        if (_auditRepository == null)
        {
          _auditRepository = UnityHelper.Resolve<IAuditLog>();
        }

        return _auditRepository;
      }
    }

    /// <summary>
    /// Use To Write The Audit
    /// </summary>
    /// <param name="auditFunctionality">audit Functionality</param>
    /// <param name="auditType">audit Type </param>
    /// <typeparam name="T">The first generic type parameter.</typeparam>
    /// <param name="beforeValueObj">before value</param>
    /// <param name="afterValueObj">after value</param>
    /// <param name="description">description object</param>
    public static void Write(string auditFunctionality, AuditType auditType, string description)
    {
      Task.Run(() => AuditRepository.WriteAudit<string>(auditFunctionality, auditType, (string)null, (string)null, description));
    }
    public static void Write<T1,T2>(string auditFunctionality, AuditType auditType, T1 beforeValueObj, T2 afterValueObj, string description)
    {
      Task.Run(() => AuditRepository.WriteAudit<T1,T2>(auditFunctionality, auditType, beforeValueObj, afterValueObj, description));
    }
  }
}