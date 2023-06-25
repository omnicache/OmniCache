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
    public class GreaterThanOrderChangeInListTests : BaseTest
    {
        public GreaterThanOrderChangeInListTests(ITestOutputHelper output) : base(output)
        {
            new StoreStockSeed(cachedDB).Seed();
            CachedDatabase.LoadAllQueries(this.GetType());

        }

        public static Query<StoreStock> query1 = new Query<StoreStock>(
                               stock => stock.MovieId > new QueryParam(1)).OrderByDesc(s => s.CopiesInStore).Take(4);

        [Fact]
        public async Task GreaterThanOrderChangeInListAsync()
        {
            List<StoreStock> stocks = await cachedDB.GetMultipleAsync(query1, 101);

            stocks.ShouldNotBeNull();
            stocks.Count.ShouldBe(4);

            stocks.Select(stock => stock.CopiesInStore).ShouldBeSubsetOf(new int?[] { 94, 92, 79, 53 });

            DebugLogger.Log.ShouldContain(s => s.Contains("query1") && s.Contains("GetHashAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            StoreStock stock1 = stocks.FirstOrDefault(s => s.CopiesInStore == 92);
            stock1.CopiesInStore = 58;
            await cachedDB.UpdateAsync(stock1);

            DebugLogger.Log.ShouldContain(s => s.Contains("query1") && s.Contains("RemoveHashItem") && s.Contains("REMOVED"));
            DebugLogger.ClearLogData();
        }

        public static Query<StoreStock> query2 = new Query<StoreStock>(
                               stock => stock.CopiesInStore > new QueryParam(1)).OrderBy(s => s.CopiesInStore);

        [Fact]
        public async Task GreaterThanOrderChangeIntoListAsync()
        {
            List<StoreStock> stocks = await cachedDB.GetMultipleAsync(query2, 30);

            stocks.ShouldNotBeNull();
            stocks.Count.ShouldBe(6);

            stocks.Select(stock => stock.CopiesInStore).ShouldBeSubsetOf(new int?[] { 41, 53, 79, 92, 94, 100 });

            DebugLogger.Log.ShouldContain(s => s.Contains("query2") && s.Contains("GetHashAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            StoreStock stock1 = await cachedDB.GetByKeyAsync<StoreStock>(1);    //copies = 21
            stock1.ShouldNotBeNull();

            stock1.CopiesInStore = 42;
            await cachedDB.UpdateAsync(stock1);

            DebugLogger.Log.ShouldContain(s => s.Contains("query2") && s.Contains("RemoveHashItem") && s.Contains("query2") && s.Contains("REMOVED"));
            DebugLogger.ClearLogData();
        }
    }
}

