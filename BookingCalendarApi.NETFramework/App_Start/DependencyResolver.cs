using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;

namespace BookingCalendarApi.NETFramework
{
    public class DependencyResolver : IDependencyResolver
    {
        private readonly IServiceProvider _provider;
        private IServiceScope _scope;

        public DependencyResolver(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IDependencyScope BeginScope()
        {
            _scope = _provider.CreateScope();
            return new DependencyResolver(_scope.ServiceProvider);
        }

        public void Dispose()
        {
            _scope?.Dispose();
            GC.SuppressFinalize(this);
        }

        public object GetService(Type serviceType)
        {
            return _provider.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _provider.GetServices(serviceType);
        }
    }
}