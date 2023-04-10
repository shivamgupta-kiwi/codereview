using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class ParentMenuModel
  {
    public int ParentMenuId { get; set; }

    public string ParentMenuName { get; set; }

    public List<SubMenuModel> SubMenuList { get; set; }
  }
}
