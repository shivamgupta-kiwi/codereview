using BCMStrategy.Common.Kendo;
using BCMStrategy.Data.Abstract.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.Abstract
{
  public interface IUserAccessRights
  {
    Task<ApiOutput> GetDDUserList();

    Task<ApiOutput> GetAllMenuList();

    Task<bool> UpdateUserAccessRights(UserAccessRightsModel userAccessRightsModel);

    Task<bool> DeleteUserAccessRights(string userAccessRightsMasterHashId);

    Task<ApiOutput> GetAllUserAccessRightsList(GridParameters parametersJson);

  }
}
