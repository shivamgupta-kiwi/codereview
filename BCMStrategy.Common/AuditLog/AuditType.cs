namespace BCMStrategy.Common.AuditLog
{
  /// <summary>
  /// Enum For Audit Type 1
  /// </summary>
  public enum AuditType
  {
    /// <summary>
    /// For Insert
    /// </summary>
    Insert = 1,

    /// <summary>
    /// For Insert Failure
    /// </summary>
    InsertFailure = 4,

    /// <summary>
    /// For Update
    /// </summary>
    Update = 2,

    /// <summary>
    /// For Update Failure
    /// </summary>
    UpdateFailure = 5,

    /// <summary>
    /// For Delete
    /// </summary>
    Delete = 3,

    /// <summary>
    /// For Failure of Delete
    /// </summary>
    DeleteFailure = 6,

    /// <summary>
    /// For Email
    /// </summary>
    Email = 10,

    /// <summary>
    /// For Email Failure
    /// </summary>
    EmailFailure = 8,

    Login = 11,

    Logout = 12,

    ResultView = 13
  }
}
