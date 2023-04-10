using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using BCMStrategy.Common.AuditLog;
using BCMStrategy.Common.Email;
using BCMStrategy.Common.Kendo;
using BCMStrategy.Common.Unity;
using BCMStrategy.DAL.Context;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Email;
using BCMStrategy.Logger;
using BCMStrategy.Resources;

namespace BCMStrategy.Data.Repository.Concrete
{
  public class UserMasterRepository : IUserMaster
  {
    private static readonly EventLogger<UserMasterRepository> _log = new EventLogger<UserMasterRepository>();

    private IEmailHelper _emailHelper;

    private IEmailHelper EmailHelper
    {
      get
      {
        if (_emailHelper == null)
        {
          _emailHelper = new EmailHelper();
        }

        return _emailHelper;
      }
    }

    /// <summary>
    /// The _audit repository
    /// </summary>
    private IAuditLog _auditRepository;

    /// <summary>
    /// Gets the audit repository.
    /// </summary>
    /// <value>
    /// The audit repository.
    /// </value>
    private IAuditLog AuditRepository
    {
      get
      {
        if (this._auditRepository == null)
        {
          this._auditRepository = UnityHelper.Resolve<IAuditLog>();
        }

        return this._auditRepository;
      }
    }

    private ICommonRepository _commonRepository;

    private ICommonRepository CommonRepository
    {
      get
      {
        if (_commonRepository == null)
        {
          _commonRepository = new CommonRepository();
        }

        return _commonRepository;
      }
    }

    #region Operations

    /// <summary>
    /// User Insertion and Updation
    /// </summary>
    /// <param name="model">User Model</param>
    /// <returns>Return saved or not saved value</returns>
    public async Task<bool> UpdateUserMaster(UserModel model)
    {
      bool isSave = false;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        DateTime currentTimeStamp = Helper.GetCurrentDateTime();

        if (string.IsNullOrEmpty(model.UserMasterHashId))
        {
          string randomSalt = Helper.GetRandomPasswordSalt();
          string randomPassword = Helper.GetRandomPassword();

          user objUser = new user()
          {
            PasswordHash = Helper.GetPasswordHash(randomPassword, randomSalt),
            PasswordSalt = randomSalt,
            FirstName = model.FirstName,
            MiddleName = model.MiddleName,
            LastName = model.LastName,
            EmailAddress = model.EmailAddress,
            UserType = model.UserType,
            CountryId = model.CountryMasterHashId.ToDecrypt().ToInt32(),
            State = model.State,
            City = model.City,
            ZipCode = model.ZipCode,
            Created = currentTimeStamp,
            Address = model.Address,
            CreatedBy = UserAccessHelper.CurrentUserIdentity.ToString(),
            IsActive = false,
            CompanyName = model.CompanyName,
            Title = model.Designation
          };
          db.user.Add(objUser);
          isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
          if (isSave)
          {
            await SendConfirmationMail(model);
          }

          UserManagementAuditViewModel auditmodel = GetUserManagementAuditViewModel(objUser);

          if (objUser.UserType == "CUSTOMER")
            Task.Run(() => AuditRepository.WriteAudit<UserManagementAuditViewModel>(AuditConstants.CustomerUser, AuditType.Insert, null, auditmodel, AuditConstants.InsertSuccessMsg));

          if (objUser.UserType == "ADMIN")
            Task.Run(() => AuditRepository.WriteAudit<UserManagementAuditViewModel>(AuditConstants.AdminUser, AuditType.Insert, null, auditmodel, AuditConstants.InsertSuccessMsg));
        }
        else
        {
          int decryptUserId = model.UserMasterHashId.ToDecrypt().ToInt32();

          var objUser = await db.user.Where(x => x.Id == decryptUserId).FirstOrDefaultAsync();
          UserManagementAuditViewModel beforeModel = GetUserManagementAuditViewModel(objUser);
          if (objUser != null)
          {
            objUser.FirstName = model.FirstName;
            objUser.MiddleName = model.MiddleName;
            objUser.LastName = model.LastName;
            objUser.CountryId = model.CountryMasterHashId.ToDecrypt().ToInt32();
            objUser.Address = model.Address;
            objUser.State = model.State;
            objUser.City = model.City;
            objUser.ZipCode = model.ZipCode;
            objUser.Modified = currentTimeStamp;
            objUser.ModifiedBy = UserAccessHelper.CurrentUserIdentity.ToString();
            objUser.IsActive = (model.Status == BCMStrategy.Resources.Enums.Status.Inactive.ToString() ? Helper.saveChangesNotSuccessful : Helper.saveChangesSuccessful);
            objUser.CompanyName = model.CompanyName;
            objUser.Title = model.Designation;
            isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;

            UserManagementAuditViewModel afterModel = GetUserManagementAuditViewModel(objUser);

            if (objUser.UserType == "CUSTOMER")
              Task.Run(() => AuditRepository.WriteAudit<UserManagementAuditViewModel>(CustomerAuditConstants.CustomerProfileUpdate, AuditType.Update, beforeModel, afterModel, AuditConstants.UpdateSuccessMsg, decryptUserId));

            if (objUser.UserType == "ADMIN")
              Task.Run(() => AuditRepository.WriteAudit<UserManagementAuditViewModel>(AuditConstants.AdminProfileUpdate, AuditType.Update, beforeModel, afterModel, AuditConstants.UpdateSuccessMsg, decryptUserId));
          }
        }
      }
      return isSave;
    }

    private UserManagementAuditViewModel GetUserManagementAuditViewModel(user userModel)
    {
      UserManagementAuditViewModel model = null;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        var userObj = db.user.Where(a => a.Id == userModel.Id).FirstOrDefault();
        model = new UserManagementAuditViewModel()
        {
          FirstName = userObj.FirstName,
          MiddleName = userObj.MiddleName,
          LastName = userObj.LastName,
          Country = userObj.country != null ? userObj.country.Name : string.Empty,
          Address = userObj.Address,
          State = userObj.State,
          City = userObj.City,
          ZipCode = userObj.ZipCode,
          Created = userObj.Created.HasValue ? Helper.GetCurrentFormatedDateTime() : string.Empty,
          Modified = userObj.Modified.HasValue ? userObj.Modified.ToFormatedDateTime() : string.Empty,
          CreatedBy = userObj.CreatedBy,
          ModifiedBy = userObj.ModifiedBy
        };
      }
      return model;
    }

    ////private UserManagementChangePasswordModel GetChangePasswordAudiViewModel(ChangePasswordModel passwordModel)
    ////{
    ////  UserManagementChangePasswordModel model = null;

    ////  using (BCMStrategyEntities db = new BCMStrategyEntities())
    ////  {
    ////    model = new UserManagementChangePasswordModel()
    ////    {
    ////      ConfirmPassword = passwordModel.ConfirmPassword,
    ////      Modified = Helper.GetCurrentFormatedDateTime(),
    ////      NewPassword = passwordModel.NewPassword,
    ////      OldPassword = passwordModel.OldPassword
    ////    };
    ////  }

    ////  return model;
    ////}

    /// <summary>
    /// Logic for Forgot Password
    /// </summary>
    /// <param name="ForgotPasswordModel">Forgot Password Model</param>
    /// <returns>Sends Forgot password details</returns>
    public async Task<bool> ForgotPassword(ForgotPasswordModel model)
    {
      bool result = false;
      if (!string.IsNullOrWhiteSpace(model.EmailAddress))
      {
        using (BCMStrategyEntities db = new BCMStrategyEntities())
        {
          var userDetail = await db.user.Where(x => x.EmailAddress == model.EmailAddress && !x.IsDeleted).FirstOrDefaultAsync();
          if (userDetail != null && userDetail.IsActive == true)
          {
            result = await SendForgotPasswordEmail(userDetail, model.ResetOrForgotPasswordLink);
          }
          else if (userDetail != null && userDetail.IsActive == false)
          {
            UserModel userModel = new UserModel()
            {
              EmailAddress = userDetail.EmailAddress,
              FirstName = userDetail.FirstName,
              LastName = userDetail.LastName
            };
            result = await SendConfirmationMail(userModel);
          }
        }
      }

      return result;
    }

    public async Task<UserModel> GetUserByActivationCode(string userActivationCode)
    {
      UserModel userProfile;
      DateTime currentTimeStamp = Helper.GetCurrentDateTime();
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        userProfile = await (from x in db.useractivationlink.Where(x => x.UserActivationCode == userActivationCode
                                    && x.ExpiredDateTime > currentTimeStamp)
                             join y in db.user on x.UserId equals y.Id
                             where !y.IsDeleted
                             select new UserModel()
                             {
                               UserId = y.Id,
                               FirstName = y.FirstName,
                               LastName = y.LastName,
                               MiddleName = y.MiddleName,
                               EmailAddress = y.EmailAddress,
                               CountryMasterId = y.CountryId,
                               City = y.City,
                               State = y.State,
                               Address = y.Address,
                               ZipCode = y.ZipCode
                             }).FirstOrDefaultAsync();
      }

      return userProfile;
    }

    /// <summary>
    /// Updates the password.
    /// </summary>
    /// <param name="setPasswordModel">The set password model.</param>
    /// <returns>Update user password</returns>
    public async Task<ApiOutput> UpdatePassword(SetPasswordModel setPasswordModel)
    {
      ApiOutput apiOutput = new ApiOutput();
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        var userActivationLinkEntry = await db.useractivationlink.Where(x => x.UserActivationCode == setPasswordModel.HashKey).FirstOrDefaultAsync();
        if (userActivationLinkEntry != null)
        {
          if (userActivationLinkEntry.ForceReset == Helper.Activation.PASSWORD.ToString() && userActivationLinkEntry.ExpiredDateTime < Helper.GetCurrentDateTime())
          {
            apiOutput.Data = false;
            apiOutput.ErrorMessage = Resources.Resource.ValidationLinkExpired_String;
            apiOutput.TotalRecords = 0;
          }
          else
          {
            string randomSalt = Helper.GetRandomPasswordSalt();
            var userEntry = await db.user.Where(x => x.Id == userActivationLinkEntry.UserId && !x.IsDeleted).FirstOrDefaultAsync();
            userEntry.PasswordSalt = randomSalt;
            userEntry.PasswordHash = Helper.GetPasswordHash(setPasswordModel.Password, randomSalt);
            userEntry.IsDeleted = false;
            userEntry.IsActive = true;
            userEntry.Modified = Helper.GetCurrentDateTime();
            db.useractivationlink.Remove(userActivationLinkEntry);
            bool isUpdated = (await db.SaveChangesAsync() > 0);

            apiOutput.Data = isUpdated;
            apiOutput.ErrorMessage = Resources.Resource.SuccessfulPasswordSet;
            apiOutput.TotalRecords = 0;
          }
        }
        else
        {
          apiOutput.Data = false;
          apiOutput.ErrorMessage = Resources.Resource.ValidationLinkUsed_String;
          apiOutput.TotalRecords = 0;
        }
      }

      return apiOutput;
    }

    /// <summary>
    /// Change password
    /// </summary>
    /// <param name="model">Pass Change model details</param>
    /// <returns>return output with saved or not</returns>
    public async Task<ApiOutput> ChangePassword(ChangePasswordModel model)
    {
      ApiOutput apiOutput = new ApiOutput();
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        int userId = UserAccessHelper.CurrentUserIdentity;
        if (userId > 0)
        {
          var userDetails = await db.user.Where(x => x.Id == userId).FirstOrDefaultAsync();
          string randomSalt = Helper.GetRandomPasswordSalt();
          if (userDetails != null)
          {
            userDetails.PasswordHash = Helper.GetPasswordHash(model.NewPassword, randomSalt);
            userDetails.PasswordSalt = randomSalt;
            bool isUpdated = (await db.SaveChangesAsync() > 0);
            apiOutput.Data = isUpdated;
            apiOutput.ErrorMessage = Resources.Resource.ChangePasswordSuccessMessage;
            apiOutput.TotalRecords = 0;

            if (userDetails.UserType == Enums.UserType.CUSTOMER.ToString())
            {
              Task.Run(() => AuditRepository.WriteAudit<UserManagementAuditViewModel>(CustomerAuditConstants.ChangePassword, AuditType.Update, null, null, AuditConstants.ChangeCredentialSuccessful, userId));
            }
            else if (userDetails.UserType == Enums.UserType.ADMIN.ToString())
            {
              Task.Run(() => AuditRepository.WriteAudit<UserManagementAuditViewModel>(CustomerAuditConstants.ChangePassword, AuditType.Update, null, null, AuditConstants.ChangeCredentialSuccessful));
            }
          }
          else
          {
            apiOutput.Data = false;
            apiOutput.ErrorMessage = Resources.Resource.InternalServerError;
            apiOutput.TotalRecords = 0;
          }
        }
        return apiOutput;
      }
    }

    /// <summary>
    /// Get User detail based on UserId
    /// </summary>
    /// <param name="userHashId">User Id</param>
    /// <returns>return user detail</returns>
    public async Task<UserModel> GetUserByHashID(string userHashId)
    {
      UserModel userMaster;
      int decryptUserId = userHashId.ToDecrypt().ToInt32();
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        userMaster = await (from x in db.user.Where(x => x.Id == decryptUserId && !x.IsDeleted)
                            select new UserModel()
                            {
                              UserId = x.Id,
                              EmailAddress = x.EmailAddress,
                              FirstName = x.FirstName,
                              LastName = x.LastName,
                              MiddleName = x.MiddleName,
                              Address = x.Address,
                              City = x.City,
                              CountryMasterId = x.CountryId,
                              State = x.State,
                              ZipCode = x.ZipCode,
                              Active = x.IsActive.Value,
                              CompanyName = x.CompanyName,
                              Designation = x.Title
                            }).FirstOrDefaultAsync();
      }
      return userMaster;
    }

    /// <summary>
    /// Get the State Head list
    /// </summary>
    /// <returns>State Head list </returns>
    public async Task<ApiOutput> GetUserManagementList(GridParameters parametersJson, string userType)
    {
      ApiOutput apiOutput = new ApiOutput();
      
      List<UserModel> stateHeadList;
      int totalRecord = 0;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        IQueryable<UserModel> query = db.user
          .Where(x => !x.IsDeleted && x.UserType == userType && x.Id != UserAccessHelper.CurrentUserIdentity)
            .Select(x => new UserModel()
            {
              UserId = x.Id,
              FirstName = x.FirstName,
              LastName = x.LastName,
              MiddleName = x.MiddleName,
              EmailAddress = x.EmailAddress,
              Address = x.Address,
              CountryMasterId = x.CountryId,
              State = x.State,
              City = x.City,
              ZipCode = x.ZipCode,
              UserType = x.UserType,
              Active = x.IsActive.Value,
              Status = x.IsActive.Value ? Enums.Status.Active.ToString() : Enums.Status.Inactive.ToString(),
              CompanyName = x.CompanyName,
              Designation = x.Title,
              DefaultLexicon = db.defaultlexiconterms.Count(y => y.UserId == x.Id) > 0 ? db.defaultlexiconterms.Count(y => y.UserId == x.Id) : 0,
            });
        if (parametersJson.Sort == null || parametersJson.Sort.Count == 0)
        {
          query = query.OrderByDescending(x => x.UserId);
        }
        stateHeadList = await query.ModifyList(parametersJson, out totalRecord).ToListAsync();
      }
      apiOutput.Data = stateHeadList;
      apiOutput.TotalRecords = totalRecord;
      return apiOutput;
    }

    /// <summary>
    /// Delete User
    /// </summary>
    /// <param name="userMasterHashId">User Id based on this record will deleted.</param>
    /// <returns>Deleted Records</returns>
    public async Task<bool> DeleteUser(string userMasterHashId)
    {
      bool isSave = false;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        ////DateTime currentTimeStamp = Helper.GetCurrentDateTime();

        if (!string.IsNullOrEmpty(userMasterHashId))
        {
          int decryptUserId = userMasterHashId.ToDecrypt().ToInt32();

          var objStateHead = await db.user.Where(x => x.Id == decryptUserId && !x.IsDeleted).FirstOrDefaultAsync();

          if (objStateHead != null)
          {
            objStateHead.IsDeleted = true;
          }
          isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
          UserManagementAuditViewModel model = GetUserManagementAuditViewModel(objStateHead);

          if (objStateHead != null && objStateHead.UserType == "CUSTOMER")
          {
            Task.Run(() => AuditRepository.WriteAudit<UserManagementAuditViewModel>(AuditConstants.CustomerUser, AuditType.Delete, model, null, AuditConstants.DeleteSuccessMsg));
          }
          else if (objStateHead != null && objStateHead.UserType == "ADMIN")
          {
            Task.Run(() => AuditRepository.WriteAudit<UserManagementAuditViewModel>(AuditConstants.AdminUser, AuditType.Delete, model, null, AuditConstants.DeleteSuccessMsg));
          }
        }

        ////isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
      }
      return isSave;
    }

    public List<UserModel> GetAllCustomerList(Enums.UserType userType)
    {
      List<UserModel> customerList;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        customerList = db.user
          .Where(x => !x.IsDeleted && x.IsActive == true && x.UserType == userType.ToString())
            .Select(x => new UserModel()
            {
              UserId = x.Id,
              FirstName = x.FirstName,
              LastName = x.LastName,
              MiddleName = x.MiddleName,
              EmailAddress = x.EmailAddress,
              Address = x.Address,
              CountryMasterId = x.CountryId,
              State = x.State,
              City = x.City,
              ZipCode = x.ZipCode,
              UserType = x.UserType,
              Active = x.IsActive.Value,
              Status = x.IsActive.Value ? Enums.Status.Active.ToString() : Enums.Status.Inactive.ToString(),
              CompanyName = x.CompanyName,
              Designation = x.Title
            }).ToList();
      }
      return customerList;
    }

    public async Task<ApiOutput> GetDefaultLexiconList(GridParameters parametersJson, string userMasterHashId)
    {
      ApiOutput apiOutput = new ApiOutput();

      int userId = userMasterHashId.ToDecrypt().ToInt32();

      List<LexiconModel> lexiconList;
      int totalRecord = 0;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        IQueryable<LexiconModel> query = (
                                          from sld in db.lexiconissues
                                          join dlt in db.defaultlexiconterms on sld.Id equals dlt.LexiconIssuesId
                                          where sld.IsDeleted == Helper.saveChangesNotSuccessful
                                          && dlt.UserId == userId
                                          select new LexiconModel
                                          {
                                            LexiconeIssueMasterId = sld.Id,
                                            LexiconeTypeMasterId = sld.LexiconTypeId,
                                            LexiconIssue = sld.LexiconIssue,
                                            CombinationValue = sld.CombinationValue,
                                            LexiconType = sld.lexicontype.Type,
                                            IsNested = sld.IsNested,
                                            Linker = sld.lexiconissuelinker.Where(s => !s.IsDeleted).Select(y => y.Linkers).ToList()
                                          });

        if (parametersJson.Sort == null || parametersJson.Sort.Count == 0)
        {
          query = query.OrderByDescending(x => x.LexiconeIssueMasterId);
        }
        lexiconList = await query.ModifyList(parametersJson, out totalRecord).ToListAsync();
      }

      lexiconList.ForEach(a =>
      {
        a.TotalRecordCount = totalRecord;
      });

      apiOutput.Data = lexiconList;
      apiOutput.TotalRecords = totalRecord;
      return apiOutput;
    }

    #endregion Operations

    #region Email Notification

    /// <summary>
    /// Send Confirmation mail to Registered User
    /// </summary>
    /// <param name="model">User Model</param>
    /// <returns>Return success</returns>
    public async Task<bool> SendConfirmationMail(UserModel model)
    {
      try
      {
        using (BCMStrategyEntities db = new BCMStrategyEntities())
        {
          var userModelId = db.user.Where(x => x.EmailAddress == model.EmailAddress && !x.IsDeleted).Select(x => x.Id).FirstOrDefault();
          var activationCode = Guid.NewGuid().ToString("N");
          var linkExpiryDateTime = Convert.ToInt32(await db.globalconfiguration.Where(x => x.Name == GlobalConfigurationKeys.EmailLinkExpirationTime).Select(y => y.Value).FirstOrDefaultAsync());
          useractivationlink userActivationlist = new useractivationlink()
          {
            UserId = userModelId,
            UserActivationCode = activationCode,
            ForceReset = Helper.Activation.PROFILE.ToString(),
            ExpiredDateTime = Helper.GetCurrentDateTime().AddMinutes(linkExpiryDateTime)
          };

          db.useractivationlink.Add(userActivationlist);

          bool isSave = (await db.SaveChangesAsync() > 0);

          if (isSave)
          {
            var emailTemplate = db.emailtemplate.Where(x => x.TemplateName == Helper.EMailTemplateName.REGISTRATION_TEMPLATE.ToString() && x.IsTemplateActive).FirstOrDefault();

            string emailBody = string.Empty;

            var mailFormat_Body = emailTemplate.BodyHtml;
            var subject = emailTemplate.Subject;
            string webApplicationURL = await CommonRepository.GetWebApplicationBasePath();
            string activationLink = string.Format("{0}{1}{2}", webApplicationURL, model.ResetOrForgotPasswordLink, userActivationlist.UserActivationCode);

            emailBody += mailFormat_Body.Replace("{@UserName@}", string.Format("{0} {1}", model.FirstName, model.LastName))
                                        .Replace("{@ActivationLink@}", activationLink);

            var emailConfiguration = await CommonRepository.GetEmailConfiguration();

            EmailHelper.Configuration = emailConfiguration;
            await Task.Run(() => EmailHelper.Send(model.EmailAddress, subject, emailBody));
          }
        }
      }
      catch (System.Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "SendConfirmationMail", "Exception is thrown.", ex, model);
      }
      return true;
    }

    /// <summary>
    /// Sends the activation email.
    /// </summary>
    /// <param name="userDetail">The user identifier.</param>
    /// <returns>User Detail</returns>
    private async Task<bool> SendForgotPasswordEmail(user userDetail, string resetLink)
    {
      try
      {
        using (BCMStrategyEntities db = new BCMStrategyEntities())
        {
          var activationCode = Guid.NewGuid().ToString("N");
          var linkExpiryDateTime = Convert.ToInt32(await db.globalconfiguration.Where(x => x.Name == GlobalConfigurationKeys.EmailLinkExpirationTime).Select(y => y.Value).FirstOrDefaultAsync());
          useractivationlink userActivationlist = new useractivationlink()
          {
            UserId = userDetail.Id,
            UserActivationCode = activationCode,
            ForceReset = Helper.Activation.PASSWORD.ToString(),
            ExpiredDateTime = Helper.GetCurrentDateTime().AddMinutes(linkExpiryDateTime)
          };

          db.useractivationlink.Add(userActivationlist);

          bool isSave = (await db.SaveChangesAsync() > 0);
          if (isSave && userDetail != null)
          {
            var emailTemplate = db.emailtemplate.Where(x => x.TemplateName == Helper.EMailTemplateName.FORGOTPASSWORD_TEMPLATE.ToString() && x.IsTemplateActive).FirstOrDefault();

            string emailBody = string.Empty;
            ////string linkPrefix = string.Empty;
            if (emailTemplate != null)
            {
              var mailFormat_Body = emailTemplate.BodyHtml;

              string webApplicationURL = await CommonRepository.GetWebApplicationBasePath();

              string activationLink = string.Format("{0}{1}{2}", webApplicationURL, resetLink, userActivationlist.UserActivationCode);
              var subject = emailTemplate.Subject;
              emailBody += mailFormat_Body.Replace("{%UserName%}", userDetail.FirstName)
                                          .Replace("{%PasswordResetLink%}", activationLink)
                                          .Replace("{%LinkExpiryTime%}", linkExpiryDateTime.ToString());

              var emailConfiguration = await CommonRepository.GetEmailConfiguration();

              EmailHelper.Configuration = emailConfiguration;
              await Task.Run(() => EmailHelper.Send(userDetail.EmailAddress, subject, emailBody));
            }
          }
        }
      }
      catch (System.Exception ex)
      {
        _log.LogError(LoggingLevel.Error, "SendForgotPasswordEmail", "Exception is thrown.", ex, userDetail);
      }
      return true;
    }

    #endregion Email Notification
  }
}