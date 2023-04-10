using System.Collections.Generic;
using System.Threading.Tasks;
using BCMStrategy.Data.Abstract.ViewModels;

namespace BCMStrategy.Data.Abstract.Abstract
{
  public interface IDashboard
  {
    /// <summary>
    /// Get Lexicon Wise ActionTypeValues
    /// </summary>
    /// <returns></returns>
    //// List<ReportViewModel> GetChartLexiconValues(bool isAggregateDisplay, string lexiconTypehashId, string selectedDate, List<string> selectedLexicons, string userHashId = "");
    List<ReportViewModel> GetChartLexiconValues(ReportViewModel dashboardModel);
    //List<ActivityType> GetActivityType

    /// <summary>
    /// Get Lexicons For Dashboard
    /// </summary>
    /// <returns></returns>
    Task<List<DashboardLexiconTypeViewModel>> GetLexiconsForDashboard(string selectedDate, string lexiconTypeHashId);

    /// <summary>
    /// Get Activity Type Values
    /// </summary>
    /// <returns></returns>
    Task<ApiOutput> GetActivityTypeValues(string selectedDate, string actionHashId, string lexiconHashId);


    Task<bool> AuthenticateUserForVirtualDashboard(EmailServiceModel emailServiceModel);

    /// <summary>
    /// Update Lexicon Default Filter
    /// </summary>
    /// <param name="model">model</param>
    /// <returns>Updated lexicon with default filter</returns>
    bool UpdateLexiconDefaultFilter(ReportViewModel model);
  }
}
