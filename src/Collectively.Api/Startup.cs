﻿using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Collectively.Api.Framework;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nancy.Owin;
using Lockbox.Client.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Collectively.Common.Logging;
using Collectively.Common.Caching;

namespace Collectively.Api
{
    public class Startup
    {
        public string EnvironmentName { get; set; }
        public IConfiguration Configuration { get; set; }
        public IServiceCollection Services { get; set; }

        public Startup(IHostingEnvironment env)
        {
            EnvironmentName = env.EnvironmentName.ToLowerInvariant();
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .SetBasePath(env.ContentRootPath);

            if (env.IsProduction() || env.IsDevelopment())
            {
                builder.AddLockbox();
            }
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSerilog(Configuration);
            services.AddWebEncoders();
            services.AddCors();
            Services = services;
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseSerilog(loggerFactory);
            app.UseCors(builder => builder.AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin()
                .AllowCredentials());
            app.UseOwin().UseNancy(x => x.Bootstrapper = new Bootstrapper(Configuration, Services));
        }
    }
}