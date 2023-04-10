using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.Abstract
{
  public interface ISector
  {
    /// <summary>
    /// Get all sector name from db.
    /// </summary>
    /// <returns></returns>
    List<string> GetSectorNameList();
  }
}
