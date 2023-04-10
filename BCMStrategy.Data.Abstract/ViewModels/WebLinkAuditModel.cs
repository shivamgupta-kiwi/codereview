using BCMStrategy.Data.Abstract.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract
{
  public class WebLinkAuditModel : AuditBaseModel
  {
    public string WebsiteType  { get; set; }
    public string WeblinkURL { get; set; }
    public bool HardCoded { get; set; }
    public string EntityName { get; set; }
    public string EntityType { get; set; }
    public string Country { get; set; }
    public string PageType { get; set; }
    public bool Active{ get; set; }
  }
}
