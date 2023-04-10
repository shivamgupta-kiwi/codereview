using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Common.Kendo;

namespace BCMStrategy.Data.Abstract.Abstract
{
  public interface ILexicon
  {
    /// <summary>
    /// update lexicon data
    /// </summary>
    /// <param name="lexiconModel"></param>
    /// <returns></returns>
    Task<bool> UpdateLexicon(LexiconModel lexiconModel);

    /// <summary>
    /// get lexicon data for scrapping
    /// </summary>
    /// <returns></returns>
    List<LexiconModel> GetLexiconListForScraping();

    /// <summary>
    /// get all lexicon data for grid
    /// </summary>
    /// <param name="parametersJson"></param>
    /// <returns></returns>
    Task<ApiOutput> GetAllLexiconList(GridParameters parametersJson);

    /// <summary>
    /// delete lexicon data based on hash id
    /// </summary>
    /// <param name="lexiconIssueMasterHashId"></param>
    /// <returns></returns>
    Task<bool> DeleteLexicon(string lexiconIssueMasterHashId);

    /// <summary>
    /// import lexicon using excel
    /// </summary>
    /// <param name="lexiconImportModelList"></param>
    /// <returns></returns>
    Task<bool> ImportLexiconRecords(List<LexiconCsvImportModel> lexiconImportModelList);

    /// <summary>
    /// Get Lexicon Based on Hash Id
    /// </summary>
    /// <param name="lexiconHashId"></param>
    /// <returns></returns>
    Task<LexiconModel> GetLexiconByHashId(string lexiconHashId);
  }
}
