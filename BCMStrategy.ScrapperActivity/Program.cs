using System;
using BCMStrategy.Logger;
using BCMStrategy.ScrapperActivity.Abstract;
using BCMStrategy.ScrapperActivity.Repository;
using System.Diagnostics;
using System.Configuration;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Repository.Concrete;

namespace BCMStrategy.ScrapperActivity
{
	public class Program
	{
		protected Program()
		{
		}

		private static IScrapperActivityProcess _scrapperActivityProcess;

		private static IScrapperActivityProcess ScrapperActivityProcess
		{
			get
			{
				if (_scrapperActivityProcess == null)
				{
					_scrapperActivityProcess = new ScrapperActivityProcessRepository();
				}

				return _scrapperActivityProcess;
			}
		}

    /// <summary>
    /// The WebLink repository
    /// </summary>
    private static IWebLink _webLink;

    private static IWebLink WebLink
    {
      get
      {
        if (_webLink == null)
        {
          _webLink = new WebLinkRepository();
        }

        return _webLink;
      }
    }
		private static readonly EventLogger<Program> log = new EventLogger<Program>();

		private static void Main(string[] args)
		{
			try
			{
				if (args.Length > 0 && args[0] != null && args[1] != null)
				{
					int processId = string.IsNullOrEmpty(args[0]) ? 0 : Convert.ToInt32(args[0]);
					int processInstanceId = string.IsNullOrEmpty(args[1]) ? 0 : Convert.ToInt32(args[1]);
					if (processId > 0 && processInstanceId > 0)
					{
						ScrapperActivityProcess.ReadLexiconFromSolr(processId, processInstanceId);

            if (WebLink.IsFullScrapperActivityProcessCompleted(processId, processInstanceId))
            {
              //// Code to start calling Scrapper Activity Process for the given processId and processInstanceId
              Process pageApplicationProcess = new Process();
              string processArguments = Convert.ToString(Convert.ToInt32(processId)) + " " + Convert.ToString(processInstanceId);
              pageApplicationProcess.StartInfo.FileName = ConfigurationManager.AppSettings["PDFGeneratorPath"];
              pageApplicationProcess.StartInfo.Arguments = processArguments;
              pageApplicationProcess.Start();
              pageApplicationProcess.PriorityClass = ProcessPriorityClass.Normal;
            }
					}
				}
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in Main method", ex, null);
			}
		}
	}
}