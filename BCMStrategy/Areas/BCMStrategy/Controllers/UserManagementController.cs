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
  public class UserManagementController : BaseController
  {
    // GET: BCMStrategy/UserManagement
    public ActionResult Index()
    {
      ViewBag.Title = Resource.LblAdminUserTitle;
      ViewBag.UserType = Enums.UserType.ADMIN.ToString();
      ViewBag.ModuleName = Enums.ModuleName.ADMINUSER;
      return View("AdminUser");
    }

    public ActionResult CustomerUser()
    {
      ViewBag.Title = Resource.LblCustomerUserTitle;
      ViewBag.UserType = Enums.UserType.CUSTOMER.ToString();
      ViewBag.ModuleName = Enums.ModuleName.CUSTOMERUSER;
      return View("CustomerUser");
    }

    public ActionResult DefaultLexicon(string userMasterHashId, string userType)
    {
      if (userType.ToLower() == Enums.UserType.ADMIN.ToString().ToLower())
      {
        ViewBag.Title = Resource.LblDefaultLexicon + " (ADMIN)";
        ViewBag.UserType = Enums.UserType.ADMIN.ToString();
        ViewBag.ModuleName = Enums.ModuleName.ADMINUSER;
      }
      else
      {
        ViewBag.Title = Resource.LblDefaultLexicon + " (CUSTOMER)";
        ViewBag.UserType = Enums.UserType.CUSTOMER.ToString();
        ViewBag.ModuleName = Enums.ModuleName.CUSTOMERUSER;
      }
      
      return View("DefaultLexicon");
    }
  }
}