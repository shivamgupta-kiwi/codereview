using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCMStrategy.Data.Abstract.ViewModels;

namespace BCMStrategy.Data.Abstract.Abstract
{
	public interface IGlobalSetting
	{
		Task<GlobalSettingModel> GetGlobalConfigurationDetails();

		Task<bool> UpdateGlobalConfiguration(GlobalSettingModel globalSettings);
	}
}