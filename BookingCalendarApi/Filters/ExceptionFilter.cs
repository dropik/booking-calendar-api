using BookingCalendarApi.Exceptions;
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

                var bcException = context.Exception as BookingCalendarException;
                if (bcException != null)
                {
                    if (bcException.ErrorCode == BCError.NOT_FOUND)
                    {
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    }
                    else if (bcException.ErrorCode == BCError.CONNECTION_ERROR)
                    {
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
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
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    }

                    context.Result = new ObjectResult(new { bcException.ErrorCode, bcException.Message });
                }
            }
        }
    }
}
