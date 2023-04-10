using System.Collections.Generic;
using System.Threading.Tasks;
using BCMStrategy.Common.Kendo;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Resources;

namespace BCMStrategy.Data.Abstract.Abstract
{
	public interface IUserMaster
	{
		Task<bool> UpdateUserMaster(UserModel model);

		Task<bool> ForgotPassword(ForgotPasswordModel model);

		Task<UserModel> GetUserByActivationCode(string userActivationCode);

		Task<ApiOutput> UpdatePassword(SetPasswordModel setPasswordModel);

		Task<UserModel> GetUserByHashID(string userHashId);

		Task<ApiOutput> ChangePassword(ChangePasswordModel model);

		Task<ApiOutput> GetUserManagementList(GridParameters parametersJson, string userType);

		Task<bool> DeleteUser(string userMasterHashId);

		List<UserModel> GetAllCustomerList(Enums.UserType userType);

    /// <summary>
    /// Get Default Lexicon List
    /// </summary>
    /// <param name="parametersJson">parameters Json object</param>
    /// <param name="userMasterHashId">User Master Hash Id</param>
    /// <returns>Returns list of Default Lexicons</returns>
    Task<ApiOutput> GetDefaultLexiconList(GridParameters parametersJson, string userMasterHashId);
	}
}