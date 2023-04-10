using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BCMStrategy.Areas.BCMStrategy.Controllers
{
  [Authorize]
  public class ChangePasswordController : Controller
    {
        // GET: BCMStrategy/ChangePassword
        public ActionResult Index()
        {
            return View();
        }
    }
}