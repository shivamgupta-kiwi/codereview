namespace BCMStrategy.Common.AuditLog
{
	/// <summary>
	/// Audit Constants
	/// </summary>
	public static class AuditConstants
	{
		
		/// <summary>
		/// Status Code For Admin Table
		/// </summary>
		public const string InstitutionType = "INSTITUTIONTYPE";

		/// <summary>
		/// Insert Message
		/// </summary>
		public const string InsertSuccessMsg = "Record inserted Successfully";

    public const string ImportInsertSuccessMsg = "Import: Record inserted Successfully";

		/// <summary>
		/// Update Message
		/// </summary>
		public const string UpdateSuccessMsg = "Record updated Successfully";

		/// <summary>
		/// Delete Message
		/// </summary>
		public const string DeleteSuccessMsg = "Record Deleted Successfully";

    public const string LoginSuccessful = "Login Successful";

    public const string LoginNotSuccessful = "Login not Successful";

    public const string LogOutSuccessful = "Logout Successful";

    public const string ChangeCredentialSuccessful = "Changed Credentials Successfully";

		/// <summary>
		/// Institution
		/// </summary>
		public static string Institution
		{
			get
			{
				return "INSTITUTION";
			}
		}

		/// <summary>
		/// Policy Maker
		/// </summary>
		public static string PolicyMaker
		{
			get
			{
				return "POLICYMAKERS";
			}
		}

		/// <summary>
		/// Head Of State And Government
		/// </summary>
		public static string HeadOfStateAndGovrnment
		{
			get
			{
				return "HEADOFSTATEANDGOVERNMENT";
			}
		}

		/// <summary>
		/// Internal Organization
		/// </summary>
		public static string InternalOrganization
		{
			get
			{
				return "INTERNATIONALORGANIZATION";
			}
		}

		/// <summary>
		/// Lexicon Term
		/// </summary>
		public static string LexiconTerm
		{
			get
			{
				return "LEXICONTERM";
			}
		}

		/// <summary>
		/// Lexicon Term
		/// </summary>
		public static string IndividualLegislator
		{
			get
			{
				return "INDIVIDUALLEGISLATOR";
			}
		}

		/// <summary>
		/// Lexicon Term
		/// </summary>
		public static string ActionType
		{
			get
			{
				return "ACTIONTYPE";
			}
		}

		/// <summary>
		/// Lexicon Term
		/// </summary>
		public static string ActivityType
		{
			get
			{
				return "ACTIVITYTYPE";
			}
		}

		/// <summary>
		/// Lexicon Term
		/// </summary>
		public static string Phrases
		{
			get
			{
				return "PHRASES";
			}
		}

		/// <summary>
		/// Lexicon Term
		/// </summary>
		public static string ActionNounPlusVerb
		{
			get
			{
				return "ACTIONNOUNPLUSVERB";
			}
		}

		/// <summary>
		/// Lexicon Term
		/// </summary>
		public static string ClientMaster
		{
			get
			{
				return "CLIENTMASTER";
			}
		}

		/// <summary>
		/// Lexicon Term
		/// </summary>
		public static string CustomerUser
		{
			get
			{
				return "CUSTOMERUSER";
			}
		}

    public static string AdminProfileUpdate
    {
      get
      {
        return "ADMINPROFILEUPDATE";
      }
    }

		/// <summary>
		/// Lexicon Term
		/// </summary>
		public static string AdminUser
		{
			get
			{
				return "ADMINUSER";
			}
		}

		/// <summary>
		/// Lexicon Term
		/// </summary>
		public static string UserAccessManagement
		{
			get
			{
				return "USERACCESSMANAGEMENT";
			}
		}

		/// <summary>
		/// Lexicon Term
		/// </summary>
		public static string LexiconSubscription
		{
			get
			{
				return "LEXICONSUBSCRIPTION";
			}
		}

		/// <summary>
		/// Lexicon Term
		/// </summary>
		public static string CustomerSubscription
		{
			get
			{
				return "CUSTOMERSUBSCRIPTION";
			}
		}

		/// <summary>
		/// Lexicon Term
		/// </summary>
		public static string Scheduler
		{
			get
			{
				return "SCHEDULER";
			}
		}

		/// <summary>
		/// WebLinks
		/// </summary>
		public static string WebLinks
		{
			get
			{
				return "WEBLINKS";
			}
		}

		/// <summary>
		/// Global Setting
		/// </summary>
		public static string GlobalSetting
		{
			get
			{
				return "GLOBALSETTING";
			}
		}
	}

  public static class CustomerAuditConstants
  {
    /// <summary>
    /// Customer Profile Update
    /// </summary>
    public static string CustomerProfileUpdate
    {
      get
      {
        return "CUSTOMERPROFILEUPDATE";
      }
    }

    /// <summary>
    /// Customer LogIn
    /// </summary>
    public static string CustomerLogIn 
    {
      get
      {
        return "CUSTOMERLOGIN";
      }
    }

    /// <summary>
    /// Customer Logout
    /// </summary>
    public static string CustomerLogout
    {
      get
      {
        return "CUSTOMERLOGOUT";
      }
    }

    /// <summary>
    /// Dashboard ViewGraph
    /// </summary>
    public static string DashboardViewGraph
    {
      get
      {
        return "DASHBOARDVIEWGRAPH";
      }
    }

    /// <summary>
    /// Dashboard Search Lexicon
    /// </summary>
    public static string DashboardSearchLexicon
    {
      get
      {
        return "DASHBOARDSEARCHLEXICON";
      }
    }

    public static string ChangePassword
    {
      get
      {
        return "CHANGEPASSWORD";
      }
    }
	}
}