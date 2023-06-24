using System;
using OmniCache.EntityFramework;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using RepoDemo.Data;
using RepoDemo.Data.Repository.Contract;
using RepoDemo.Data.Repository.Impl;
using RepoDemo.Services;
using OmniCache;
using StackExchange.Redis;

namespace RepoDemo.Api
{
    public static class ConfigureServices
    {

        public static IServiceCollection AddWebUIServices(this IServiceCollection services)
        {
           
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            IConfiguration configuration = configBuilder.Build();


            //Set up database
            bool useInMemoryDB = configuration.GetValue<bool>("UseInMemoryDatabase");

            services.AddDbContext<ApplicationDbContext>(opt =>
            {
                if (useInMemoryDB)
                {
                    opt.UseInMemoryDatabase("RepoDemo");
                }
                else
                {
                    string connectionString = configuration.GetConnectionString("DefaultConnection");
                    opt.UseMySQL(connectionString);
                }                
            });

            //Set up Caching
            var omniConfig = new OmniCacheConfig
            {
                CacheProvider = CacheProviderType.LocalMemory
            };

            CachedDatabase.SetConfig<OmniCacheConfig>(omniConfig);

            if (omniConfig.CacheProvider == CacheProviderType.Redis)
            {
                services.AddOmniCacheConfig<ConfigurationOptions>(new ConfigurationOptions()
                {
                    AbortOnConnectFail = false,
                    ResolveDns = true,
                    EndPoints = { "localhost" }
                });
            }

            //Other DI

            services.AddScoped<DatabaseSeeder>();
            services.AddScoped<ApplicationInitialiser>();

            services.AddDataServices();
            services.AddServiceServices();
            return services;
        }
    }
}

