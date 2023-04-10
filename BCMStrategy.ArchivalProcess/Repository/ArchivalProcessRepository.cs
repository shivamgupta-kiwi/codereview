using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCMStrategy.ArchivalProcess.Abstract;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Data.Repository.Concrete;
using BCMStrategy.Logger;

namespace BCMStrategy.ArchivalProcess.Repository
{
	public class ArchivalProcessRepository : IArchivalProcess
	{
		private static readonly EventLogger<ArchivalProcessRepository> log = new EventLogger<ArchivalProcessRepository>();

		#region Declare Variables
		private static IGlobalConfiguration _globalConfiguration;

		private static IGlobalConfiguration GlobalConfiguration
		{
			get
			{
				if (_globalConfiguration == null)
				{
					_globalConfiguration = new GlobalConfigurationRepository();
				}

				return _globalConfiguration;
			}
		}

		private static ISolrPageDetail _solrPageDetail;

		private static ISolrPageDetail SolrPageDetailHistory
		{
			get
			{
				if (_solrPageDetail == null)
				{
					_solrPageDetail = new SolrPageDetailRepository();
				}

				return _solrPageDetail;
			}
		}

		private static IScrapperActivityRepository _scrapperActivity;

		private static IScrapperActivityRepository ScrapperActivity
		{
			get
			{
				if (_scrapperActivity == null)
				{
					_scrapperActivity = new ScrapperActivityRepository();
				}

				return _scrapperActivity;
			}
		}
		#endregion

		public async Task RemoveDataFromSolrDB()
		{
			try
			{
				int numberOfDays = Convert.ToInt32(Helper.GetSolrRetentionInDays());
				DateTime dat = DateTime.Today.AddDays(-numberOfDays);
				SolrSearchParameters searchParameters = new SolrSearchParameters();
				searchParameters.FreeSearch = "addedDateTime:{* TO " + string.Format("{0}", dat.ToString("yyyy-MM-ddTHH:mm:ssZ")) + "}";
				await RemoveFromPageDetails(searchParameters);
				await RemoveFromLexiconDetails(searchParameters);
				await RemoveFromPageDetailHistory(searchParameters);
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in RemoveDataFromSolrDB method", ex, null);
			}
		}

		#region Remove Data from SolrDB
		private async Task RemoveFromPageDetails(SolrSearchParameters searchParameters)
		{
			try
			{
				SolrSearchParameters searchParametersTemp = new SolrSearchParameters();
				searchParametersTemp.FreeSearch = searchParameters.FreeSearch;
				List<string> guidListOfParentSites = ScrapperActivity.GetGuidParentSite();

				////SolrSearchParameters searchParameters1 = new SolrSearchParameters();
				////searchParameters1.FreeSearch = searchParameters.FreeSearch + " AND guidId:(" + "\"" + string.Join("\",\"", guidListOfParentSites) + "\"" + ")";
				////var ABC = GlobalConfiguration.Get(searchParameters1).TotalCount;

				////SolrSearchParameters searchParameters2 = new SolrSearchParameters();
				////searchParameters2.FreeSearch = searchParameters.FreeSearch;
				////var ABC1 = GlobalConfiguration.Get(searchParameters2).TotalCount;

				////SolrSearchParameters searchParameters3 = new SolrSearchParameters();
				////searchParameters3.FreeSearch = searchParameters.FreeSearch + " AND NOT guidId:(" + "\"" + string.Join("\",\"", guidListOfParentSites) + "\"" + ")";
				////var ABC3 = GlobalConfiguration.Get(searchParameters3).TotalCount;

				searchParametersTemp.FreeSearch += " AND NOT guidId:(" + "\"" + string.Join("\",\"", guidListOfParentSites) + "\"" + ")";
				GlobalConfiguration.DeleteByQuery(searchParametersTemp);

				////int numberOfDays = Convert.ToInt32(Helper.GetMySQLRetentionInDays());
				////DateTime dat = DateTime.Today.AddDays(-numberOfDays);
			  ////UpdateScrappedContent(dat);
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in UpdateScrappedContent method", ex, null);
			}
		}

		private async Task RemoveFromLexiconDetails(SolrSearchParameters searchParameters)
		{
			try
			{
				GlobalConfiguration.DeleteByQueryFromLexicon(searchParameters);
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in RemoveFromLexiconDetails method", ex, null);
			}
		}

		private async Task RemoveFromPageDetailHistory(SolrSearchParameters searchParameters)
		{
			try
			{
				SolrPageDetailHistory.DeleteByQuery(searchParameters);
			}
			catch (Exception ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown in RemoveFromPageDetailHistory method", ex, null);
			}
		}
		#endregion
	}
}
