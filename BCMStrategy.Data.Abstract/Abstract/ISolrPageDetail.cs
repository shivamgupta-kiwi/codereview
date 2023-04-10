using BCMStrategy.Data.Abstract.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.Abstract
{
  public interface ISolrPageDetail
  {
    void InsertPageDetailHistory(List<PageDetailHistory> pageDetails);

		void DeleteByQuery(SolrSearchParameters parameters);

	}
}
