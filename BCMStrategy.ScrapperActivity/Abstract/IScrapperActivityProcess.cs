using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCMStrategy.Data.Abstract.ViewModels;

namespace BCMStrategy.ScrapperActivity.Abstract
{
  public interface IScrapperActivityProcess
  {
    #region Useless Code for Temporary
    ////int SaveScraperEvent(Events scraperEvents);

    ////int GetWebLinkCount(int processId, int processInstanceId);

    ////List<ProcessInstances> InsertProcesssInstances(ProcessConfiguration processConfig);

    ////bool UpdateScraperEvent(Events scraperEvents);
    #endregion

    void ReadLexiconFromSolr(int processId, int processInstanceId);
  }
}
