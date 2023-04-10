using System;
using System.Threading.Tasks;
using System.Web.Http;
using BCMStrategy.API.Filter;
using BCMStrategy.Common.Unity;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using BCMStrategy.Logger;

namespace BCMStrategy.API.Controllers
{
	[Authentication]
	[RoutePrefix("api/GlobalSettings")]
	public class GlobalSettingApiController : BaseApiController
	{
    private static readonly EventLogger<GlobalSettingApiController> _log = new EventLogger<GlobalSettingApiController>();
		private IGlobalSetting _globalSettingRepository;

		private IGlobalSetting GlobalSettingRepository
		{
			get
			{
				if (_globalSettingRepository == null)
				{
					_globalSettingRepository = UnityHelper.Resolve<IGlobalSetting>();
				}

				return _globalSettingRepository;
			}
		}

		[Route("GetGlobalSettingDetails")]
		[HttpGet]
		public async Task<IHttpActionResult> GetGlobalSettingDetails()
		{
			var globalSetting = await GlobalSettingRepository.GetGlobalConfigurationDetails();
			return Ok(FormatResult(globalSetting));
		}

		[HttpPost]
		[Route("SaveGlobalSettings")]
		public async Task<IHttpActionResult> SaveGlobalSettings(GlobalSettingModel model)
		{
			try
			{
				bool isSave = false;

				if (!ModelState.IsValid)
				{
					return Ok(FormatResult(false, ModelState));
				}

				isSave = await GlobalSettingRepository.UpdateGlobalConfiguration(model);

				return Ok(FormatResult(isSave, (isSave ? Resources.Resource.SuccessfulMessageForGlobalSetting : Resources.Resource.ErrorWhileSaving)));
			}
			catch (Exception ex)
			{
				_log.LogError(LoggingLevel.Error, "BadRequest", "Exception is thrown.", ex, model);
				return BadRequest(ex.Message);
			}
		}
	}
}