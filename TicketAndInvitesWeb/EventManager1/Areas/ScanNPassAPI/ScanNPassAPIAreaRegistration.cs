using System.Web.Mvc;

namespace EventManager1.Areas.ScanNPassAPI
{
    public class ScanNPassAPIAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ScanNPassAPI";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ScanNPassAPI_default",
                "ScanNPassAPI/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}