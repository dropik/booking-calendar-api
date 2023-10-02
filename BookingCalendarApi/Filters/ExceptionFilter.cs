using BookingCalendarApi.Models.Exceptions;
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

                if (context.Exception is BookingCalendarException bcException)
                {
                    context.HttpContext.Response.StatusCode = (int)bcException.GetStatusCode();
                    context.Result = new ObjectResult(new { bcException.ErrorCode, bcException.Message });
                }
            }
        }
    }
}
