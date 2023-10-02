using BookingCalendarApi.Repository;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace BookingCalendarApi.NETFramework.Filters
{
    public class AdminAttribute : AuthorizationFilterAttribute
    {
        public override Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            if (!HttpContext.Current.User.IsInRole(User.ADMIN_ROLE))
            {
                actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
            }
            return Task.CompletedTask;
        }
    }
}