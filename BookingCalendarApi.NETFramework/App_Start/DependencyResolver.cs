using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;

namespace BookingCalendarApi.NETFramework
{
    public class DependencyResolver : IDependencyResolver
    {
        private readonly ServiceProvider _provider;

        public DependencyResolver(ServiceProvider provider)
        {
            _provider = provider;
        }

        public IDependencyScope BeginScope()
        {
            var scope = _provider.CreateScope();
            return new Scope(scope.ServiceProvider);
        }

        public void Dispose()
        {
            _provider.Dispose();
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

        private class Scope : IDependencyScope
        {
            private readonly IServiceProvider _provider;

            public Scope(IServiceProvider provider)
            {
                _provider = provider;
            }

            public void Dispose()
            {
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
}