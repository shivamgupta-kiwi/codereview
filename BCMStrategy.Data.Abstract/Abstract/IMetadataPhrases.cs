using BCMStrategy.Common.Kendo;
using BCMStrategy.Data.Abstract.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.Abstract
{
  public interface IMetadataPhrases
  {
    /// <summary>
    /// update metadata phrases by hash id
    /// </summary>
    /// <param name="metadataPhrasesModel"></param>
    /// <returns></returns>
    Task<bool> UpdateMetadataPhrases(MetadataPhrasesModel metadataPhrasesModel);

    /// <summary>
    /// delete metadata phrases by hash id
    /// </summary>
    /// <param name="metadataPhrasesMasterHashId"></param>
    /// <returns></returns>
    Task<bool> DeleteMetadataPhrases(string metadataPhrasesMasterHashId);

    /// <summary>
    /// get all metadata phrases for grid
    /// </summary>
    /// <param name="parametersJson"></param>
    /// <returns></returns>
    Task<ApiOutput> GetAllMetadataPhrasesList(GridParameters parametersJson);

    /// <summary>
    /// get metadata phrases data based on hash id for edit
    /// </summary>
    /// <param name="phrasesHashId"></param>
    /// <returns></returns>
    Task<MetadataPhrasesModel> GetMetadataPhrasesByHashId(string phrasesHashId);
  }
}
