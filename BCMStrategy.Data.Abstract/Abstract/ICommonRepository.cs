using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCMStrategy.Common.Email;

namespace BCMStrategy.Data.Abstract.Abstract
{
  public interface ICommonRepository
  {
    Task<EmailConfiguration> GetEmailConfiguration();

    Task<string> GetWebApplicationBasePath();

    Task<string> GetCSVFileSize();

    List<List<T>> Split<T>(List<T> source, int parts);
  }

}
