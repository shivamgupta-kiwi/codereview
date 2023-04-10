using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class LexiconTermViewModel : AuditBaseModel
  {
    public string LexiconType { get; set; }
    public string LexiconTerm { get; set; }
    public bool Nested { get; set; }
  }
}
