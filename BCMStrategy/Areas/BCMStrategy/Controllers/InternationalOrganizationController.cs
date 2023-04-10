using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BCMStrategy.Controllers;

namespace BCMStrategy.Areas.BCMStrategy.Controllers
{
  [Authorize(Roles = "ADMIN")]
  public class InternationalOrganizationController : BaseController
  {
    // GET: BCMStrategy/InternationalOrganization
    public ActionResult Index()
    {
      return View();
    }

    public ActionResult ImportInternationalOrganization()
    {
      return View();
    }
  }
}