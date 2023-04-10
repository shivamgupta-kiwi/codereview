using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BCMStrategy.Areas.BCMStrategy.Controllers
{
  [Authorize(Roles = "ADMIN")]
  public class PrivilegeController : Controller
  {
    //
    // GET: /BCMStrategy/Priviledge/
    public ActionResult LexiconAccessManagement()
    {
      return View();
    }

    public ActionResult LexiconAccessCustomer()
    {
      return View();
    }
    public ActionResult Test()
    {
      return View();
    }
    public ActionResult Test1()
    {
      return View();
    }
    public ActionResult Test2()
    {
      return View();
    }
    public ActionResult Test3()
    {
      return View();
    }
    public ActionResult Test4()
    {
      return View();
    }

    public ActionResult DetailChart(int? officialSectorId, int? mediaSectorId, bool isEropionUnion, string countryHashId, string actionTypeHashId)
    {
      ViewBag.officialSectorId = officialSectorId;
      ViewBag.mediaSectorId = mediaSectorId;
      ViewBag.isEropionUnion = isEropionUnion;
      ViewBag.countryHashId = countryHashId;
      ViewBag.actionTypeHashId = actionTypeHashId;
      return View();
    }
  }
}