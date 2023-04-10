using System.Collections.Generic;
using System.Threading.Tasks;
using BCMStrategy.Common.Kendo;
using BCMStrategy.Data.Abstract.ViewModels;

namespace BCMStrategy.Data.Abstract.Abstract
{
  public interface IPolicyMaker
  {
    /// <summary>
    /// Add and update of Policy Makers
    /// </summary>
    /// <param name="policyMakerModel">Policy Model with Institution values</param>
    /// <returns>Is Saved or not</returns>
    Task<bool> UpdatePolicyMaker(PolicyMakerModel policyMakerModel);

    /// <summary>
    /// Delete Policy Makers
    /// </summary>
    /// <param name="policyMakerHashId">Policy Maker Id to Delete</param>
    /// <returns>return successful message</returns>
    Task<bool> DeletePolicyMaker(string policyMakerHashId);

    /// <summary>
    /// Get all the list of Policy Makers
    /// </summary>
    /// <param name="parametersJson">Grid Parameter to filter or sorting</param>
    /// <returns>return the list</returns>
    Task<ApiOutput> GetAllPolicyMakerList(GridParameters parametersJson);

    /// <summary>
    /// Get designation Drop down list
    /// </summary>
    /// <returns></returns>
    Task<ApiOutput> GetDropdownDesignationList();

    /// <summary>
    /// Import the data's of Policy Makers
    /// </summary>
    /// <param name="policyMakersImportModelList"></param>
    /// <returns></returns>
    Task<bool> ImportPolicyMakerRecords(List<PolicyMakersCsvImportModel> policyMakersImportModelList);

    int GetDesignationIdByName(string designationName);

    /// <summary>
    /// Get policy maker Based on Hash Id
    /// </summary>
    /// <param name="policyMakerHashId"></param>
    /// <returns></returns>
    Task<PolicyMakerModel> GetPolicyMakerByHashId(string policyMakerHashId);

  }
}