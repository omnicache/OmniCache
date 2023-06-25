using System;
using OmniCache.IntegrationTests.Model;
using OmniCache.IntegrationTests.Seed;
using System.Reflection;
using OmniCache.IntegrationTests.Core;
using Shouldly;
using Xunit;
using Xunit.Abstractions;
using OmniCache;
using OmniCache.EntityFramework;

namespace OmniCache.IntegrationTests.Test.OrderBy
{
    [Collection("Tests")]
    public class OrderByTests : BaseTest
    {		
        public OrderByTests(ITestOutputHelper output) : base(output)
        {
            new SimpleSeed(cachedDB).Seed();
            CachedDatabase.LoadAllQueries(this.GetType());

        }

        public static Query<Movie> query1 = new Query<Movie>(
                               movie => movie.AgeRestriction == new QueryParam(1)).OrderByDesc(movie => movie.Id);

        [Fact]
        public async Task GetOrderedDescAsync()
        {
            List<Movie> movies = await cachedDB.GetMultipleAsync(query1, 18);

            movies.ShouldNotBeNull();            
            movies.Count.ShouldBe(3);            
            bool isSortedDescending = movies.SequenceEqual(movies.OrderByDescending(obj => obj.Id));
            DebugLogger.Log.ShouldContain(s => s.Contains("query1") && s.Contains("GetAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            movies = await cachedDB.GetMultipleAsync(query1, 18);

            movies.ShouldNotBeNull();            
            movies.Count.ShouldBe(3);
            isSortedDescending = movies.SequenceEqual(movies.OrderByDescending(obj => obj.Id));
        }

        public static Query<Movie> query2 = new Query<Movie>(
                               movie => movie.AgeRestriction == new QueryParam(1)).OrderBy(movie => movie.Id);

        [Fact]
        public async Task GetOrderedAscAsync()
        {
            List<Movie> movies = await cachedDB.GetMultipleAsync(query2, 18);

            movies.ShouldNotBeNull();            
            movies.Count.ShouldBe(3);            
            bool isSortedDescending = movies.SequenceEqual(movies.OrderBy(obj => obj.Id));
            DebugLogger.Log.ShouldContain(s => s.Contains("query2") && s.Contains("GetAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            movies = await cachedDB.GetMultipleAsync(query2, 18);

            movies.ShouldNotBeNull();            
            movies.Count.ShouldBe(3);
            isSortedDescending = movies.SequenceEqual(movies.OrderBy(obj => obj.Id));
            DebugLogger.Log.ShouldContain(s => s.Contains("query2") && s.Contains("GetAsync") && !s.Contains("NULL"));
            DebugLogger.ClearLogData();
        }

    }
}

