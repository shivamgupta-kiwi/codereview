using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BCMStrategy.Data.Abstract;

namespace BCMStrategy.Data.Abstract
{
  public static class UserAccessHelper
  {
    public static int CurrentUserIdentity
    {
      get
      {
        return UserIdentity();
      }
    }

    private static int UserIdentity()
    {
      var identity = System.Threading.Thread.CurrentPrincipal.Identity as ClaimsIdentity;
      var userHashId = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

      return userHashId != null && !string.IsNullOrWhiteSpace(userHashId.Value) ? userHashId.Value.ToDecrypt().ToInt32() : 0;
    }
  }
}
