using System.Threading.Tasks;
using BCMStrategy.Common.Kendo;
using BCMStrategy.Data.Abstract.ViewModels;

namespace BCMStrategy.Data.Abstract.Abstract
{
  public interface ILegislator
  {
    /// <summary>
    /// Get All Web Link DDL
    /// </summary>
    /// <returns></returns>
    Task<LegislatorViewModel> GetAllLegislatorPageDDL(bool isEdit = false);

    /// <summary>
    ///  Used to get All WebLinks
    /// </summary>
    /// <param name="searchTerm"></param>
    /// <returns></returns>
    Task<ApiOutput> GetAllLegislatorList(GridParameters parametersJson);

    /// <summary>
    /// Update Web Link
    /// </summary>
    /// <param name="webLinkViewModel"></param>
    /// <returns></returns>
    Task<bool> UpdateLegislator(LegislatorViewModel lagislatorViewModel);

    /// <summary>
    /// Delete Web Link
    /// </summary>
    /// <param name="webLinkHashId"></param>
    /// <returns></returns>
    Task<bool> DeleteLegislator(string legislatorHashId);

    /// <summary>
    /// Get WebLink Based on Hash Id
    /// </summary>
    /// <param name="webLinkHashId"></param>
    /// <returns></returns>
    Task<LegislatorViewModel> GetLegislatorBasedOnHashId(string legislatorHashId);
  }
}