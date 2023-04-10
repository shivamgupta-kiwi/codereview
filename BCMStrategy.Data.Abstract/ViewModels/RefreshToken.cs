using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class RefreshToken
  {
    public string ID { get; set; }
    public int UserID { get; set; }
    public string ClientID { get; set; }
    public DateTime? IssuedDateTime { get; set; }
    public DateTime? ExpiresDateTime { get; set; }
    public string ProtectedTicket { get; set; }
  }
}
