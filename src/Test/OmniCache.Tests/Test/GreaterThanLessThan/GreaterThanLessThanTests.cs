using System;
using OmniCache;
using OmniCache.EntityFramework;
using OmniCache.IntegrationTests.Core;
using OmniCache.IntegrationTests.Model;
using OmniCache.IntegrationTests.Seed;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace OmniCache.IntegrationTests.Test.GreaterThanLessThan
{	
    [Collection("Tests")]
    public class GreaterThanLessThanTests : BaseTest
    {
        public GreaterThanLessThanTests(ITestOutputHelper output) : base(output)
        {
            new StoreStockSeed(cachedDB).Seed();
            CachedDatabase.LoadAllQueries(this.GetType());

        }


        public static Query<StoreStock> query1 = new Query<StoreStock>(
                               stock => stock.CopiesInStore > new QueryParam(1)
                                        && stock.CopiesInStore < new QueryParam(2))
                                .OrderByDesc(s => s.CopiesInStore);

        [Fact]
        public async Task GreaterThanLessThanAsync()
        {
            List<StoreStock> stocks = await cachedDB.GetMultipleAsync(query1, 10, 95);

            stocks.ShouldNotBeNull();
            stocks.Count.ShouldBe(6);

            stocks.Select(stock => stock.CopiesInStore).ShouldBeSubsetOf(new int?[] { 21, 41, 53, 79, 92, 94 });

            DebugLogger.Log.ShouldContain(s => s.Contains("query1") && s.Contains("GetHashAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

        }

        public static Query<StoreStock> query2 = new Query<StoreStock>(
                               stock => stock.CopiesInStore > new QueryParam(1)
                                        && stock.CopiesInStore < new QueryParam(2))
                                .OrderBy(s => s.CopiesInStore);

        [Fact]
        public async Task GreaterThanLessThanInvalidateAsync()
        {
            List<StoreStock> stocks = await cachedDB.GetMultipleAsync(query2, 10, 95);

            stocks.ShouldNotBeNull();
            stocks.Count.ShouldBe(6);

            stocks.Select(stock => stock.CopiesInStore).ShouldBeSubsetOf(new int?[] { 21, 41, 53, 79, 92, 94 });

            DebugLogger.Log.ShouldContain(s => s.Contains("query2") && s.Contains("GetHashAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            StoreStock stock = stocks.FirstOrDefault(s => s.CopiesInStore == 41);
            stock.ShouldNotBeNull();
            stock.CopiesInStore = 3;          
            await cachedDB.UpdateAsync(stock);

            DebugLogger.Log.ShouldContain(s => s.Contains("query2") && s.Contains("RemoveHashItem") && s.Contains("REMOVED"));
            DebugLogger.ClearLogData();

        }

        public static Query<StoreStock> query3 = new Query<StoreStock>(
                               stock => stock.CopiesInStore > new QueryParam(1)
                                        && stock.CopiesInStore < new QueryParam(2))
                                .OrderBy(s => s.CopiesInStore);

        [Fact]
        public async Task GreaterThanLessThanIntoListInvalidateAsync()
        {
            List<StoreStock> stocks = await cachedDB.GetMultipleAsync(query3, 10, 95);

            stocks.ShouldNotBeNull();
            stocks.Count.ShouldBe(6);

            stocks.Select(stock => stock.CopiesInStore).ShouldBeSubsetOf(new int?[] { 21, 41, 53, 79, 92, 94 });

            DebugLogger.Log.ShouldContain(s => s.Contains("query3") && s.Contains("GetHashAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            StoreStock stock1 = await cachedDB.GetByKeyAsync<StoreStock>(7);
            stock1.ShouldNotBeNull();
            stock1.CopiesInStore = 11;
            await cachedDB.UpdateAsync(stock1);

            DebugLogger.Log.ShouldContain(s => s.Contains("query3") && s.Contains("RemoveHashItem") && s.Contains("REMOVED"));
            DebugLogger.ClearLogData();

        }

        public static Query<StoreStock> query4 = new Query<StoreStock>(
                               stock => stock.CopiesInStore > new QueryParam(1)
                                        && stock.CopiesInStore < new QueryParam(2))
                                .OrderBy(s => s.CopiesInStore);

        [Fact]
        public async Task GreaterThanLessThanIntoListEndInvalidateAsync()
        {
            List<StoreStock> stocks = await cachedDB.GetMultipleAsync(query4, 10, 95);

            stocks.ShouldNotBeNull();
            stocks.Count.ShouldBe(6);

            stocks.Select(stock => stock.CopiesInStore).ShouldBeSubsetOf(new int?[] { 21, 41, 53, 79, 92, 94 });

            DebugLogger.Log.ShouldContain(s => s.Contains("query4") && s.Contains("GetHashAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            StoreStock stock1 = await cachedDB.GetByKeyAsync<StoreStock>(2);
            stock1.ShouldNotBeNull();
            stock1.CopiesInStore = 93;
            await cachedDB.UpdateAsync(stock1);

            DebugLogger.Log.ShouldContain(s => s.Contains("query4") && s.Contains("RemoveHashItem") && s.Contains("REMOVED"));
            DebugLogger.ClearLogData();

        }
    }
}

