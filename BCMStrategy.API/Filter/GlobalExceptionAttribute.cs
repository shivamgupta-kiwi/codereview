using BCMStrategy.API.Enums;
using BCMStrategy.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;

namespace BCMStrategy.API.Filter
{
  public class GlobalExceptionAttribute : ExceptionFilterAttribute
  {
    private static readonly EventLogger<GlobalExceptionAttribute> log = new EventLogger<GlobalExceptionAttribute>();

    public override void OnException(HttpActionExecutedContext actionExecutedContext)
    {
      string contextDetail = GetContextDetail(actionExecutedContext);

      Exception ex = actionExecutedContext.Exception;

      string errorCode = string.Empty;
      string errorMessage = string.Empty;

      // Add exception new error handling code at here
      if (actionExecutedContext.Exception is ArgumentException)
      {
        // This exception should be thrown when requested data is not available into the database
        errorCode = GenerateErrorCode(EnumHelper.ErrorType.ArgumentException);
        errorMessage = "Internal server error. Kindly contact Helpdesk for support. Your Error code is : " + errorCode;
      }
      else
      {
        errorCode = GenerateErrorCode(EnumHelper.ErrorType.Exception);
        errorMessage = "Internal server error. Kindly contact Helpdesk for support. Your Error code is : " + errorCode;
      }

      log.LogError(LoggingLevel.Error, errorCode, contextDetail, ex);

      var response = new HttpResponseMessage()
      {
        Content = new StringContent(errorMessage),
        StatusCode = HttpStatusCode.InternalServerError,
      };

      actionExecutedContext.Response = response;
    }

    private string GetContextDetail(HttpActionExecutedContext context)
    {
      string contextDetail = Environment.NewLine;

      contextDetail += "    Method : " + context.Request.Method.Method + Environment.NewLine;
      contextDetail += "    URL : " + context.Request.RequestUri.AbsoluteUri + Environment.NewLine;
      contextDetail += "    Controller : " + context.ActionContext.ControllerContext.ControllerDescriptor.ControllerType.FullName + Environment.NewLine;
      contextDetail += "    Action Name : " + context.ActionContext.ActionDescriptor.ActionName + Environment.NewLine;
      contextDetail += "    Action Parameter : " + Newtonsoft.Json.JsonConvert.SerializeObject(context.ActionContext.ActionArguments.Values) + Environment.NewLine;

      return contextDetail;
    }

    private string GenerateErrorCode(EnumHelper.ErrorType errorType)
    {
      Random _rdm = new Random();
      int randomErrorNumber = _rdm.Next(0, 9999);

      return ((int)errorType).ToString() + randomErrorNumber.ToString().PadLeft(4, '0');
    }
  }
}