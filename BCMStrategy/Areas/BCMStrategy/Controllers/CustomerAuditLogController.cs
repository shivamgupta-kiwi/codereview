using BCMStrategy.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BCMStrategy.Areas.BCMStrategy.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class CustomerAuditLogController : BaseController
    {
        // GET: BCMStrategy/CustomerAuditLog
        public ActionResult Index()
        {
            return View();
        }
    }
}