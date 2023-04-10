using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class UserAccessRightsModel
  {
    private string _userMasterHashId;


    public int UserMasterId { get; set; }

    public string UserMasterHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_userMasterHashId))
        {
          _userMasterHashId = this.UserMasterId == 0 ? string.Empty : this.UserMasterId.ToEncrypt();
        }
        return _userMasterHashId;
      }
      set
      {
        _userMasterHashId = value;
      }
    }

    public string UserEmail { get; set; }


    private string _userAccessRightsMasterHashId;
    [JsonIgnore]
    public int UserAccessRightsMasterId { get; set; }

    public string UserAccessRightsMasterHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_userAccessRightsMasterHashId))
        {
          _userAccessRightsMasterHashId = this.UserAccessRightsMasterId == 0 ? string.Empty : this.UserAccessRightsMasterId.ToEncrypt();
        }
        return _userAccessRightsMasterHashId;
      }
      set
      {
        _userAccessRightsMasterHashId = value;
      }
    }

    public string SubMenuId { get; set; }


  }
}
