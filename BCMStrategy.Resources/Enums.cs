using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Resources
{
  public class Enums
  {
    public enum UserType
    {
      /// <summary>
      /// Super Admin
      /// </summary>
      SUPERADMIN = 0,

      /// <summary>
      /// General Admin
      /// </summary>
      ADMIN = 1,

      /// <summary>
      /// Customer
      /// </summary>
      CUSTOMER = 2
    }

    public enum ModuleName
    {
      /// <summary>
      /// Institution Type
      /// </summary>
      INSTITTUTIONTYPE,
      /// <summary>
      /// DASHBOARD
      /// </summary>
      DASHBOARD,
      /// <summary>
      /// Admin User
      /// </summary>
      ADMINUSER,
      /// <summary>
      /// Customer User
      /// </summary>
      CUSTOMERUSER,
      /// <summary>
      /// Head state Government
      /// </summary>
      HEADSTATEGOVN,
      /// <summary>
      /// Policy Maker
      /// </summary>
      POLICYMAKER,
      /// <summary>
      /// Institutions
      /// </summary>
      INSTITUTIONS,
      /// <summary>
      /// User Profile
      /// </summary>
      USERPROFILE,
      /// <summary>
      /// International Organization
      /// </summary>
      INTERNATIONALORGANIZATION,

      /// <summary>
      /// Web Link Management
      /// </summary>
      WEBLINKMANAGMENT,

      /// <summary>
      /// Change Password
      /// </summary>
      CHANGEPASSWORD,

      /// <summary>
      /// Scrapping Process
      /// </summary>
      SCRAPPINGPROCESS,

      /// <summary>
      /// Lexicon
      /// </summary>
      LEXICON,

      /// <summary>
      /// LAGISLATOR
      /// </summary>
      LAGISLATORMANAGMENT,

      /// <summary>
      /// Metadata Types
      /// </summary>
      METADATATYPES,

      /// <summary>
      /// Activity Type
      /// </summary>
      ACTIVITYTYPE,

      /// <summary>
      /// LEXICONACCESSMANAGEMENT
      /// </summary>
      LEXICONACCESSMANAGEMENT,

      /// <summary>
      /// metadata phrases
      /// </summary>
      METADATAPHRASES,

      /// <summary>
      /// metadata noun plus verb
      /// </summary>
      METADATANOUNPLUSVERB,

      /// <summary>
      /// audit log
      /// </summary>
      AUDITLOG,

      /// <summary>
      /// user access rights
      /// </summary>
      USERACCESSRIGHTS,

      /// <summary>
      /// LEXICONACCESSCUSTOMER
      /// </summary>
      LEXICONACCESSCUSTOMER,
      /// <summary>
      /// scheduler
      /// </summary>
      SCHEDULER,

      /// <summary>
      /// scheduler
      /// </summary>
      PROCESSDETAIL,

      /// <summary>
      /// OFFICIAL SECTOR
      /// </summary>
      OFFICIAlSECTOR,

      /// <summary>
      /// MEDIA SECTOR
      /// </summary>
      MEDIASECTOR,

			/// <summary>
			/// Searchable PDF
			/// </summary>
			SEARCHABLEPDF,

      /// <summary>
      /// Customer Audit Log
      /// </summary>
      CUSTOMERAUDITLOG,

			/// <summary>
			/// Global Setting
			/// </summary>
			GLOBALSETTING
		}

    public enum Status
    {
      /// <summary>
      /// Active
      /// </summary>
      Active,

      /// <summary>
      /// InActive
      /// </summary>
      Inactive,
      /// <summary>
      /// Yes
      /// </summary>
      Yes,

      /// <summary>
      /// No
      /// </summary>
      No,

      /// <summary>
      /// NA
      /// </summary>
      NA,

      Official_Sector_Pages,

      Media_Pages,

      OfficialSector_1,

      MediaSector_1,

      MediaSector,

      OfficialSector,
    }

    public enum WeekDays
    {
      Sunday,
      Monday,
      Tuesday,
      Wednesday,
      Thursday,
      Friday,
      Saturday
    }

    public enum WebsiteType
    {
      MediaSector = 1,
      OfficialSector = 2
    }

    public enum ApiMetodCall
    {
      POST,
      GET,
      DELETE,
      PUT,
    }

    public enum TimeZone
    {
      UTC,
      Eastern_Standard_Time
    }

    public static string ActiveParentNavigation(Enums.ModuleName pageCurrentChildName, string pageParentName)
    {
      string activeFlag = string.Empty;

			if (pageParentName == Resource.LblDashboard)
			{
				switch (pageCurrentChildName)
				{
					case Enums.ModuleName.DASHBOARD:
						activeFlag = "active";
						break;
				}
			}
			else if (pageParentName == Resource.LblSearchablePDF)
			{
				switch (pageCurrentChildName)
				{
					case Enums.ModuleName.SEARCHABLEPDF:
						activeFlag = "active";
						break;
				}
			}
			else if (pageParentName == Resource.LblMenuMasterData)
			{
				switch (pageCurrentChildName)
				{
					case Enums.ModuleName.INSTITTUTIONTYPE:
					case Enums.ModuleName.INSTITUTIONS:
					case Enums.ModuleName.HEADSTATEGOVN:
					case Enums.ModuleName.POLICYMAKER:
					case Enums.ModuleName.INTERNATIONALORGANIZATION:
					case Enums.ModuleName.WEBLINKMANAGMENT:
					case Enums.ModuleName.LAGISLATORMANAGMENT:
					case Enums.ModuleName.LEXICON:
						activeFlag = "active";
						break;
				}
			}
			else if (pageParentName == Resource.LblMetaDataMgmt)
			{
				switch (pageCurrentChildName)
				{
					case Enums.ModuleName.METADATATYPES:
					case Enums.ModuleName.ACTIVITYTYPE:
					case Enums.ModuleName.METADATAPHRASES:
					case Enums.ModuleName.METADATANOUNPLUSVERB:
						activeFlag = "active";
						break;
				}
			}
			else if (pageParentName == Resource.LblUserManagement)
			{
				switch (pageCurrentChildName)
				{
					case Enums.ModuleName.ADMINUSER:
					case Enums.ModuleName.CUSTOMERUSER:
						activeFlag = "active";
						break;
				}
			}
			else if (pageParentName == Resource.LblPrivileges)
			{
				switch (pageCurrentChildName)
				{
					case Enums.ModuleName.LEXICONACCESSMANAGEMENT:
					case Enums.ModuleName.LEXICONACCESSCUSTOMER:
						activeFlag = "active";
						break;
				}
			}
			else if (pageParentName == Resource.LblScheduler)
			{
				switch (pageCurrentChildName)
				{
					case Enums.ModuleName.SCHEDULER:
						activeFlag = "active";
						break;
				}
			}
      else if (pageParentName == Resource.LblAuditLog)
			{
				switch (pageCurrentChildName)
				{
					case Enums.ModuleName.AUDITLOG:
          case Enums.ModuleName.CUSTOMERAUDITLOG:
						activeFlag = "active";
						break;
				}
			}
			else if (pageParentName == Resource.LblScrappingProcess)
			{
				switch (pageCurrentChildName)
				{
					case Enums.ModuleName.OFFICIAlSECTOR:
					case Enums.ModuleName.MEDIASECTOR:
						activeFlag = "active";
						break;
				}
			}
      return activeFlag;
    }

    public static string ActiveChildNavigation(Enums.ModuleName pageCurrentChildName, Enums.ModuleName pageChildName)
    {
      string activeFlag = string.Empty;

      if (pageCurrentChildName == pageChildName)
      {
        activeFlag = "active";
      }

      return activeFlag;
    }


  }


}
