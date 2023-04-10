using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BCMStrategy.Controllers;
using BCMStrategy.Resources;

namespace BCMStrategy.Areas.BCMStrategy.Controllers
{
  [Authorize(Roles = "ADMIN")]
  public class InstitutionsController : BaseController
  {
    // GET: BCMStrategy/Institutions
    public ActionResult Index()
    {
      return View();
    }

    public ActionResult ImportInstitutions()
    {
      return View();
    }
  }
}