using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BCMStrategy.Controllers
{
  public class ErrorController : BaseController
  {
    // GET: Error
    public ActionResult Index()
    {
      return View();
    }

    /// <summary>
    /// Unauthorized this instance.
    /// </summary>
    /// <returns>Unauthorized view</returns>
    public ActionResult Unauthorized()
    {
      return View();
    }

    /// <summary>
    /// Error Occurs in Process .
    /// </summary>
    /// <returns>Error 500</returns>
    public ActionResult InternalServerError()
    {
      return View();
    }
  }
}