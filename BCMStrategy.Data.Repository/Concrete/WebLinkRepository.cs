using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCMStrategy.Common.Kendo;
using BCMStrategy.DAL.Context;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;

using DALContext = BCMStrategy.DAL.Context;
using BCMStrategy.Common.AuditLog;
using BCMStrategy.Common.Unity;

namespace BCMStrategy.Data.Repository.Concrete
{
  public class WebLinkRepository : IWebLink
  {
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
    /// <summary>
    /// Get All WebLinks
    /// </summary>
    /// <param name="type">Type </param>
    /// <returns>All the Weblinks for the given type</returns>
    public List<string> GetAllWebLinks(int type, int processId)
    {
      List<string> result = null;

      using (DALContext.BCMStrategyEntities db = new DALContext.BCMStrategyEntities())
      {
        List<WebLink> listOfWebLinks = (from w in db.weblinks.AsNoTracking().Where(x => x.IsActive && !x.IsDeleted && x.WebSiteTypeId == type)
                                        join y in db.category.AsNoTracking() on w.CategoryId equals y.Id
                                        select new WebLink
                                        {
                                          CategoryName = y.Name
                                        }).ToList();

        result = listOfWebLinks.Select(x => x.CategoryName).Distinct().ToList();
      }

      return result;
    }

    /// <summary>
    /// Insert Link Record to the Database
    /// </summary>
    /// <param name="type">type</param>
    /// <param name="processId">processId</param>
    /// <param name="processInstanceId">Process Instance Id</param>
    /// <returns>Boolean Value for the link records</returns>
    public bool InsertLinkRecords(ProcessLinkDetails processLink)
    {
      bool result = false;

      //// int[] totalWebLinks = { 374 };
      //// totalWebLinks.Contains(x.Id) && 

      using (DALContext.BCMStrategyEntities db = new DALContext.BCMStrategyEntities())
      {
        List<WebLink> listOfWebLinks = (from w in db.weblinks.Where(x => x.IsActive && !x.IsDeleted && x.WebSiteTypeId == processLink.Websitetypes && x.CategoryId == processLink.EngineCategory)
                                        join y in db.category.AsNoTracking() on w.CategoryId equals y.Id
                                        select new WebLink
                                        {
                                          Id = w.Id,
                                          WebSiteTypeId = w.WebSiteTypeId,
                                          WebLinkURL = w.WebLinkURL,
                                          CategoryName = y.Name
                                        }).OrderBy(x => x.Id)
                          .Skip(processLink.NoOfRecords * processLink.PageNumber)
                          .Take(processLink.NoOfRecords)
                          .ToList();

        foreach (WebLink link in listOfWebLinks)
        {
          loaderlinkqueue linkQueue = new loaderlinkqueue();

          linkQueue.Guid = Guid.NewGuid().ToString();
          linkQueue.SiteURL = link.WebLinkURL;
          linkQueue.InstanceName = link.CategoryName;
          linkQueue.WebSiteId = link.Id;

          linkQueue.Created = Helper.GetCurrentDateTime();
          linkQueue.CreatedBy = Helper.ShowScrapperName();
          linkQueue.ReadTaken = 0;
          linkQueue.ProcessInstanceId = processLink.ProcessInstanceId;
          linkQueue.ProcessId = processLink.ProcessId;

          linkQueue.LinkLevel = 0;

          db.loaderlinkqueue.Add(linkQueue);
        }

        result = db.SaveChanges() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
      }

      return result;
    }

    public bool InsertSubLinkLogRecords(LoaderLinkQueue processLink)
    {
      bool result = false;

      if (processLink != null && processLink.SiteURL != null)
      {
        using (DALContext.BCMStrategyEntities db = new DALContext.BCMStrategyEntities())
        {
          bool isInserted = db.loaderlinklog.Any(x => x.SiteURL == processLink.SiteURL);

          if (!isInserted)
          {
            loaderlinklog loaderLinkLogDB = new loaderlinklog();

            loaderLinkLogDB.InstanceName = processLink.InstanceName;
            loaderLinkLogDB.WebSiteId = processLink.WebSiteId;

            loaderLinkLogDB.SiteURL = processLink.SiteURL;
            loaderLinkLogDB.ProcessId = processLink.ProcessId;
            loaderLinkLogDB.ProcessInstanceId = processLink.ProcessInstanceId;

            loaderLinkLogDB.Guid = processLink.GUID;
            loaderLinkLogDB.StatusId = 1;

            loaderLinkLogDB.LinkLevel = processLink.LinkLevel;
            loaderLinkLogDB.WebLinkBytes = processLink.WebLinkBytes;
            loaderLinkLogDB.IsSuccessful = processLink.IsSuccessful;

            loaderLinkLogDB.Created = Helper.GetCurrentDateTime();
            loaderLinkLogDB.CreatedBy = Helper.ShowScrapperName();
            loaderLinkLogDB.LinkCreatedDate = Helper.GetCurrentDateTime();

            db.loaderlinklog.Add(loaderLinkLogDB);

            result = db.SaveChanges() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
          }
        }
      }

      return result;
    }

    /// <summary>
    /// Store Document records in the database
    /// </summary>
    /// <param name="listDocuments">Process list of documents</param>
    /// <returns>True or false for the document processed</returns>
    public bool ProcessDocumentRecords(List<LoaderLinkQueue> listDocuments)
    {
      bool result = true;

      using (DALContext.BCMStrategyEntities db = new DALContext.BCMStrategyEntities())
      {
        List<loaderlinkqueue> loaderLinkListDB = new List<loaderlinkqueue>();

        foreach (LoaderLinkQueue linkQueue in listDocuments)
        {
          loaderlinkqueue loaderLinkDB = new loaderlinkqueue();

          loaderLinkDB.Guid = linkQueue.GUID;
          loaderLinkDB.SiteURL = linkQueue.SiteURL;
          loaderLinkDB.InstanceName = linkQueue.InstanceName;

          loaderLinkDB.ProcessId = linkQueue.ProcessId;
          loaderLinkDB.ProcessInstanceId = linkQueue.ProcessInstanceId;
          loaderLinkDB.WebSiteId = linkQueue.WebSiteId;

          loaderLinkDB.Created = Helper.GetCurrentDateTime();
          loaderLinkDB.CreatedBy = Helper.ShowScrapperName();
          loaderLinkDB.ReadTaken = 0;

          loaderLinkListDB.Add(loaderLinkDB);
        }

        db.loaderlinkqueue.AddRange(loaderLinkListDB);

        result = db.SaveChanges() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
      }

      return result;
    }

    /// <summary>
    /// Get All Messages based upon their Type
    /// </summary>
    /// <param name="messageType">Message Type</param>
    /// <param name="processId">Process Id</param>
    /// <param name="processInstanceId">Process Instance id of the message</param>
    /// <returns>All the webLinks for the given Message Type</returns>
    public List<LoaderLinkQueue> GetMessageBasedUponType(string messageType, int processId, int processInstanceId)
    {
      List<LoaderLinkQueue> listOfWebLinks;

      using (DALContext.BCMStrategyEntities db = new DALContext.BCMStrategyEntities())
      {
        listOfWebLinks = (from w in db.loaderlinkqueue.AsNoTracking().Where(x => x.InstanceName == messageType
                          && x.ProcessId == processId && x.ProcessInstanceId == processInstanceId)
                          select new LoaderLinkQueue
                          {
                            Id = w.WebSiteId != null ? w.WebSiteId.Value : 0,
                            SiteURL = w.SiteURL,
                            InstanceName = w.InstanceName,
                            GUID = w.Guid
                          }).ToList();
      }

      return listOfWebLinks;
    }

    public LoaderLinkQueue GetWebLinkHtmlLinksRegEx(int webLinkId)
    {
      LoaderLinkQueue linkDetails;

      using (DALContext.BCMStrategyEntities db = new DALContext.BCMStrategyEntities())
      {
        linkDetails = (from w in db.weblinks.AsNoTracking().Where(x => x.Id == webLinkId
                          && x.IsActive && !x.IsDeleted)
                       select new LoaderLinkQueue
                       {
                         Id = w.Id,
                         SiteURL = w.WebLinkURL,
                         AllHtmlLinksFetch = w.AllHtmlLinksFetch,
                         RegEx = w.RegEx
                       }).FirstOrDefault();
      }

      return linkDetails;
    }

    /// <summary>
    /// Get All Messages based upon their Type
    /// </summary>
    /// <param name="type">Type of the Message</param>
    /// <param name="processId">Process Id</param>
    /// <returns>All the webLinks for the given Message Type</returns>
    public int GetWebLinkCount(int type, string categoryName)
    {
      int webLinkCount = 0;

      //// int[] totalWebLinks = { 374 };
      //// totalWebLinks.Contains(x.Id) && 

      using (DALContext.BCMStrategyEntities db = new DALContext.BCMStrategyEntities())
      {
        webLinkCount = (from w in db.weblinks.AsNoTracking().Where(x => x.IsActive && !x.IsDeleted
                          && x.WebSiteTypeId == type)
                        join y in db.category.AsNoTracking() on w.CategoryId equals y.Id
                        where y.Name == categoryName
                        select w.Id
                          ).Count();
      }

      return webLinkCount;
    }

    /// <summary>
    /// Delete record from Loader Link Queue Table
    /// </summary>
    /// <param name="guid">GUID of the given website link</param>
    /// <returns>Boolean value for deleting the record</returns>
    public bool DeleteLoaderLinkQueueRecord(ProcessLinkDetails processLinkDetails)
    {
      bool result = false;

      using (DALContext.BCMStrategyEntities db = new DALContext.BCMStrategyEntities())
      {
        var linkQueue = db.loaderlinkqueue.Where(x => x.Guid == processLinkDetails.Guid && x.ProcessId == processLinkDetails.ProcessId).FirstOrDefault();

        db.loaderlinkqueue.Remove(linkQueue);
        result = db.SaveChanges() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
      }

      return result;
    }

    public bool UpdateLoaderLinkLogRecordBytes(LoaderLinkQueue linkDetails)
    {
      bool result = false;

      using (DALContext.BCMStrategyEntities db = new DALContext.BCMStrategyEntities())
      {
        var linkLoaderLog = db.loaderlinklog.Where(x => x.Guid == linkDetails.GUID).FirstOrDefault();

        linkLoaderLog.WebLinkBytes = linkDetails.WebLinkBytes;
        linkLoaderLog.NewProcessId = linkDetails.NewerProcessId;
        linkLoaderLog.Modified = Helper.GetCurrentDateTime();

        result = db.SaveChanges() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
      }

      return result;
    }

    public bool UpdateLoaderLinkLogMasterRecord(LoaderLinkQueue linkDetails)
    {
      bool result = false;

      using (DALContext.BCMStrategyEntities db = new DALContext.BCMStrategyEntities())
      {
        var linkLoaderLog = db.loaderlinklog.Where(x => x.SiteURL == linkDetails.SiteURL).FirstOrDefault();

        if (linkLoaderLog != null)
        {
          linkLoaderLog.WebLinkBytes = linkDetails.WebLinkBytes;
          linkLoaderLog.NewProcessId = linkDetails.NewerProcessId;
          linkLoaderLog.Modified = Helper.GetCurrentDateTime();
        }
        else
        {
          loaderlinklog linkLog = new loaderlinklog();

          linkLog.Guid = linkDetails.GUID;
          linkLog.SiteURL = linkDetails.SiteURL;
          linkLog.WebSiteId = linkDetails.WebSiteId;

          linkLog.ProcessInstanceId = linkDetails.ProcessInstanceId;
          linkLog.ProcessId = linkDetails.ProcessId;
          linkLog.StatusId = 1;
          linkLog.WebLinkBytes = linkDetails.WebLinkBytes;

          linkLog.Created = Helper.GetCurrentDateTime();
          linkLog.CreatedBy = Helper.ShowScrapperName();
          linkLog.InstanceName = linkDetails.InstanceName != null ? linkDetails.InstanceName : Convert.ToString(Helper.WebSiteCategory.ClickThroughPages);
          linkLog.LinkLevel = linkDetails.LinkLevel;
          linkLog.IsSuccessful = true;
          linkLog.LexiconCount = linkDetails.LexiconCount;

          db.loaderlinklog.Add(linkLog);
        }

        result = db.SaveChanges() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
      }

      return result;
    }

    public bool UpdateContentLoaderLog(ContentLoaderLogViewModel contentLoaderViewModel)
    {
      bool result = false;

      using (DALContext.BCMStrategyEntities db = new DALContext.BCMStrategyEntities())
      {
        ////var linkLoaderLog = db.loaderlinklog.Where(x => x.SiteURL == contentLoaderViewModel.SiteUrl).FirstOrDefault();

        contentloaderlog log = new contentloaderlog();
        log.ProcessId = contentLoaderViewModel.ProcessId;
        log.ProcessInstanceId = contentLoaderViewModel.ProcessInstanceId;
        log.WebSiteId = contentLoaderViewModel.WebSiteId;
        log.SiteURL = contentLoaderViewModel.SiteUrl;
        log.LinkLevel = contentLoaderViewModel.LinkLevel;
        log.IsContentInserted = contentLoaderViewModel.IsContentInserted;
        log.IsContentUpdated = contentLoaderViewModel.IsContentUpdated;
        log.Created = Helper.GetCurrentDateTime();
        db.contentloaderlog.Add(log);
        result = db.SaveChanges() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
      }
      return result;
    }

    public bool UpdateLoaderErrorLog(LoaderErrorLogViewModel viewModel)
    {
      bool result = false;

      using (DALContext.BCMStrategyEntities db = new DALContext.BCMStrategyEntities())
      {
        ////var linkLoaderLog = db.loaderlinklog.Where(x => x.SiteURL == viewModel.SiteUrl).FirstOrDefault();

        loadererrorlog log = new loadererrorlog();
        log.ProcessId = viewModel.ProcessId;
        log.ProcessIntanceId = viewModel.ProcessInstanceId;
        log.WebSiteId = viewModel.WebSiteId;
        log.SiteURL = viewModel.SiteUrl;
        log.ErrorDesc = ASCIIEncoding.ASCII.GetBytes(viewModel.ErrorDesc.ToString());
        log.Created = Helper.GetCurrentDateTime();
        db.loadererrorlog.Add(log);
        result = db.SaveChanges() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
      }
      return result;
    }

    /// <summary>
    /// Delete from the loader link log record
    /// </summary>
    /// <param name="processLinkDetails">Process Link details</param>
    /// <returns>Boolean value for deleting the record</returns>
    public bool DeleteLoaderLinkLogRecord(ProcessLinkDetails processLinkDetails)
    {
      bool result = false;

      using (DALContext.BCMStrategyEntities db = new DALContext.BCMStrategyEntities())
      {
        var linkLog = db.loaderlinklog.Where(x => x.Guid == processLinkDetails.Guid && x.ProcessId == processLinkDetails.ProcessId).FirstOrDefault();

        db.loaderlinklog.Remove(linkLog);
        result = db.SaveChanges() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
      }

      return result;
    }

    public List<LoaderLinkQueue> GetAllLinksInnerLinks()
    {
      List<LoaderLinkQueue> processLinkDetails;

      using (DALContext.BCMStrategyEntities db = new DALContext.BCMStrategyEntities())
      {
        processLinkDetails = (from load in db.loaderlinklog.Where(x => x.StatusId == 1)
                              select new LoaderLinkQueue
                              {
                                WebSiteId = load.WebSiteId != null ? load.WebSiteId : 0,
                                SiteURL = load.SiteURL,
                                GUID = load.Guid,
                                ProcessId = load.ProcessId,
                                ProcessInstanceId = load.ProcessInstanceId,

                                InstanceName = load.InstanceName,
                                LinkLevel = load.LinkLevel != null ? load.LinkLevel.Value : 0,
                                WebLinkBytes = load.WebLinkBytes != null ? load.WebLinkBytes.Value : 0,
                                IsSuccessful = load.IsSuccessful != null ? load.IsSuccessful.Value : Helper.saveChangesNotSuccessful,
                                NewerProcessId = load.NewProcessId != null ? load.NewProcessId.Value : 0
                              }).ToList();
      }

      return processLinkDetails;
    }

    public bool GetAllLinksBasedUponWebSiteURL(string siteURL)
    {
      bool recordExist = false;

      using (DALContext.BCMStrategyEntities db = new DALContext.BCMStrategyEntities())
      {
        recordExist = db.loaderlinklog.Any(x => x.SiteURL.ToLower() == siteURL.ToLower());
      }

      return recordExist;
    }

    public int InsertInScanningLinkQueue(LoaderLinkQueue linkQueueRecord)
    {
      int scanningLinkId = 0;

      using (DALContext.BCMStrategyEntities db = new DALContext.BCMStrategyEntities())
      {
        scanninglinkqueue linkQueueDB = new scanninglinkqueue();

        linkQueueDB.Guid = linkQueueRecord.GUID;
        linkQueueDB.WebSiteId = linkQueueRecord.WebSiteId;
        linkQueueDB.ReadTaken = 1;

        linkQueueDB.ProcessId = linkQueueRecord.ProcessId;
        linkQueueDB.CreatedBy = Helper.ShowScrapperName();
        linkQueueDB.Created = Helper.GetCurrentDateTime();

        db.scanninglinkqueue.Add(linkQueueDB);
        db.SaveChanges();

        scanningLinkId = linkQueueDB.Id;
      }

      return scanningLinkId;
    }

    public bool InsertInScanningLinkDetail(LoaderLinkQueue linkQueueRecord, int scanningLinkId)
    {
      bool result = false;

      using (DALContext.BCMStrategyEntities db = new DALContext.BCMStrategyEntities())
      {
        scanninglinkdetails linkDetailsDB = new scanninglinkdetails()
        {
          Guid = linkQueueRecord.GUID,
          WebSiteURL = linkQueueRecord.SiteURL,
          ScannerLinkId = scanningLinkId,
          ProcessId = linkQueueRecord.ProcessId,
          ProcessInstanceId = linkQueueRecord.ProcessInstanceId,
          StatusId = 1,
          PublishDate = linkQueueRecord.PublishDate,
          PageType = linkQueueRecord.PageType,
          CreatedBy = Helper.ShowScrapperName(),
          Created = Helper.GetCurrentDateTime()
        };

        db.scanninglinkdetails.Add(linkDetailsDB);
        result = db.SaveChanges() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
      }

      return result;
    }

    /// <summary>
    /// Get the count of all the processed contents
    /// </summary>
    /// <param name="processId"></param>
    /// <param name="processInstanceId"></param>
    /// <returns></returns>
    public List<LoaderLinkQueue> GetWebLinkForLexicons(int processId, int processInstanceId)
    {
      List<LoaderLinkQueue> listOfWebLinks;

      using (DALContext.BCMStrategyEntities db = new DALContext.BCMStrategyEntities())
      {
        listOfWebLinks = (from w in db.scanninglinkdetails.AsNoTracking().Where(x => x.ProcessId == processId && x.ProcessInstanceId == processInstanceId)
                          select new LoaderLinkQueue
                          {
                            Id = w.Id,
                            SiteURL = w.WebSiteURL,
                            GUID = w.Guid,
                            WebSiteId = w.scanninglinkqueue.WebSiteId,
                            IsHardCoded = w.scanninglinkqueue.weblinks.IsHardCoded,
                            WebLinkTypeId = w.scanninglinkqueue.weblinks.websitetypes.TypeId.HasValue ? w.scanninglinkqueue.weblinks.websitetypes.TypeId.Value : 200,
                            PageType = w.PageType,
                            ProcessId = processId,
                            ProcessInstanceId = processInstanceId,
														PublishDate = w.PublishDate,
														ParentURL = w.scanninglinkqueue.weblinks.WebLinkURL
													}).ToList();
      }

      return listOfWebLinks;
    }

    /// <summary>
    /// Get All WebLink PageDDL
    /// </summary>
    /// <returns></returns>
    public async Task<WebLinkViewModel> GetAllWebLinkPageDDL(bool isEdit = false)
    {
      WebLinkViewModel model = new WebLinkViewModel();
      using (DALContext.BCMStrategyEntities db = new DALContext.BCMStrategyEntities())
      {
        if (!isEdit)
        {
          model.PageTypeDDL = await db.category.Select(a => new DropdownMaster()
          {
            Key = a.Id,
            Value = a.Name
          }).ToListAsync();

          model.WebSiteTypeDDL = await db.websitetypes.Select(a => new DropdownMaster()
          {
            Key = a.Id,
            Value = a.TypeName
          }).ToListAsync();
          model.EntityTypeDDL = await db.institutiontypes.Select(a => new DropdownMaster()
          {
            Key = a.Id,
            Value = a.InstitutionType
          }).ToListAsync();
        }
        else
        {
          model.ActivityTypeDDL = await db.metadatavalue.Where(a => a.activitytype != null && a.activitytype.ActivityName != null).Select(a => new DropdownMaster()
          {
            Key = a.Id,
            Value = a.activitytype.ActivityName
          }).ToListAsync();

          model.MetaDataProprietaryDDL = await db.metadatatypes.Select(a => new DropdownMaster()
          {
            Key = a.Id,
            Value = a.MetaData
          }).ToListAsync();
        }
        model.SectorDDL = await db.sector.Select(a => new DropdownMaster()
        {
          Key = a.Id,
          Value = a.SectorName
        }).ToListAsync();
      }
      return model;
    }

    /// <summary>
    /// Get Action Type Based On WebsiteType
    /// </summary>
    /// <param name="websiteTypeHashId"></param>
    /// <returns></returns>
    public async Task<WebLinkViewModel> GetActionTypeBasedOnWebsiteType(WebLinkViewModel webLinkModel)
    {
      WebLinkViewModel model = new WebLinkViewModel();
      int webLinkId = webLinkModel.WebLinkMasterHashId.ToDecrypt().ToInt32();
      int websiteTypeId = webLinkModel.WebSiteTypeHashId.ToDecrypt().ToInt32();
      using (DALContext.BCMStrategyEntities db = new DALContext.BCMStrategyEntities())
      {
        if (webLinkModel.IsEdit)
        {
          var actionTypeIds = db.weblinkproprietarytags.Where(a => a.WeblinkId == webLinkId).Select(a => a.MetaDataTypeId).ToList();
          var activityTypeIds = db.weblinkactivitytype.Where(a => a.WebLinkId == webLinkId).Select(a => a.MetaDataTypeId).ToList();
          model.MetaDataProprietaryDDL = await db.metadatatypes.Where(a => a.WebSiteTypeId == websiteTypeId).Select(a => new DropdownMaster()
          {
            Key = a.Id,
            Value = a.MetaData
          }).ToListAsync();

          model.ActivityTypeDDL = await db.metadatavalue.Where(a => a.activitytype != null && a.activitytype.ActivityName != null && a.ActivityTypeId.HasValue && actionTypeIds.Contains(a.MetaDataTypeId)).Select(a => new DropdownMaster()
          {
            Key = a.Id,
            Value = a.activitytype.ActivityName
          }).ToListAsync();

          model.MetaDataProprietaryDDL.Where(b => actionTypeIds.Contains((int)b.Key)).ToList().ForEach(a =>
          {
            a.data = true;
          });

          model.ActivityTypeDDL.Where(b => activityTypeIds.Contains((int)b.Key)).ToList().ForEach(a =>
          {
            a.data = true;
          });
        }
        else
        {
          model.MetaDataProprietaryDDL = await db.metadatatypes.Where(a => a.WebSiteTypeId == websiteTypeId).Select(a => new DropdownMaster()
          {
            Key = a.Id,
            Value = a.MetaData
          }).ToListAsync();
        }
      }
      return model;
    }

    /// <summary>
    /// Get Activity Type BasedOnActionType
    /// </summary>
    /// <param name="actionTypeHashIds"></param>
    /// <returns></returns>
    public async Task<WebLinkViewModel> GetActivityTypeBasedOnActionType(List<string> actionTypeHashIds, bool isEdit)
    {
      WebLinkViewModel model = new WebLinkViewModel();
      List<int> actionTypeIds = actionTypeHashIds.Any() ? actionTypeHashIds.Select(a => a.ToDecrypt().ToInt32()).ToList() : new List<int>();
      using (DALContext.BCMStrategyEntities db = new DALContext.BCMStrategyEntities())
      {
        model.ActivityTypeDDL = await db.metadatavalue.Where(a => a.activitytype != null && a.activitytype.ActivityName != null && actionTypeIds.Contains(a.MetaDataTypeId)).Select(a => new DropdownMaster()
        {
          Key = a.Id,
          Value = a.activitytype.ActivityName
        }).ToListAsync();
      }
      return model;
    }



    /// <summary>
    /// Get All Individual Person
    /// </summary>
    /// <param name="searchTerm"></param>
    /// <returns></returns>
    public async Task<ApiOutput> GetAllIndividualPerson(GridParameters parameters)
    {
      ApiOutput apiOutput = new ApiOutput();
      List<IndividualPersonViewModel> list = null;
      int totalRecord = 0;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        IQueryable<IndividualPersonViewModel> query = db.policymakers
            .Select(x => new IndividualPersonViewModel()
            {
              PolicyMakerMasterId = x.Id,
              FirstName = x.PolicyFirstName,
              LastName = x.PolicyLastName,
              Designation = x.designation != null ? x.designation.Designation1 : string.Empty
            });
        if (parameters.Sort == null || parameters.Sort.Count == 0)
        {
          query = query.OrderByDescending(x => x.PolicyMakerMasterId);
        }
        list = await query.ModifyList(parameters, out totalRecord).ToListAsync();
      }
      apiOutput.Data = list;
      apiOutput.TotalRecords = totalRecord;
      return apiOutput;
    }

    /// <summary>
    /// Get All Web Links
    /// </summary>
    /// <param name="searchTerm"></param>
    /// <returns></returns>
    public async Task<ApiOutput> GetAllWebLinks(GridParameters parametersJson)
    {
      ApiOutput apiOutput = new ApiOutput();
      List<WebLinkViewModel> list = null;
      int totalRecord = 0;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        IQueryable<WebLinkViewModel> query = db.weblinks.Where(a => !a.IsDeleted)
            .Select(x => new WebLinkViewModel()
            {
              WebLinkMasterId = x.Id,
              IsHardCodedString = x.IsHardCoded ? "Yes" : "No",
              CreatedDate = x.Created,
              EntityFullName = x.institution.InstitutionName,
              WebSiteType = x.websitetypes.TypeName,
              WebLinkUrl = x.WebLinkURL,
              PageType = x.category.Name,
              EntityType = x.institutiontypes.InstitutionType,
              CountryName = x.country != null ? x.country.Name : string.Empty,
              IsActiveString = x.IsActive ? "Yes" : "No"
            });
        if (parametersJson.Sort == null || parametersJson.Sort.Count == 0)
        {
          query = query.OrderByDescending(x => x.CreatedDate);
        }
        list = await query.ModifyList(parametersJson, out totalRecord).ToListAsync();
      }
      apiOutput.Data = list;
      apiOutput.TotalRecords = totalRecord;
      return apiOutput;
    }

    /// <summary>
    /// Get All Entity Name
    /// </summary>
    /// <param name="searchTerm"></param>
    /// <returns></returns>
    public async Task<List<DropdownMaster>> GetAllEntityFullName(string searchTerm)
    {
      using (DALContext.BCMStrategyEntities db = new DALContext.BCMStrategyEntities())
      {
        List<DropdownMaster> dropDownList = await db.institution.Where(a => (a.InstitutionName).Contains(searchTerm)).Select(a => new DropdownMaster()
        {
          Key = a.Id,
          Value = a.InstitutionName
        }).ToListAsync();

        return dropDownList;
      }
    }

    /// <summary>
    /// Update Web Link
    /// </summary>
    /// <param name="webLinkViewModel"></param>
    /// <returns></returns>
    public async Task<bool> UpdateWebLink(WebLinkViewModel webLinkViewModel)
    {
      using (DALContext.BCMStrategyEntities db = new DALContext.BCMStrategyEntities())
      {
        #region Get List Of Ids

        List<int> activityTypeIds = webLinkViewModel.ActivityTypeHashIds.Any() ? webLinkViewModel.ActivityTypeHashIds.Select(a => a.ToDecrypt().ToInt32()).ToList() : new List<int>();
        List<int> sectorIds = webLinkViewModel.SectorHashIds.Any() ? webLinkViewModel.SectorHashIds.Select(a => a.ToDecrypt().ToInt32()).ToList() : new List<int>();
        List<int> proprierityIds = webLinkViewModel.ProprietaryHashIds.Any() ? webLinkViewModel.ProprietaryHashIds.Select(a => a.ToDecrypt().ToInt32()).ToList() : new List<int>();
        List<int> personIds = webLinkViewModel.IndividualPersonHashIds.Any() ? webLinkViewModel.IndividualPersonHashIds.Split(';').Select(a => a.ToDecrypt().ToInt32()).ToList() : new List<int>();

        #endregion Get List Of Ids

        if (!string.IsNullOrEmpty(webLinkViewModel.WebLinkMasterHashId))
        {
          #region Update

          int webLinkId = webLinkViewModel.WebLinkMasterHashId.ToDecrypt().ToInt32();
          weblinks dbWebLink = db.weblinks.Where(a => a.Id == webLinkId).FirstOrDefault();
          WebLinkAuditModel beforeModel = GetWebLinkAuditModel(dbWebLink);
          dbWebLink.WebSiteTypeId = webLinkViewModel.WebSiteTypeHashId.ToDecrypt().ToInt32();
          dbWebLink.WebLinkURL = webLinkViewModel.WebLinkUrl;
          dbWebLink.IsHardCoded = webLinkViewModel.IsBlocked;
          dbWebLink.CountryId = !string.IsNullOrEmpty(webLinkViewModel.CountryHashId) ? webLinkViewModel.CountryHashId.ToDecrypt().ToInt32() : (int?)null;
          ////dbWebLink.CategoryId = !string.IsNullOrEmpty(webLinkViewModel.PageTypeHashId) ? webLinkViewModel.PageTypeHashId.ToDecrypt().ToInt32() : (int?)null;
          dbWebLink.InstitutionTypeId = !string.IsNullOrEmpty(webLinkViewModel.EntityTypeHashId) ? webLinkViewModel.EntityTypeHashId.ToDecrypt().ToInt32() : (int?)null;
          dbWebLink.EntityId = !string.IsNullOrEmpty(webLinkViewModel.EntityFullNameHashId) ? webLinkViewModel.EntityFullNameHashId.ToDecrypt().ToInt32() : (int?)null;
          dbWebLink.Modified = DateTime.Now;
          dbWebLink.IsActive = webLinkViewModel.IsActive;
          dbWebLink.ModifiedBy = UserAccessHelper.CurrentUserIdentity.ToString();

          #region sector

          List<int> dbWebLinkSectorIds = dbWebLink.weblinksector.Select(a => a.SectorId).ToList();
          List<int> dbSectorIdsToReove = dbWebLinkSectorIds.Where(a => !sectorIds.Contains(a)).ToList();
          List<int> newSectorsIdsToAdd = sectorIds.Where(a => !dbWebLinkSectorIds.Contains(a)).ToList();
          List<weblinksector> sectorsToRemove = dbWebLink.weblinksector.Where(a => dbSectorIdsToReove.Contains(a.SectorId)).ToList();
          db.weblinksector.RemoveRange(sectorsToRemove);

          List<weblinksector> webLinkSectorsToAdd = newSectorsIdsToAdd.Select(id => new weblinksector()
          {
            SectorId = id,
            WebLinkId = webLinkId
          }).ToList();

          db.weblinksector.AddRange(webLinkSectorsToAdd);

          #endregion sector

          #region Proprietary Tag

          List<int> dbWebLinkProprieteryIds = dbWebLink.weblinkproprietarytags.Select(a => a.MetaDataTypeId).ToList();
          List<int> dbProprieteryIdsToRemove = dbWebLinkProprieteryIds.Where(a => !proprierityIds.Contains(a)).ToList();
          List<int> newProprieteryIdsToAdd = proprierityIds.Where(a => !dbWebLinkProprieteryIds.Contains(a)).ToList();
          List<weblinkproprietarytags> proprieteryToRemove = dbWebLink.weblinkproprietarytags.Where(a => dbProprieteryIdsToRemove.Contains(a.MetaDataTypeId)).ToList();
          db.weblinkproprietarytags.RemoveRange(proprieteryToRemove);

          List<weblinkproprietarytags> webLinkProprieteryToAdd = newProprieteryIdsToAdd.Select(id => new weblinkproprietarytags()
          {
            MetaDataTypeId = id,
            WeblinkId = webLinkId
          }).ToList();

          db.weblinkproprietarytags.AddRange(webLinkProprieteryToAdd);

          #endregion Proprietary Tag

          #region Activity Type

          List<int> dbWebLinkActivityTypeIds = dbWebLink.weblinkactivitytype.Select(a => a.MetaDataTypeId).ToList();
          List<int> dbActivityTypeIdsToRemove = dbWebLinkActivityTypeIds.Where(a => !activityTypeIds.Contains(a)).ToList();
          List<int> newActivityTypeIdsToAdd = activityTypeIds.Where(a => !dbWebLinkActivityTypeIds.Contains(a)).ToList();
          List<weblinkactivitytype> activityTypesToRemove = dbWebLink.weblinkactivitytype.Where(a => dbActivityTypeIdsToRemove.Contains(a.MetaDataTypeId)).ToList();
          db.weblinkactivitytype.RemoveRange(activityTypesToRemove);

          List<weblinkactivitytype> webLinkActivityTypeToAdd = newActivityTypeIdsToAdd.Select(id => new weblinkactivitytype()
          {
            MetaDataTypeId = id,
            WebLinkId = webLinkId
          }).ToList();

          db.weblinkactivitytype.AddRange(webLinkActivityTypeToAdd);

          #endregion Activity Type

          #region policyMakers

          List<int> dbWebLinkPolicyMakerIds = dbWebLink.weblinkpolicymaker.Select(a => a.PolicyMakerId).ToList();
          List<int> dbPolicyMakersToReove = dbWebLinkPolicyMakerIds.Where(a => !personIds.Contains(a)).ToList();
          List<int> newPolicyMakersIdsToAdd = personIds.Where(a => !dbWebLinkPolicyMakerIds.Contains(a)).ToList();
          List<weblinkpolicymaker> policyMakerToRemove = dbWebLink.weblinkpolicymaker.Where(a => dbPolicyMakersToReove.Contains(a.PolicyMakerId)).ToList();
          db.weblinkpolicymaker.RemoveRange(policyMakerToRemove);

          List<weblinkpolicymaker> weblinkPolicymakerToAdd = newPolicyMakersIdsToAdd.Select(id => new weblinkpolicymaker()
          {
            PolicyMakerId = id,
            WebLinkId = webLinkId
          }).ToList();

          db.weblinkpolicymaker.AddRange(weblinkPolicymakerToAdd);

          #endregion policyMakers

          #region PageTypeConfig

          ////List<weblinkrss> dbWebLinkPageTypeConfigIds = dbWebLink.weblinkrss.ToList();
          ////db.weblinkrss.RemoveRange(dbWebLinkPageTypeConfigIds);

          ////List<weblinkrss> configList = webLinkViewModel.RSSFeedURLs.Any() ? (webLinkViewModel.RSSFeedURLs.Select(x => new weblinkrss()
          ////{
          ////  ////IsRSSFeedAvailable = true,
          ////  WebSiteId = webLinkId,
          ////  RSSFeedURL = x
          ////}).ToList()) : (webLinkViewModel.SearchKeywords.Any() ? (webLinkViewModel.SearchKeywords.Select(x => new weblinkrss()
          ////{
          ////  ////IsSearchFunctionality = true,
          ////  WebSiteId = webLinkId
          ////  ////SearchKeyWord = x
          ////}).ToList()) : new List<weblinkrss>());

          ////db.weblinkrss.AddRange(configList);

          #endregion PageTypeConfig

          db.SaveChanges();
          WebLinkAuditModel afterModel = GetWebLinkAuditModel(dbWebLink);
          Task.Run(() => AuditRepository.WriteAudit<WebLinkAuditModel>(AuditConstants.WebLinks, AuditType.Update, beforeModel, afterModel, AuditConstants.UpdateSuccessMsg));
          #endregion Update
        }
        else
        {
          #region ADD

          weblinks weblinkObj = new weblinks()
          {
            WebSiteTypeId = webLinkViewModel.WebSiteTypeHashId.ToDecrypt().ToInt32(),
            WebLinkURL = webLinkViewModel.WebLinkUrl,
            IsHardCoded = webLinkViewModel.IsBlocked,
            CountryId = !string.IsNullOrEmpty(webLinkViewModel.CountryHashId) ? webLinkViewModel.CountryHashId.ToDecrypt().ToInt32() : (int?)null,
            CategoryId = !string.IsNullOrEmpty(webLinkViewModel.PageTypeHashId) ? webLinkViewModel.PageTypeHashId.ToDecrypt().ToInt32() : (int?)null,
            InstitutionTypeId = !string.IsNullOrEmpty(webLinkViewModel.EntityTypeHashId) ? webLinkViewModel.EntityTypeHashId.ToDecrypt().ToInt32() : (int?)null,
            EntityId = !string.IsNullOrEmpty(webLinkViewModel.EntityFullNameHashId) ? webLinkViewModel.EntityFullNameHashId.ToDecrypt().ToInt32() : (int?)null,
            Created = DateTime.Now,
            IsActive = webLinkViewModel.IsActive,
            weblinkactivitytype = activityTypeIds.Select(id => new weblinkactivitytype()
            {
              MetaDataTypeId = id
            }).ToList(),
            weblinkpolicymaker = personIds.Select(id => new weblinkpolicymaker()
            {
              PolicyMakerId = id
            }).ToList(),
            weblinkproprietarytags = proprierityIds.Select(id => new weblinkproprietarytags()
            {
              MetaDataTypeId = id
            }).ToList(),
            weblinksector = sectorIds.Select(id => new weblinksector()
            {
              SectorId = id
            }).ToList()

            ////weblinkrss = webLinkViewModel.RSSFeedURLs.Any() ? (webLinkViewModel.RSSFeedURLs.Select(x => new weblinkrss()
            ////{
            ////  ////IsRSSFeedAvailable = true,
            ////  RSSFeedURL = x
            ////}).ToList()) : (webLinkViewModel.SearchKeywords.Any() ? (webLinkViewModel.SearchKeywords.Select(x => new weblinkrss()
            ////{
            ////  ////IsSearchFunctionality = true,
            ////  ////SearchKeyWord = x
            ////}).ToList()) : null)

          };

          db.weblinks.Add(weblinkObj);
          await db.SaveChangesAsync();
          WebLinkAuditModel model = GetWebLinkAuditModel(weblinkObj);
          Task.Run(() => AuditRepository.WriteAudit<WebLinkAuditModel>(AuditConstants.WebLinks, AuditType.Insert, null, model, AuditConstants.InsertSuccessMsg));
          #endregion ADD
        }
      }
      return true;
    }

    /// <summary>
    /// Delete Web Link
    /// </summary>
    /// <param name="institutionMasterHashId">Institution id to Delete</param>
    /// <returns>return successfull message</returns>
    public async Task<bool> DeleteWebLink(string webLinkHashId)
    {
      bool isSave = false;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        if (!string.IsNullOrEmpty(webLinkHashId))
        {
          int decryptWebLinkId = webLinkHashId.ToDecrypt().ToInt32();

          var objWebLink = await db.weblinks.Where(x => x.Id == decryptWebLinkId && !x.IsDeleted).FirstOrDefaultAsync();

          if (objWebLink != null && !objWebLink.contentloaderlog.Any())
          {
            objWebLink.IsDeleted = true;
            isSave = await db.SaveChangesAsync() > 0;

            WebLinkAuditModel webLinkAuditModel = GetWebLinkAuditModel(objWebLink);
            Task.Run(() => AuditRepository.WriteAudit<WebLinkAuditModel>(AuditConstants.WebLinks, AuditType.Delete, webLinkAuditModel, null, AuditConstants.DeleteSuccessMsg));
          }
        }
      }
      return isSave;
    }

    private WebLinkAuditModel GetWebLinkAuditModel(weblinks webLinkModel)
    {
      WebLinkAuditModel model = null;
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        var webLinkObj = db.weblinks.Where(a => a.Id == webLinkModel.Id).FirstOrDefault();
        model = new WebLinkAuditModel()
        {
          WebsiteType = webLinkObj.websitetypes != null ? webLinkObj.websitetypes.TypeName : string.Empty,
          WeblinkURL = webLinkObj.WebLinkURL,
          HardCoded = webLinkObj.IsHardCoded,
          EntityName = webLinkObj.institution != null ? webLinkObj.institution.EntityName : string.Empty,
          EntityType = webLinkObj.institutiontypes != null ? webLinkObj.institutiontypes.InstitutionType : string.Empty,
          Country = webLinkObj.country != null ? webLinkObj.country.Name : string.Empty,
          PageType = webLinkObj.category != null ? webLinkObj.category.Name : string.Empty,
          Active = webLinkObj.IsActive,
          Created = webLinkObj.Created.HasValue ? Helper.GetCurrentFormatedDateTime() : string.Empty,
          Modified = webLinkObj.Modified.HasValue ? webLinkObj.Modified.ToFormatedDateTime() : string.Empty,
          CreatedBy = webLinkObj.CreatedBy,
          ModifiedBy = webLinkObj.ModifiedBy
        };
      }
      return model;
    }

    /// <summary>
    /// Get WebLink Based On Hash Id
    /// </summary>
    /// <param name="webLinkHashId"></param>
    /// <returns></returns>
    public async Task<WebLinkViewModel> GetWebLinkBasedOnHashId(string webLinkHashId)
    {
      WebLinkViewModel model = new WebLinkViewModel();
      int webLinkId = webLinkHashId.ToDecrypt().ToInt32();
      using (DALContext.BCMStrategyEntities db = new DALContext.BCMStrategyEntities())
      {
        var webLinkObj = db.weblinks.Where(a => a.Id == webLinkId).FirstOrDefault();
        var webLinkSectorIds = webLinkObj.weblinksector.Select(a => a.SectorId).ToList();
        var activityTypeIds = webLinkObj.weblinkactivitytype.Select(a => a.MetaDataTypeId).ToList();
        var webLinkproprietaryIds = webLinkObj.weblinkproprietarytags.Select(a => a.MetaDataTypeId).ToList();
        if (!string.IsNullOrEmpty(webLinkHashId))
        {
          model.WebSiteTypeId = webLinkObj.WebSiteTypeId;
          model.WebSiteType = webLinkObj.websitetypes.TypeName;
          model.WebLinkUrl = webLinkObj.WebLinkURL;
          model.IsBlocked = webLinkObj.IsHardCoded;
          model.pageTypeId = webLinkObj.category != null ? (int)webLinkObj.CategoryId : 0;
          model.EntityFullNameId = webLinkObj.EntityId.HasValue ? (int)webLinkObj.EntityId : 0;
          model.EntityFullName = webLinkObj.institution != null ? (webLinkObj.institution.InstitutionName) : string.Empty;
          model.EntityTypeId = webLinkObj.institutiontypes != null ? webLinkObj.institutiontypes.Id : 0;
          model.CountryId = webLinkObj.country != null ? (int)webLinkObj.CountryId : 0;
          model.IsActive = webLinkObj.IsActive ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
          var pageTypes = webLinkObj.weblinkrss.ToList();
          ////model.SearchKeywords = pageTypes.Any(a => a.IsSearchFunctionality != null && (bool)a.IsSearchFunctionality) ? pageTypes.Select(a => a.SearchKeyWord).ToList() : new List<string>();
          model.RSSFeedURLs = pageTypes.Select(a => a.RSSFeedURL).ToList();

          List<string> policyMakerHashIdList = webLinkObj.weblinkpolicymaker.Select(a => a.PolicyMakerId.ToEncrypt()).ToList();
          model.IndividualPersonHashIds = policyMakerHashIdList.Any() ? string.Join(";", policyMakerHashIdList) : string.Empty;

          model.PolicyMakerModelList = webLinkObj.weblinkpolicymaker.Select(a => new PolicyMakerModel()
          {
            PolicyMakerId = a.PolicyMakerId,
            PolicyFirstName = a.policymakers.PolicyFirstName,
            PolicyLastName = a.policymakers.PolicyLastName,
          }).ToList();

          WebLinkViewModel DDLModel = await GetAllWebLinkPageDDL(true);
          DDLModel.SectorDDL.Where(a => webLinkSectorIds.Contains((int)a.Key)).ToList().ForEach(b =>
          {
            b.data = true;
          });

          DDLModel.ActivityTypeDDL.Where(a => activityTypeIds.Contains((int)a.Key)).ToList().ForEach(b =>
          {
            b.data = true;
          });

          DDLModel.MetaDataProprietaryDDL.Where(a => webLinkproprietaryIds.Contains((int)a.Key)).ToList().ForEach(b =>
          {
            b.data = true;
          });

          model.SectorDDL = DDLModel.SectorDDL;
          model.ActivityTypeDDL = DDLModel.ActivityTypeDDL;
          model.MetaDataProprietaryDDL = DDLModel.MetaDataProprietaryDDL;
        }
      }

      return model;
    }

    public async Task<bool> IsDuplicateWebLink(WebLinkViewModel webLinkViewModel)
    {
      bool isDuplicate = false;
      using (DALContext.BCMStrategyEntities db = new DALContext.BCMStrategyEntities())
      {
        if (!string.IsNullOrEmpty(webLinkViewModel.WebLinkMasterHashId))
        {
          int webLinkId = webLinkViewModel.WebLinkMasterHashId.ToDecrypt().ToInt32();
          isDuplicate = db.weblinks.Any(a => a.Id != webLinkId && a.WebLinkURL == webLinkViewModel.WebLinkUrl && !a.IsDeleted);
        }
        else
        {
          isDuplicate = db.weblinks.Any(a => a.WebLinkURL == webLinkViewModel.WebLinkUrl && !a.IsDeleted);
        }
        return isDuplicate;
      }
    }

    public bool IsFullScrapperActivityCompleted(int scrapperDetailId)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        ScrappingProcessSummaryModel scrappingProcessModel = (from data in db.scanninglinkqueue
                                                              where data.ReadTaken == 1 && data.scanninglinkdetails.Any(x => x.Id == scrapperDetailId)
                                                              select new ScrappingProcessSummaryModel()
                                                              {
                                                                ChildURLAvaliable = data.scanninglinkdetails.Any(),
                                                                ChildURLList = data.scanninglinkdetails.Select(scanDetail => new ScrappingSummaryData()
                                                                {
                                                                  LexiconCount = scanDetail.scrapedcontents.Count,
                                                                  ProprietaryTagsCount = scanDetail.scrappedproprietorytags.Count,
                                                                  StandardTagsCount = scanDetail.scrappedstandardtags.Count
                                                                }).ToList()
                                                              }).FirstOrDefault();

        return scrappingProcessModel.ChildURLAvaliable &&
               scrappingProcessModel.ChildURLList.Any(x => x.LexiconCount > 0) &&
               scrappingProcessModel.ChildURLList.Any(x => x.ProprietaryTagsCount > 0) &&
               scrappingProcessModel.ChildURLList.Any(x => x.StandardTagsCount > 0);
      }
    }

    public bool IsFullScrapperActivityProcessCompleted(int processId, int ProcessInstanceId)
    {
      using (BCMStrategyEntities db = new BCMStrategyEntities())
      {
        List<ScrappingProcessSummaryModel> scrappingProcessModel = (from data in db.scanninglinkqueue
                                                                    where data.ReadTaken == 1
                                                                    && data.ProcessId == processId
                                                                    && data.scanninglinkdetails.Any(x => x.ProcessInstanceId == ProcessInstanceId)
                                                                    select new ScrappingProcessSummaryModel()
                                                                    {
                                                                      ChildURLAvaliable = data.scanninglinkdetails.Any(),
                                                                      ChildURLList = data.scanninglinkdetails.Select(scanDetail => new ScrappingSummaryData()
                                                                      {
                                                                        LexiconCount = scanDetail.scrapedcontents.Count,
                                                                        ProprietaryTagsCount = scanDetail.scrappedproprietorytags.Count,
                                                                        StandardTagsCount = scanDetail.scrappedstandardtags.Count
                                                                      }).ToList()
                                                                    }).ToList();


        var result = scrappingProcessModel.Any(x =>
         x.ChildURLAvaliable
         && x.ChildURLList.Any(y => y.LexiconCount > 0)
         && x.ChildURLList.Any(y => y.ProprietaryTagsCount > 0)
         && x.ChildURLList.Any(y => y.StandardTagsCount > 0));

        return result;
      }
    }

    /// <summary>
    /// Get Web Link Lexicon Count details
    /// </summary>
    /// <param name="webSiteId">webSiteId details</param>
    /// <returns>Lexicon count record</returns>
    public int GetWebLinkLexiconCount(int webSiteId)
    {
      int lexiconCount = 0;

      using (DALContext.BCMStrategyEntities db = new DALContext.BCMStrategyEntities())
      {
        lexiconCount = Convert.ToInt32(db.loaderlinklog.Where(x => x.WebSiteId == webSiteId).Select(y => y.LexiconCount).FirstOrDefault());
      }

      return lexiconCount;
    }

    /// <summary>
    /// Update Loader Link Log Lexicon Count
    /// </summary>
    /// <param name="linkDetails">link Details</param>
    /// <returns>Boolean value for the successful / un-successful deletion </returns>
    public bool UpdateLoaderLinkLogLexiconCount(LoaderLinkQueue linkDetails)
    {
      bool result = false;

      using (DALContext.BCMStrategyEntities db = new DALContext.BCMStrategyEntities())
      {
        var linkLoaderLog = db.loaderlinklog.Where(x => x.Guid == linkDetails.GUID && x.LinkLevel == 0).FirstOrDefault();

        if (linkLoaderLog != null)
        {
          linkLoaderLog.LexiconCount = linkDetails.LexiconCount;

          result = db.SaveChanges() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
        }
      }

      return result;
    }

    /// <summary>
    /// Delete Scanning Link Details
    /// </summary>
    /// <param name="linkDetails">linkDetails</param>
    /// <returns>Boolean value for the successful / un-successful deletion </returns>
    public bool DeleteScanningLinkQueue(int scanningLinkQueueId)
    {
      bool result = false;

      using (DALContext.BCMStrategyEntities db = new DALContext.BCMStrategyEntities())
      {
        var scanningLinkQueue = db.scanninglinkqueue.Where(x => x.Id == scanningLinkQueueId).FirstOrDefault();

        db.scanninglinkqueue.Remove(scanningLinkQueue);
        result = db.SaveChanges() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
      }

      return result;
    }

    public List<RssFeedDetails> GetRSSFeedByWebLinkId(int webLinkId)
    {
      List<RssFeedDetails> rssFeedInfo;

      using (DALContext.BCMStrategyEntities db = new DALContext.BCMStrategyEntities())
      {
        rssFeedInfo = (from z in db.weblinkrss.Where(x => x.WebSiteId == webLinkId)
                       select new RssFeedDetails
                       {
                         WebSiteId = z.WebSiteId,
                         RSSFeedURL = z.RSSFeedURL,
                         RegEx = z.RSSFeedClickRegEx
                       }).ToList();
      }
      return rssFeedInfo;
    }

    public List<ExcludedWebLinks> GetAllExcludedWebLinks()
    {
      List<ExcludedWebLinks> excludedLinks;

      using (DALContext.BCMStrategyEntities db = new DALContext.BCMStrategyEntities())
      {
        excludedLinks = (from z in db.urlexclusionlist
                         select new ExcludedWebLinks
                         {
                           URLExcluded = z.ExcludedURL
                         }).ToList();
      }

      return excludedLinks;
    }

    /// <summary>
    /// Get regular expression for the given Website Id
    /// </summary>
    /// <param name="websiteId">Website Id</param>
    /// <returns>List of Regular expression for the given page</returns>
    public List<WebLinkPageContentRegEx> GetRegularExpressionBasedUponWebsiteId(int websiteId)
    {
      List<WebLinkPageContentRegEx> pageContentRegEx;

      using (DALContext.BCMStrategyEntities db = new DALContext.BCMStrategyEntities())
      {
        pageContentRegEx = (from x in db.weblinkpagecontentregex.Where(x => x.WebSiteId == websiteId)
                            select new WebLinkPageContentRegEx
                            {
                              Id = x.Id,
                              PageContentRegEx = x.PageContentRegEx,
                              WebSiteId = x.WebSiteId
                            }).ToList();
      }
      return pageContentRegEx;
    }
  }
}