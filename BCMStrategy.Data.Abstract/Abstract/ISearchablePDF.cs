using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCMStrategy.Common.Kendo;
using BCMStrategy.Data.Abstract.ViewModels;

namespace BCMStrategy.Data.Abstract.Abstract
{
  public interface ISearchablePdf
	{
		/// <summary>
		/// Get the list of PDf Based on Lexicon terms and Date
		/// </summary>
		/// <param name="searchablePDFParameters">Properties to search for pdf</param>
		/// <returns></returns>
		Task<ApiOutput> GetListOfPDFBasedOnLexicon(SearchablePdfParameters searchablePDFParameters, GridParameters parametersJson);

		List<SearchablePdfLineChartModel> GetLineChartDataBasedOnLexicon(SearchablePdfParametersForDrillDown searchablePDFParameters);

		List<SearchablePdfLineChartModel> GetLineMonthWiseChartDataBasedOnLexicon(SearchablePdfParameters searchablePDFParameters);

		Task<List<DropdownMaster>> GetAllLexicons(string searchTerm);
	}
}
