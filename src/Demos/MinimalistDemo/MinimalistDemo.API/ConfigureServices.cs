using System;
using Microsoft.EntityFrameworkCore;
using MinimalistDemo.Data;
using OmniCache;
using OmniCache.EntityFramework;
using StackExchange.Redis;

namespace MinimalistDemo.API
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


            //set up database
            bool useInMemoryDB = configuration.GetValue<bool>("UseInMemoryDatabase");

            services.AddScoped<ICachedDBSession, CachedDBSession>();
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

            //set up caching
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

            
            return services;
        }
    }
}

