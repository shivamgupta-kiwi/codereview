using System.Web.Mvc;
using BCMStrategy.Controllers;

namespace BCMStrategy.Areas.BCMStrategy.Controllers
{
	[Authorize(Roles = "ADMIN")]
	public class GlobalSettingsController : BaseController
	{
		// GET: BCMStrategy/GlobalSettings
		public ActionResult Index()
		{
			return View();
		}
	}
}