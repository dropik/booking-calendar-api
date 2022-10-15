namespace BookingCalendarApi.Services
{
    public class ContextBoundAsyncSplitter : IAsyncScheduler
    {
        private readonly Func<Task> _contextBoundAction;
        private readonly Func<Task> _nonContextAction;

        public ContextBoundAsyncSplitter(Func<Task> contextBoundAction, Func<Task> nonContextAction)
        {
            _contextBoundAction = contextBoundAction;
            _nonContextAction = nonContextAction;
        }

        public async Task Execute()
        {
            await Task.WhenAll(_contextBoundAction(), _nonContextAction());
        }
    }
}
