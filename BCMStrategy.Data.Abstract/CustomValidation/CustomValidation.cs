using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using BCMStrategy.DAL.Context;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Resources;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Text;

namespace BCMStrategy.Data.Abstract.CustomValidation
{
  public class IsEmailIdExistAttribute : ValidationAttribute
  {
    public bool CheckForForgotPassword { get; set; }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (value != null)
        {
          var emailId = value.ToString();
          if (!CheckForForgotPassword)
          {
            UserModel registrationModel = (UserModel)validationContext.ObjectInstance;

            if (!string.IsNullOrEmpty(registrationModel.UserMasterHashId))
            {
              int userDecryptID = registrationModel.UserMasterHashId.ToDecrypt().ToInt32();

              bool isExist = db.user.Any(x => x.Id != userDecryptID && x.EmailAddress == emailId && !x.IsDeleted);
              if (isExist)
              {
                var errorMessage = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult(errorMessage);
              }
            }
            else
            {
              bool isExist = db.user.Any(x => x.EmailAddress == emailId && !x.IsDeleted);
              if (isExist)
              {
                var errorMessage = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult(errorMessage);
              }
            }
          }
          else
          {
            if (!string.IsNullOrEmpty(emailId) && ValidEmailAddress(emailId))
            {
              bool isExist = db.user.Any(x => x.EmailAddress == emailId && !x.IsDeleted);
              if (!isExist)
              {
                var errorMessage = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult(errorMessage);
              }
            }
          }
        }
        return ValidationResult.Success;
      }
    }

    private bool ValidEmailAddress(string emailAddress)
    {
      Regex regex = new Regex("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$");
      Match match = regex.Match(emailAddress);
      if (match.Success)
        return true;
      else
        return false;
    }
  }

  public class IsCountryExistAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (value != null)
        {
          var countryName = value.ToString();

          if (!string.IsNullOrEmpty(countryName))
          {
            bool isExist = db.country.Any(x => x.Name == countryName);
            if (!isExist)
            {
              var errorMessage = FormatErrorMessage(validationContext.DisplayName);
              return new ValidationResult(errorMessage);
            }
          }

        }
        return ValidationResult.Success;
      }
    }
  }

  public class IsCountryEuropeanUnionAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (value != null)
        {
          InstitutionModel institutionModel = (InstitutionModel)validationContext.ObjectInstance;
          int countryId = !string.IsNullOrEmpty(institutionModel.CountryMasterHashId) ? institutionModel.CountryMasterHashId.ToDecrypt().ToInt32() : 0;

          int institutionId = institutionModel.InstitutionTypeHashId.ToDecrypt().ToInt32();

          string institutionTypeName = db.institutiontypes.Where(x => x.Id == institutionId).Select(x => x.InstitutionType).FirstOrDefault();

          if (institutionTypeName != "Multilateral" && !institutionModel.IsEuropeanUnion && countryId == 0)
          {
            var errorMessage = string.Format(Resource.ValidateRequiredField, "Country");
            return new ValidationResult(errorMessage);
          }
        }
        return ValidationResult.Success;
      }
    }
  }

  public class IsHeadStateForImportExistAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (value != null)
        {
          var lastName = value.ToString();

          HeadStateImportModel headStateImportModel = (HeadStateImportModel)validationContext.ObjectInstance;

          int countryId = db.country.Where(x => x.Name == headStateImportModel.CountryName).Select(x => x.Id).FirstOrDefault();

          int designationId = headStateImportModel.DesignationMasterHashId.ToDecrypt().ToInt32();
          if (countryId > 0)
          {
            if (!string.IsNullOrEmpty(headStateImportModel.HeadStateMasterHashId))
            {
              int headStateDecryptID = headStateImportModel.HeadStateMasterHashId.ToDecrypt().ToInt32();

              if (db.statehead.Any(x => x.Id != headStateDecryptID && x.CountryId == countryId && x.DesignationId == designationId && (x.FirstName == (headStateImportModel.FirstName ?? string.Empty)) && x.LastName == lastName && !x.IsDeleted))
              {
                var errorMessage = string.Format(Resource.HeadOfStateExist, (headStateImportModel.FirstName + " " + lastName));
                return new ValidationResult(errorMessage);
              }
            }
            else
            {
              if (db.statehead.Any(x => x.CountryId == countryId && x.DesignationId == designationId && (x.FirstName == (headStateImportModel.FirstName ?? string.Empty)) && x.LastName == lastName && !x.IsDeleted))
              {
                var errorMessage = string.Format(Resource.HeadOfStateExist, (headStateImportModel.FirstName + " " + lastName));

                return new ValidationResult(errorMessage);
              }
            }
          }
        }
        return ValidationResult.Success;
      }
    }
  }

  public class IsHeadStateExistAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (value != null)
        {
          var lastName = value.ToString();

          StateHeadModel headStateModel = (StateHeadModel)validationContext.ObjectInstance;

          int countryId = headStateModel.CountryMasterHashId.ToDecrypt().ToInt32();
          int designationId = headStateModel.DesignationHashId.ToDecrypt().ToInt32();
          if (countryId > 0)
          {
            if (!string.IsNullOrEmpty(headStateModel.StateHeadMasterHashId))
            {
              int headStateDecryptID = headStateModel.StateHeadMasterHashId.ToDecrypt().ToInt32();

              if (db.statehead.Any(x => x.Id != headStateDecryptID && x.CountryId == countryId && x.DesignationId == designationId && (x.FirstName == (headStateModel.FirstName ?? string.Empty)) && x.LastName == lastName && !x.IsDeleted))
              {
                var errorMessage = string.Format(Resource.HeadOfStateExist, (headStateModel.FirstName + " " + lastName));
                return new ValidationResult(errorMessage);
              }
            }
            else
            {
              if (db.statehead.Any(x => x.CountryId == countryId && x.DesignationId == designationId && (x.FirstName == (headStateModel.FirstName ?? string.Empty)) && x.LastName == lastName && !x.IsDeleted))
              {
                var errorMessage = string.Format(Resource.HeadOfStateExist, (headStateModel.FirstName + " " + lastName));
                return new ValidationResult(errorMessage);
              }
            }
          }
        }
        return ValidationResult.Success;
      }
    }
  }

  public class ValidateOldPasswordAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (value != null)
        {
          var oldPassword = value.ToString();
          if (!string.IsNullOrEmpty(oldPassword))
          {
            int userId = UserAccessHelper.CurrentUserIdentity;

            if (userId > 0)
            {
              var userDetail = db.user.Where(x => x.Id == userId && !x.IsDeleted).FirstOrDefault();
              if (Helper.GetPasswordHash(oldPassword, userDetail.PasswordSalt) != userDetail.PasswordHash)
              {
                var errorMessage = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult(errorMessage);
              }
            }
          }
        }
        return ValidationResult.Success;
      }
    }
  }

  public class CompareOldNNewPasswordAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (value != null)
        {
          var newPassword = value.ToString();

          ChangePasswordModel changePasswordModel = (ChangePasswordModel)validationContext.ObjectInstance;

          if ((!string.IsNullOrEmpty(newPassword) && !string.IsNullOrEmpty(changePasswordModel.OldPassword)) && (newPassword == changePasswordModel.OldPassword))
          {
            var errorMessage = FormatErrorMessage(validationContext.DisplayName);
            return new ValidationResult(errorMessage);
          }
        }
        return ValidationResult.Success;
      }
    }
  }

  public class IsInstitutionsExistAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (value != null)
        {
          var institutionsName = value.ToString();

          InstitutionModel institutionModel = (InstitutionModel)validationContext.ObjectInstance;

          int? countryId = !string.IsNullOrEmpty(institutionModel.CountryMasterHashId) ? institutionModel.CountryMasterHashId.ToDecrypt().ToInt32() : (int?)null;
          int institutionTypeId = institutionModel.InstitutionTypeHashId.ToDecrypt().ToInt32();

          if (institutionTypeId > 0)
          {
            if (!string.IsNullOrEmpty(institutionModel.InstitutionMasterHashId))
            {
              int institutionDecryptID = institutionModel.InstitutionMasterHashId.ToDecrypt().ToInt32();

              bool isExist = db.institution.Any(x => x.Id != institutionDecryptID && x.CountryId == countryId && x.InstitutionTypeId == institutionTypeId && x.InstitutionName == institutionsName && !x.IsDeleted);
              if (isExist)
              {
                var errorMessage = string.Format(Resource.ValidateInstitutionExist, institutionsName);
                return new ValidationResult(errorMessage);
              }
            }
            else
            {
              bool isExist = db.institution.Any(x => x.CountryId == countryId && x.InstitutionTypeId == institutionTypeId && x.InstitutionName == institutionsName && !x.IsDeleted);
              if (isExist)
              {
                var errorMessage = string.Format(Resource.ValidateInstitutionExist, institutionsName);
                return new ValidationResult(errorMessage);
              }
            }


          }
        }
        return ValidationResult.Success;
      }
    }
  }

  public class IsInstitutionTypesExistAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (value != null)
        {
          var institionTypesName = value.ToString();

          InstitutionTypesModel institionTypesModel = (InstitutionTypesModel)validationContext.ObjectInstance;

          if (!string.IsNullOrEmpty(institionTypesName))
          {
            if (!string.IsNullOrEmpty(institionTypesModel.InstitutionTypesHashId))
            {
              int institionTypesDecryptID = institionTypesModel.InstitutionTypesHashId.ToDecrypt().ToInt32();

              if (db.institutiontypes.Any(x => x.Id != institionTypesDecryptID && x.InstitutionType == institionTypesName && !x.IsDeleted))
              {
                var errorMessage = string.Format(Resource.ValidateInstitutionTypesExist, institionTypesName);
                return new ValidationResult(errorMessage);
              }
            }
            else
            {
              if (db.institutiontypes.Any(x => x.InstitutionType == institionTypesName && !x.IsDeleted))
              {
                var errorMessage = string.Format(Resource.ValidateInstitutionTypesExist, institionTypesName);
                return new ValidationResult(errorMessage);
              }
            }
          }
        }
        return ValidationResult.Success;
      }
    }
  }

  public class IsInstitutionTypesForImportExistAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (value != null)
        {
          var institutionTypesName = value.ToString();

          bool isExist = db.institutiontypes.Any(x => x.InstitutionType == institutionTypesName && !x.IsDeleted);

          if (isExist)
          {
            var errorMessage = FormatErrorMessage(validationContext.DisplayName);
            return new ValidationResult(errorMessage);
          }
        }
        return ValidationResult.Success;
      }
    }
  }

  public class IsInstitutionsImportExistAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (value != null)
        {
          var institutionsName = value.ToString();

          InstitutionCsvImportModel institutionCSVImportModel = (InstitutionCsvImportModel)validationContext.ObjectInstance;

          int? countryId = !string.IsNullOrEmpty(institutionCSVImportModel.CountryMasterHashId) ? institutionCSVImportModel.CountryMasterHashId.ToDecrypt().ToInt32() : (int?)null;

          bool isExist = db.institution.Any(x => x.CountryId == countryId && x.InstitutionName == institutionsName && !x.IsDeleted);
          if (isExist)
          {
            var errorMessage = FormatErrorMessage(validationContext.DisplayName);
            return new ValidationResult(errorMessage);
          }
        }
      }
      return ValidationResult.Success;
    }
  }

  public class IsDesignationExistAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (value != null)
        {
          var designationName = value.ToString();
          if (!string.IsNullOrEmpty(designationName))
          {
            bool isExist = db.designation.Any(x => x.Designation1 == designationName);
            if (!isExist)
            {
              var errorMessage = FormatErrorMessage(validationContext.DisplayName);
              return new ValidationResult(errorMessage);
            }
          }
        }
        return ValidationResult.Success;
      }
    }
  }
  public class IsPolicyLastNameExistAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (value != null)
        {
          var lastName = value.ToString();

          PolicyMakerModel policyMakerModel = (PolicyMakerModel)validationContext.ObjectInstance;

          int countryId = policyMakerModel.CountryMasterHashId.ToDecrypt().ToInt32();
          int institutionTypeId = policyMakerModel.InstitutionTypeHashId.ToDecrypt().ToInt32();
          int designationId = policyMakerModel.DesignationHashId.ToDecrypt().ToInt32();
          if (countryId > 0 && institutionTypeId > 0)
          {
            if (!string.IsNullOrEmpty(policyMakerModel.PolicyMakerHashId))
            {
              int policyMakerDecryptID = policyMakerModel.PolicyMakerHashId.ToDecrypt().ToInt32();

              if (db.policymakers.Any(x => x.Id != policyMakerDecryptID && x.CountryId == countryId && x.InstitutionId == institutionTypeId && x.DesignationId == designationId && (x.PolicyFirstName == (policyMakerModel.PolicyFirstName ?? string.Empty)) && x.PolicyLastName == lastName && !x.IsDeleted))
              {

                var errorMessage = string.Format(Resource.ValidatePolicyLastNameExist, (policyMakerModel.PolicyFirstName + " " + lastName));
                return new ValidationResult(errorMessage);
              }
            }
            else
            {
              if (db.policymakers.Any(x => x.CountryId == countryId && x.InstitutionId == institutionTypeId && x.DesignationId == designationId && (x.PolicyFirstName == (policyMakerModel.PolicyFirstName ?? string.Empty)) && x.PolicyLastName == lastName && !x.IsDeleted))
              {
                var errorMessage = string.Format(Resource.ValidatePolicyLastNameExist, (policyMakerModel.PolicyFirstName + " " + lastName));
                return new ValidationResult(errorMessage);
              }
            }
          }
        }
        return ValidationResult.Success;
      }
    }
  }
  public class IsPolicyLastNameImportExistAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (value != null)
        {
          var policyLastName = value.ToString();

          if (!string.IsNullOrEmpty(policyLastName))
          {
            PolicyMakersCsvImportModel policyMakersCSVImportModel = (PolicyMakersCsvImportModel)validationContext.ObjectInstance;

            int countryId = policyMakersCSVImportModel.CountryMasterHashId.ToDecrypt().ToInt32();
            int institutionTypeId = policyMakersCSVImportModel.InstitutionTypeMasterHashId.ToDecrypt().ToInt32();
            int designationId = policyMakersCSVImportModel.DesignationMasterHashId.ToDecrypt().ToInt32();
            bool isExist = db.policymakers.Any(x => x.CountryId == countryId && x.InstitutionId == institutionTypeId && x.DesignationId == designationId && (x.PolicyFirstName == (policyMakersCSVImportModel.PolicyFirstName ?? "")) && x.PolicyLastName == policyLastName && !x.IsDeleted);
            if (isExist)
            {

              var errorMessage = string.Format(Resource.ValidatePolicyLastNameExist, (policyMakersCSVImportModel.PolicyFirstName + " " + policyLastName));
              return new ValidationResult(errorMessage);
            }
          }
        }
        return ValidationResult.Success;
      }
    }
  }

  public class IsInternationOrganizationExistAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (value != null)
        {
          var organizationName = value.ToString();

          InternationalOrganizationModel internationalOrganization = (InternationalOrganizationModel)validationContext.ObjectInstance;

          if (!string.IsNullOrEmpty(internationalOrganization.InternaltionalOrganizationMasterHashId))
          {
            int internationOrgDecryptID = internationalOrganization.InternaltionalOrganizationMasterHashId.ToDecrypt().ToInt32();

            bool isExist = db.internationalorganizations.Any(x => x.Id != internationOrgDecryptID && x.OrganizationName == organizationName && !x.IsDeleted);
            if (isExist)
            {
              var errorMessage = FormatErrorMessage(validationContext.DisplayName);
              return new ValidationResult(errorMessage);
            }
          }
          else
          {
            bool isExist = db.internationalorganizations.Any(x => x.OrganizationName == organizationName && !x.IsDeleted);
            if (isExist)
            {
              var errorMessage = FormatErrorMessage(validationContext.DisplayName);
              return new ValidationResult(errorMessage);
            }
          }

        }
        return ValidationResult.Success;
      }
    }
  }
  public class IsInternationOrganizationLastNameExistAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (value != null)
        {
          var lastName = value.ToString();

          InternationalOrganizationModel internationalOrganization = (InternationalOrganizationModel)validationContext.ObjectInstance;
          int internationDesigDecryptId = !string.IsNullOrEmpty(internationalOrganization.DesignationMasterHashId) ? internationalOrganization.DesignationMasterHashId.ToDecrypt().ToInt32() : 0;
          if (!string.IsNullOrEmpty(internationalOrganization.InternaltionalOrganizationMasterHashId))
          {
            int internationOrgDecryptID = internationalOrganization.InternaltionalOrganizationMasterHashId.ToDecrypt().ToInt32();

            bool isExist = db.internationalorganizations.Any(x => x.Id != internationOrgDecryptID && x.OrganizationName == internationalOrganization.OrganizationName && x.DesignationId == internationDesigDecryptId && (x.LeaderFirstName == (internationalOrganization.LeaderFirstName ?? string.Empty)) && x.LeaderLastName == lastName && x.EntityName == internationalOrganization.EntityName && !x.IsDeleted);
            if (isExist)
            {
              var errorMessage = string.Format(Resource.ValidateOrgLastNameExist, (internationalOrganization.LeaderFirstName + " " + lastName));
              return new ValidationResult(errorMessage);
            }
          }
          else
          {
            bool isExist = db.internationalorganizations.Any(x => x.OrganizationName == internationalOrganization.OrganizationName && x.DesignationId == internationDesigDecryptId && (x.LeaderFirstName == (internationalOrganization.LeaderFirstName ?? string.Empty)) && x.LeaderLastName == internationalOrganization.LeaderLastName && x.EntityName == internationalOrganization.EntityName && !x.IsDeleted);
            if (isExist)
            {
              var errorMessage = string.Format(Resource.ValidateOrgLastNameExist, (internationalOrganization.LeaderFirstName + " " + lastName));
              return new ValidationResult(errorMessage);
            }
          }

        }
        return ValidationResult.Success;
      }
    }
  }
  public class IsInternationOrganizationLastNameExistImportAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (value != null)
        {
          var lastName = value.ToString();

          InternationalOrganizationImportModel internationalOrganization = (InternationalOrganizationImportModel)validationContext.ObjectInstance;

          if (!string.IsNullOrEmpty(internationalOrganization.InternationalOrganizationMasterHashId))
          {
            int internationOrgDecryptID = internationalOrganization.InternationalOrganizationMasterHashId.ToDecrypt().ToInt32();

            bool isExist = db.internationalorganizations.Any(x => x.Id != internationOrgDecryptID && x.OrganizationName == internationalOrganization.OrganizationName && x.DesignationId == internationalOrganization.DesignationMasterId && (x.LeaderFirstName == (internationalOrganization.LeaderFirstName ?? string.Empty)) && x.LeaderLastName == lastName && x.EntityName == internationalOrganization.EntityName && !x.IsDeleted);
            if (isExist)
            {
              var errorMessage = string.Format(Resource.ValidateOrgLastNameExist, (internationalOrganization.LeaderFirstName + " " + lastName));

              return new ValidationResult(errorMessage);
            }
          }
          else
          {
            bool isExist = db.internationalorganizations.Any(x => x.OrganizationName == internationalOrganization.OrganizationName && x.DesignationId == internationalOrganization.DesignationMasterId && (x.LeaderFirstName == (internationalOrganization.LeaderFirstName ?? string.Empty)) && x.LeaderLastName == internationalOrganization.LeaderLastName && x.EntityName == internationalOrganization.EntityName && !x.IsDeleted);
            if (isExist)
            {
              var errorMessage = string.Format(Resource.ValidateOrgLastNameExist, (internationalOrganization.LeaderFirstName + " " + lastName));

              return new ValidationResult(errorMessage);
            }
          }

        }
        return ValidationResult.Success;
      }
    }
  }
  public class IsInternationOrganizationForImportExistAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (value != null)
        {
          var organizationName = value.ToString();
          bool isExist = db.internationalorganizations.Any(x => x.OrganizationName == organizationName && !x.IsDeleted);
          if (isExist)
          {
            var errorMessage = FormatErrorMessage(validationContext.DisplayName);
            return new ValidationResult(errorMessage);
          }
        }
        return ValidationResult.Success;
      }
    }
  }

  public class IsLexiconExistAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (value != null)
        {
          var lexiconTerm = value.ToString();

          LexiconModel lexiconModel = (LexiconModel)validationContext.ObjectInstance;
          int lexiconTypeId = lexiconModel.LexiconeTypeMasterHashId.ToDecrypt().ToInt32();
          bool IsLexiconNested = lexiconModel.IsNested;
          string CombinationValue = lexiconModel.CombinationValue;
          if (lexiconTypeId > 0)
          {
            if (!string.IsNullOrEmpty(lexiconModel.LexiconeIssueMasterHashId))
            {
              int lexiconDecryptID = lexiconModel.LexiconeIssueMasterHashId.ToDecrypt().ToInt32();
              bool isLexiconExist = false;
              if (IsLexiconNested)
              {
                isLexiconExist = db.lexiconissues.Any(x => x.Id != lexiconDecryptID && x.LexiconTypeId == lexiconTypeId && x.LexiconIssue == lexiconTerm && x.CombinationValue == CombinationValue && !x.IsDeleted);
              }
              else
              {
                isLexiconExist = db.lexiconissues.Any(x => x.Id != lexiconDecryptID && x.LexiconTypeId == lexiconTypeId && x.LexiconIssue == lexiconTerm && x.IsNested == IsLexiconNested && !x.IsDeleted);
              }
              if (isLexiconExist)
              {
                var errorMessage = string.Format(Resource.ValidateLexiconExist, lexiconTerm);
                return new ValidationResult(errorMessage);
              }
            }
            else
            {
              bool isExist = false;
              if (IsLexiconNested)
              {
                isExist = db.lexiconissues.Any(x => x.LexiconTypeId == lexiconTypeId && x.LexiconIssue == lexiconTerm && x.CombinationValue == CombinationValue && !x.IsDeleted);
              }
              else
              {
                isExist = db.lexiconissues.Any(x => x.LexiconTypeId == lexiconTypeId && x.LexiconIssue == lexiconTerm && x.IsNested == IsLexiconNested && !x.IsDeleted);
              }
              if (isExist)
              {
                var errorMessage = string.Format(Resource.ValidateLexiconExist, lexiconTerm);
                return new ValidationResult(errorMessage);
              }
            }
          }
        }
        return ValidationResult.Success;
      }
    }
  }

  public class IsLexiconImportExistAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (value != null)
        {
          var lexiconTerm = value.ToString();

          LexiconCsvImportModel lexiconCSVImportModel = (LexiconCsvImportModel)validationContext.ObjectInstance;
          int lexiconTypeId = lexiconCSVImportModel.LexiconTypeMasterHashId.ToDecrypt().ToInt32();
          bool IsNested = lexiconCSVImportModel.IsNested;
          string CombinationValue = lexiconCSVImportModel.CombinationValue;
          if (lexiconTypeId > 0)
          {
            if (!string.IsNullOrEmpty(lexiconCSVImportModel.LexiconIssueMasterHashId))
            {
              int lexiconDecryptID = lexiconCSVImportModel.LexiconIssueMasterHashId.ToDecrypt().ToInt32();
              bool isExist = false;
              if (IsNested)
              {
                isExist = db.lexiconissues.Any(x => x.Id != lexiconDecryptID && x.LexiconTypeId == lexiconTypeId && x.LexiconIssue == lexiconTerm && x.CombinationValue == CombinationValue && !x.IsDeleted);
              }
              else
              {
                isExist = db.lexiconissues.Any(x => x.Id != lexiconDecryptID && x.LexiconTypeId == lexiconTypeId && x.LexiconIssue == lexiconTerm && x.IsNested == IsNested && !x.IsDeleted);
              }
              if (isExist)
              {
                var errorMessage = string.Format(Resource.ValidateLexiconExist, lexiconTerm);
                return new ValidationResult(errorMessage);
              }
            }
            else
            {
              bool isExist = false;
              if (IsNested)
              {
                isExist = db.lexiconissues.Any(x => x.LexiconTypeId == lexiconTypeId && x.LexiconIssue == lexiconTerm && x.CombinationValue == CombinationValue && !x.IsDeleted);
              }
              else
              {
                isExist = db.lexiconissues.Any(x => x.LexiconTypeId == lexiconTypeId && x.LexiconIssue == lexiconTerm && x.IsNested == IsNested && !x.IsDeleted);
              }
              if (isExist)
              {
                var errorMessage = string.Format(Resource.ValidateLexiconExist, lexiconTerm);
                return new ValidationResult(errorMessage);
              }
            }
          }
        }
        return ValidationResult.Success;
      }
    }
  }

  public class CheckDuplicateStringAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (value != null)
        {
          string result = DuplicateString(value.ToString(), ",");
          if (result != "")
          {
            var errorMessage = string.Format(Resource.ValidateDuplicateValue, result);
            return new ValidationResult(errorMessage);
          }
        }
        return ValidationResult.Success;
      }
    }

    public string DuplicateString(string LexiconLinkers, string Separator)
    {
      StringBuilder DuplicateString = new StringBuilder();
      List<String> listOfValues = new List<string>();
      string[] values = LexiconLinkers.ToLower().Split(Separator.ToCharArray());
      foreach (string val in values)
      {
        if (listOfValues.Contains(val))
        {
          if (DuplicateString.ToString() == "")
          {
            DuplicateString.Append(val);
          }
          else
            DuplicateString.Append(DuplicateString + Separator + val);
        }
        else
          listOfValues.Add(val);
      }
      return DuplicateString.ToString();
    }
  }
  public class ListHasElementsAttribute : ValidationAttribute
  {
    public override bool IsValid(object value)
    {
      if (value == null)
        return false;

      var result = ((IEnumerable)value).Cast<object>().ToList();
      return result.Any();
    }
  }

  public class IsMetadataTypesExistAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (value != null)
        {
          var metaData = value.ToString();

          MetadataTypesModel metadataTypesModel = (MetadataTypesModel)validationContext.ObjectInstance;
          int websiteTypeId = metadataTypesModel.WebsiteTypeHashId.ToDecrypt().ToInt32();

          if (websiteTypeId > 0)
          {
            if (!string.IsNullOrEmpty(metadataTypesModel.MetadataTypesMasterHashId))
            {
              int metadataTypesDecryptID = metadataTypesModel.MetadataTypesMasterHashId.ToDecrypt().ToInt32();

              if (db.metadatatypes.Any(x => x.Id != metadataTypesDecryptID && x.WebSiteTypeId == websiteTypeId && x.MetaData == metaData && !x.IsDeleted))
              {
                var errorMessage = string.Format(Resource.ValidateMetadataTypesExist, metaData);
                return new ValidationResult(errorMessage);
              }
            }
            else
            {
              if (db.metadatatypes.Any(x => x.WebSiteTypeId == websiteTypeId && x.MetaData == metaData && !x.IsDeleted))
              {
                var errorMessage = string.Format(Resource.ValidateMetadataTypesExist, metaData);
                return new ValidationResult(errorMessage);
              }
            }
          }
        }
        return ValidationResult.Success;
      }
    }
  }

  public class IsActivityTypeExistAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (value != null)
        {
          var ActivityName = value.ToString();

          ActivityTypeModel activityTypeModel = (ActivityTypeModel)validationContext.ObjectInstance;

          int metadataTypeId = activityTypeModel.MetadataTypeMasterHashId.ToDecrypt().ToInt32();

          if (metadataTypeId > 0)
          {
            if (!string.IsNullOrEmpty(activityTypeModel.ActivityTypeMasterHashId))
            {
              int activityTypeDecryptID = activityTypeModel.ActivityTypeMasterHashId.ToDecrypt().ToInt32();

              bool isExist = db.metadatavalue.Where(w => w.MetaDataTypeId == metadataTypeId).Select(s => new
              {
                activityTypeId = s.activitytype.Id,
                activityName = s.activitytype.ActivityName,
                isdeleted = s.activitytype.IsDeleted
              }).Where(y => y.activityTypeId != activityTypeDecryptID && y.activityName == ActivityName && !y.isdeleted).Any();

              if (isExist)
              {
                var errorMessage = string.Format(Resource.ValidateActivityTypeExist, ActivityName);
                return new ValidationResult(errorMessage);
              }
            }
            else
            {
              bool isExist = db.metadatavalue.Where(w => w.MetaDataTypeId == metadataTypeId).Select(s => new
              {
                activityTypeId = s.activitytype.Id,
                activityName = s.activitytype.ActivityName,
                isdeleted = s.activitytype.IsDeleted
              }).Where(y => y.activityName == ActivityName && !y.isdeleted).Any();

              if (isExist)
              {
                var errorMessage = string.Format(Resource.ValidateActivityTypeExist, ActivityName);
                return new ValidationResult(errorMessage);
              }
            }
          }
        }
        return ValidationResult.Success;
      }
    }
  }

  public class IsMetadataPhrasesExistAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (value != null)
        {
          var metaDataPhrases = value.ToString();

          MetadataPhrasesModel metadataPhrasesModel = (MetadataPhrasesModel)validationContext.ObjectInstance;
          int metadataTypeId = metadataPhrasesModel.MetadataTypeMasterHashId.ToDecrypt().ToInt32();

          if (metadataTypeId > 0)
          {
            if (!string.IsNullOrEmpty(metadataPhrasesModel.MetadataPhrasesMasterHashId))
            {
              int metadataPhrasesDecryptID = metadataPhrasesModel.MetadataPhrasesMasterHashId.ToDecrypt().ToInt32();

              if (db.metadataphrases.Any(x => x.MetaDataTypeId == metadataTypeId && x.Id != metadataPhrasesDecryptID && x.Phrases == metaDataPhrases && !x.IsDeleted))
              {
                var errorMessage = string.Format(Resource.ValidateMetadataPhrasesExist, metaDataPhrases);
                return new ValidationResult(errorMessage);
              }
            }
            else
            {
              if (db.metadataphrases.Any(x => x.MetaDataTypeId == metadataTypeId && x.Phrases == metaDataPhrases && !x.IsDeleted))
              {
                var errorMessage = string.Format(Resource.ValidateMetadataPhrasesExist, metaDataPhrases);
                return new ValidationResult(errorMessage);
              }
            }
          }
        }
        return ValidationResult.Success;
      }
    }
  }

  public class IsMetadataNounExistAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (value != null)
        {
          var Noun = value.ToString();

          MetadataNounplusVerbModel metadataNounplusVerbModel = (MetadataNounplusVerbModel)validationContext.ObjectInstance;
          int metadataTypeId = metadataNounplusVerbModel.MetadataTypeMasterHashId.ToDecrypt().ToInt32();
          string verb = metadataNounplusVerbModel.Verb;
          if (metadataTypeId > 0)
          {
            if (!string.IsNullOrEmpty(metadataNounplusVerbModel.MetadataNounplusVerbMasterHashId))
            {
              int metadataNounplusVerbDecryptID = metadataNounplusVerbModel.MetadataNounplusVerbMasterHashId.ToDecrypt().ToInt32();

              bool isExist = db.metadatanounplusverb.Any(x => x.Id != metadataNounplusVerbDecryptID && x.Noun == Noun && x.Verb == verb && !x.IsDeleted);
              if (isExist)
              {
                var errorMessage = string.Format(Resource.ValidateMetadataNounExist, Noun);
                return new ValidationResult(errorMessage);
              }
            }
            else
            {
              bool isExist = db.metadatanounplusverb.Any(x => x.MetaDataTypeId == metadataTypeId && x.Noun == Noun && x.Verb == verb && !x.IsDeleted);
              if (isExist)
              {
                var errorMessage = string.Format(Resource.ValidateMetadataNounExist, Noun);
                return new ValidationResult(errorMessage);
              }
            }
          }
        }
        return ValidationResult.Success;
      }
    }
  }

  public class IsMetadataVerbExistAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (value != null)
        {
          var Verb = value.ToString();

          MetadataNounplusVerbModel metadataNounplusVerbModel = (MetadataNounplusVerbModel)validationContext.ObjectInstance;
          int metadataTypeId = metadataNounplusVerbModel.MetadataTypeMasterHashId.ToDecrypt().ToInt32();
          string noun = metadataNounplusVerbModel.IsHardCoded ? metadataNounplusVerbModel.Noun.Trim() : metadataNounplusVerbModel.MetadataDynamicNounplusVerb.Trim();
          if (metadataTypeId > 0)
          {
            if (!string.IsNullOrEmpty(metadataNounplusVerbModel.MetadataNounplusVerbMasterHashId))
            {
              int metadataNounplusVerbDecryptID = metadataNounplusVerbModel.MetadataNounplusVerbMasterHashId.ToDecrypt().ToInt32();

              if (db.metadatanounplusverb.Any(x => x.Id != metadataNounplusVerbDecryptID && x.MetaDataTypeId == metadataTypeId && x.Verb == Verb && x.Noun == noun && !x.IsDeleted))
              {
                var errorMessage = string.Format(Resource.ValidateMetadataVerbExist, Verb);
                return new ValidationResult(errorMessage);
              }
            }
            else
            {
              if (db.metadatanounplusverb.Any(x => x.MetaDataTypeId == metadataTypeId && x.Verb == Verb && x.Noun == noun && !x.IsDeleted))
              {
                var errorMessage = string.Format(Resource.ValidateMetadataVerbExist, Verb);
                return new ValidationResult(errorMessage);
              }
            }
          }
        }
        return ValidationResult.Success;
      }
    }
  }

  public class IsMetadataNounVerbDuplicateAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        MetadataNounplusVerbModel metadataNounplusVerbModel = (MetadataNounplusVerbModel)validationContext.ObjectInstance;

        if ((metadataNounplusVerbModel.IsHardCoded ? metadataNounplusVerbModel.Noun.Trim().ToLower() : metadataNounplusVerbModel.MetadataDynamicNounplusVerb.Trim().ToLower()) == metadataNounplusVerbModel.Verb.Trim().ToLower() && (metadataNounplusVerbModel.Noun.Trim() != "" || metadataNounplusVerbModel.Verb.Trim() != ""))
        {
          var errorMessage = Resource.ValidateNounVerbDiplicate;
          return new ValidationResult(errorMessage);
        }
        return ValidationResult.Success;
      }
    }
  }

  public class EndDateGreaterThanStartDateAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        SchedulerModel schedulerModel = (SchedulerModel)validationContext.ObjectInstance;
        if (value != null && value.ToString() != "" && schedulerModel.StartDateFinal != "")
        {
          DateTime startDate = Convert.ToDateTime(schedulerModel.StartDateFinal);
          DateTime endDate = Convert.ToDateTime(value.ToString());
          if (endDate < startDate)
          {
            var errorMessage = Resource.ValidateEnddateGreaterthenStartdate;
            return new ValidationResult(errorMessage);
          }
        }
        return ValidationResult.Success;

      }
    }
  }

  public class ValidateMinimumHoursAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        SchedulerModel schedulerModel = (SchedulerModel)validationContext.ObjectInstance;
        if (schedulerModel.WebsiteType != "" && value != null && value.ToString() != "")
        {
          string WebsiteType = schedulerModel.WebsiteType;
          double RepeatHour = schedulerModel.RepeatEveryHour.ToInt32();
          int HoursForOfficialSector = 12;
          int HoursForMediaSector = 3;

          if (WebsiteType == Resources.Enums.Status.Official_Sector_Pages.ToString().Replace("_", " "))
          {
            if (RepeatHour < HoursForOfficialSector)
            {
              var errorMessage = string.Format(Resource.ValidateMin12_3Hour, HoursForOfficialSector, WebsiteType);
              return new ValidationResult(errorMessage);
            }
          }
          else
            if ((WebsiteType == Resources.Enums.Status.Media_Pages.ToString().Replace("_", " ")) && (RepeatHour < HoursForMediaSector))
            {
              var errorMessage = string.Format(Resource.ValidateMin12_3Hour, HoursForMediaSector, WebsiteType);
              return new ValidationResult(errorMessage);
            }
        }
        return ValidationResult.Success;
      }
    }
  }

  public class IsSchedulerExistAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (value != null)
        {
          var schedulerName = value.ToString();

          SchedulerModel schedulerModel = (SchedulerModel)validationContext.ObjectInstance;
          int frquencyTypeId = schedulerModel.FrequencyTypeMasterHashId.ToDecrypt().ToInt32();

          if (frquencyTypeId > 0)
          {
            if (!string.IsNullOrEmpty(schedulerModel.SchedulerMasterHashId))
            {
              int schedulerDecryptID = schedulerModel.SchedulerMasterHashId.ToDecrypt().ToInt32();

              if (db.schedular.Any(x => x.Id != schedulerDecryptID && x.Name == schedulerName && !x.IsDeleted))
              {
                var errorMessage = string.Format(Resource.ValidateSchedulerExist, schedulerName);
                return new ValidationResult(errorMessage);
              }
            }
            else
            {
              if (db.schedular.Any(x => x.Name == schedulerName && !x.IsDeleted))
              {
                var errorMessage = string.Format(Resource.ValidateSchedulerExist, schedulerName);
                return new ValidationResult(errorMessage);
              }
            }
          }
        }
        return ValidationResult.Success;
      }
    }
  }


  public class IsPhraseActivityTypeAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (value != null)
        {
          var activityTypeHashId = value.ToString();
          if (string.IsNullOrEmpty(activityTypeHashId))
          {
            MetadataPhrasesModel metadataPhrasesModel = (MetadataPhrasesModel)validationContext.ObjectInstance;
            int MetaDataTypeId = metadataPhrasesModel.MetadataTypeMasterHashId.ToDecrypt().ToInt32();

            if (MetaDataTypeId > 0)
            {
              var metaDateTypes = db.metadatatypes.Where(x => x.Id == MetaDataTypeId).FirstOrDefault();
              if (metaDateTypes.IsActivityTypeExist.HasValue && metaDateTypes.IsActivityTypeExist.Value)
              {
                var errorMessage = string.Format(Resource.ValidationMsgForPhraseActivtyType, metaDateTypes.MetaData);
                return new ValidationResult(errorMessage);
              }
            }
          }
        }
        return ValidationResult.Success;
      }
    }
  }

  public class IsNounPlusVerbActivityTypeAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (value != null)
        {
          var activityTypeHashId = value.ToString();
          if (string.IsNullOrEmpty(activityTypeHashId))
          {
            MetadataNounplusVerbModel metadataPhrasesModel = (MetadataNounplusVerbModel)validationContext.ObjectInstance;
            int MetaDataTypeId = metadataPhrasesModel.MetadataTypeMasterHashId.ToDecrypt().ToInt32();

            if (MetaDataTypeId > 0)
            {
              var metaDateTypes = db.metadatatypes.Where(x => x.Id == MetaDataTypeId).FirstOrDefault();
              if (metaDateTypes.IsActivityTypeExist.HasValue && metaDateTypes.IsActivityTypeExist.Value)
              {
                var errorMessage = string.Format(Resource.ValidationMsgForPhraseActivtyType, metaDateTypes.MetaData);
                return new ValidationResult(errorMessage);
              }
            }
          }
        }
        return ValidationResult.Success;
      }
    }
  }
}