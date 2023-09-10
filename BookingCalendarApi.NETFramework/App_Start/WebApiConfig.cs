﻿using Microsoft.Extensions.DependencyInjection;
using System.Web.Http;

namespace BookingCalendarApi.NETFramework
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Servizi e configurazione dell'API Web
            var services = new ServiceCollection();
            config.DependencyResolver = new DependencyResolver(services.BuildServiceProvider());

            // Route dell'API Web
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "BookingCalendarApi",
                routeTemplate: "api/v1/{controller}"
            );
        }
    }
}
