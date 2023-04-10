using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BCMStrategy.Areas.BCMStrategy.Controllers
{
  [Authorize(Roles = "ADMIN")]
  public class UserAccessRightsController : Controller
  {
    // GET: BCMStrategy/UserAccessRights
    public ActionResult Index()
    {
      return View();
    }
  }
}