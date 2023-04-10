using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCMStrategy.Data.Abstract.ViewModels;

namespace BCMStrategy.Data.Abstract.Provider
{
  public interface IAuth
  {
    /// <summary>
    /// Find client id from database
    /// </summary>
    /// <param name="userId">User Id</param>
    /// <param name="password">User Password</param>
    /// <returns>result for weather user id exist or not</returns>
    Task<UserModel> FindUser(string userId, string password);

    /// <summary>
    /// Find the API Client Details based on Client Id
    /// </summary>
    /// <param name="clientId">Client Id</param>
    /// <returns>Return Client Details</returns>
    ApiClient FindClient(string clientId);

    /// <summary>
    /// Add Token in the database for user
    /// </summary>
    /// <param name="token">The token.</param>
    /// <returns>token will be stored in database</returns>
    Task<bool> AddRefreshToken(RefreshToken token);

    /// <summary>
    /// find refresh token
    /// </summary>
    /// <param name="refreshTokenId">The hashedTokenId.</param>
    /// <returns>FindRefreshToken token from  database</returns>
    Task<RefreshToken> FindRefreshTokenAsync(string refreshTokenId);

    /// <summary>
    /// remove token from database
    /// </summary>
    /// <param name="refreshTokenId">The hashedTokenId.</param>
    /// <returns>remove token from  database</returns>
    Task<bool> RemoveRefreshToken(string refreshTokenId);
  }
}
