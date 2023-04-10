
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using BCMStrategy.Data.Abstract;
using BCMStrategy.EmailScheduler.Abstract;
using BCMStrategy.EmailScheduler.Repository;
using BCMStrategy.Logger;

namespace BCMStrategy.EmailScheduler
{
	public partial class EmailService : ServiceBase
	{
		private static readonly EventLogger<EmailService> log = new EventLogger<EmailService>();
		public EmailService()
		{
			InitializeComponent();
		}

		private static IEmailServiceScheduler _emailService;

		private static IEmailServiceScheduler EmailServiceSchedulerRepository
		{
			get
			{
				if (_emailService == null)
				{
					_emailService = new EmailServiceSchedulerRepository();
				}

				return _emailService;
			}
		}
		protected override void OnStart(string[] args)
		{
			StartService();
		}

		protected override void OnStop()
		{
			log.LogSimple(LoggingLevel.Information, "Service is stopped at " + DateTime.Now);
		}

		public void StartService()
		{
			log.LogSimple(LoggingLevel.Information, "StartService " + DateTime.Now);
			try
			{

				Timer monitorEmailTimer = new Timer();
				monitorEmailTimer.Interval = Helper.ScheduleInterval * 1000;
				monitorEmailTimer.Elapsed += async (sender, e) => await MonitorEmailServiceElapsedTime();

				monitorEmailTimer.Start();
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "E01", "Error is occur during start service.", ex);
			}
		}


		private async Task MonitorEmailServiceElapsedTime()
		{
			try
			{
				log.LogSimple(LoggingLevel.Information, "Timer method is called MonitorEmailServiceElapsedTime.");
				await EmailServiceSchedulerRepository.GetCustomerEmailDataAndSendEmail();
				log.LogSimple(LoggingLevel.Information, "Service is recall at " + DateTime.Now);
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "E01", "Error is occur during MonitorEmailServiceElapsedTime.", ex);
			}
		}
	}
}
