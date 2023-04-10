using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace BCMStrategy.Areas.Account.Models
{
  public class SignInResult
  {
    [JsonProperty("userName")]
    public string UserName { get; set; }

    [JsonProperty("access_token")]
    public string AccessToken { get; set; }

    //Included to show all the available properties, but unused in this sample
    [JsonProperty("token_type")]
    public string TokenType { get; set; }

    [JsonProperty("expires_in")]
    public uint ExpiresIn { get; set; }

    [JsonProperty(".issued")]
    public DateTimeOffset Issued { get; set; }

    [JsonProperty(".expires")]
    public DateTimeOffset Expires { get; set; }

    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; }

    [JsonProperty("userMasterHashId")]
    public string UserMasterHashId { get; set; }

    [JsonProperty("userFullName")]
    public string UserFullName { get; set; }

    [JsonProperty("userType")]
    public string UserType { get; set; }
  }
}