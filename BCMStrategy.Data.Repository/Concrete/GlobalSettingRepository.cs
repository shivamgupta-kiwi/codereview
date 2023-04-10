using System.Data.Entity;
using System.Threading.Tasks;
using BCMStrategy.Common.Unity;
using BCMStrategy.DAL.Context;
using BCMStrategy.Data.Abstract;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;

namespace BCMStrategy.Data.Repository.Concrete
{
	public class GlobalSettingRepository : IGlobalSetting
	{
		/// <summary>
		/// Get Global Configuration Details
		/// </summary>
		/// <returns>Global configuration details</returns>
		public async Task<GlobalSettingModel> GetGlobalConfigurationDetails()
		{
			using (BCMStrategyEntities db = new BCMStrategyEntities())
			{
				GlobalSettingModel globalConfig = new GlobalSettingModel();

				var objGlobalConfiguration = await db.globalconfiguration.AsNoTracking().ToListAsync();

				if (objGlobalConfiguration != null)
				{
					foreach (globalconfiguration gbl in objGlobalConfiguration)
					{
						switch (gbl.Name)
						{
							case GlobalConfigurationKeys.SMTPDetails:
								globalConfig.SMTPDetails = gbl.Value;
								break;
						}
					}
				}

				return globalConfig;
			}
		}

		// <summary>
		/// Update Global Configuration Details
		/// </summary>
		/// <returns>Update Global configuration details</returns>
    public async Task<bool> UpdateGlobalConfiguration(GlobalSettingModel globalSettings)
		{
			bool isSave = false;

      if (globalSettings != null)
			{
				using (BCMStrategyEntities db = new BCMStrategyEntities())
				{
					var dbGlobalConfiguration = await db.globalconfiguration.ToListAsync();

					if (dbGlobalConfiguration != null)
					{
            ////GlobalSettingModel globalSetting = dbGlobalConfiguration.ToViewModel();
            ////string serializeGlobalConfiguration = Helper.SerializeObjectTojson(globalSetting);

						foreach (globalconfiguration gbl in dbGlobalConfiguration)
						{
							switch (gbl.Name)
							{
								case GlobalConfigurationKeys.SMTPDetails:
                  gbl.Value = globalSettings.SMTPDetails;
									gbl.Modified = Helper.GetCurrentDateTime();
									gbl.ModifiedBy = UserAccessHelper.CurrentUserIdentity.ToString();
									break;
							}
						}
						isSave = (await db.SaveChangesAsync() > 0);

						////if (isSave)
						////{
						////	Task.Run(() => AuditRepository.WriteAudit<string, GlobalSettingModel>(AuditConstants.GlobalSetting, AuditType.Update, serializeGlobalConfiguration, globalSettingsModel, AuditConstants.UpdateSuccessMsg));
						////}
					}
				}
			}

			return isSave;
		}
	}
}