using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Common.Email
{
  public class EmailConfiguration
  {
    public string SmtpServer { get; set; }

    public int SmtpPort { get; set; }

    public NetworkCredential Credentials { get; set; }

    public bool UseSsl { get; set; }

    public string FromAddress { get; set; }

    public string UserName { get; set; }
  }
}