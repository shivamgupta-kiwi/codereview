using BCMStrategy.Common.AuditLog;
using BCMStrategy.Common.Unity;
using BCMStrategy.DAL.Context;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Logger;
using BCMStrategy.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Repository.Concrete
{
  public class DashboardRepository : IDashboard
  {
    private static readonly EventLogger<DashboardRepository> log = new EventLogger<DashboardRepository>();

    /// <summary>
    /// The _audit repository
    /// </summary>
    private IAuditLog _auditRepository;

    /// <summary>
    /// Gets the audit repository.
    /// </summary>
    /// <value>
    /// The audit repository.
    /// </value>
    private IAuditLog AuditRepository
    {
      get
      {
        if (this._auditRepository == null)
        {
          this._auditRepository = UnityHelper.Resolve<IAuditLog>();
        }

        return this._auditRepository;
      }
    }

    ////private readonly int processIdGeneric = 206;
    /// <summary>
    /// DashBoard Lexicon List
    /// </summary>
    /// <returns></returns>
    public async Task<List<DashboardLexiconTypeViewModel>> GetLexiconsForDashboard(string selectedDate, string lexiconTypeHashId)
    {
      List<DashboardLexiconTypeViewModel> list = new List<DashboardLexiconTypeViewModel>();
      int lexiconTypeId = !string.IsNullOrEmpty(lexiconTypeHashId) ? lexiconTypeHashId.ToDecrypt().ToInt32() : 0;

      DateTime? parsedDate = DateTime.ParseExact(selectedDate, "MM/dd/yyyy", null);

      List<int?> lexiconAccessList = new List<int?>();
      List<int?> processIdList = new List<int?>();
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        int user = UserAccessHelper.CurrentUserIdentity.ToString().ToInt32();

        int totalDefaultLexiconCount = db.defaultlexiconterms.Count(x => x.UserId == user && x.LexiconTypeId == lexiconTypeId);

        string userType = (from e in db.user
                           where e.Id == user
                           select e.UserType).FirstOrDefault();
        if (userType == Enums.UserType.CUSTOMER.ToString())
        {
          lexiconAccessList = db.lexiconprivilege.Where(e => e.UserId == user && e.IsDeleted != 1).Select(e => e.LexiconIssueId).ToList();
        }
        processIdList = db.scanninglinkdetails.
                                        Where(a => a.Created.Value.Year == parsedDate.Value.Year &&
                                                   a.Created.Value.Month == parsedDate.Value.Month &&
                                                   a.Created.Value.Day == parsedDate.Value.Day).Select(s => s.ProcessId).Distinct().ToList();

        List<DashBoardLexiconTermsViewModel> DashBoardLexiconTermsList = new List<DashBoardLexiconTermsViewModel>();

        if (totalDefaultLexiconCount > 0)
        {
          DashBoardLexiconTermsList = (
           from sld in db.scanninglinkdetails
           join sc in db.scrapedcontents on sld.Id equals sc.ScanningLinkDetailId
           join slm in db.scrapedlexiconmapping on sc.Id equals slm.ScrapedContentid
           join dlt in db.defaultlexiconterms on slm.LexiconId equals dlt.LexiconIssuesId
           join spt in db.scrappedproprietorytags on sld.Id equals spt.ScanningLinkDetailId
           join sptm in db.scrappedproprietorytagsmapping on spt.Id equals sptm.ScrappedProprietoryTagId
           join sst in db.scrappedstandardtags on sld.Id equals sst.ScanningLinkDetailsId
           where slm.lexiconissues.LexiconTypeId == lexiconTypeId && dlt.UserId == user
           select new DashBoardLexiconTermsViewModel
             
           {
             LexiconType = slm.lexiconissues.lexicontype.Type,
             LexiconTermId = slm.LexiconId,
             ProcessId = sld.ProcessId,
             LexiconTerm = slm.lexiconissues.LexiconIssue,
             CombinationValue = slm.lexiconissues.CombinationValue,
             LexiconTypeId = slm.lexiconissues.LexiconTypeId,
             Value = sptm.SearchValue.HasValue ? (decimal)sptm.SearchValue : 0,
           }).Distinct().OrderByDescending(m => m.LexiconTermId).Where(k => (userType == "ADMIN" ||
            lexiconAccessList.Contains(k.LexiconTermId)) && processIdList.Contains(k.ProcessId)).ToList();
        }
        else
        {
          DashBoardLexiconTermsList = (
           from sld in db.scanninglinkdetails
           join sc in db.scrapedcontents on sld.Id equals sc.ScanningLinkDetailId
           join slm in db.scrapedlexiconmapping on sc.Id equals slm.ScrapedContentid
           join spt in db.scrappedproprietorytags on sld.Id equals spt.ScanningLinkDetailId
           join sptm in db.scrappedproprietorytagsmapping on spt.Id equals sptm.ScrappedProprietoryTagId
           join sst in db.scrappedstandardtags on sld.Id equals sst.ScanningLinkDetailsId
           where slm.lexiconissues.LexiconTypeId == lexiconTypeId
           select new DashBoardLexiconTermsViewModel
           {
             LexiconTerm = slm.lexiconissues.LexiconIssue,
             CombinationValue = slm.lexiconissues.CombinationValue,
             LexiconTermId = slm.LexiconId,
             ProcessId = sld.ProcessId,
             LexiconTypeId = slm.lexiconissues.LexiconTypeId,
             LexiconType = slm.lexiconissues.lexicontype.Type,
             Value = sptm.SearchValue.HasValue ? (decimal)sptm.SearchValue : 0,
           }).Distinct().OrderByDescending(m => m.LexiconTermId).Where(k => (userType == "ADMIN" ||
            lexiconAccessList.Contains(k.LexiconTermId)) && processIdList.Contains(k.ProcessId)).ToList();
        }

        List<int> lexiconTypeIds = DashBoardLexiconTermsList.Select(j => j.LexiconTypeId).Distinct().ToList();

        if (lexiconTypeIds.Count == 0)
        {

          DashBoardLexiconTermsList = (
           from sld in db.scanninglinkdetails
           join sc in db.scrapedcontents on sld.Id equals sc.ScanningLinkDetailId
           join slm in db.scrapedlexiconmapping on sc.Id equals slm.ScrapedContentid
           join spt in db.scrappedproprietorytags on sld.Id equals spt.ScanningLinkDetailId
           join sptm in db.scrappedproprietorytagsmapping on spt.Id equals sptm.ScrappedProprietoryTagId
           where slm.lexiconissues.LexiconTypeId == lexiconTypeId && sptm.SearchValue > 0
           select new DashBoardLexiconTermsViewModel
           {
             LexiconTerm = slm.lexiconissues.LexiconIssue,
             CombinationValue = slm.lexiconissues.CombinationValue,
             LexiconTermId = slm.LexiconId,
             ProcessId = sld.ProcessId,
             LexiconTypeId = slm.lexiconissues.LexiconTypeId,
             Value = sptm.SearchValue.HasValue ? (decimal)sptm.SearchValue : 0
           }).OrderByDescending(m => m.LexiconTermId).Where(k => (userType == "ADMIN" ||
            lexiconAccessList.Contains(k.LexiconTermId)) && processIdList.Contains(k.ProcessId)).ToList();

          lexiconTypeIds = DashBoardLexiconTermsList.Select(j => j.LexiconTypeId).Distinct().ToList();
        }

        lexiconTypeIds.ForEach(p =>
        {
          DashboardLexiconTypeViewModel model = new DashboardLexiconTypeViewModel();
          model.LexiconTypeId = p;
          model.LexiconType = db.lexicontype.Where(b => b.Id == p).Select(j => j.Type).FirstOrDefault();

          List<DashBoardLexiconTermsViewModel> tempList = 
                          DashBoardLexiconTermsList
                          .Where(o => o.LexiconTypeId == p)
                          .GroupBy(x => new {
                            x.LexiconTerm,
                            x.LexiconTermId,
                            x.CombinationValue
                          })
                          .Select(y => new DashBoardLexiconTermsViewModel()
                          {
                            LexiconTermId = y.Key.LexiconTermId,
                            LexiconTerm = y.Key.LexiconTerm,
                            CombinationValue = y.Key.CombinationValue,
                            HasValue = true,
                            Value = y.Sum(x => x.Value)
                          }).Distinct().ToList();

          // Remaining Lexicons which has value but are not set as default lexicons
          List<DashBoardLexiconTermsViewModel> totalLexiconDetailsWithValues = (
         from sld in db.scanninglinkdetails
         join sc in db.scrapedcontents on sld.Id equals sc.ScanningLinkDetailId
         join slm in db.scrapedlexiconmapping on sc.Id equals slm.ScrapedContentid
         join spt in db.scrappedproprietorytags on sld.Id equals spt.ScanningLinkDetailId
         join sptm in db.scrappedproprietorytagsmapping on spt.Id equals sptm.ScrappedProprietoryTagId
         where slm.lexiconissues.LexiconTypeId == lexiconTypeId && sptm.SearchValue > 0
         select new DashBoardLexiconTermsViewModel
         {
           LexiconTermId = slm.LexiconId,
           LexiconTerm = slm.lexiconissues.LexiconIssue,
           CombinationValue = slm.lexiconissues.CombinationValue,
           ProcessId = sld.ProcessId,
           LexiconType = slm.lexiconissues.lexicontype.Type,
           Value = sptm.SearchValue.HasValue ? (decimal)sptm.SearchValue : 0
         }).OrderByDescending(m => m.LexiconTermId).Where(k => (userType == "ADMIN" ||
          lexiconAccessList.Contains(k.LexiconTermId)) && processIdList.Contains(k.ProcessId)).ToList();

          List<int> totalLexicons = totalLexiconDetailsWithValues.Select(x => x.LexiconTermId).Distinct().ToList();

          List<int> remainingLexiconWithValues = totalLexicons.Except(tempList.Select(x => x.LexiconTermId)).ToList();
          List<DashBoardLexiconTermsViewModel> appendRemainingLexiconIds;

          if (userType == Enums.UserType.ADMIN.ToString())
          {
            appendRemainingLexiconIds = (
                                                 from sld in db.lexiconissues
                                                 where !sld.IsDeleted
                                                 && sld.LexiconTypeId == lexiconTypeId
                                                 select new DashBoardLexiconTermsViewModel
                                                 {
                                                   LexiconTermId = sld.Id,
                                                   LexiconTerm = sld.LexiconIssue,
                                                   CombinationValue = sld.CombinationValue,
                                                   HasValue = false,
                                                   Value = 0
                                                 }).Distinct().ToList();
          }
          else
          {
            appendRemainingLexiconIds = (
                                                 from sld in db.lexiconissues
                                                 join lp in db.lexiconprivilege on sld.Id equals lp.LexiconIssueId
                                                 where !sld.IsDeleted
                                                 && sld.LexiconTypeId == lexiconTypeId
                                                 && lp.UserId == user
                                                 select new DashBoardLexiconTermsViewModel
                                                 {
                                                   LexiconTermId = sld.Id,
                                                   LexiconTerm = sld.LexiconIssue,
                                                   CombinationValue = sld.CombinationValue,
                                                   HasValue = false,
                                                   Value = 0
                                                 }).Distinct().ToList();
          }

          if (appendRemainingLexiconIds.Count > 0)
          {
            List<int> excludedLexicons = appendRemainingLexiconIds.Select(x => x.LexiconTermId).Except(tempList.Select(x => x.LexiconTermId)).ToList();

            foreach (int lexiconTerm in excludedLexicons)
            {
              DashBoardLexiconTermsViewModel dashBoardModel = appendRemainingLexiconIds.FirstOrDefault(x => x.LexiconTermId == lexiconTerm);
              tempList.Add(dashBoardModel);
            }
          }

          foreach (var lexicons in tempList.Where(x => remainingLexiconWithValues.Contains(x.LexiconTermId)))
          {
            lexicons.HasValue = true;
            lexicons.Value = 0.5M;
          }

          model.DashBoardLexiconTermsList = tempList.Select(u => new DashBoardLexiconTermsViewModel()
          {
            LexiconTermId = u.LexiconTermId,
            LexiconTerm = u.LexiconTerm,
            CombinationValue = u.CombinationValue,
            HasValue = u.HasValue,
            Value = u.Value
          }).Distinct().OrderBy(x => x.LexiconTerm).ToList();
          list.Add(model);
        });
      }
      return list;
    }

    /// <summary>
    /// Chart 2
    /// </summary>
    /// <param name="lexiconTypehashId"></param>
    /// <param name="selectedDate"></param>
    /// <returns></returns>
    public List<ReportViewModel> GetChartLexiconValues(ReportViewModel dashboardModel)
    {

      List<ReportViewModel> reportList = new List<ReportViewModel>();
      List<ReportViewModel> finalResult = new List<ReportViewModel>();

      int lexiconTypeId = !string.IsNullOrEmpty(dashboardModel.LexiconTypeHashId) ? dashboardModel.LexiconTypeHashId.ToDecrypt().ToInt32() : 0;

      int user = dashboardModel.IsDirect || !string.IsNullOrEmpty(dashboardModel.RefHashId) ? dashboardModel.RefHashId.ToDecrypt().ToInt32() : UserAccessHelper.CurrentUserIdentity.ToString().ToInt32();

      List<int> selectedLexiconIds = dashboardModel.SelectedLexicons != null ? dashboardModel.SelectedLexicons.Any() ? dashboardModel.SelectedLexicons.Select(a => a.ToDecrypt().ToInt32()).ToList() : new List<int>() : new List<int>();

      List<int?> lexiconAccessList = new List<int?>();
      List<int?> processIdList = new List<int?>();

      var numberOfChartColumns = Convert.ToInt32(Helper.GetNumberOfChartColumns());

      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        List<int> totalDefaultLexicons = db.defaultlexiconterms.Where(x => x.LexiconTypeId == lexiconTypeId && x.UserId == user).Select(x => x.LexiconIssuesId).ToList();

        string userType = (from e in db.user
                           where e.Id == user
                           select e.UserType).FirstOrDefault();

        DateTime? parsedDate = !string.IsNullOrEmpty(dashboardModel.SelectedDate) ? DateTime.ParseExact(dashboardModel.SelectedDate, "MM/dd/yyyy", null) : (DateTime?)null;

        if (userType == Enums.UserType.CUSTOMER.ToString())
        {
          lexiconAccessList = db.lexiconprivilege.Where(a => a.UserId == user && a.IsDeleted != 1).Select(a => a.LexiconIssueId).ToList();
        }

        processIdList = db.scanninglinkdetails.Where(a => a.Created.Value.Year == parsedDate.Value.Year && a.Created.Value.Month == parsedDate.Value.Month && a.Created.Value.Day == parsedDate.Value.Day).Select(s => s.ProcessId).Distinct().ToList();

        List<LexiconSearchValueViewModel> lexiconValueList = new List<LexiconSearchValueViewModel>();

        // Business logic has been written to filter data based upon user selection results
        if (!dashboardModel.IsAggregateDisplay && totalDefaultLexicons.Count > 0)
        {
          lexiconValueList = (
          from sld in db.scanninglinkdetails
          join sc in db.scrapedcontents on sld.Id equals sc.ScanningLinkDetailId
          join slm in db.scrapedlexiconmapping on sc.Id equals slm.ScrapedContentid
          join dlt in db.defaultlexiconterms on slm.LexiconId equals dlt.LexiconIssuesId
          join spt in db.scrappedproprietorytags on sld.Id equals spt.ScanningLinkDetailId
          join sptm in db.scrappedproprietorytagsmapping on spt.Id equals sptm.ScrappedProprietoryTagId
          join sst in db.scrappedstandardtags on sld.Id equals sst.ScanningLinkDetailsId
          join mdt in db.metadatatypes on sptm.MetaDataTypeId equals mdt.Id
          where processIdList.Contains(sld.ProcessId) && dlt.UserId == user
          select new LexiconSearchValueViewModel
          {
            LexiconId = slm.LexiconId,
            ProcessId = sld.ProcessId,
            LexiconType = slm.lexiconissues.lexicontype.Type,
            Lexicon = slm.lexiconissues.LexiconIssue,
            CombinationValue = slm.lexiconissues.CombinationValue,
            LexiconTypeId = slm.lexiconissues.LexiconTypeId,
            Value = sptm.SearchValue.HasValue ? (decimal)sptm.SearchValue : 0,
            ActionTypeId = sptm.MetaDataTypeId != null ? (int)sptm.MetaDataTypeId : 0,
            ActionType = mdt.MetaData != null ? mdt.MetaData : string.Empty
          }).OrderByDescending(a => a.LexiconId).Where(a => (string.IsNullOrEmpty(dashboardModel.LexiconTypeHashId) || a.LexiconTypeId == lexiconTypeId) && (userType == "ADMIN" ||
           lexiconAccessList.Contains(a.LexiconId)) && ((!selectedLexiconIds.Any()) || selectedLexiconIds.Contains(a.LexiconId))).ToList();

          if (selectedLexiconIds.Count == 0)
          {
            totalDefaultLexicons = totalDefaultLexicons.Except(lexiconValueList.Select(x => x.LexiconId).Distinct()).ToList();

            var lexiconList = LexiconWithNoValue(totalDefaultLexicons);

            lexiconValueList.AddRange(lexiconList);
          }
          else
          {
            totalDefaultLexicons = selectedLexiconIds.Except(lexiconValueList.Select(x => x.LexiconId).Distinct()).ToList();

            var selectedLexiconList = (
              from sld in db.scanninglinkdetails
              join sc in db.scrapedcontents on sld.Id equals sc.ScanningLinkDetailId
              join slm in db.scrapedlexiconmapping on sc.Id equals slm.ScrapedContentid
              join spt in db.scrappedproprietorytags on sld.Id equals spt.ScanningLinkDetailId
              join sptm in db.scrappedproprietorytagsmapping on spt.Id equals sptm.ScrappedProprietoryTagId
              join sst in db.scrappedstandardtags on sld.Id equals sst.ScanningLinkDetailsId
              join mdt in db.metadatatypes on sptm.MetaDataTypeId equals mdt.Id
              where processIdList.Contains(sld.ProcessId) && totalDefaultLexicons.Contains(slm.LexiconId)
              select new LexiconSearchValueViewModel
              {
                ActionType = mdt.MetaData != null ? mdt.MetaData : string.Empty,
                LexiconId = slm.LexiconId,
                ProcessId = sld.ProcessId,
                Lexicon = slm.lexiconissues.LexiconIssue,
                CombinationValue = slm.lexiconissues.CombinationValue,
                LexiconTypeId = slm.lexiconissues.LexiconTypeId,
                LexiconType = slm.lexiconissues.lexicontype.Type,
                Value = sptm.SearchValue.HasValue ? (decimal)sptm.SearchValue : 0,
                ActionTypeId = sptm.MetaDataTypeId != null ? (int)sptm.MetaDataTypeId : 0
              }).OrderByDescending(a => a.LexiconId).ToList();

            lexiconValueList.AddRange(selectedLexiconList);

            totalDefaultLexicons = selectedLexiconIds.Except(lexiconValueList.Select(x => x.LexiconId).Distinct()).ToList();

            var lexiconList = LexiconWithNoValue(totalDefaultLexicons);

            lexiconValueList.AddRange(lexiconList);
          }
        }
        else
        {
          lexiconValueList = (
          from sld in db.scanninglinkdetails
          join sc in db.scrapedcontents on sld.Id equals sc.ScanningLinkDetailId
          join slm in db.scrapedlexiconmapping on sc.Id equals slm.ScrapedContentid
          join spt in db.scrappedproprietorytags on sld.Id equals spt.ScanningLinkDetailId
          join sptm in db.scrappedproprietorytagsmapping on spt.Id equals sptm.ScrappedProprietoryTagId
          join sst in db.scrappedstandardtags on sld.Id equals sst.ScanningLinkDetailsId
          join mdt in db.metadatatypes on sptm.MetaDataTypeId equals mdt.Id
          where processIdList.Contains(sld.ProcessId)
          select new LexiconSearchValueViewModel
          {

            LexiconId = slm.LexiconId,
            ProcessId = sld.ProcessId,
            Lexicon = slm.lexiconissues.LexiconIssue,
            CombinationValue = slm.lexiconissues.CombinationValue,
            LexiconTypeId = slm.lexiconissues.LexiconTypeId,
            LexiconType = slm.lexiconissues.lexicontype.Type,
            Value = sptm.SearchValue.HasValue ? (decimal)sptm.SearchValue : 0,
            ActionTypeId = sptm.MetaDataTypeId != null ? (int)sptm.MetaDataTypeId : 0,
            ActionType = mdt.MetaData != null ? mdt.MetaData : string.Empty
          }).OrderByDescending(a => a.LexiconId).Where(a => (string.IsNullOrEmpty(dashboardModel.LexiconTypeHashId) || a.LexiconTypeId == lexiconTypeId) && (userType == "ADMIN" ||
           lexiconAccessList.Contains(a.LexiconId)) && ((!selectedLexiconIds.Any()) || selectedLexiconIds.Contains(a.LexiconId))).ToList();

          List<int> remainingLexicons = selectedLexiconIds.Except(lexiconValueList.Select(x => x.LexiconId).Distinct()).ToList();

          if (remainingLexicons.Count > 0)
          {
            var remainingLexiconList = LexiconWithNoValue(remainingLexicons);

            lexiconValueList.AddRange(remainingLexiconList);
          }
        }

        List<int> actionTypeIdList = lexiconValueList.Select(a => a.ActionTypeId).Distinct().ToList();
        List<metadatatypes> metadatatypesList = db.metadatatypes.Where(a => actionTypeIdList.Contains(a.Id)).ToList();

        if (!dashboardModel.IsAggregateDisplay)
        {
          if (lexiconValueList.Any())
          {
            List<int> lexiconIds = lexiconValueList.Select(a => a.LexiconId).Distinct().ToList();

            lexiconIds.ForEach(a =>
            {
              ReportViewModel model = new ReportViewModel();

              model.LexiconId = a;
              model.Lexicon = lexiconValueList.Where(m => m.LexiconId == a).Select(n => !string.IsNullOrEmpty(n.CombinationValue) ? n.Lexicon + " + " + n.CombinationValue : n.Lexicon).FirstOrDefault();
              metadatatypesList.ForEach(b =>
              {
                ActionType actionTypeModel = new ActionType();
                actionTypeModel.ActionTypeMasterId = b.Id;
                actionTypeModel.Name = b.MetaData;
                actionTypeModel.Value = lexiconValueList.Where(h => h.ActionTypeId == b.Id && h.LexiconId == a).Sum(u => u.Value);

                model.ActionType.Add(actionTypeModel);
              });
              model.LexiconValuesSum = model.ActionType.Sum(o => o.Value);
              reportList.Add(model);
            });
          }
          if (reportList.Any())
          {
            finalResult = reportList.OrderByDescending(q => q.LexiconValuesSum).Take(numberOfChartColumns).ToList();
          }

          int lexiconId = dashboardModel.LexiconTypeHashId.ToDecrypt().ToInt32();

          string LexiconName = db.lexicontype.Where(x => x.Id == lexiconId).Select(x => x.Type).FirstOrDefault();

          try
          {
            AuditRepository.WriteAudit<ReportViewModel>(CustomerAuditConstants.DashboardViewGraph, AuditType.ResultView, null, dashboardModel, "View Policy Risk Indicators for: " + LexiconName + " on Date: " + parsedDate.ToFormatedDateTime("MM/dd/yyyy"), user);
          }
          catch (Exception ex)
          {
            log.LogError(LoggingLevel.Error, "BadRequest", "DashboardRepository: Exception is thrown in GetChartLexiconValues method", ex, null);
          }

          return finalResult;
        }
        else
        {
          if (lexiconValueList.Any())
          {
            try
            {
              AuditRepository.WriteAudit<ReportViewModel>(CustomerAuditConstants.DashboardViewGraph, AuditType.ResultView, null, dashboardModel, "View Policy Risk Indicators for Date: " + parsedDate.ToFormatedDateTime("MM/dd/yyyy"), user);
            }
            catch (Exception ex)
            {
              log.LogError(LoggingLevel.Error, "BadRequest", "DashboardRepository: Exception is thrown in GetChartLexiconValues method", ex, null);
            }

            List<int> lexiconTypeIds = lexiconValueList.Select(a => a.LexiconTypeId).Distinct().ToList();

            lexiconTypeIds.ForEach(a =>
            {
              ReportViewModel model = new ReportViewModel();

              model.LexiconTypeId = a;
              model.LexiconType = lexiconValueList.Where(m => m.LexiconTypeId == a).Select(n => n.LexiconType).FirstOrDefault();
              metadatatypesList.ForEach(b =>
              {
                ActionType actionTypeModel = new ActionType();
                actionTypeModel.ActionTypeMasterId = b.Id;
                actionTypeModel.Name = b.MetaData;


                actionTypeModel.Value = lexiconValueList.Where(h => h.ActionTypeId == b.Id && h.LexiconTypeId == a).Sum(u => u.Value);

                model.ActionType.Add(actionTypeModel);
              });
              model.LexiconValuesSum = model.ActionType.Sum(o => o.Value);
              reportList.Add(model);
            });
          }
          if (reportList.Any())
          {
            finalResult = reportList.OrderByDescending(q => q.LexiconValuesSum).Take(numberOfChartColumns).ToList();
          }
          return finalResult;
        }
      }
    }

    private List<LexiconSearchValueViewModel> LexiconWithNoValue(List<int> selectedLexicons)
    {
      List<LexiconSearchValueViewModel> defaultLexicons;

      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        defaultLexicons = (
        from li in db.lexiconissues
        join lil in db.lexiconissuelinker on li.Id equals lil.LexiconIssueId
        where selectedLexicons.Contains(li.Id)
        select new LexiconSearchValueViewModel
        {
          LexiconId = li.Id,
          ProcessId = 0,
          Lexicon = li.LexiconIssue,
          CombinationValue = li.CombinationValue,
          LexiconTypeId = li.LexiconTypeId,
          LexiconType = li.lexicontype.Type,
          Value = 0,
          ActionTypeId = 0,
          ActionType = string.Empty
        }).ToList();
      }

      return defaultLexicons;
    }

    /// <summary>
    /// Get Activity Type Values
    /// </summary>
    /// <returns></returns>
    public async Task<ApiOutput> GetActivityTypeValues(string selectedDate, string actionHashId, string lexiconHashId)
    {
      List<ActivityType> model = new List<ActivityType>();
      ConsolidateList finallist = new ConsolidateList();
      int actionId = actionHashId.ToDecrypt().ToInt32();
      int lexiconId = lexiconHashId.ToDecrypt().ToInt32();
      DateTime? parsedDate = !string.IsNullOrEmpty(selectedDate) ? DateTime.ParseExact(selectedDate, "MM/dd/yyyy", null) : (DateTime?)null;
      List<int?> processIdList = new List<int?>();
      ApiOutput apiOutput = new ApiOutput();
      finallist.Date = selectedDate;

      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        processIdList = db.scanninglinkdetails.Where(a => a.Created.Value.Year == parsedDate.Value.Year && a.Created.Value.Month == parsedDate.Value.Month && a.Created.Value.Day == parsedDate.Value.Day).Select(s => s.ProcessId).Distinct().ToList();

        var lexiconValueList = (
        from sld in db.scanninglinkdetails
        join sc in db.scrapedcontents on sld.Id equals sc.ScanningLinkDetailId
        join slm in db.scrapedlexiconmapping on sc.Id equals slm.ScrapedContentid
        join spt in db.scrappedproprietorytags on sld.Id equals spt.ScanningLinkDetailId
        join sptm in db.scrappedproprietorytagsmapping on spt.Id equals sptm.ScrappedProprietoryTagId
        join sst in db.scrappedstandardtags on sld.Id equals sst.ScanningLinkDetailsId
        join mdt in db.metadatatypes on sptm.MetaDataTypeId equals mdt.Id

        where sptm.MetaDataTypeId == actionId && slm.LexiconId == lexiconId ////&& processIdList.Contains(sld.ProcessId)*/
        select new ProprietoryTagData
        {
          ProcessId = sld.ProcessId,
          ProprietoryTagDataId = sld.Id,
          MetadatatypeId = sptm.MetaDataTypeId.HasValue ? sptm.MetaDataTypeId.Value : 0,
          SearchCount = sptm.SearchCounts.HasValue ? sptm.SearchCounts.Value : 0,
          SearchTypeId = sptm.SearchTypeId.HasValue ? sptm.SearchTypeId.Value : 0,
          SearchValue = sptm.SearchValue.HasValue ? sptm.SearchValue.Value : 0,
          SearchByType = sptm.SearchType.Replace("_", " "),
          MetadataTypeName = mdt.MetaData,
          LexiconTerm = slm.lexiconissues.LexiconIssue,
          WebsiteUrl = sld.WebSiteURL,
          CombinationValue = slm.lexiconissues.CombinationValue
        }).Where(k => processIdList.Contains(k.ProcessId)).ToList();

        List<WebsiteUrl> UrlList = lexiconValueList.Select(u => new WebsiteUrl()
        {
          Url = u.WebsiteUrl,
          ActivityType = u.SearchType
        }).Distinct().OrderBy(a => a.ActivityType).ToList();

        string LexiconTerm = db.lexiconissues.Where(x => x.Id == lexiconId).Select(s => !string.IsNullOrEmpty(s.CombinationValue) ? s.LexiconIssue + " + " + s.CombinationValue : s.LexiconIssue).FirstOrDefault();
        finallist.LexiconTerm = !string.IsNullOrEmpty(LexiconTerm) ? LexiconTerm : string.Empty;

        var objMetadataTtypes = db.metadatatypes.Where(x => x.Id == actionId).FirstOrDefault();
        bool metadataTtypes = false;
        if (objMetadataTtypes != null)
        {
          metadataTtypes = (bool)objMetadataTtypes.IsActivityTypeExist;
          finallist.ActivityTypeName = objMetadataTtypes.MetaData;
        }
        if (metadataTtypes)
        {
          List<ProprietoryTagData> activityTypeList = lexiconValueList.GroupBy(x => new { x.MetadataTypeName, x.SearchType, x.SearchValue })
                                .Select(x => new ProprietoryTagData
                                {
                                  MetadatatypeId = x.First().MetadatatypeId,
                                  SearchCount = x.Sum(c => c.SearchCount),
                                  SearchValue = x.Sum(c => c.SearchValue),
                                  SearchTypeId = x.First().SearchTypeId,
                                  SearchByType = x.First().SearchByType,
                                  MetadataTypeName = x.First().MetadataTypeName,
                                  // SearchType = x.Key.SearchType,
                                  LexiconTerm = x.First().LexiconTerm,
                                  CombinationValue = x.First().CombinationValue
                                }).OrderBy(x => x.MetadatatypeId).ThenBy(x => x.SearchValue).ToList();

          foreach (var item in activityTypeList)
          {
            model.Add(new ActivityType()
            {
              Name = item.SearchType,
              SearchValue = item.SearchValue,
              AcivityType = !string.IsNullOrEmpty(item.CombinationValue) ? item.LexiconTerm + " + " + item.CombinationValue : item.LexiconTerm,
              ColorCode = item.ColorCode
            });
          }
        }

        finallist.IsActivityTypeExists = metadataTtypes;
        finallist.activityTypeList = model;
        finallist.websiteURL = UrlList;

        if (model.Any())
        {
          int currentUserId = UserAccessHelper.CurrentUserIdentity.ToString().ToInt32();
          string activityType = model[0].AcivityType;

          var lexiconName = (from x in db.lexiconissues.Where(y => y.Id == lexiconId)
                             join z in db.lexicontype on x.LexiconTypeId equals z.Id
                             select new LexiconTypeModel
                             {
                               LexiconType = z.Type
                             }).FirstOrDefault();

          AuditRepository.WriteAudit<ConsolidateList>(CustomerAuditConstants.DashboardViewGraph, AuditType.ResultView, null, finallist, "View Policy Risk Indicators for: " + lexiconName.LexiconType.ToString() + " on Date: " + selectedDate + " for Lexicon Term: " + activityType, currentUserId);
        }

      }
      apiOutput.Data = finallist;
      apiOutput.TotalRecords = 0;
      ////apiOutput.ErrorMessage = finalist. ? string.Empty : string.Format(Resource.ValidateMessageNoDataFound, Resource.LblDashboard);
      return apiOutput;
    }

    public List<LexiconSearchValueViewModel> GetChartModel(int processId, string lexiconTypehashId, string selectedDate, List<string> selectedLexicons, bool isPanelList)
    {
      List<LexiconSearchValueViewModel> lexiconValueList = new List<LexiconSearchValueViewModel>();
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        int lexiconTypeId = !string.IsNullOrEmpty(lexiconTypehashId) ? lexiconTypehashId.ToDecrypt().ToInt32() : 0;
        DateTime? parsedDate = !string.IsNullOrEmpty(selectedDate) ? DateTime.ParseExact(selectedDate, "MM/dd/yyyy", null) : (DateTime?)null;
        int maxProcessId = db.scanninglinkdetails.Where(a => a.Created <= parsedDate).Max(a => (int)a.ProcessId);
        List<int> selectedLexiconIds = selectedLexicons.Any() ? selectedLexicons.Select(a => a.ToDecrypt().ToInt32()).ToList() : new List<int>();
        List<int?> lexiconAccessList = new List<int?>();
        int user = UserAccessHelper.CurrentUserIdentity.ToString().ToInt32();
        string userType = (from e in db.user
                           where e.Id == user
                           select e.UserType).FirstOrDefault();

        if (userType == Enums.UserType.CUSTOMER.ToString())
        {
          lexiconAccessList = db.lexiconprivilege.Where(a => a.UserId == user && a.IsDeleted != 1).Select(a => a.LexiconIssueId).ToList();
        }

        var scrappedLexiconMappingList = db.scrapedlexiconmapping.Where(a => a.scrapedcontents.scanninglinkdetails.ProcessId == maxProcessId)
        .Select(a => new Test1()
        {
          ScanningLinkDetailId = a.scrapedcontents.ScanningLinkDetailId,
          Lexicon = a.lexiconissues.LexiconIssue,
          LexiconId = a.LexiconId,
          CombinationValue = a.lexiconissues.CombinationValue,
          LexiconTypeId = a.lexiconissues.LexiconTypeId,
          LexiconType = a.lexiconissues.lexicontype.Type
        }).Where(b => userType == "ADMIN" || lexiconAccessList.Contains(b.LexiconId)
      && (string.IsNullOrEmpty(lexiconTypehashId) || b.LexiconTypeId == lexiconTypeId)
      && (!selectedLexiconIds.Any() || selectedLexiconIds.Contains(b.LexiconId)))
      .ToList();

        List<int> scanningLinkDetailIds = scrappedLexiconMappingList.Select(a => a.ScanningLinkDetailId).ToList();
        var mappingList = db.scrappedproprietorytagsmapping.Where(a => scanningLinkDetailIds.Contains(a.scrappedproprietorytags.ScanningLinkDetailId)).ToList();

        mappingList.ForEach(n =>
        {
          var tempObj = scrappedLexiconMappingList.FirstOrDefault(o => o.ScanningLinkDetailId == n.scrappedproprietorytags.ScanningLinkDetailId);


          if (isPanelList)
          {
            LexiconSearchValueViewModel model = new LexiconSearchValueViewModel()
            {
              LexiconId = tempObj.LexiconId,
              Lexicon = tempObj.Lexicon,
              CombinationValue = tempObj.CombinationValue,
              LexiconTypeId = tempObj.LexiconTypeId,
              LexiconType = tempObj.LexiconType,
            };
            lexiconValueList.Add(model);
          }
          else
          {
            LexiconSearchValueViewModel model = new LexiconSearchValueViewModel()
            {
              LexiconId = tempObj.LexiconId,
              Lexicon = tempObj.Lexicon,
              CombinationValue = tempObj.CombinationValue,
              Value = n.SearchValue.HasValue ? (decimal)n.SearchValue : 0,
              ActionTypeId = n.metadatatypes != null ? n.metadatatypes.Id : 0,
              ActionType = n.metadatatypes != null ? n.metadatatypes.MetaData : string.Empty,
              LexiconTypeId = tempObj.LexiconTypeId,
              LexiconType = tempObj.LexiconType
            };
            lexiconValueList.Add(model);
          }

        });
      }
      return lexiconValueList.Distinct().ToList();
    }

    public async Task<bool> AuthenticateUserForVirtualDashboard(EmailServiceModel emailServiceModel)
    {
      bool isValid = false;
      try
      {
        if (emailServiceModel != null)
        {
          int currentUser = UserAccessHelper.CurrentUserIdentity.ToString().ToInt32();
          int userDecryptId = !string.IsNullOrEmpty(emailServiceModel.RefHashId) ? emailServiceModel.RefHashId.ToDecrypt().ToInt32() : 0;
          using (BCMStrategyEntities db = new BCMStrategyEntities())
          {
            var objEmailGenerationStatus = db.emailgenerationstatus.FirstOrDefault(x => x.ValidationKey == emailServiceModel.Key && x.UserId == userDecryptId);
            isValid = objEmailGenerationStatus != null && currentUser == userDecryptId ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
          }
        }
      }
      catch (Exception)
      {
        return isValid;
      }
      return isValid;
    }

    /// <summary>
    /// Update Lexicon Default Filter
    /// </summary>
    /// <param name="model">Report View Model</param>
    /// <returns></returns>
    public bool UpdateLexiconDefaultFilter(ReportViewModel model)
    {
      bool isSave = false;

      if (model != null)
      {
        int currentUser = UserAccessHelper.CurrentUserIdentity.ToString().ToInt32();
        int lexiconTypeId = model.LexiconTypeHashId.ToDecrypt().ToInt32();

        using (BCMStrategyEntities db = new BCMStrategyEntities())
        {
          var deleteQuery = DeleteLexiconDefaultQuery(currentUser, lexiconTypeId);
          db.Database.ExecuteSqlCommand(deleteQuery);

          foreach (string lexicon in model.SelectedLexicons)
          {
            defaultlexiconterms defaultLexicons = new defaultlexiconterms()
            {
              LexiconIssuesId = lexicon.ToDecrypt().ToInt32(),
              LexiconTypeId = lexiconTypeId,
              UserId = currentUser,
              Created = Helper.GetCurrentDate(),
              CreatedBy = UserAccessHelper.CurrentUserIdentity.ToString()
            };

            db.defaultlexiconterms.Add(defaultLexicons);
          }

          isSave = db.SaveChanges() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
        }
      }

      return isSave;
    }

    private string DeleteLexiconDefaultQuery(int customerId, int lexiconTypeId)
    {
      string deleteQuery = string.Format("Delete from defaultlexiconterms where userid={0} and LexiconTypeId={1}", customerId, lexiconTypeId);
      return deleteQuery;
    }
  }

  public class Test1
  {
    public int ScanningLinkDetailId { get; set; }

    public string Lexicon { get; set; }

    public int LexiconId { get; set; }

    public string CombinationValue { get; set; }

    public int LexiconTypeId { get; set; }

    public string LexiconType { get; set; }
  }


  public class ScanningLinkDetailModel
  {
    public int ScanningLinkDetailId { get; set; }

    public int LexiconId { get; set; }

    public string LexiconName { get; set; }

    public string CombinationValue { get; set; }

    public int LexiconTypeId { get; set; }

    public string LexiconType { get; set; }
  }

}


