using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace BookingCalendarApi.Filters
{
    public class ExceptionFilter : TypeFilterAttribute
    {
        public ExceptionFilter() : base(typeof(ExceptionFilterImpl))
        {
        }

        private class ExceptionFilterImpl : IExceptionFilter
        {
            public void OnException(ExceptionContext context)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.HttpContext.Response.ContentType = "application/json";
                context.ExceptionHandled = true;
                context.Result = new ObjectResult(context.Exception.Message);
            }
        }
    }
}
