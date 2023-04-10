﻿using BCMStrategy.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BCMStrategy.Areas.BCMStrategy.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class WebLinkManagementController : Controller
    {
        public ActionResult Index()
        {
          return View();
        }
        public ActionResult Test()
        {
          return View();
        }
	}
}