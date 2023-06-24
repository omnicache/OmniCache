using System;
using OmniCache;
using OmniCache.EntityFramework;
using OmniCache.IntegrationTests.Core;
using OmniCache.IntegrationTests.Model;
using OmniCache.IntegrationTests.Seed;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace OmniCache.IntegrationTests.Test.Load
{    
    [Collection("Tests")]
    public class LoadAllQueriesTests : BaseTest
    {
        public LoadAllQueriesTests(ITestOutputHelper output) : base(output)
        {
            new SimpleSeed(cachedDB).Seed();
            
        }

        [Fact]
        public async Task LoadAllAssembliesAsync()
        {
            CachedDatabase.LoadAllQueries();

            Movie movie = await cachedDB.GetByKeyAsync<Movie>(102);

            movie.ShouldNotBeNull();
            DebugLogger.Log.ShouldContain(s => s.Contains("GetAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            movie = await cachedDB.GetByKeyAsync<Movie>(102);

            movie.ShouldNotBeNull();
            DebugLogger.Log.ShouldContain(s => s.Contains("GetAsync") && !s.Contains("NULL"));
            DebugLogger.ClearLogData();
        }

    }
}

