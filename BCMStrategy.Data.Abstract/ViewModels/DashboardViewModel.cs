using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.ViewModels
{
  public class DashboardLexiconTypeViewModel
  {
    public int LexiconTypeId { get; set; }

    private string _lexiconTypeHashId { get; set; }

    public string LexiconTypeMasterHashId
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

    public string LexiconType { get; set; }
    ////public DashBoardProcessIdViewModel ProcessHashId { get; set; }
    public List<DashBoardLexiconTermsViewModel> DashBoardLexiconTermsList { get; set; }

    public decimal Value { get; set; }
  }

  public class DashBoardProcessIdViewModel
  {
    public string ProcessHashId { get; set; }
  }

  public class DashBoardLexiconTermsViewModel
  {
    public int LexiconTermId { get; set; }

    private string _lexiconTermHashId { get; set; }

    public string LexiconTermssHashId
    {
      get
      {
        if (string.IsNullOrEmpty(_lexiconTermHashId))
        {
          _lexiconTermHashId = this.LexiconTermId == 0 ? string.Empty : this.LexiconTermId.ToEncrypt();
        }
        return _lexiconTermHashId;
      }
      set
      {
        _lexiconTermHashId = value;
      }
    }

    public string LexiconTerm { get; set; }

    public string CombinationValue { get; set; }

    public int LexiconTypeId { get; set; }

    public string LexiconType { get; set; }

    public string ProcessHashId { get; set; }

    public int? ProcessId { get; set; }

    public bool HasValue { get; set; }

    public decimal Value { get; set; }
  }
}
