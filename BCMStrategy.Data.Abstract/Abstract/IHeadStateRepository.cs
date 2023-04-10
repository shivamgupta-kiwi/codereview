using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCMStrategy.Common.Kendo;
using BCMStrategy.Data.Abstract.ViewModels;

namespace BCMStrategy.Data.Abstract.Abstract
{
  public interface IHeadStateRepository
  {
    Task<bool> ImportHeadStateRecord(List<HeadStateImportModel> headStateImportModel);

    Task<ApiOutput> GetStateHeadList(GridParameters parameters);

    Task<bool> UpdateStateHead(StateHeadModel stateHeadModel);

    Task<bool> DeleteStateHead(string stateHeadMasterHashId);

    /// <summary>
    /// get head of state data based on hash id
    /// </summary>
    /// <param name="stateHeadHashId"></param>
    /// <returns></returns>
    Task<StateHeadModel> GetStateHeadByHashId(string stateHeadHashId);
  }
}
