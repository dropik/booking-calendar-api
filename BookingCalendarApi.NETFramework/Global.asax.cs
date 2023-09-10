using BookingCalendarApi.Repository.NETFramework;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace BookingCalendarApi.NETFramework
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            using (var context = new BookingCalendarContext())
            {

            }
        }
    }
}
