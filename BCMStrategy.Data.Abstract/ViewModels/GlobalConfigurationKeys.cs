namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class GlobalConfigurationKeys
  {
    protected GlobalConfigurationKeys()
    {

    }
    /// <summary>
    /// The system email identifier
    /// </summary>
    public const string SysEmailID = "SysEmailID";

		/// <summary>
		/// The email pass code
		/// </summary>
		public const string EmailPass = "EmailPassword";

		/// <summary>
		/// The SMTP details
		/// </summary>
		public const string SMTPDetails = "SMTPDetails";

		/// <summary>
		/// The SMTP port
		/// </summary>
		public const string SMTPPort = "SMTPPort";

		/// <summary>
		/// The SSL enabled
		/// </summary>
		public const string SSLEnabled = "SSLEnabled";

		/// <summary>
		/// The Web Application URL
		/// </summary>
		public const string WebApplicationURL = "WebApplicationURL";

		/// <summary>
		/// The email link expiration time
		/// </summary>
		public const string EmailLinkExpirationTime = "EmailLinkExpiration";

		/// <summary>
		/// Get Import File size
		/// </summary>
		public const string CsvFileSize = "CsvFileSize";

		/// <summary>
		/// Get UserName
		/// </summary>
		public const string UserName = "UserName";

		/// <summary>
		/// Itext Sharp Licence Key
		/// </summary>
		public const string ITextSharp_LincenceKey = "ITextSharp_LincenceKey";

		/// <summary>
		/// FT API query string parameter
		/// </summary>
		public const string FTAPI_Querstring_Param = "FT_API_Query_Param";

		/// <summary>
		/// FT Licence Key For HeadLine News
		/// </summary>
		public const string FT_Headline_Licence_Key = "FT_Headline_Licence_Key";

		/// <summary>
		/// FT Licence Key For All News
		/// </summary>
		public const string FT_B2B_All_Licence_Key = "FT_B2B_All_Licence_Key";

		/// <summary>
		/// FT HeadLine API Url.
		/// </summary>
		public const string FT_HeadLine_API_URL = "FT_HeadLine_API_URL";

		/// <summary>
		/// FT B2B Content API Url.
		/// </summary>
		public const string FT_B2B_Content_API_URL = "FT_B2B_Content_API_URL";

		/// <summary>
		/// Number Of Chart Columns
		/// </summary>
		public const string NumberOfChartColumns = "NumberOfChartColumns";

		/// <summary>
		/// Token for Thomson Reuters
		/// </summary>
		public const string TR_TOKEN = "TR_TOKEN";

		/// <summary>
		/// Api headline for Thomson Reuters
		/// </summary>
		public const string TR_GET_TOKEN_URL = "TR_GET_TOKEN_URL";

		public const string TR_GET_CONTENT_URL = "TR_GET_CONTENT_URL";

		public const string TR_USERNAME = "TR_USERNAME";

		public const string TR_PASSWORD = "TR_PASSWORD";

		public const string ARCHIVAL_PROCESS_FREQUENCY_DAYS = "ARCHIVAL_PROCESS_FREQUENCY_DAYS";

		public const string SOLR_DATARETENTION_DAYS = "SOLR_DATARETENTION_DAYS";

		public const string MYSQL_DATARETENTION_DAYS = "MYSQL_DATARETENTION_DAYS";

		/// <summary>
		/// SQS Email Generation URL
		/// </summary>
		public const string SQS_EmailGeneration = "SQS_EMAILGENERATION";

		/// <summary>
		/// SQS AWS Access key
		/// </summary>
		public const string SQS_AWSAccessKeyId = "AWSAccessKeyId";

		/// <summary>
		/// SQS AWS Secret Access Key
		/// </summary>
		public const string SQS_AWSSecretAccessKey = "AWSSecretAccessKey";

		/// <summary>
		/// SQS AWS Content Loader URL
		/// </summary>
		public const string SQS_ContentLoader = "ContentLoader";

		/// <summary>
		/// Retrieval of Hours from Configuration for the execution of EmailGeneration process
		/// </summary>
		public const string Auto_EmailGeneration_Hours_FailOver = "AUTO_EMAILGENERATION_HOURS_FAILOVER";
	}
}