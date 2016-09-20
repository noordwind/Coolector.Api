﻿using Autofac;
using Coolector.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Nancy;
using Nancy.Bootstrapper;

namespace Collector.Api
{
    public class CoolectorBootstrapper : AutofacNancyBootstrapper
    {
        private readonly IConfiguration _configuration;

        public CoolectorBootstrapper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void ApplicationStartup(ILifetimeScope container, IPipelines pipelines)
        {
            // No registrations should be performed in here, however you may
            // resolve things that are needed during application startup.
        }

        protected override void ConfigureApplicationContainer(ILifetimeScope container)
        {
            base.ConfigureApplicationContainer(container);

            container.Update(builder =>
            {
                builder.Register(x => GetConfigurationValue<GeneralSettings>("general"))
                    .As<GeneralSettings>();
                builder.Register(x => GetConfigurationValue<DatabaseSettings>("database"))
                    .As<DatabaseSettings>();
            });
        }

        protected override void ConfigureRequestContainer(ILifetimeScope container, NancyContext context)
        {
            // Perform registrations that should have a request lifetime
        }

        protected override void RequestStartup(ILifetimeScope container, IPipelines pipelines, NancyContext context)
        {
            // No registrations should be performed in here, however you may
            // resolve things that are needed during request startup.
        }

        private T GetConfigurationValue<T>(string section) where T : new()
        {
            var configurationValue = new T();
            _configuration.GetSection(section).Bind(configurationValue);

            return configurationValue;
        }
    }
}
