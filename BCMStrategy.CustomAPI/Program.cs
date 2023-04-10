using BCMStrategy.ContentLoader.HtmlPages.Abstract;
using BCMStrategy.CustomAPI.Abstract;
using BCMStrategy.CustomAPI.Repository;
using BCMStrategy.Logger;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;

namespace BCMStrategy.CustomAPI
{
	public class Program
	{
    /// <summary>
    /// Default Constructor
    /// </summary>
    protected Program()
    {

    }

		#region Common Prameters
		private static readonly EventLogger<Program> log = new EventLogger<Program>();

		private static IFTApiOperation _ftOperationRepository;

		private static IFTApiOperation FTOperationRepository
		{
			get
			{
				if (_ftOperationRepository == null)
				{
					_ftOperationRepository = new FTOperationRepository();
				}

				return _ftOperationRepository;
			}
		}

		private static IThomsonReutersOperation _thomsonReutersOperation;

		private static IThomsonReutersOperation thomsonReutersOperationRepository
		{
			get
			{
				if (_thomsonReutersOperation == null)
				{
					_thomsonReutersOperation = new ThomsonReutersOperationRepository();
				}

				return _thomsonReutersOperation;
			}
		}


		#endregion

		#region MAIN METHOD
		static void Main(string[] args)
		{
			try
			{
				if (args != null && args[0] != null && args[1] != null)
				{
					int processId = Convert.ToInt32(args[0]);
					int processInstanceId = Convert.ToInt32(args[1]);

					//// Below business logic is for FT.com site
					int result = FTOperationRepository.SaveScraperEvent(processId, processInstanceId);

					if (result > 0)
					{
						try
						{
							FTOperationRepository.GetHeadLinesURLList(processId, processInstanceId);
						}
						catch (Exception ex)
						{
							log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in GetInnerChildContents method", ex, null);
						}
						//// Below business logic is for Thomson Reuters.com site
						try
						{
							thomsonReutersOperationRepository.GetTRIdList(processId, processInstanceId);
						}
						catch (Exception ex)
						{
							log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in thomsonReutersOperationRepository.GetTRIdList method", ex, null);
						}
						//// Code to start calling Scrapper Activity Process for the given processId and processInstanceId
						FTOperationRepository.UpdateScraperEvent(result, processId, processInstanceId);
					}
					Process pageApplicationProcess = new Process();
					string processArguments = Convert.ToString(Convert.ToInt32(processId)) + " " + Convert.ToString(processInstanceId);
					pageApplicationProcess.StartInfo.FileName = ConfigurationManager.AppSettings["ScraperActivityPath"];
					pageApplicationProcess.StartInfo.Arguments = processArguments;
					pageApplicationProcess.Start();
					pageApplicationProcess.PriorityClass = ProcessPriorityClass.Normal;
					//// Code ends here

				}

			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in Main method", ex, null);
			}
		}
		#endregion

	}
}
