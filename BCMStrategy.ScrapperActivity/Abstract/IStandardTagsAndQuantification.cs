using System.Text;
using BCMStrategy.Data.Abstract.ViewModels;

namespace BCMStrategy.ScrapperActivity.Abstract
{
	public interface IStandardTagsAndQuantification
	{
		/// <summary>
		/// Finding the Standard tags
		/// </summary>
		/// <param name="loaderLinkQueue"></param>
		void StandardTags(LoaderLinkQueue loaderLinkQueue);

		/// <summary>
		/// Extract ActivityTypeResult From html content
		/// </summary>
		/// <param name="htmlContent">Page Content</param>
		/// <returns>Fetch Paragraph from content and store in StringBuilder</returns>
		StringBuilder FetchHighlightedResult(string htmlContent);
	}
}