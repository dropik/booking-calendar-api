namespace BookingCalendarApi.Exceptions
{
    public class BCError
    {
        // generic errors
        public static int SERVER_ERROR { get; } = 0;
        public static int NOT_FOUND { get; } = 1;
        public static int ID_CHANGE_ATTEMPT { get; } = 2;
        public static int MISSING_ORIGIN_DATA { get; } = 3;
        public static int CONNECTION_ERROR { get; } = 4;

        // domain errors
        public static int MAX_STAY_EXCEEDED { get; } = 100;
        public static int POLICE_SERVICE_ERROR { get; } = 101;
        public static int IPERBOOKING_ERROR { get; } = 102;
        public static int ISTAT_ERROR { get; } = 103;
        public static int MISSING_NATION { get; } = 104;
        public static int ROOMS_COLLISION { get; } = 105;
    }
}
