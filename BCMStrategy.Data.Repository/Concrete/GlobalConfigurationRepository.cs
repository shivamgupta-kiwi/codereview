using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using CommonServiceLocator;
using SolrNet;
using SolrNet.Exceptions;
using SolrNet.Impl;
using System;
using System.Collections.Generic;
using System.Configuration;
using SolrNet.Commands.Parameters;
using System.Linq;
using SolrNet.DSL;
using BCMStrategy.Logger;
using BCMStrategy.DAL.Context;
using BCMStrategy.Data.Abstract;

namespace BCMStrategy.Data.Repository.Concrete
{
	/// <summary>
	/// Global Configuration Repository Object
	/// </summary>
	public class GlobalConfigurationRepository : IGlobalConfiguration
	{
		private static readonly EventLogger<GlobalConfigurationRepository> log = new EventLogger<GlobalConfigurationRepository>();

		private static readonly string solrURL = ConfigurationManager.AppSettings["solrUrl"];
		private static readonly string solrURLLexicon = ConfigurationManager.AppSettings["solrUrlLexicon"];

		private readonly ISolrOperations<PageDetails> solr;
		private static System.Lazy<ISolrOperations<PageDetails>> lSolrLazy { get; set; }

		private readonly ISolrOperations<LexiconDetails> solrLexiconDetails;
		private static System.Lazy<ISolrOperations<LexiconDetails>> lSolrLazyLexiconDetails { get; set; }
		public GlobalConfigurationRepository()
		{
			if (lSolrLazy == null)
			{
				var connection1 = new SolrConnection(solrURL);
				var connection = new PostSolrConnection(connection1, solrURL);
				lSolrLazy = new System.Lazy<ISolrOperations<PageDetails>>(() =>
				{
					Startup.Init<PageDetails>(connection);
					ISolrOperations<PageDetails> s = ServiceLocator.Current.GetInstance<ISolrOperations<PageDetails>>();
					return s;
				}, System.Threading.LazyThreadSafetyMode.PublicationOnly);
			}
			solr = lSolrLazy.Value;

			if (lSolrLazyLexiconDetails == null)
			{
				var connectionLexicon = new SolrConnection(solrURLLexicon);
				////var connectionLexiconDetails = new PostSolrConnection(connectionLexicon, solrURL);

				lSolrLazyLexiconDetails = new System.Lazy<ISolrOperations<LexiconDetails>>(() =>
				{
					Startup.Init<LexiconDetails>(connectionLexicon);
					ISolrOperations<LexiconDetails> s = ServiceLocator.Current.GetInstance<ISolrOperations<LexiconDetails>>();
					return s;
				}, System.Threading.LazyThreadSafetyMode.PublicationOnly);
			}
			solrLexiconDetails = lSolrLazyLexiconDetails.Value;
			////var connection = new SolrConnection(solrURL);
			////var loggingConnection = new LoggingConnectionRepository(connection);
			////Startup.Container.Clear();
			////Startup.InitContainer();
			////Startup.Init<PageDetails>(loggingConnection);
			////this.solr = System.Lazy<ServiceLocator.Current.GetInstance<ISolrOperations<PageDetails>>>();
		}

		/// <summary>
		/// Add documents to the SOLR database
		/// </summary>
		/// <param name="pageDetails">pageDetails</param>
		public void InsertInRange(List<PageDetails> pageDetails)
		{
			try
			{

				var solrCon = ServiceLocator.Current.GetInstance<ISolrOperations<PageDetails>>();
				solrCon.AddRange(pageDetails);
				solrCon.Commit();
			}
			catch (SolrConnectionException ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", string.Format("Couldn't connect to Solr. Please make sure that Solr is running on '{0}' or change the address in your web.config, then restart the application.", solrURL), ex, null);
			}
		}

		/// <summary>
		/// Instert into Solr
		/// </summary>
		/// <param name="lexiconDetails">Pass Lexicone Parameters</param>
		public void InsertInRangeLexicons(List<LexiconDetails> lexiconDetails)
		{
			try
			{
				var solrCon = ServiceLocator.Current.GetInstance<ISolrOperations<LexiconDetails>>();
				solrCon.AddRange(lexiconDetails);
				solrCon.Commit();
			}
			catch (SolrConnectionException ex)
			{
				log.LogError(LoggingLevel.Error, "BadRequest", string.Format("Couldn't connect to Solr. Please make sure that Solr is running on '{0}' or change the address in your web.config, then restart the application.", solrURL), ex, null);
			}
		}

		/// <summary>
		/// Read the Page contents with Highlighted.
		/// </summary>
		/// <param name="parameters"></param>
		/// <param name="AllFacetFields"></param>
		/// <returns></returns>
		public PageDetailsView Get(SolrSearchParameters parameters, string[] AllFacetFields = null)
		{
			var start = (parameters.PageIndex - 1) * parameters.PageSize;
			var matchingProducts = solr.Query(BuildQuery(parameters), new QueryOptions
			{
				FilterQueries = BuildFilterQueries(parameters),

				Rows = parameters.PageSize,
				Start = start,
				////OrderBy = GetSelectedSort(parameters),
				////SpellCheck = new SpellCheckingParameters(),
				Facet = new FacetParameters
				{
					Queries = AllFacetFields != null ? AllFacetFields.Except(SelectedFacetFields(parameters))
																															.Select(f => new SolrFacetFieldQuery(f) { MinCount = 1 })
																															.Cast<ISolrFacetQuery>()
																															.ToList() : null,
					Limit = 20000
				},
				Highlight = new HighlightingParameters
				{
					Fields = new[] { "pageSource" },
					UsePhraseHighlighter = true,
					UseFastVectorHighlighter = true,
					MaxAlternateFieldLength = 1999999999,
					BeforeTerm = " <b style=\"background:#7fffd4\">",
					AfterTerm = "</b> ",
					MergeContiguous = true,
					Fragmenter = SolrHighlightFragmenter.Gap,
					Fragsize = 1999999999,
					RegexMaxAnalyzedChars = 1999999999,
					RegexSlop = 9999999999999,
					MaxAnalyzedChars = 1999999999
				}
			});
			var view = new PageDetailsView
			{
				Products = matchingProducts,
				Search = parameters,
				TotalCount = matchingProducts.NumFound,
				Facets = matchingProducts.FacetFields,
				Highlight = matchingProducts.Highlights

			};
			return view;
		}

		/// <summary>
		/// Get the Lexicon Page content from the solr dB 
		/// </summary>
		/// <param name="parameters">Pass search Parameters</param>
		/// <returns>Return Page Details</returns>
		public LexiconDetailsView GetLexicon(SolrSearchParameters parameters)
		{
			var start = (parameters.PageIndex - 1) * parameters.PageSize;
			var matchingProducts = solrLexiconDetails.Query(BuildQuery(parameters), new QueryOptions
			{
				FilterQueries = BuildFilterQueries(parameters),

				Rows = parameters.PageSize,
				Start = start
			});
			var view = new LexiconDetailsView
			{
				Products = matchingProducts,
				Search = parameters,
				TotalCount = matchingProducts.NumFound
			};
			return view;
		}

		/// <summary>
		/// Get Page Source By Guid Id
		/// </summary>
		/// <param name="parameters">parameters</param>
		/// <returns>Page Detail</returns>
		public PageDetailsView GetPageSourceIdByGuidId(SolrSearchParameters parameters)
		{
			var start = (parameters.PageIndex - 1) * parameters.PageSize;
			var matchingProducts = solr.Query(BuildQuery(parameters), new QueryOptions
			{
				FilterQueries = BuildFilterQueries(parameters),
				Rows = parameters.PageSize,
				Start = start
			});
			var view = new PageDetailsView
			{
				Products = matchingProducts
			};
			return view;
		}

		/// <summary>
		/// Gets the selected facet fields
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public IEnumerable<string> SelectedFacetFields(SolrSearchParameters parameters)
		{
			return parameters.Facets.Select(f => f.Key);
		}

		/// <summary>
		/// Builds the Solr query from the search parameters
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public ISolrQuery BuildQuery(SolrSearchParameters parameters)
		{
			if (!string.IsNullOrEmpty(parameters.FreeSearch))
				return new SolrQuery(parameters.FreeSearch);
			return SolrQuery.All;
		}

		/// <summary>
		/// Filter Querys
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public ICollection<ISolrQuery> BuildFilterQueries(SolrSearchParameters parameters)
		{
			var queriesFromFacets = from p in parameters.Facets
															select (ISolrQuery)Query.Field(p.Key).Is(p.Value);
			return queriesFromFacets.ToList();
		}

		/// <summary>
		/// Get all FT sector name from GlobalConfigration Table.
		/// </summary>
		/// <returns></returns>
		public List<string> GetAllSectorList()
		{
			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{
				string sectorList = db.globalconfiguration.Where(x => x.Name == Helper.Sectors.FT_SECTOR.ToString()).Select(s => s.Value).FirstOrDefault();
				return sectorList.Split(',').ToList();
			}
		}

		/// <summary>
		/// Delete from Solr Page Details
		/// </summary>
		/// <param name="parameters"></param>
		public void DeleteByQuery(SolrSearchParameters parameters)
		{
			ISolrQuery query = BuildQuery(parameters);
			solr.Delete(query);
			solr.Commit();
			solr.Optimize();
		}

		/// <summary>
		/// Delete Data from Solr from Lexicon
		/// </summary>
		/// <param name="parameters"></param>
		public void DeleteByQueryFromLexicon(SolrSearchParameters parameters)
		{
			ISolrQuery query = BuildQuery(parameters);
			solrLexiconDetails.Delete(query);
			solrLexiconDetails.Commit();
			solrLexiconDetails.Optimize();
		}
	}
}