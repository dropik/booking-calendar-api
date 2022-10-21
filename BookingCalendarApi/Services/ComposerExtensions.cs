namespace BookingCalendarApi.Services
{
    public static class ComposerExtensions
    {
        public static IEnumerable<TOut> UseComposer<TIn, TOut>(this IEnumerable<TIn> source, IComposer<TIn, TOut> composer)
        {
            return composer.Compose(source);
        }
    }
}
