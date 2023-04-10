using BCMStrategy.Common.Kendo;
using BCMStrategy.Data.Abstract.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.Abstract
{
 public interface IMetadataTypes
  {
    /// <summary>
    /// get all meta data list for grid
    /// </summary>
    /// <param name="parametersJson"></param>
    /// <returns></returns>
    Task<ApiOutput> GetAllMetadataTypesList(GridParameters parametersJson);

    /// <summary>
    /// update meta data values
    /// </summary>
    /// <param name="metadataTypesModel"></param>
    /// <returns></returns>
    Task<bool> UpdateMetadataTypes(MetadataTypesModel metadataTypesModel);

    /// <summary>
    /// delete meta data record based on hash id
    /// </summary>
    /// <param name="metadataTypesMasterHashId"></param>
    /// <returns></returns>
    Task<ApiOutput> DeleteMetadataTypes(string metadataTypesMasterHashId);

    /// <summary>
    /// import meta data record using excel
    /// </summary>
    /// <param name="metadataTypesImportModelList"></param>
    /// <returns></returns>
    Task<bool> ImportMetadataTypesRecords(List<MetadataTypesCsvImportModel> metadataTypesImportModelList);

    /// <summary>
    /// get meta data list for dropdown list
    /// </summary>
    /// <returns></returns>
    Task<ApiOutput> GetDropdownMetadataTypesList();

    /// <summary>
    /// get meta data type based on hash id for edit
    /// </summary>
    /// <param name="metadataTypeHashId"></param>
    /// <returns></returns>
    Task<MetadataTypesModel> GetMetadataTypeByHashId(string metadataTypeHashId);
  }
}
