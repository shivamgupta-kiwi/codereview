using System.Web.Mvc;

namespace BCMStrategy.Areas.BCMStrategy
{
    public class BcmStrategyAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "BCMStrategy";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "BCMStrategy_default",
                "BCMStrategy/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}