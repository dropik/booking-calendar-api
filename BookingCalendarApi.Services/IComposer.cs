using System.Collections.Generic;

namespace BookingCalendarApi.Services
{
    public interface IComposer<TIn, TOut>
    {
        IEnumerable<TOut> Compose(IEnumerable<TIn> source);
    }
}
