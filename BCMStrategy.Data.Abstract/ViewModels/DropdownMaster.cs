using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class DropdownMaster
  {

    [JsonIgnore]
    public long Key { get; set; }

    private string _KeyHash;

    public string KeyHash
    {
      get
      {
        if (string.IsNullOrEmpty(_KeyHash))
        {
          _KeyHash = this.Key == 0 ? string.Empty : this.Key.ToEncrypt();
        }
        return _KeyHash;
      }
      set
      {
        _KeyHash = value;
      }
    }

    public string Value { get; set; }

    public object data { get; set; }
  }
}
