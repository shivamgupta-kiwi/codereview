using BCMStrategy.Data.Abstract.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.Abstract
{
  public interface IScrappingProcess
  {
    Task<ApiOutput> GetAllScrappedURLList(string webSiteHashId, int webSiteType , string processEventId);


    Task<ApiOutput> GetAllScrappedURLSummaryList(int webSiteType, string processId);


  }
}
