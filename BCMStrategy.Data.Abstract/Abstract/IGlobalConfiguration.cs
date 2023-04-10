using System.Collections.Generic;
using BCMStrategy.Data.Abstract.ViewModels;
using SolrNet;

namespace BCMStrategy.Data.Abstract.Abstract
{
  public interface IGlobalConfiguration
  {
    void InsertInRange(List<PageDetails> pageDetails);

    ISolrQuery BuildQuery(SolrSearchParameters parameters);

    PageDetailsView Get(SolrSearchParameters parameters, string[] AllFacetFields = null);

    PageDetailsView GetPageSourceIdByGuidId(SolrSearchParameters parameters);

    void InsertInRangeLexicons(List<LexiconDetails> lexiconDetails);

    /// <summary>
    /// Get the Lexicon Page content from the solr dB
    /// </summary>
    /// <param name="parameters">Pass search Parameters</param>
    /// <returns>Return Page Details</returns>
    LexiconDetailsView GetLexicon(SolrSearchParameters parameters);

    List<string> GetAllSectorList();

		void DeleteByQuery(SolrSearchParameters parameters);

		void DeleteByQueryFromLexicon(SolrSearchParameters parameters);

	}
}