using BCMStrategy.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BCMStrategy.Areas.BCMStrategy.Controllers
{
  [Authorize(Roles = "ADMIN")]
  public class SchedulerController : Controller
  {
    // GET: BCMStrategy/Scheduler
    public ActionResult Index()
    {
      return View();
    }

    public ActionResult ProcessDetail(string webSiteType)
    {
      ViewBag.Title = webSiteType == Enums.Status.Official_Sector_Pages.ToString().Replace("_", " ") ? Resource.LblProcessDetail + " (" + Resource.LblOfficialSector + ")" : webSiteType == Enums.Status.Media_Pages.ToString().Replace("_", " ") ? Resource.LblProcessDetail + " (" + Resource.LblMediaSector + ")": Resource.LblProcessDetail;
      return View();
    }
  }
}