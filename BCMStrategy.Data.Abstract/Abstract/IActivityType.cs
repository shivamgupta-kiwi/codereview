using BCMStrategy.Common.Kendo;
using BCMStrategy.Data.Abstract.ViewModels;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.Abstract
{
  public interface IActivityType
  {
    /// <summary>
    /// update activity type based in hash id
    /// </summary>
    /// <param name="activityTypeModel"></param>
    /// <returns></returns>
    Task<bool> UpdateActivityType(ActivityTypeModel activityTypeModel);


    /// <summary>
    /// delete activity type based on hash id
    /// </summary>
    /// <param name="activityTypeMasterHashId"></param>
    /// <returns></returns>
    Task<ApiOutput> DeleteActivityType(string activityTypeMasterHashId);

    /// <summary>
    /// get all activity type list for grid
    /// </summary>
    /// <param name="parametersJson"></param>
    /// <returns></returns>
    Task<ApiOutput> GetAllActivityTypeList(GridParameters parametersJson);

    /// <summary>
    /// get actvity type list for dropdown
    /// </summary>
    /// <param name="actionTypeMasterHashId"></param>
    /// <returns></returns>
    Task<ApiOutput> GetDDActivityTypeList(string actionTypeMasterHashId);

    /// <summary>
    /// get activity type based on hash id for edit
    /// </summary>
    /// <param name="activityTypeHashId"></param>
    /// <returns></returns>
    Task<ActivityTypeModel> GetActivityTypeByHashId(string activityTypeHashId);
  }
}