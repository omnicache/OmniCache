using System;
using OmniCache;
using OmniCache.EntityFramework;
using OmniCache.IntegrationTests.Core;
using OmniCache.IntegrationTests.Model;
using OmniCache.IntegrationTests.Seed;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace OmniCache.IntegrationTests.Test.GreaterThan
{
    [Collection("Tests")]
    public class GreaterThanTests : BaseTest
    {		
        public GreaterThanTests(ITestOutputHelper output) : base(output)
        {
            new SimpleSeed(cachedDB).Seed();
            CachedDatabase.LoadAllQueries(this.GetType());

        }

        public static Query<Movie> query1 = new Query<Movie>(
                               movie => movie.Id > new QueryParam(1));

        [Fact]
        public async Task GetGreaterThanAsync()
        {
            List<Movie> movies = await cachedDB.GetMultipleAsync(query1, 100);

            movies.ShouldNotBeNull();            
            movies.Count.ShouldBe(2);
            DebugLogger.Log.ShouldContain(s => s.Contains("GetHashAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            movies = await cachedDB.GetMultipleAsync(query1, 100);

            movies.ShouldNotBeNull();            
            movies.Count.ShouldBe(2);
            DebugLogger.Log.ShouldContain(s => s.Contains("GetHashAsync") && !s.Contains("NULL"));
            DebugLogger.ClearLogData();

        }

        public static Query<Movie> query2 = new Query<Movie>(
                               movie => movie.Id > new QueryParam(1));

        [Fact]
        public async Task GetGreaterThanTwiceAsync()
        {
            List<Movie> movies = await cachedDB.GetMultipleAsync(query2, 100);

            movies.ShouldNotBeNull();            
            movies.Count.ShouldBe(2);
            DebugLogger.Log.ShouldContain(s => s.Contains("GetHashAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            List<Movie> movies2 = await cachedDB.GetMultipleAsync(query2, 101);

            movies2.ShouldNotBeNull();            
            movies2.Count.ShouldBe(1);
            DebugLogger.Log.ShouldContain(s => s.Contains("GetHashAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            movies = await cachedDB.GetMultipleAsync(query2, 100);

            movies.ShouldNotBeNull();            
            movies.Count.ShouldBe(2);
            DebugLogger.Log.ShouldContain(s => s.Contains("GetHashAsync") && !s.Contains("NULL"));
            DebugLogger.ClearLogData();

            movies2 = await cachedDB.GetMultipleAsync(query2, 101);

            movies2.ShouldNotBeNull();            
            movies2.Count.ShouldBe(1);
            DebugLogger.Log.ShouldContain(s => s.Contains("GetHashAsync") && !s.Contains("NULL"));
            DebugLogger.ClearLogData();
        }
    }
}

