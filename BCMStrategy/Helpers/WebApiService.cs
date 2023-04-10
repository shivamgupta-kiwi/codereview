using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using BCMStrategy.Areas.Account.Models;

namespace BCMStrategy.Helpers
{
  public class WebApiService
  {
    private WebApiService(string baseUri)
    {
      BaseUri = baseUri;
    }

    private static WebApiService _instance;

    public static WebApiService Instance
    {
      get { return _instance ?? (_instance = new WebApiService(ConfigurationManager.AppSettings["APIHost"])); }
    }

    public string BaseUri { get; private set; }

    public async Task<T> AuthenticateAsync<T>(string userName, string password)
    {
      using (var client = new HttpClient())
      {
        var result = await client.PostAsync(BuildActionUri("/token"), new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("userName", userName),
                    new KeyValuePair<string, string>("password", password),
                    new KeyValuePair<string, string>("client_id", "bcmStrategyWeb")
                }));

        string json = await result.Content.ReadAsStringAsync();

        if (result.IsSuccessStatusCode)
        {
          ////SignInResult userInfo = JsonConvert.DeserializeObject<SignInResult>(json);

          ////if (userInfo.UserType == "CUSTOMER")
          ////{

          ////}

          return JsonConvert.DeserializeObject<T>(json);
        }

        throw new ApiException(result.StatusCode, json);
      }
    }

    public async Task<T> AuthenticateAsyncDirectLogin<T>(string userName)
    {
      using (var client = new HttpClient())
      {
        var result = await client.PostAsync(BuildActionUri("/token"), new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("userName", userName),
                    new KeyValuePair<string, string>("password", "DirectLogin"),
                    new KeyValuePair<string, string>("client_id", "opsWebApp"),
                }));

        string json = await result.Content.ReadAsStringAsync();
        if (result.IsSuccessStatusCode)
        {
          return JsonConvert.DeserializeObject<T>(json);
        }

        throw new ApiException(result.StatusCode, json);
      }
    }

    public async Task<bool> LogoutAsync(string id)
    {
      HttpResponseMessage result;
      using (var client = new HttpClient())
      {
        var identity = (ClaimsIdentity)Thread.CurrentPrincipal.Identity;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", identity.FindFirst("Token").Value);
        result = await client.DeleteAsync(BuildActionUri("api/RefreshTokens?id=" + id));
      }
      if (result.IsSuccessStatusCode)
      {
        return true;
      }
      return false;
    }

    private string BuildActionUri(string action)
    {
      return BaseUri + action;
    }
  }
}