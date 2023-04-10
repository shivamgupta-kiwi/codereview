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
using BCMStrategy.ArchivalProcess.Abstract;
using BCMStrategy.ArchivalProcess.Repository;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Logger;

namespace BCMStrategy.ArchivalProcess
{
	public partial class ArchivalProcess : ServiceBase
	{
		#region Declare Variables
		private static readonly EventLogger<ArchivalProcess> log = new EventLogger<ArchivalProcess>();


		private static IArchivalProcess _archivalProcess;

		private static IArchivalProcess ArchivalProcessRepository
		{
			get
			{
				if (_archivalProcess == null)
				{
					_archivalProcess = new ArchivalProcessRepository();
				}

				return _archivalProcess;
			}
		}

		#endregion

		public ArchivalProcess()
		{
			InitializeComponent();
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
				int numberOfDays = Convert.ToInt32(Helper.GetArchivalProcessFrequency());
				TimeSpan ts = new TimeSpan(numberOfDays, 0, 0, 0);
				////TimeSpan ts = new TimeSpan(0, 0, 1, 0);
				Timer monitorEmailTimer = new Timer();
				monitorEmailTimer.Interval = ts.TotalMilliseconds;
				monitorEmailTimer.Elapsed += async (sender, e) => await MonitorArchivalProcessElapsedTime();
				monitorEmailTimer.Start();

				////	MonitorArchivalProcessElapsedTime();
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "E01", "Error is occur during start service.", ex);
			}
		}

		private async Task MonitorArchivalProcessElapsedTime()
		{
			try
			{
				log.LogSimple(LoggingLevel.Information, "Timer method is called MonitorArchivalProcessElapsedTime.");
				await ArchivalProcessRepository.RemoveDataFromSolrDB();
				log.LogSimple(LoggingLevel.Information, "Service is recall at " + DateTime.Now);
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "E01", "Error is occur during MonitorArchivalProcessElapsedTime.", ex);
			}
		}
	}
}
