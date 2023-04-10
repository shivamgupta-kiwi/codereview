using System.Web.Mvc;
using BCMStrategy.Controllers;
using BCMStrategy.Resources;

namespace BCMStrategy.Areas.BCMStrategy.Controllers
{
  [Authorize(Roles = "ADMIN")]
  public class ScrappingProcessController : BaseController
  {
    // GET: BCMStrategy/ScrappingProcess
    public ActionResult OfficialSector()
    {
      ViewBag.WebSiteSector = (int)Enums.WebsiteType.OfficialSector;

      return View();
    }

    public ActionResult Detail(string webSiteHashId,int webSiteType,string processEventId)
    {
      ViewBag.WebSiteHashId = webSiteHashId;
      ViewBag.WebSiteType = webSiteType;
      ViewBag.ProcessEventId = processEventId;
      ViewBag.Title = webSiteType == (int)Enums.WebsiteType.OfficialSector? "Scraper Execution Results (Official Sector)" : webSiteType == (int)Enums.WebsiteType.MediaSector? "Scraper Execution Results (Media Sector)" : "Scraper Execution Results"; 

      return View();
    }
    
    public ActionResult MediaSector()
    {
      ViewBag.WebSiteSector = (int)Enums.WebsiteType.MediaSector;

      return View();
    }
  }
}