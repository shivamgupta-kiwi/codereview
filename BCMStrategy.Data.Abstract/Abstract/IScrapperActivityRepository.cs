using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCMStrategy.Data.Abstract.ViewModels;

namespace BCMStrategy.Data.Abstract.Abstract
{
	public interface IScrapperActivityRepository
	{
		/// <summary>
		/// Insert the tagged paragraphs in the DAtabase
		/// </summary>
		/// <param name="loaderLinkQueue">List of DAta</param>
		/// <param name="htmlResult">Tagged Paragraphs.</param>
		/// <returns></returns>
		bool AddLexiconResults(LoaderLinkQueue loaderLinkQueue, StringBuilder htmlResult, List<ScrappedContentMapping> accutalIssueCountList);

		/// <summary>
		/// Insertion of Tagged results and counts
		/// </summary>
		/// <param name="loaderLinkQueue">Pass the processId and Process InstanceId</param>
		/// <param name="htmlResult">Html Content</param>
		/// <param name="accutalProrietoryTagCountList">List of Actual Proprietory Tags</param>
		/// <returns>return saved or not.</returns>
		bool InsertTaggedResults(LoaderLinkQueue loaderLinkQueue, List<ScrappedProprietoryTagsMapping> accutalProrietoryTagCountList);

		bool AddActivityTypeResultsForHardCoded(LoaderLinkQueue loaderLinkQueue, List<WebLinkMetaDataModel> metaDataList);

		bool AddStandardResults(LoaderLinkQueue loaderLinkQueue, WebLinkStandardTagsModel webLinkStandardList);

		ScrappedContentMappingCounts FetchLexiconResult(string htmlContent, List<LexiconModel> lexiconList, out string updatedHtmlContent);

		List<ScrappedContentMapping> GetLexiconsIssueCounts(string htmlResult, List<LexiconModel> lexiconList);

		void UpdateScrappedContent(DateTime date);

		List<string> GetGuidParentSite();
	}
}
