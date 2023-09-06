namespace BookingCalendarApi.Models.Exceptions
{
    public class BCError
    {
        // generic errors
        public const int SERVER_ERROR = 0;
        public const int NOT_FOUND = 1;
        public const int ID_CHANGE_ATTEMPT = 2;
        public const int MISSING_ORIGIN_DATA = 3;
        public const int CONNECTION_ERROR = 4;

        // domain errors
        public const int MAX_STAY_EXCEEDED = 100;
        public const int POLICE_SERVICE_ERROR = 101;
        public const int IPERBOOKING_ERROR = 102;
        public const int ISTAT_ERROR = 103;
        public const int MISSING_NATION = 104;
        public const int ROOMS_COLLISION = 105;
        public const int INVALID_ISTAT_MOVEMENTS = 106;
    }
}
