using System;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class EmailServiceModel
  {
    public int Id { get; set; }

    public string CustomerName { get; set; }

    public string EmailAddress { get; set; }

    public byte[] HtmlBodyByte { get; set; }

    public string HtmlBody
    {
      get
      {
        return HtmlBodyByte != null ? System.Text.Encoding.UTF8.GetString(HtmlBodyByte) : string.Empty;
      }
    }

    public string EmailSubject { get; set; }

    public string Status { get; set; }

    public DateTime SendBeforeTime { get; set; }

    public DateTime SendAfterTime { get; set; }

    public string RefHashId { get; set; }

    public string ScanDate { get; set; }

    public string Key { get; set; }

    public bool IsDirect { get; set; }
  }

  public class EmailGenerationModel
  {
    public int UserId { get; set; }

    public int EmailTemplateId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime SendBeforeTime { get; set; }

    public DateTime SendAfterTime { get; set; }

    public string Status { get; set; }

    public string EmailBody { get; set; }

    public byte[] HtmlBody
    {
      get
      {
        return System.Text.Encoding.UTF8.GetBytes(EmailBody);
      }
    }

    public string EmailSubject { get; set; }

    public string ValidationKey { get; set; }
  }

  public class EmailTemplateModel
  {
    public int EmailTemplateId { get; set; }

    public string EmailBody { get; set; }

    public string EmailSubject { get; set; }
  }

	public class EmailGenerationChartImage
	{
		public int EmailGenerationChartImageId { get; set; }

		public DateTime? EmailGenerationDate { get; set; }

		public DateTime? EmailGenerationUpdateDate { get; set; }

		public byte[] EmailGenerationImage { get; set; }
	}

}