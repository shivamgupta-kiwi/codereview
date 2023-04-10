using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class ApiClient
  {
    public string ID { get; set; }
    public string SecretHash { get; set; }
    public string EmailId { get; set; }
    public int ApplicationType { get; set; }
    public bool IsActive { get; set; }
    public int RefreshTokenLifeTime { get; set; }
    public string AllowedOrigin { get; set; }
  }
}
