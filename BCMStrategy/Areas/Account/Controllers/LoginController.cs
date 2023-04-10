using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BCMStrategy.Areas.Account.Models;
using BCMStrategy.Controllers;
using BCMStrategy.Helpers;
using BCMStrategy.Resources;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Repository.Concrete;
using BCMStrategy.Common.AuditLog;

namespace BCMStrategy.Areas.Account.Controllers
{
  public class LoginController : BaseController
  {

    [AllowAnonymous]
    public ActionResult Index()
    {
      if (User.Identity.IsAuthenticated)
      {
        //redirect to the dashboard page
        return Redirect(Url.Action("Index", "Dashboard", new { Area = "BCMStrategy" }));
      }
      return View();
    }

    [AllowAnonymous]
    public ActionResult ForgotPassword()
    {
      return View();
    }

    [AllowAnonymous]
    public ActionResult SetPassword()
    {
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> Index(LoginModel model, string returnUrl = "")
    {
      string[] ReturnUrl = Request.UrlReferrer.Query.Split('=');
      if (!string.IsNullOrWhiteSpace(ReturnUrl[0]) && ReturnUrl[0] != "?")
      {
        string HttpReturnUrl = ReturnUrl[1].Substring(0, 5);
        string WwwReturnUrl = ReturnUrl[1].Substring(0, 4);
        if (HttpReturnUrl == "http:" || HttpReturnUrl == "https" || WwwReturnUrl == "www.")
        {
          return Json(new LoginResult { Success = false });
        }
      }
      if (!model.IsDirectLogin && !ModelState.IsValid)
      {
        return Json(new LoginResult { Success = false, ResponseMessage = Resource.ValidateUserNamePassword });
      }
      try
      {
        var result = new SignInResult();
        if (model.IsDirectLogin)
        {
          result = await WebApiService.Instance.AuthenticateAsyncDirectLogin<SignInResult>(model.Username);
        }
        else
        {
          result = await WebApiService.Instance.AuthenticateAsync<SignInResult>(model.Username, model.Password);
        }

        var identity = new ClaimsIdentity(DefaultAuthenticationTypes.ApplicationCookie, ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        identity.AddClaim(new System.Security.Claims.Claim(ClaimTypes.Name, result.UserName));
        identity.AddClaim(new System.Security.Claims.Claim(ClaimTypes.NameIdentifier, result.UserMasterHashId));
        identity.AddClaim(new System.Security.Claims.Claim(ClaimTypes.Role, result.UserType));
        identity.AddClaim(new System.Security.Claims.Claim("Token", result.AccessToken));

        IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;
        authenticationManager.SignIn(new AuthenticationProperties()
        {
          IssuedUtc = result.Issued,
          IsPersistent = model.RememberMe,
        }, identity);

        return Json(new LoginResult { Success = true, Result = result });
      }
      catch (ApiException ex)
      {
        if (ex.StatusCode == HttpStatusCode.BadRequest)
        {
          //There are, at the moment, two types of bad requests: when signing in, JsonData might contain an error and error_description field.
          //When returning a BadRequest from the Web API, we'll get a message and ModelState error dictionary back.

          var badRequestData = JsonConvert.DeserializeObject<JsonBadRequest>(ex.JsonData);

          if (badRequestData.ModelState != null)
          {
            foreach (var modelStateItem in badRequestData.ModelState)
            {
              foreach (var message in modelStateItem.Value)
              {
                ModelState.AddModelError(modelStateItem.Key, message);
              }
            }
          }

          //When an error occurs while signing in, Error equals "invalid_grant" and ErrorDescription will contain more detail.
          //This error is being set in the Web API project in the YetAnotherTodo.WebApi.Providers.ApplicationOAuthProvider class
          if (string.Equals(badRequestData.Error, "invalid_grant"))
          {
            ModelState.AddModelError("", badRequestData.ErrorDescription);
          }

          if (!ModelState.IsValid)
          {
            return Json(new LoginResult { Success = false, ResponseMessage = badRequestData.ErrorDescription });
          }

          throw;
        }
      }
      return Json(new LoginResult { Success = false, ResponseMessage = Resource.ValidateUserNamePassword });
    }

    /// <summary>
    /// Performs Logout and returns to Login view
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    public async Task<ActionResult> Logout()
    {
      var res = await WebApiService.Instance.LogoutAsync(User.Identity.GetUserId());
      if (res)
      {
        IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;

        Response.Cache.SetCacheability(HttpCacheability.NoCache);

        Response.Cache.SetExpires(DateTime.Now.AddHours(-1));

        Response.Cache.SetNoStore();

        authenticationManager.SignOut();
      }
      return Redirect(Url.Action(@"Index", "Login", new { Area = "Account" }));
    }
  }
}