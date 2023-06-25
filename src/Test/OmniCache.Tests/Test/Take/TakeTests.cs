using System;
using OmniCache.IntegrationTests.Core;
using OmniCache.IntegrationTests.Model;
using OmniCache.IntegrationTests.Seed;
using OmniCache.IntegrationTests.Core;
using Shouldly;
using Xunit;
using Xunit.Abstractions;
using OmniCache;
using OmniCache.EntityFramework;

namespace OmniCache.IntegrationTests.Test.Take
{	

    [Collection("Tests")]
    public class TakeTests : BaseTest
    {
        public TakeTests(ITestOutputHelper output) : base(output)
        {
            new SimpleSeed(cachedDB).Seed();
            CachedDatabase.LoadAllQueries(this.GetType());

        }

        public static Query<Movie> query1 = new Query<Movie>(
                               movie => movie.AgeRestriction == new QueryParam(1)).OrderByDesc(movie => movie.Id).Take(2);

        [Fact]
        public async Task GetOrderedDescAsync()
        {
            List<Movie> movies = await cachedDB.GetMultipleAsync(query1, 18);

            movies.ShouldNotBeNull();            
            movies.Count.ShouldBe(2);            
            bool isSortedDescending = movies.SequenceEqual(movies.OrderByDescending(obj => obj.Id));
            DebugLogger.Log.ShouldContain(s => s.Contains("query1") && s.Contains("GetAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            movies = await cachedDB.GetMultipleAsync(query1, 18);
            movies.ShouldNotBeNull();            
            movies.Count.ShouldBe(2);
            isSortedDescending = movies.SequenceEqual(movies.OrderByDescending(obj => obj.Id));
            DebugLogger.Log.ShouldContain(s => s.Contains("query1") && s.Contains("GetAsync") && !s.Contains("NULL"));
            DebugLogger.ClearLogData();
        }

    }
}

