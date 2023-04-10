using BCMStrategy.Common.Kendo;
using BCMStrategy.DAL.Context;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Logger;
using BCMStrategy.Resources;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Repository.Concrete
{
  public class UserAccessRightsRepository : IUserAccessRights
  {
    
    public async Task<ApiOutput> GetDDUserList()
    {
      ApiOutput apiOutput = new ApiOutput();
      List<DropdownMaster> UserList;

      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        IQueryable<DropdownMaster> query = db.user
          .Where(x => !x.IsDeleted && x.IsActive == true)
            .Select(x => new DropdownMaster()
            {
              Key = x.Id,
              Value = x.EmailAddress
            }).OrderBy(x => x.Value).Take(50);
        UserList = await query.ToListAsync();
      }
      apiOutput.Data = UserList;
      apiOutput.TotalRecords = 0;
      apiOutput.ErrorMessage = UserList.Any() ? string.Empty : string.Format(Resource.ValidateMessageNoDataFound, Resource.LblUser);
      return apiOutput;
    }

    /// <summary>
    /// Add and update of activity type
    /// </summary>
    /// <param name="activityTypeModel">activitytype Model with activitytype values</param>
    /// <returns>Is Saved or not</returns>
    public async Task<bool> UpdateUserAccessRights(UserAccessRightsModel userAccessRightsModel)
    {
      bool isSave = false;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        DateTime currentTimeStamp = Helper.GetCurrentDateTime();

        if (string.IsNullOrEmpty(userAccessRightsModel.UserAccessRightsMasterHashId))
        {
          useraccessrights objUserAccessRights = new useraccessrights()
          {
            UserId = userAccessRightsModel.UserMasterHashId.ToDecrypt().ToInt32(),
            //SubMenuId = userAccessRightsModel.SubMenuId,
            Created = currentTimeStamp,
            CreatedBy = UserAccessHelper.CurrentUserIdentity.ToString()
          };
          db.useraccessrights.Add(objUserAccessRights);
        }
        else
        {
          int decryptUserAccessRightsId = userAccessRightsModel.UserAccessRightsMasterHashId.ToDecrypt().ToInt32();

          var objUserAccessRights = await db.useraccessrights.Where(x => x.Id == decryptUserAccessRightsId && !x.IsDeleted).FirstOrDefaultAsync();

          if (objUserAccessRights != null)
          {
            objUserAccessRights.Modified = currentTimeStamp;
            objUserAccessRights.ModifiedBy = UserAccessHelper.CurrentUserIdentity.ToString();
          }
        }
        isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
      }
      return isSave;
    }

    /// <summary>
    /// Delete user access rights
    /// </summary>
    /// <param name="userAccessRightsMasterHashId"></param>
    /// <returns>return successfull message</returns>
    public async Task<bool> DeleteUserAccessRights(string userAccessRightsMasterHashId)
    {
      bool isSave = false;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {

        if (!string.IsNullOrEmpty(userAccessRightsMasterHashId))
        {
          int decryptUserAccessRightsId = userAccessRightsMasterHashId.ToDecrypt().ToInt32();

          var objUserAccessRights = await db.useraccessrights.Where(x => x.Id == decryptUserAccessRightsId && !x.IsDeleted).FirstOrDefaultAsync();

          if (objUserAccessRights != null)
          {
            ////objUserAccessRights.IsDeleted = true;
          }
        }
        isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
      }
      return isSave;
    }

    /// <summary>
    /// Get all the list Activity type
    /// </summary>
    /// <param name="parametersJson">Grid Parameter to filter or sorting</param>
    /// <returns>return the list</returns>
    public async Task<ApiOutput> GetAllUserAccessRightsList(GridParameters parametersJson)
    {
      ApiOutput apiOutput = new ApiOutput();
      List<UserAccessRightsModel> userAccessRightsList;
      int totalRecord = 0;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        IQueryable<UserAccessRightsModel> query = db.useraccessrights
          .Where(x => !x.IsDeleted)
            .Select(x => new UserAccessRightsModel()
            {
              UserAccessRightsMasterId = x.Id,
              UserEmail = x.user.EmailAddress
            });
        if (parametersJson.Sort == null || parametersJson.Sort.Count == 0)
        {
          query = query.OrderByDescending(x => x.UserAccessRightsMasterId);
        }
        userAccessRightsList = await query.ModifyList(parametersJson, out totalRecord).ToListAsync();
      }
      apiOutput.Data = userAccessRightsList;
      apiOutput.TotalRecords = totalRecord;
      return apiOutput;
    }

    public async Task<ApiOutput> GetAllMenuList()
    {
      ApiOutput apiOutput = new ApiOutput();
      List<ParentMenuModel> parentMenuList;
      int totalRecord = 0;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        parentMenuList = (from data in db.parentmenu
                          select new ParentMenuModel
                          {
                            ParentMenuId = data.Id,
                            ParentMenuName = data.MenuName
                          }).ToList();

        foreach (var data in parentMenuList)
        {
          List<SubMenuModel> SubMenuList = (from d in db.submenu
                                            where d.ParentMenuId == data.ParentMenuId
                                            select new SubMenuModel
                                            {
                                              SubMenuMasterId = d.Id,
                                              SubMenuName = d.SubMenuName,
                                            }).ToList();
          data.SubMenuList = SubMenuList;
        }
      }
      apiOutput.Data = parentMenuList;
      apiOutput.TotalRecords = totalRecord;
      return apiOutput;
    }
  }
}
