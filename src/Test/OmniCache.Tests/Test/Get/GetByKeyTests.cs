using System;
using System.Reflection;
using OmniCache;
using OmniCache.KeyProviders;
using OmniCache.IntegrationTests.Core;
using OmniCache.IntegrationTests.Model;
using OmniCache.IntegrationTests.Seed;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;
using Xunit.Abstractions;
using OmniCache.EntityFramework;

namespace OmniCache.IntegrationTests.Test.Get
{
    [Collection("Tests")]
    public class GetByKeyTests: BaseTest
	{
		public GetByKeyTests(ITestOutputHelper output):base(output)
        {
            new SimpleSeed(cachedDB).Seed();            
            CachedDatabase.LoadAllQueries(typeof(Movie));
     
        }

        [Fact]
        public async Task GetByIdAsync()
        {            
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

