namespace BookingCalendarApi.Services
{
    public static class SessionExtensions
    {
        public static IEnumerable<FlattenedRoom> ExcludeBySession(this IEnumerable<FlattenedRoom> rooms, ISession session)
        {
            return session.ExcludeRooms(rooms);
        }
    }
}
