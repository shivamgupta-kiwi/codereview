using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BCMStrategy.Controllers;

namespace BCMStrategy.Areas.BCMStrategy.Controllers
{
	[Authorize]
	public class SearchablePdfController : BaseController
	{
		// GET: BCMStrategy/SearchablePDF
		public ActionResult Index()
		{
			return View();
		}
	}
}