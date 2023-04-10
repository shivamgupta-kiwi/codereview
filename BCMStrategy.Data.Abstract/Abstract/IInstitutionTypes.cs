using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Common.Kendo;

namespace BCMStrategy.Data.Abstract.Abstract
{
  public interface IInstitutionTypes
  {
    Task<ApiOutput> GetDDLInstitutionTpeList();
    

    Task<ApiOutput> GetAllInstitutionTypesList(GridParameters parameters);

    Task<bool> UpdateInstitutionTypes(InstitutionTypesModel institutionTypesModel);

    Task<ApiOutput> DeleteInstitutionTypes(string institutionTypesHashId);

    Task<bool> ImportInstitutionTypeRecord(List<InstitutionTypesImportModel> institutionTypesImportModel);

    /// <summary>
    /// get Institution Type based on Hash id for edit
    /// </summary>
    /// <param name="nounVerbHashId"></param>
    /// <returns></returns>
    Task<InstitutionTypesModel> GetInstitutionTypeByHashId(string institutionTypeHashId);
  }
}
