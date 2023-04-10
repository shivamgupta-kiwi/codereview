using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BCMStrategy.Resources;

namespace BCMStrategy.Controllers
{
  public class CommonController : Controller
  {
    // GET: Common
    public ActionResult Common()
    {
      return View();
    }

    public ActionResult LinkExpired()
    {
      return View();
    }


   
  }
}