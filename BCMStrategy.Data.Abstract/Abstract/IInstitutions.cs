using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCMStrategy.Common.Kendo;
using BCMStrategy.Data.Abstract.ViewModels;

namespace BCMStrategy.Data.Abstract.Abstract
{
  public interface IInstitutions
  {
    Task<bool> UpdateInstitutions(InstitutionModel institutionModel);

    Task<bool> DeleteInstitution(string institutionMasterHashId);

    Task<ApiOutput> GetAllInstitutionsList(GridParameters parametersJson);

    Task<bool> ImportInstitutionRecords(List<InstitutionCsvImportModel> institutionImportModelList);

    string GetInstitutionName(int instTypeId, int countryId);

    Task<InstitutionModel> GetInstitutionByHashId(string institutionHashId);

  }
}
