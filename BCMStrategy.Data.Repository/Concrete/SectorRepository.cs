using BCMStrategy.DAL.Context;
using BCMStrategy.Data.Abstract.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Repository.Concrete
{
  public class SectorRepository : ISector
  {

    /// <summary>
    /// Get all sector name from db.
    /// </summary>
    /// <returns></returns>
    public List<string> GetSectorNameList()
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        var sectorList = db.sector.Select(x => new {
          SectorName = x.SectorName,
        }).ToList();
        return sectorList.Select(r => r.SectorName).ToList();
      }
    }
  }
}
