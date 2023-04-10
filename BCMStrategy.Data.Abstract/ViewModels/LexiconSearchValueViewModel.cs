using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class LexiconSearchValueViewModel
  {
    public int LexiconId { get; set; }
    public string Lexicon { get; set; }
    public decimal Value { get; set; }
    public string ActionType { get; set; }
    public int ActionTypeMasterId { get; set; }
    public int ActionTypeId { get; set; }
    public string LexiconType { get; set; }
    public int LexiconTypeId { get; set; }
    public int? ProcessId { get; set; }

    private string _lexiconTypeHashId { get; set; }

    public string CombinationValue { get; set; }
    public string LexiconTypeHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_lexiconTypeHashId))
        {
          _lexiconTypeHashId = this.LexiconTypeId == 0 ? string.Empty : this.LexiconTypeId.ToEncrypt();
        }
        return _lexiconTypeHashId;
      }
      set
      {
        _lexiconTypeHashId = value;
      }
    }

  }
}
