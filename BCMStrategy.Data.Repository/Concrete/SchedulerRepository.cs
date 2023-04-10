using BCMStrategy.Common.AuditLog;
using BCMStrategy.Common.Kendo;
using BCMStrategy.Common.Unity;
using BCMStrategy.DAL.Context;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Logger;
using BCMStrategy.Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Repository.Concrete
{
	public class SchedulerRepository : IScheduler
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

		public async Task<ApiOutput> GetDDSchedulerFrequencyList()
		{
			ApiOutput apiOutput = new ApiOutput();
			List<DropdownMaster> schedularfrequencyList = new List<DropdownMaster>();

			var ddlFrequency = Enum.GetValues(typeof(Helper.DdlFrequency)).Cast<object>().ToList();
			apiOutput.Data = ddlFrequency.Select(x => new DropdownMaster() { Key = (int)x, Value = x.ToString().Replace("_", " ") });
			apiOutput.TotalRecords = 0;
			apiOutput.ErrorMessage = schedularfrequencyList.Any() ? string.Empty : string.Format(Resource.ValidateMessageNoDataFound, Resource.LblFrequencyType);
			return apiOutput;
		}

		/// <summary>
		/// Add and update of Scheduler
		/// </summary>
		/// <param name="schedulerModel">scheduler Model with scheduler values</param>
		/// <returns>Is Saved or not</returns>
		public async Task<bool> UpdateScheduler(SchedulerModel schedulerModel)
		{
			bool isSave = false;
			DateTime? nullval = null;
			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{
				DateTime currentTimeStamp = Helper.GetCurrentDateTime();

				int decryptSchedularId = schedulerModel.SchedulerMasterHashId.ToDecrypt().ToInt32();

				if (string.IsNullOrEmpty(schedulerModel.SchedulerMasterHashId))
				{
					schedular objScheduler = new schedular()
					{
						FrequencyType = schedulerModel.FrequencyTypeMasterHashId.ToDecrypt().ToInt32(),
						Name = schedulerModel.WebsiteType,
						Description = schedulerModel.Description,
						StartDate = DateTime.Parse(schedulerModel.StartDateFinal),
						EndDate = schedulerModel.EndDateFinal != null && schedulerModel.EndDateFinal != "" ? DateTime.Parse(schedulerModel.EndDateFinal) : nullval,
						StartTime = CommonUtilities.ToUTCTimezone(DateTime.Parse(schedulerModel.StartTimeFinal)).TimeOfDay,
						IsEnabled = schedulerModel.IsEnabled,
						IsUpdated = true,
						Created = currentTimeStamp,
						CreatedBy = UserAccessHelper.CurrentUserIdentity.ToString()
					};

					if (schedulerModel.FrequencyTypeMasterHashId.ToDecrypt().ToInt32() == 2 || schedulerModel.FrequencyTypeMasterHashId.ToDecrypt().ToInt32() == 3)
					{
						objScheduler.schedularfrequency = SchedulerFrequencyList(schedulerModel);
					}
					db.schedular.Add(objScheduler);
				}
				else
				{
					int decryptSchedulerId = schedulerModel.SchedulerMasterHashId.ToDecrypt().ToInt32();
					int DecryptFrequencyTypeId = schedulerModel.FrequencyTypeMasterHashId.ToDecrypt().ToInt32();
					var objScheduler = await db.schedular.Where(x => x.Id == decryptSchedulerId && !x.IsDeleted).FirstOrDefaultAsync();

					var objSchedularObject = await db.schedular.Where(x => x.Id == decryptSchedularId && !x.IsDeleted).FirstOrDefaultAsync();

					SchedulerModel beforeUpdateModel = GetSchedularAuditModel(objSchedularObject);

					if (objScheduler != null)
					{
						objScheduler.FrequencyType = schedulerModel.FrequencyTypeMasterHashId.ToDecrypt().ToInt32();
						objScheduler.Name = schedulerModel.WebsiteType;
						objScheduler.Description = schedulerModel.Description;
						objScheduler.StartDate = DateTime.Parse(schedulerModel.StartDateFinal);
						objScheduler.EndDate = schedulerModel.EndDateFinal != null && schedulerModel.EndDateFinal != "" ? DateTime.Parse(schedulerModel.EndDateFinal) : nullval;
						objScheduler.StartTime = CommonUtilities.ToUTCTimezone(DateTime.Parse(schedulerModel.StartTimeFinal)).TimeOfDay;    ////DateTime.Parse(schedulerModel.StartTime).ToUCTTimezone();
						objScheduler.IsEnabled = schedulerModel.IsEnabled;
						objScheduler.IsUpdated = true;
						objScheduler.Modified = currentTimeStamp;
						objScheduler.ModifiedBy = UserAccessHelper.CurrentUserIdentity.ToString();
					}

					SchedulerModel afterUpdateModel = GetSchedularAuditModel(objScheduler);

					Task.Run(() => AuditRepository.WriteAudit<SchedulerModel>(AuditConstants.Scheduler, AuditType.Update, beforeUpdateModel, afterUpdateModel, AuditConstants.UpdateSuccessMsg));

					var objSchedulerFrequency = await db.schedularfrequency.Where(z => z.SchedularId == decryptSchedulerId).FirstOrDefaultAsync();
					if (DecryptFrequencyTypeId == 1 && objSchedulerFrequency != null)
					{
						db.schedularfrequency.Remove(objSchedulerFrequency);
					}
					else if (objSchedulerFrequency != null)
					{
						objSchedulerFrequency.RecurEvery = schedulerModel.RepeatEveryHour.ToInt32();
						objSchedulerFrequency.Sunday = schedulerModel.Sunday;
						objSchedulerFrequency.Monday = schedulerModel.Monday;
						objSchedulerFrequency.Tuesday = schedulerModel.Tuesday;
						objSchedulerFrequency.Thursday = schedulerModel.Thursday;
						objSchedulerFrequency.Wednesday = schedulerModel.Wednesday;
						objSchedulerFrequency.Friday = schedulerModel.Friday;
						objSchedulerFrequency.Saturday = schedulerModel.Saturday;
					}
					else
					{
						if (DecryptFrequencyTypeId == 2 || DecryptFrequencyTypeId == 3)
						{
							schedularfrequency objSchedulerForEdit = new schedularfrequency()
							{
								SchedularId = decryptSchedulerId,
								RecurEvery = schedulerModel.RepeatEveryHour.ToInt32(),
								Sunday = schedulerModel.Sunday,
								Monday = schedulerModel.Monday,
								Tuesday = schedulerModel.Tuesday,
								Thursday = schedulerModel.Thursday,
								Wednesday = schedulerModel.Wednesday,
								Friday = schedulerModel.Friday,
								Saturday = schedulerModel.Saturday
							};
							db.schedularfrequency.Add(objSchedulerForEdit);
						}
					}
				}
				isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
			}
			return isSave;
		}

		private List<schedularfrequency> SchedulerFrequencyList(SchedulerModel schedulerModel)
		{
			List<schedularfrequency> schedulerFrequencyList = new List<schedularfrequency>();
			schedularfrequency Objschedularfrequency = new schedularfrequency()
			{
				RecurEvery = schedulerModel.RepeatEveryHour.ToInt32(),
				Sunday = schedulerModel.Sunday,
				Monday = schedulerModel.Monday,
				Tuesday = schedulerModel.Tuesday,
				Wednesday = schedulerModel.Wednesday,
				Thursday = schedulerModel.Thursday,
				Friday = schedulerModel.Friday,
				Saturday = schedulerModel.Saturday
			};
			schedulerFrequencyList.Add(Objschedularfrequency);
			return schedulerFrequencyList;
		}

		private SchedulerModel GetSchedularAuditModel(schedular schedularModel)
		{
			SchedulerModel model = null;
			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{
				var institutionObj = db.schedular.Where(a => a.Id == schedularModel.Id).FirstOrDefault();
				model = new SchedulerModel()
				{
					Description = institutionObj.Description,
					EndDate = institutionObj.EndDate.HasValue ? institutionObj.EndDate.ToString() : string.Empty,
					StartDate = institutionObj.StartDate.ToString(),
					StartTime = institutionObj.StartTime.HasValue ? institutionObj.StartTime.ToString() : string.Empty,
					FrequencyType = institutionObj.FrequencyType.ToString(),
					IsDeleted = institutionObj.IsDeleted,
					IsEnabled = institutionObj.IsEnabled,
					CreatedBy = institutionObj.CreatedBy,
					ModifiedBy = institutionObj.ModifiedBy
				};
			}
			return model;
		}
		/// <summary>
		/// Delete scheduler
		/// </summary>
		/// <param name="schedulerMasterHashId">scheduler id to Delete</param>
		/// <returns>return successfull message</returns>
		public async Task<bool> DeleteScheduler(string schedulerMasterHashId)
		{
			bool isSave = false;
			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{

				if (!string.IsNullOrEmpty(schedulerMasterHashId))
				{
					int decryptSchedulerId = schedulerMasterHashId.ToDecrypt().ToInt32();

					var objScheduler = await db.schedular.Where(x => x.Id == decryptSchedulerId && !x.IsDeleted).FirstOrDefaultAsync();

					if (objScheduler != null)
					{
						objScheduler.IsDeleted = true;
						objScheduler.IsUpdated = true;
					}
				}
				isSave = await db.SaveChangesAsync() > 0 ? Helper.saveChangesSuccessful : Helper.saveChangesNotSuccessful;
			}
			return isSave;
		}

		/// <summary>
		/// Get all the list Scheduler
		/// </summary>
		/// <param name="parametersJson">Grid Parameter to filter or sorting</param>
		/// <returns>return the list</returns>
		public async Task<ApiOutput> GetAllSchedulerList(GridParameters parametersJson)
		{
			ApiOutput apiOutput = new ApiOutput();
			List<SchedulerModel> schedulerList;
			int totalRecord = 0;
			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{

				var query = db.schedular
					.Where(x => !x.IsDeleted)
						.Select(x => new
						{
							SchedulerMasterId = x.Id,
							Name = x.Name,
							Description = x.Description,
							FrequencyTypeMasterId = x.FrequencyType,
							FrequencyType = ((Helper.DdlFrequency)x.FrequencyType).ToString().Replace("_", " "),
							FrequencyTypeValue = x.FrequencyType,
							IsEnabled = x.IsEnabled,
							StartDate = x.StartDate,
							EndDate = x.EndDate,
							StartTime = x.StartTime,
							RecurEvery = x.schedularfrequency.Where(a => a.SchedularId == x.Id).Select(s => s.RecurEvery).FirstOrDefault().ToString(),
							Sunday = x.schedularfrequency.Where(a => a.SchedularId == x.Id).Select(s => s.Sunday).FirstOrDefault(),
							Monday = x.schedularfrequency.Where(a => a.SchedularId == x.Id).Select(s => s.Monday).FirstOrDefault(),
							Tuesday = x.schedularfrequency.Where(a => a.SchedularId == x.Id).Select(s => s.Tuesday).FirstOrDefault(),
							Wednesday = x.schedularfrequency.Where(a => a.SchedularId == x.Id).Select(s => s.Wednesday).FirstOrDefault(),
							Thursday = x.schedularfrequency.Where(a => a.SchedularId == x.Id).Select(s => s.Thursday).FirstOrDefault(),
							Friday = x.schedularfrequency.Where(a => a.SchedularId == x.Id).Select(s => s.Friday).FirstOrDefault(),
							Saturday = x.schedularfrequency.Where(a => a.SchedularId == x.Id).Select(s => s.Saturday).FirstOrDefault(),
							Status = x.IsEnabled ? Resources.Enums.Status.Yes.ToString() : Enums.Status.No.ToString()
						});


				if (parametersJson.Sort == null || parametersJson.Sort.Count == 0)
				{
					query = query.OrderByDescending(x => x.SchedulerMasterId);
				}
				var result = await query.ModifyList(parametersJson, out totalRecord).ToListAsync();

				schedulerList = result
						.Select(x => new SchedulerModel()
						{
							SchedulerMasterId = x.SchedulerMasterId,
							WebsiteType = x.Name,
							Name = x.Name,
							Description = x.Description,
							FrequencyTypeMasterId = x.FrequencyTypeMasterId,
							FrequencyType = x.FrequencyType,
							FrequencyTypeValue = x.FrequencyTypeValue,
							IsEnabled = x.IsEnabled,
							StartDate = x.StartDate.ToString("MM/dd/yyyy"),
							EndDate = x.EndDate.HasValue ? x.EndDate.Value.ToString("MM/dd/yyyy") : string.Empty,
							StartTime = x.StartTime.HasValue ? CommonUtilities.ToESTTimezone(DateTime.Today.Add(x.StartTime.Value)).ToString("hh:mm tt") : string.Empty,
							RecurEvery = x.RecurEvery,
							Sunday = x.Sunday,
							Monday = x.Monday,
							Tuesday = x.Tuesday,
							Wednesday = x.Wednesday,
							Thursday = x.Thursday,
							Friday = x.Friday,
							Saturday = x.Saturday,
							Status = x.Status

						}).ToList();

				schedulerList.ForEach(x => x.Details = x.FrequencyTypeValue == 2 ? "Occurs daily at " + x.StartTime + " every " + x.RecurEvery + " hours, starting on " + x.StartDate + "." :
																							 x.FrequencyTypeValue == 3 ? "Occurs at " + x.StartTime + " every " +
																								Weekdays(x.Sunday, x.Monday, x.Tuesday, x.Wednesday, x.Thursday, x.Friday, x.Saturday) + " every " + x.RecurEvery + " hours, starting on " +
																								x.StartDate + "." : string.Empty);

			}
			apiOutput.Data = schedulerList;
			apiOutput.TotalRecords = totalRecord;
			return apiOutput;
		}

		public async Task<ApiOutput> GetProcessDetailBasedOnScheduler(GridParameters parametersJson, string schedulerMasterHashId)
		{
			ApiOutput apiOutput = new ApiOutput();
			List<ProcessDetailModel> processDeatailList;
			List<EventsData> eventDataList = new List<EventsData>();
			int Record = ConfigurationManager.AppSettings["PageSize"].ToInt32();

			int htmlPages = Convert.ToInt32(Helper.ProcessType.HtmlPages);////3
			int documents = Convert.ToInt32(Helper.ProcessType.Documents);////5
			int scraperActivity = Convert.ToInt32(Helper.ProcessType.ScraperActivity);////6
			int rssFeeds = Convert.ToInt32(Helper.ProcessType.RSSFeeds);

			int totalRecord = 0;
			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{
				string Name = "";
				if (schedulerMasterHashId == Resources.Enums.Status.Official_Sector_Pages.ToString().Replace("_", " "))
				{
					Name = Resources.Enums.Status.OfficialSector.ToString();
				}
				if (schedulerMasterHashId == Resources.Enums.Status.Media_Pages.ToString().Replace("_", " "))
				{
					Name = Resources.Enums.Status.MediaSector.ToString();
				}
				IQueryable<ProcessDetailModel> query = db.processevents
					.Where(x => x.ScraperName == Name).OrderByDescending(d => d.Id)
						.Select(x => new ProcessDetailModel()
						{
							ProcessId = x.Id,
							StartDateTime = x.StartDateTime
						}).Take(Record);
				if (parametersJson.Sort == null || parametersJson.Sort.Count == 0)
				{
					query = query.OrderByDescending(x => x.ProcessId);
				}
				processDeatailList = await query.ModifyList(parametersJson, out totalRecord).ToListAsync();

				if (Name == Resources.Enums.Status.OfficialSector.ToString())
				{
					foreach (var item in processDeatailList)
					{
						EventsData tempObj = db.events
						.Where(x => x.ProcessEventId == item.ProcessId && (x.ProcessTypeId == htmlPages ||
						x.ProcessTypeId == documents || x.ProcessTypeId == rssFeeds ||
						x.ProcessTypeId == scraperActivity)).OrderByDescending(d => d.EndDateTime)
							.Select(x => new EventsData()
							{
								ProcessEventId = item.ProcessId,
								StartDateTime = item.StartDateTime,
								EndDateTime = x.EndDateTime,
								ScraperName = Name,
								status = db.events.Any(a => a.ProcessEventId == item.ProcessId && a.EndDateTime == null &&
							 (a.ProcessTypeId == htmlPages ||
								a.ProcessTypeId == documents ||
								a.ProcessTypeId == rssFeeds ||
								a.ProcessTypeId == scraperActivity)) ? Resource.LblInProcess : Resource.LblSuccess

							}).FirstOrDefault();
						if (tempObj != null)
							eventDataList.Add(tempObj);
					}
				}
				if (Name == Resources.Enums.Status.MediaSector.ToString())
				{
					foreach (var item in processDeatailList)
					{
						EventsData tempObj = db.events
						.Where(x => x.ProcessEventId == item.ProcessId && (x.ProcessTypeId == htmlPages || x.ProcessTypeId == rssFeeds || x.ProcessTypeId == scraperActivity)).OrderByDescending(d => d.EndDateTime)
							.Select(x => new EventsData()
							{
								ProcessEventId = item.ProcessId,
								StartDateTime = item.StartDateTime,
								EndDateTime = x.EndDateTime,
								ScraperName = Name,
								status = db.events.Any(a => a.ProcessEventId == item.ProcessId && a.EndDateTime == null &&
							 (a.ProcessTypeId == htmlPages ||
							 a.ProcessTypeId == rssFeeds ||
								a.ProcessTypeId == scraperActivity)) ? Resource.LblInProcess : Resource.LblSuccess
							}).FirstOrDefault();
						if (tempObj != null)
							eventDataList.Add(tempObj);
					}
				}
				eventDataList.ForEach(x => x.second = getseconds(x.StartDateTime, x.EndDateTime));

				eventDataList.Where(x => x.status == Resource.LblInProcess).ToList().
					ForEach(x => x.status = getStatus(x.EndDateTime, x.StartDateTime));
			}
			apiOutput.Data = eventDataList;
			apiOutput.TotalRecords = totalRecord;
			return apiOutput;
		}

		public string getseconds(DateTime? startdate, DateTime? enddate)
		{
			string minStr = "0";
			if (enddate.HasValue)
			{
				TimeSpan span = (Convert.ToDateTime(enddate) - Convert.ToDateTime(startdate));
				minStr = Convert.ToInt32(span.TotalMinutes).ToString();
			}
			return minStr;
		}

		public string getStatus(DateTime? enddate, DateTime? startdate)
		{
			string statusStr = "";
			if (enddate.HasValue)
			{
				////statusStr = Convert.ToDateTime(enddate).Date < DateTime.Now.Date ? Resource.LblFailure : Resource.LblInProcess; ////old code
				statusStr = Convert.ToDateTime(enddate).Date < DateTime.Now.Date ? Resource.LblSuccess : Resource.LblInProcess;
			}
			else
			{
				////statusStr = Convert.ToDateTime(startdate).Date < DateTime.Now.Date ? Resource.LblFailure : Resource.LblInProcess; //// old code
				statusStr = Convert.ToDateTime(startdate).Date < DateTime.Now.Date ? Resource.LblSuccess : Resource.LblInProcess;
			}
			return statusStr;
		}

		public string Weekdays(bool Sunday, bool Monday, bool Tuesday, bool Wednesday, bool Thursday, bool Friday, bool Saturday)
		{
			string res = "";
			string val = ", ";
			if (Sunday)
			{
				res = Resources.Enums.WeekDays.Sunday.ToString() + val;
			}
			if (Monday)
			{
				res += Resources.Enums.WeekDays.Monday.ToString() + val;
			}
			if (Tuesday)
			{
				res += Resources.Enums.WeekDays.Tuesday.ToString() + val;
			}
			if (Wednesday)
			{
				res += Resources.Enums.WeekDays.Wednesday.ToString() + val;
			}
			if (Thursday)
			{
				res += Resources.Enums.WeekDays.Thursday.ToString() + val;
			}
			if (Friday)
			{
				res += Resources.Enums.WeekDays.Friday.ToString() + val;
			}
			if (Saturday)
			{
				res += Resources.Enums.WeekDays.Saturday.ToString();
			}
			return res.TrimEnd(' ').TrimEnd(',');
		}

		public async Task<SchedulerModel> GetSchedulerByHashId(string schedulerHashId)
		{
			SchedulerModel model = new SchedulerModel();
			int schedulerDecryptId = schedulerHashId.ToDecrypt().ToInt32();
			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{
				var objSchedular = db.schedular.Where(a => a.Id == schedulerDecryptId).FirstOrDefault();
				if (objSchedular != null)
				{
					model.SchedulerMasterId = objSchedular.Id;
					model.WebsiteType = objSchedular.Name;
					model.Name = objSchedular.Name;
					model.Description = objSchedular.Description;
					model.FrequencyTypeMasterId = objSchedular.FrequencyType;
					model.FrequencyType = ((Helper.DdlFrequency)objSchedular.FrequencyType).ToString().Replace("_", " ");
					model.FrequencyTypeValue = objSchedular.FrequencyType;
					model.IsEnabled = objSchedular.IsEnabled;
					model.StartDate = objSchedular.StartDate.ToString("MM/dd/yyyy");
					model.EndDate = objSchedular.EndDate.HasValue ? objSchedular.EndDate.Value.ToString("MM/dd/yyyy") : string.Empty;
					model.StartTime = objSchedular.StartTime.HasValue ? CommonUtilities.ToESTTimezone(DateTime.Today.Add(objSchedular.StartTime.Value)).ToString("hh:mm tt") : string.Empty;
					model.RecurEvery = objSchedular.schedularfrequency.Where(a => a.SchedularId == objSchedular.Id).Select(s => s.RecurEvery).FirstOrDefault().ToString();
					model.Sunday = objSchedular.schedularfrequency.Where(a => a.SchedularId == objSchedular.Id).Select(s => s.Sunday).FirstOrDefault();
					model.Monday = objSchedular.schedularfrequency.Where(a => a.SchedularId == objSchedular.Id).Select(s => s.Monday).FirstOrDefault();
					model.Tuesday = objSchedular.schedularfrequency.Where(a => a.SchedularId == objSchedular.Id).Select(s => s.Tuesday).FirstOrDefault();
					model.Wednesday = objSchedular.schedularfrequency.Where(a => a.SchedularId == objSchedular.Id).Select(s => s.Wednesday).FirstOrDefault();
					model.Thursday = objSchedular.schedularfrequency.Where(a => a.SchedularId == objSchedular.Id).Select(s => s.Thursday).FirstOrDefault();
					model.Friday = objSchedular.schedularfrequency.Where(a => a.SchedularId == objSchedular.Id).Select(s => s.Friday).FirstOrDefault();
					model.Saturday = objSchedular.schedularfrequency.Where(a => a.SchedularId == objSchedular.Id).Select(s => s.Saturday).FirstOrDefault();
					model.Status = objSchedular.IsEnabled ? Resources.Enums.Status.Yes.ToString() : Enums.Status.No.ToString();
				}
			}
			return model;
		}

		public DateTime RetrieveMaxTime()
		{
			DateTime datMaxtime = Helper.GetCurrentDate();
			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{
				var maxTime = db.schedular.Where(x => !x.IsDeleted && x.IsEnabled).Select(x => x.StartTime).Max();
				datMaxtime = datMaxtime.AddHours(maxTime.Value.Hours).AddMinutes(maxTime.Value.Minutes).AddHours(Helper.GetAutoEmailGenerationHours_FailOver());

				return datMaxtime;
			}
		}
	}
}

