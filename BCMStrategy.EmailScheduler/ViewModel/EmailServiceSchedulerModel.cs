using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.EmailScheduler.ViewModel
{
  public class EmailServiceSchedulerModel
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

		public DateTime CreatedAt { get; set; }

    public string Key { get; set; }

    public bool IsDirect { get; set; }
  }
}
