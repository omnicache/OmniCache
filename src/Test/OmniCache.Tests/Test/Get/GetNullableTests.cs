using System;
using OmniCache.IntegrationTests.Model;
using OmniCache.IntegrationTests.Seed;
using OmniCache.IntegrationTests.Core;
using Shouldly;
using Xunit;
using Xunit.Abstractions;
using OmniCache;
using OmniCache.EntityFramework;

namespace OmniCache.IntegrationTests.Test.Get
{	
    [Collection("Tests")]
    public class GetNullableTests : BaseTest
    {
        public GetNullableTests(ITestOutputHelper output) : base(output)
        {
            new SimpleSeed(cachedDB).Seed();
            CachedDatabase.LoadAllQueries(this.GetType());
        }

        public static Query<Movie> query1 = new Query<Movie>(
                               movie => movie.Id == new QueryParam(1));

        [Fact]
        public async Task GetByNullableLongQueryParamAsync()
        {
            long? myId = 101;

            Movie movie = await cachedDB.GetAsync(query1, myId);

            movie.ShouldNotBeNull();            
            DebugLogger.Log.ShouldContain(s => s.Contains("query1") && s.Contains("GetAsync") && s.Contains("NULL"));
            
        }

        public static Query<Movie> query2 = new Query<Movie>(
                               movie => movie.Id == new QueryParam(1));

        [Fact]
        public async Task GetByNullQueryParamAsync()
        {
            long? myId = null;

            Should.Throw<Exception>(async () =>
            {
                Movie movie = await cachedDB.GetAsync(query2, myId);
            });

        }

    }
}

