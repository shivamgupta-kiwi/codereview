using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using BCMStrategy.DAL.Context;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.Provider;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Logger;

namespace BCMStrategy.Data.Repository.Provider
{
  /// <summary>
  /// class Authentication  Repository
  /// </summary>
  public class AuthRepository : IAuth
  {
    /// <summary>
    /// Event logger for logging
    /// </summary>
    private static readonly EventLogger<AuthRepository> _Log = new EventLogger<AuthRepository>();

    /// <summary>
    /// Find client id from database
    /// </summary>
    /// <param name="userId">User Id</param>
    /// <param name="password">User Password</param>
    /// <returns>result for weather user id exist or not</returns>
    public async Task<UserModel> FindUser(string userId, string password)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        var userDetail = await db.user
                                .Where(x => x.EmailAddress.ToLower() == userId.ToLower() && !x.IsDeleted).FirstOrDefaultAsync();

        if (userDetail != null)
        {

          if (!userDetail.IsActive.Value)
          {
            return new UserModel()
            {
              UserId = userDetail.Id,
              EmailAddress = userDetail.EmailAddress,
              FirstName = userDetail.FirstName,
              LastName = userDetail.LastName,
              UserType = userDetail.UserType,
              Active = userDetail.IsActive.HasValue ? userDetail.IsActive.Value : Helper.saveChangesNotSuccessful
            };
          }
          else if (userDetail.PasswordHash == Helper.GetPasswordHash(password, userDetail.PasswordSalt))
          {
            return new UserModel()
            {
              UserId = userDetail.Id,
              EmailAddress = userDetail.EmailAddress,
              FirstName = userDetail.FirstName,
              LastName = userDetail.LastName,
              UserType = userDetail.UserType,
              Active = userDetail.IsActive.HasValue ? userDetail.IsActive.Value : Helper.saveChangesNotSuccessful
            };
          }
        }
      }
      return null;
    }

    public ApiClient FindClient(string clientId)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        var client = db.apiclient.Find(clientId);
        return client.ToViewModel();
      }
    }

    /// <summary>
    /// Add Token in the database for user
    /// </summary>
    /// <param name="token">The token.</param>
    /// <returns>token will be stored in database</returns>
    public async Task<bool> AddRefreshToken(RefreshToken token)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        refreshtoken existingToken = await db.refreshtoken.Where(r => r.UserId == token.UserID && r.ClientId == token.ClientID).FirstOrDefaultAsync();

        if (existingToken != null)
        {
          db.refreshtoken.Remove(existingToken);
          await db.SaveChangesAsync();
        }

        db.refreshtoken.Add(token.ToDBModel());

        return await db.SaveChangesAsync() > 0;
      }
    }

    /// <summary>
    /// find refresh token
    /// </summary>
    /// <param name="refreshTokenId">The hashedTokenId.</param>
    /// <returns>FindRefreshToken token from  database</returns>
    public async Task<RefreshToken> FindRefreshTokenAsync(string refreshTokenId)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        var refreshToken = await db.refreshtoken.FindAsync(refreshTokenId);
        return refreshToken.ToViewModel();
      }
    }

    /// <summary>
    /// remove token from database
    /// </summary>
    /// <param name="refreshTokenId">The hashedTokenId.</param>
    /// <returns>remove token from  database</returns>
    public async Task<bool> RemoveRefreshToken(string refreshTokenId)
    {
      _Log.LogEntry(refreshTokenId);
      try
      {

        using (BCMStrategyEntities db = new BCMStrategyEntities())
        {
          int userId = refreshTokenId.ToDecrypt().ToInt32();
          var refreshToken = await db.refreshtoken.Where(x => x.UserId == userId).ToListAsync();

          if (refreshToken != null)
          {
            db.refreshtoken.RemoveRange(refreshToken);
            await db.SaveChangesAsync();
          }
        }
      }
      catch (Exception e)
      {
        _Log.LogEntry(e);
      }
      return true;
    }
  }
}