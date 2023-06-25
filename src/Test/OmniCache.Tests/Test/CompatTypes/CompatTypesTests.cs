using System;
using OmniCache.IntegrationTests.Core;
using OmniCache.IntegrationTests.Model;
using OmniCache.IntegrationTests.Seed;
using Shouldly;
using Xunit;
using Xunit.Abstractions;
using OmniCache;
using OmniCache.EntityFramework;

namespace OmniCache.IntegrationTests.Test.CompatTypes
{	
    [Collection("Tests")]
    public class CompatTypesTests : BaseTest
    {
        public CompatTypesTests(ITestOutputHelper output) : base(output)
        {
            new SimpleSeed(cachedDB).Seed();
            CachedDatabase.LoadAllQueries(this.GetType());
        }

        public static Query<Movie> query1 = new Query<Movie>(
                               movie => movie.Id == new QueryParam(1));

        [Fact]
        public async Task IntToLongAsync()
        {
            int val = 101;

            Movie movie = await cachedDB.GetAsync(query1, val);

            movie.ShouldNotBeNull();            
            DebugLogger.Log.ShouldContain(s => s.Contains("query1") && s.Contains("GetAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();
        }

        public static Query<Movie> query2 = new Query<Movie>(
                               movie => movie.Id == new QueryParam(1));

        [Fact]
        public async Task FloatToLongAsync()
        {
            float val = 101f;

            Movie movie = await cachedDB.GetAsync(query2, val);

            movie.ShouldNotBeNull();            
            DebugLogger.Log.ShouldContain(s => s.Contains("query2") && s.Contains("GetAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();
        }

    }
}

