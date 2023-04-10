﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BCMStrategy.Areas.BCMStrategy.Controllers
{
  [Authorize(Roles = "ADMIN")]
  public class MetadataNounplusVerbController : Controller
  {
    // GET: BCMStrategy/MetadataNounplusVerb
    public ActionResult Index()
    {
      return View();
    }
  }
}