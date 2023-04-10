using BCMStrategy.Common.Email;
using BCMStrategy.DAL.Context;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Repository.Concrete;
using BCMStrategy.Email;
using BCMStrategy.EmailScheduler.Abstract;
using BCMStrategy.EmailScheduler.ViewModel;
using BCMStrategy.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.IO;

namespace BCMStrategy.EmailScheduler.Repository
{
	public class EmailServiceSchedulerRepository : IEmailServiceScheduler
	{
		private static readonly EventLogger<EmailServiceSchedulerRepository> log = new EventLogger<EmailServiceSchedulerRepository>();

		private ICommonRepository _commonRepository;

		private ICommonRepository CommonRepository
		{
			get
			{
				if (_commonRepository == null)
				{
					_commonRepository = new CommonRepository();
				}

				return _commonRepository;
			}
		}

		private IEmailHelper _emailHelper;

		private IEmailHelper EmailHelper
		{
			get
			{
				if (_emailHelper == null)
				{
					_emailHelper = new EmailHelper();
				}

				return _emailHelper;
			}
		}

		public async Task GetCustomerEmailDataAndSendEmail()
		{
			try
			{
				List<EmailServiceSchedulerModel> emailList;
				using (BCMStrategyEntities db = new BCMStrategyEntities())
				{
					emailList = db.emailgenerationstatus.Where(x => (x.Status == Helper.EmailServiceStatus.PENDING.ToString() || x.Status == Helper.EmailServiceStatus.FAILED.ToString()))
						.Select(x => new EmailServiceSchedulerModel()
						{
							Id = x.Id,
							EmailAddress = x.user.EmailAddress,
							HtmlBodyByte = x.EmailBody,
							EmailSubject = x.EmailSubject,
							Status = x.Status,
							SendAfterTime = x.SendAfterTime,
							SendBeforeTime = x.SendBeforeTime,
							CreatedAt = x.CreatedAt
						}).ToList();

					if (emailList.Count > 0)
					{
						await SendEmailToCustomer(emailList);
					}
				}
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in GetCustomerEmailDataAndSendEmail method", ex, null);
			}
		}

		public async Task SendEmailToCustomer(List<EmailServiceSchedulerModel> EmailList)
		{
			try
			{
        DateTime currentTimeStamp = Helper.GetCurrentDateTime();

				var emailConfiguration = await CommonRepository.GetEmailConfiguration();
				bool result = false;
				EmailHelper.Configuration = emailConfiguration;
				foreach (var item in EmailList)
				{
					if (item.SendAfterTime <= currentTimeStamp && item.SendBeforeTime >= currentTimeStamp)
					{

						using (BCMStrategyEntities db = new BCMStrategyEntities())
						{
							byte[] chartImageBytes = db.emailgenerationchartimage.Where(x => x.GenerationDate.Value.Year == item.CreatedAt.Year &&
																																							 x.GenerationDate.Value.Month == item.CreatedAt.Month &&
																																							 x.GenerationDate.Value.Day == item.CreatedAt.Day).Select(x => x.ChartImage).FirstOrDefault();

							////send mail
							string emailBody = string.Empty;

							var subject = item.EmailSubject;
							emailBody = item.HtmlBody;

							result = EmailHelper.SendEmailWithEmbeddedImage(subject, emailBody, item.EmailAddress, chartImageBytes);
							////update customer email table
							await UpdateEmailSendStatus(item.Id, result, false);
						}
					}
					else
					{
						if (item.SendAfterTime < currentTimeStamp && item.SendBeforeTime < currentTimeStamp)
						{
							////set mail expired
							await UpdateEmailSendStatus(item.Id, result, true);
						}
					}
				}
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in SendEmailToCustomer method", ex, null);
			}
		}

		private async Task UpdateEmailSendStatus(int Id, bool result, bool isExpired)
		{
			try
			{
        DateTime currentTimeStamp = Helper.GetCurrentDateTime();

				using (BCMStrategyEntities db = new BCMStrategyEntities())
				{
					var objEmailGenerationStatus = db.emailgenerationstatus.FirstOrDefault(x => x.Id == Id);
					if (objEmailGenerationStatus != null)
					{
						objEmailGenerationStatus.Status = isExpired ? Helper.EmailServiceStatus.EXPIRED.ToString() : result ? Helper.EmailServiceStatus.SUCCESS.ToString() : Helper.EmailServiceStatus.FAILED.ToString();
						objEmailGenerationStatus.SendMoment = currentTimeStamp;
					}
					db.SaveChanges();
				}
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in SendEmailToCustomer method", ex, null);
			}
		}
	}
}