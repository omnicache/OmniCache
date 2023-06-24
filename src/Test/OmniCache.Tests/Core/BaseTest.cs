using System;
using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore.Storage;
using OmniCache.IntegrationTests.Core;
using OmniCache.IntegrationTests.Model;
using System.Diagnostics;
using Xunit.Abstractions;
using System.Reflection;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OmniCache;
using OmniCache.EntityFramework;
using OmniCache.IntegrationTests.Seed;
using StackExchange.Redis;

namespace OmniCache.IntegrationTests.Core
{
    
    public abstract class BaseTest: IDisposable, IAsyncLifetime
    {        
        protected CachedDatabase cachedDB;
        protected ITestOutputHelper _output;
        protected ApplicationDbContext dbContext;

        public const bool USE_MEMORY_DATABASE = false;

        public BaseTest(ITestOutputHelper output)
		{
            _output = output;

            //Set up Logging
            DebugLogger.SetLogger(new TestLogger(_output));
            DebugLogger.KeepData = true;
            DebugLogger.SetDebugOn(DebugLogSource.LocalCache);
            DebugLogger.SetDebugOn(DebugLogSource.Redis);

            if (!USE_MEMORY_DATABASE)
            {
                DebugLogger.SetDebugOn(DebugLogSource.Database);
            }

            //Set up Caching
            var omniConfig = new OmniCacheConfig
            {
                CacheProvider = CacheProviderType.LocalMemory
            };

            CachedDatabase.SetConfig<OmniCacheConfig>(omniConfig);

            if (omniConfig.CacheProvider == CacheProviderType.Redis)
            {
                CachedDatabase.SetConfig<ConfigurationOptions>(new ConfigurationOptions()
                {
                    AbortOnConnectFail = false,
                    ResolveDns = true,
                    EndPoints = { "localhost" }
                });
            }

            //Set up Database
            CachedDatabase.ClearAllQueries();

            dbContext = new ApplicationDbContext(USE_MEMORY_DATABASE);

            cachedDB = new CachedDatabase(dbContext);

            if(!USE_MEMORY_DATABASE)
            {
                new UnseedAll(cachedDB).Run();
            }


            DebugLogger.ClearLogData();

        }

        public async Task InitializeAsync()
        {
            await cachedDB.ClearCacheAsync();
        }

        public async Task DisposeAsync()
        {
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            
            DebugLogger.SetLogger(null);
            dbContext.Dispose();


        }
    }

    public class TestLogger : IDebugLogger
    {
        protected ITestOutputHelper _output;

        public TestLogger(ITestOutputHelper output)
        {
            _output = output;
        }

        public void Log(string message)
        {            
            _output.WriteLine("MSG -> " + message);
        }
    }
}

