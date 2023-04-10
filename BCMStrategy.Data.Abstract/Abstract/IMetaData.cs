using System.Collections.Generic;
using BCMStrategy.Data.Abstract.ViewModels;

namespace BCMStrategy.Data.Abstract.Abstract
{
	public interface IMetaData
	{
		/// <summary>
		/// Get the List of Meta Data Values
		/// </summary>
		/// <returns></returns>
		List<WebLinkMetaDataModel> GetMetaDataListForScrapingActvityType(LoaderLinkQueue loaderLinkQueue);

		/// <summary>
		/// Get the List of Phrases Values
		/// </summary>
		/// <returns></returns>
		List<WebLinkPhraseModel> GetPhrasesListForScraping(LoaderLinkQueue loaderLinkQueue);

		/// <summary>
		/// Get the List of Noun plus Verb Values
		/// </summary>
		/// <returns></returns>
		List<WebLinkNounPVerbModel> GetNounPVerbListForScraping(LoaderLinkQueue loaderLinkQueue);

		/// <summary>
		/// Get the List of Dynamic Tabes based on Verb and Noun
		/// </summary>
		/// <param name="metaDataNounPVerbList">Noun Plus Veb Model</param>
		/// <returns></returns>
		List<DynamicNounPVerbResultModel> GetListOfDynmicTable(WebLinkNounPVerbModel nounPVerbModel);

		/// <summary>
		/// Get the list of Standard Tags List
		/// </summary>
		/// <param name="loaderLinkQueue">Weblink Details</param>
		/// <returns>list of Standard Tags List</returns>
		WebLinkStandardTagsModel StandardTagList(LoaderLinkQueue loaderLinkQueue);

		/// <summary>
		/// Get the list of PolicyMakers to find Standard tags in Media Sectors
		/// </summary>
		/// <returns>List of Individuals</returns>
		List<MediaSectorIndividualsModel> GetAllPolicyMakerListForScrapperActivity();

		/////// <summary>
		/////// Get all the lagislative list for scrapper activity
		/////// </summary>
		/////// <returns>List of Lagislative</returns>
		////List<MediaSectorLegislatorModel> GetAllLegislatorListForScrapperActivity();

		/// <summary>
		/// Get the list of State Heads for ScapperActivity
		/// </summary>
		/// <returns>List </returns>
		List<MediaSectorStateHeadModel> GetStateHeadListForScrapperActivity();

		/// <summary>
		/// Get the list of Media Sectors Institutions
		/// </summary>
		/// <returns>List of all Institutions</returns>
		List<MediaSectorInstitutionsModel> GetAllInstitutionsListForScrapperActivity();
	}
}