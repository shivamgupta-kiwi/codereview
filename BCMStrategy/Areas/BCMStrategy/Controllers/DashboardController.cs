using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BCMStrategy.Controllers;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Repository.Concrete;
using System.Globalization;

namespace BCMStrategy.Areas.BCMStrategy.Controllers
{
  [Authorize]
  public class DashboardController : BaseController
  {
    private readonly bool falseResult = false;
    private readonly bool trueResult = true;
    // GET: BCMStrategy/Dashboard
    public ActionResult Index()
    {
      string dateInput = Request.QueryString["date"] != string.Empty ? Convert.ToString(Request.QueryString["date"]) : string.Empty;
      DateTime? scanDate = !string.IsNullOrEmpty(dateInput) ? DateTime.ParseExact(dateInput, "yyyyMMdd", CultureInfo.InvariantCulture) : (DateTime?)null;

      ViewBag.refHashId = Request.QueryString["refHashId"] != string.Empty ? Convert.ToString(Request.QueryString["refHashId"]) : string.Empty;
      ViewBag.scanDate = scanDate != null ? scanDate.Value.ToString("MM/dd/yyyy") : string.Empty;
      ViewBag.key = Request.QueryString["key"] != string.Empty ? Convert.ToString(Request.QueryString["key"]) : string.Empty;
      ViewBag.isDirect = string.IsNullOrEmpty(Request.QueryString["refHashId"]) &&
                         string.IsNullOrEmpty(Request.QueryString["date"]) &&
                         string.IsNullOrEmpty(Request.QueryString["key"]) ? falseResult : trueResult;
      return View();
    }

    public ActionResult DetailChart(string selectedDate, string actionHashId, string lexiconHashId)
    {
      ViewBag.selectedDate = selectedDate;
      ViewBag.actionHashId = actionHashId;
      ViewBag.lexiconHashId = lexiconHashId;
      return View();
    }
  }
}