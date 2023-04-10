using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCMStrategy.Common.Email;
using BCMStrategy.DAL.Context;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;

namespace BCMStrategy.Data.Repository.Concrete
{
  public class CommonRepository : ICommonRepository
  {
    /// <summary>
    /// Get Email Configuration Details
    /// </summary>
    /// <returns>
    /// Email Configurations
    /// </returns>
    public async Task<EmailConfiguration> GetEmailConfiguration()
    {
      using (var context = new BCMStrategyEntities())
      {
        var result = await context.globalconfiguration.ToListAsync();

        EmailConfiguration emailConfig = new EmailConfiguration();
        emailConfig.Credentials = new System.Net.NetworkCredential();
        foreach (globalconfiguration gbl in result)
        {
          switch (gbl.Name)
          {
            case GlobalConfigurationKeys.SMTPDetails:
              emailConfig.SmtpServer = gbl.Value;
              break;

            case GlobalConfigurationKeys.SMTPPort:
              emailConfig.SmtpPort = Convert.ToInt32(gbl.Value);
              break;

            case GlobalConfigurationKeys.EmailPass:
              emailConfig.Credentials.Password = gbl.Value;
              break;

            case GlobalConfigurationKeys.SSLEnabled:
              emailConfig.UseSsl = Convert.ToBoolean(gbl.Value);
              break;

            case GlobalConfigurationKeys.SysEmailID:
              emailConfig.FromAddress = gbl.Value;
              break;

            case GlobalConfigurationKeys.UserName:
              emailConfig.UserName = gbl.Value;
              break;
          }
        }

        return emailConfig;
      }
    }

    /// <summary>
    /// Get Web Application Path
    /// </summary>
    /// <returns>
    /// Get Application path
    /// </returns>
    public async Task<string> GetWebApplicationBasePath()
    {
      using (var context = new BCMStrategyEntities())
      {
        var result = await context.globalconfiguration.Where(x => x.Name == GlobalConfigurationKeys.WebApplicationURL).Select(y => y.Value).FirstOrDefaultAsync();

        return result.ToString();
      }
    }


    /// <summary>
    /// Get Web Application Path
    /// </summary>
    /// <returns>
    /// Get Application path
    /// </returns>
    public async Task<string> GetCSVFileSize()
    {
      using (var context = new BCMStrategyEntities())
      {
        var result = await context.globalconfiguration.Where(x => x.Name == GlobalConfigurationKeys.CsvFileSize).Select(y => y.Value).FirstOrDefaultAsync();

        return result.ToString();
      }
    }

    /// <summary>
    /// Splite
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="parts"></param>
    /// <returns>List</returns>
    public List<List<T>> Split<T>(List<T> source, int parts)
    {
      return source
          .Select((x, i) => new { Index = i, Value = x })
          .GroupBy(x => x.Index / parts)
          .Select(x => x.Select(v => v.Value).ToList())
          .ToList();
    }
  }
}
