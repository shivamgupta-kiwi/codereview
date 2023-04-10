using System.Collections.Generic;
using System.Threading.Tasks;
using BCMStrategy.Common.Kendo;
using BCMStrategy.Data.Abstract.ViewModels;

namespace BCMStrategy.Data.Abstract.Abstract
{
  public interface IInternationalOrganization
  {
    Task<bool> UpdateInternationalOrganization(InternationalOrganizationModel internationalOrganization);

    Task<bool> DeleteInternationalOrganization(string internationalOrgMasterHashId);

    Task<ApiOutput> GetAllInternationalOrganizationList(GridParameters parametersJson);

    Task<bool> ImportInternationOrganizationRecord(List<InternationalOrganizationImportModel> internationalOrganizationImportModelList);

    /// <summary>
    /// get International Organization data based on hash id for edit
    /// </summary>
    /// <param name="internationalOrgHashId"></param>
    /// <returns></returns>
    Task<InternationalOrganizationModel> GetInternationalOrgByHashId(string internationalOrgHashId);
  }
}