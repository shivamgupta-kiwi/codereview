using BCMStrategy.Common.Kendo;
using BCMStrategy.Data.Abstract.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCMStrategy.Data.Abstract.Abstract
{
  public interface IScheduler
  {
    Task<ApiOutput> GetDDSchedulerFrequencyList();

    Task<bool> UpdateScheduler(SchedulerModel schedulerModel);

    Task<bool> DeleteScheduler(string schedulerMasterHashId);

    Task<ApiOutput> GetAllSchedulerList(GridParameters parametersJson);

    Task<ApiOutput> GetProcessDetailBasedOnScheduler(GridParameters parametersJson, string schedulerMasterHashId);

    Task<SchedulerModel> GetSchedulerByHashId(string schedulerHashId);

		DateTime RetrieveMaxTime();

	}
}
