﻿using Autofac;
using Coolector.Services.Extensions;
using Coolector.Services.Mongo;
using Coolector.Services.Nancy;
using Microsoft.Extensions.Configuration;
using Nancy.Bootstrapper;
using NLog;
using RawRabbit;
using RawRabbit.vNext;

namespace Coolector.Services.Storage.Framework
{
    public class Bootstrapper : AutofacNancyBootstrapper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IConfiguration _configuration;

        public static ILifetimeScope LifeTimeScope { get; private set; }
        
        public Bootstrapper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void ConfigureApplicationContainer(ILifetimeScope container)
        {
            Logger.Info("Coolector.Services.Storage Configuring application container");
            base.ConfigureApplicationContainer(container);
            container.Update(builder =>
            {
                builder.RegisterInstance(_configuration.GetSettings<MongoDbSettings>());
                builder.RegisterModule<MongoDbModule>();
                builder.RegisterType<MongoDbInitializer>().As<IDatabaseInitializer>();
                builder.RegisterInstance(BusClientFactory.CreateDefault()).As<IBusClient>();
            });
            LifeTimeScope = container;
        }

        protected override void ApplicationStartup(ILifetimeScope container, IPipelines pipelines)
        {
            var databaseSettings = container.Resolve<MongoDbSettings>();
            var databaseInitializer = container.Resolve<IDatabaseInitializer>();
            databaseInitializer.InitializeAsync();
            pipelines.AfterRequest += (ctx) =>
            {
                ctx.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                ctx.Response.Headers.Add("Access-Control-Allow-Headers", "Authorization, Origin, X-Requested-With, Content-Type, Accept");
            };
            Logger.Info("Coolector.Services.Storage API Started");
        }
    }
}