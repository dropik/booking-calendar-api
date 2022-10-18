namespace BookingCalendarApi.Services
{
    public interface IComposer<TIn, TOut>
    {
        public IEnumerable<TOut> Compose(IEnumerable<TIn> source);
    }
}
