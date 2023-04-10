using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class SubMenuModel
  {

    private string _subMenuMasterHashId;

    public int SubMenuMasterId { get; set; }

    public string SubMenuMasterHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_subMenuMasterHashId))
        {
          _subMenuMasterHashId = this.SubMenuMasterId == 0 ? string.Empty : this.SubMenuMasterId.ToEncrypt();
        }
        return _subMenuMasterHashId;
      }
      set
      {
        _subMenuMasterHashId = value;
      }
    }
    public string SubMenuName { get; set; }
  }
}
