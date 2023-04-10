using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class AuditLogModel
  {

    public int AuditLogMasterId { get; set; }

    private string _auditTableMasterHashId;

    [JsonIgnore]
    public int ActivityTableMasterId { get; set; }

    public string AuditTableMasterHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_auditTableMasterHashId))
        {
          _auditTableMasterHashId = this.ActivityTableMasterId == 0 ? string.Empty : this.ActivityTableMasterId.ToEncrypt();
        }
        return _auditTableMasterHashId;
      }
      set
      {
        _auditTableMasterHashId = value;
      }
    }

    public int AuditTypeMasterId { get; set; }

    public string AuditTable { get; set; }
    public string AuditType { get; set; }

    public string BeforeValue { get; set; }
    public string AfterValue { get; set; }
    public string AuditDescription { get; set; }
    public string IpAddress { get; set; }

    public string UserName { get; set; }

    public DateTime? CreatedDate
    {
      get
      {
        return _created;
      }
      set
      {
        if (value.HasValue)
        {
          ////_createdString = ((DateTime)value).ToString("MM-dd-yyyy HH:mm:ss");
          ////_created = value.Value;
          _created = value.Value;
          _createdString = value.Value.ToFormatedDateTime();
        }
      }
    }
    private DateTime _created { get; set; }
    private string _createdString { get; set; }
    public string Created
    {
      get
      {
        return _createdString;
      }
      set
      {
        if (!string.IsNullOrWhiteSpace(value))
        {
          ////_created = Convert.ToDateTime(value);
          ////_createdString = value;
          _createdString = value;
          DateTime? date = FromFormatedDateTime(value, "MM/dd/yyyy HH:mm");
          if (date.HasValue)
            _created = Convert.ToDateTime(value);

        }
      }
    }

    public DateTime? FromFormatedDateTime(string input, string format = "MM/dd/yyyy HH:mm")
    {
      DateTime output;
      DateTime.TryParseExact(input, format, System.Globalization.CultureInfo.InvariantCulture,
      DateTimeStyles.None, out output);
      return output;
    }
  }
}
