using BookingCalendarApi.Models.Exceptions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Web.Http.Filters;

namespace BookingCalendarApi.NETFramework.Filters
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            context.Response.StatusCode = HttpStatusCode.InternalServerError;
            context.Response.Content = new StringContent(context.Exception.Message);

            if (context.Exception is BookingCalendarException bcException)
            {
                if (bcException.ErrorCode == BCError.NOT_FOUND)
                {
                    context.Response.StatusCode = HttpStatusCode.NotFound;
                }
                else if (bcException.ErrorCode == BCError.CONNECTION_ERROR)
                {
                    context.Response.StatusCode = HttpStatusCode.RequestTimeout;
                }
                else if (
                    bcException.ErrorCode == BCError.ID_CHANGE_ATTEMPT ||
                    bcException.ErrorCode == BCError.MISSING_ORIGIN_DATA ||
                    bcException.ErrorCode == BCError.MAX_STAY_EXCEEDED ||
                    bcException.ErrorCode == BCError.POLICE_SERVICE_ERROR ||
                    bcException.ErrorCode == BCError.IPERBOOKING_ERROR ||
                    bcException.ErrorCode == BCError.ISTAT_ERROR ||
                    bcException.ErrorCode == BCError.MISSING_NATION)
                {
                    context.Response.StatusCode = HttpStatusCode.BadRequest;
                }

                context.Response.Content = JsonContent.Create(new { bcException.ErrorCode, bcException.Message });
            }
        }
    }
}
