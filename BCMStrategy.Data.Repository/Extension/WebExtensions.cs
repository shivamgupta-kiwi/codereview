using BCMStrategy.Data.Abstract.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DALContext = BCMStrategy.DAL.Context;

namespace BCMStrategy.Data.Repository.Extension
{
  public static class WebExtensions
  {
    /// <summary>
    /// Get Web link View Model
    /// </summary>
    /// <param name="objWebLinks">Web Links</param>
    /// <returns>Web Link Model</returns>
    public static WebLink ToViewModel(this DALContext.weblinks objWebLinks)
    {
      var result = new WebLink()
      {
        CategoryId = objWebLinks.CategoryId != null ? objWebLinks.CategoryId.Value: 0,
        WebLinkURL = objWebLinks.WebLinkURL,
        WebSiteTypeId = objWebLinks.WebSiteTypeId,
        IsActive = objWebLinks.IsActive,
        IsDeleted = objWebLinks.IsDeleted,

        WebLinkConfiguration = objWebLinks.weblinkrss.ToList().ToViewModel()
      };

      return result;
    }

    /// <summary>
    /// Get Web Link Configuration for the respective Web link
    /// </summary>
    /// <param name="objWebLinkConfig">WebURL Configuration</param>
    /// <returns>View Model</returns>
    public static WebLinkConfiguration ToViewModel(this DALContext.weblinkrss objWebLinkConfig)
    {
      var result = new WebLinkConfiguration()
      {
        WebLinkId = objWebLinkConfig.WebSiteId,
        ////IsRSSFeedAvailable = objWebLinkConfig.IsRSSFeedAvailable,
        ////IsSearchFunctionality = objWebLinkConfig.IsSearchFunctionality,
        ////DropDownSearchRegEx = objWebLinkConfig.DropDownSearchRegEx,
        ////HyperLinkClickRegEx = objWebLinkConfig.HyperLinkClickRegEx,
        ////HyperLinkDocumentRegEx = objWebLinkConfig.HyperLinkDocumentRegEx,
        RSSFeedClickRegEx = objWebLinkConfig.RSSFeedClickRegEx,

        RSSFeedURL = objWebLinkConfig.RSSFeedURL,
        ////SearchKeyword = objWebLinkConfig.SearchKeyWord
      };

      return result;
    }

    public static List<WebLinkConfiguration> ToViewModel(this List<DALContext.weblinkrss> objWebLinkConfigurationList)
    {
      var result = (List<WebLinkConfiguration>)null;

      if (objWebLinkConfigurationList == null)
        return result;

      result = objWebLinkConfigurationList.Select(x => x.ToViewModel()).ToList();

      return result;
    }

    /// <summary>
    /// Get Web Link List View Model
    /// </summary>
    /// <param name="objWebLinksList">Web Link List</param>
    /// <returns>List of Web Links</returns>
    public static List<WebLink> ToViewModel(this List<DALContext.weblinks> objWebLinksList)
    {
      if (objWebLinksList == null)
        return null;

      var result = objWebLinksList.Select(x => x.ToViewModel()).ToList();

      return result;
    }
  }
}