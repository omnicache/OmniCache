using System;
using OmniCache.IntegrationTests.Model;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OmniCache.IntegrationTests.Core
{
	public class ApplicationDbContext : DbContext
    {
        private bool _UseInMemoryDatabase = false;

        public static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder =>
                                                        {
                                                            builder.AddProvider(new MyLoggerProvider());
                                                        });

        

        public ApplicationDbContext(bool useInMemoryDatabase)
        {
            _UseInMemoryDatabase = useInMemoryDatabase;
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(_UseInMemoryDatabase)
            {
                optionsBuilder.UseInMemoryDatabase($"TestDatabase:{Guid.NewGuid().ToString()}");
                //optionsBuilder.ConfigureWarnings(x => x.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning));
            }
            else
            {
                optionsBuilder.UseMySQL("Server=localhost; database=cachertests; UID=root; password=chuncomp");
                optionsBuilder.UseLoggerFactory(MyLoggerFactory);
            }
            
            //optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<RentalStore> RentalStores { get; set; }
        public DbSet<StoreStock> StoreStocks { get; set; }

    }


    public class MyLogger : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            string message = formatter(state, exception);

            OmniCache.DebugLogger.Debug(OmniCache.DebugLogSource.Database, message);         
        }
    }

    public class MyLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new MyLogger();
        }

        public void Dispose() { }
    }

}

