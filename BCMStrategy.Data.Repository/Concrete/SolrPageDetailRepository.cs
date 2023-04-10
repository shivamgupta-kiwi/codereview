using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using CommonServiceLocator;
using SolrNet;
using SolrNet.Exceptions;
using SolrNet.Impl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Repository.Concrete
{
  public class SolrPageDetailRepository : ISolrPageDetail
  {
    private static readonly string solrPageDetailUrl = ConfigurationManager.AppSettings["solrPageDetailUrl"];
    private readonly ISolrOperations<PageDetailHistory> solrDetailHistory;
    private static System.Lazy<ISolrOperations<PageDetailHistory>> lSolrHistoryLazy { get; set; }

    public SolrPageDetailRepository()
    {
      if (lSolrHistoryLazy == null)
      {
        var connection = new SolrConnection(solrPageDetailUrl);
        lSolrHistoryLazy = new System.Lazy<ISolrOperations<PageDetailHistory>>(() =>
        {
          Startup.Init<PageDetailHistory>(connection);
          ISolrOperations<PageDetailHistory> s = ServiceLocator.Current.GetInstance<ISolrOperations<PageDetailHistory>>();
          return s;
        }, System.Threading.LazyThreadSafetyMode.PublicationOnly);
      }
      solrDetailHistory = lSolrHistoryLazy.Value;
    }

    /// <summary>
    /// Add documents to the SOLR database
    /// </summary>
    /// <param name="pageDetails">pageDetails</param>
    public void InsertPageDetailHistory(List<PageDetailHistory> pageDetails)
    {
      try
      {
        var solrPageDetailHistory = ServiceLocator.Current.GetInstance<ISolrOperations<PageDetailHistory>>();
        solrPageDetailHistory.AddRange(pageDetails);
        solrPageDetailHistory.Commit();
      }
      catch (SolrConnectionException e)
      {
        throw new Exception(string.Format("Couldn't connect to Solr. Please make sure that Solr is running on '{0}' or change the address in your web.config, then restart the application.", solrPageDetailUrl), e);
      }
    }

		/// <summary>
		/// Delete from Solr Page Details History
		/// </summary>
		/// <param name="parameters"></param>
		public void DeleteByQuery(SolrSearchParameters parameters)
		{
			ISolrQuery query = BuildQuery(parameters);
			solrDetailHistory.Delete(query);
			solrDetailHistory.Commit();
			solrDetailHistory.Optimize();
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

	}
}
