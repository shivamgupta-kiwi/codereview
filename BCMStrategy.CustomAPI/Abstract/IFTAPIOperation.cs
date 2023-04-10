using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.CustomAPI.Abstract
{
  public interface IFTApiOperation
  {
    void GetHeadLinesURLList(int processId, int processInstanceId);

		int SaveScraperEvent(int processId, int processInstanceId);

		bool UpdateScraperEvent(int eventSavedresult, int processId, int solrCount);

	}
}
