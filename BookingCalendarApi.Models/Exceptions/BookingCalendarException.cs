using System;
using System.Net;

namespace BookingCalendarApi.Models.Exceptions
{
    public class BookingCalendarException : Exception
    {
        public BookingCalendarException(string message) : base(message) { }

        public BookingCalendarException(int errorCode) : this(errorCode, "") { }

        public BookingCalendarException(int errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }

        public int ErrorCode { get; private set; } = BCError.SERVER_ERROR;

        public HttpStatusCode GetStatusCode()
        {
            switch (ErrorCode)
            {
                case BCError.NOT_FOUND:
                    return HttpStatusCode.NotFound;
                case BCError.CONNECTION_ERROR:
                    return HttpStatusCode.RequestTimeout;
                default:
                    return HttpStatusCode.BadRequest;
            }
        }
    }
}
