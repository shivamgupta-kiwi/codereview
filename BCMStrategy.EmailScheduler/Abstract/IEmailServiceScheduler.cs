using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.EmailScheduler.Abstract
{
 public interface IEmailServiceScheduler
  {
		Task GetCustomerEmailDataAndSendEmail();
  }
}
