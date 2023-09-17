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
            context.Response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent(context.Exception.Message)
            };

            if (context.Exception is BookingCalendarException bcException)
            {
                context.Response.StatusCode = bcException.GetStatusCode();
                context.Response.Content = JsonContent.Create(new { bcException.ErrorCode, bcException.Message });
            }
        }
    }
}
