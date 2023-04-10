using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  /// <summary>
  /// Status
  /// </summary>
  public class Status
  {
    public Status()
    {
      this.dateTimeUTC = Helper.GetCurrentDateTime().ToFormatedDateTime();
      this.version = Helper.AssemblyVersion;
    }
    public string dateTimeUTC { get; set; }
    public string version { get; set; }
  }

  public class ApiOutput
  {
    public ApiOutput()
    {
      this.Status = new Status();
    }

    public object Data { get; set; }
    public string ErrorMessage { get; set; }
    public object ErrorModel { get; set; }
    public int TotalRecords { get; set; }
    public string ExtraDetails { get; set; }
    public float? ReportTotalCount { get; set; }
    public Status Status { get; set; }
  }
}