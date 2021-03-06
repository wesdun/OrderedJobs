﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrderedJobs.Data;
using OrderedJobs.Data.Models;
using OrderedJobs.Domain;

namespace OrderedJobs
{
  public class Startup
  {
    public Startup(IHostingEnvironment env)
    {
      var builder = new ConfigurationBuilder()
        .SetBasePath(env.ContentRootPath)
        .AddJsonFile("appsettings.json", true, true)
        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
        .AddEnvironmentVariables();
      Configuration = builder.Build();
    }

    public IConfigurationRoot Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMvc();

      services.AddSingleton<JobOrderer, JobOrderer>();
      services.AddSingleton<IDatabaseGateway<TestCase>, TestCaseMongoDatabaseGateway>();
      services.AddSingleton<IOrderedJobsCaller, OrderedJobsCaller>();
      services.AddSingleton<OrderedJobsTester, OrderedJobsTester>();
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
      loggerFactory.AddConsole(Configuration.GetSection("Logging"));
      loggerFactory.AddDebug();

      app.UseMvc();
    }
  }
}