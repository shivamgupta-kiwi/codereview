using BCMStrategy.Common.Kendo;
using BCMStrategy.Data.Abstract.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.Abstract
{
  public interface IMetadataNounplusVerb
  {
    /// <summary>
    /// get Dynamic Metadata Noun plus verb list for dropdown
    /// </summary>
    /// <returns></returns>
    Task<ApiOutput> GetDropdownMetadataDynamicNounplusVerbList();

    /// <summary>
    /// update metadata noun plus verb data by hash id
    /// </summary>
    /// <param name="metadataNounplusVerbModel"></param>
    /// <returns></returns>
    Task<bool> UpdateMetadataNounplusVerb(MetadataNounplusVerbModel metadataNounplusVerbModel);

    /// <summary>
    /// delete metadata noun plus verb data by hash id
    /// </summary>
    /// <param name="metadataNounplusVerbMasterHashId"></param>
    /// <returns></returns>
    Task<bool> DeleteMetadataNounplusVerb(string metadataNounplusVerbMasterHashId);

    /// <summary>
    /// get all metadata noun plus verb data for grid
    /// </summary>
    /// <param name="parametersJson"></param>
    /// <returns></returns>
    Task<ApiOutput> GetAllMetadataNounplusVerbList(GridParameters parametersJson);

    /// <summary>
    /// get metadata noun plus verb data based on hash id
    /// </summary>
    /// <param name="nounVerbHashId"></param>
    /// <returns></returns>
    Task<MetadataNounplusVerbModel> GetMetadataNounPlusVerbByHashId(string nounVerbHashId);
  }
}
