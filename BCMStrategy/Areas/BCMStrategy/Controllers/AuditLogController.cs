using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BCMStrategy.Areas.BCMStrategy.Controllers
{
  [Authorize(Roles = "ADMIN")]
  public class AuditLogController : Controller
  {
    // GET: BCMStrategy/AuditLog
    public ActionResult Index()
    {
      return View();
    }
  }
}