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
    public class GreaterThanLessThanTakeTests : BaseTest
    {
        public GreaterThanLessThanTakeTests(ITestOutputHelper output) : base(output)
        {
            new StoreStockSeed(cachedDB).Seed();
            CachedDatabase.LoadAllQueries(this.GetType());

        }

        
        public static Query<StoreStock> query1 = new Query<StoreStock>(
                               stock => stock.CopiesInStore > new QueryParam(1)
                                        && stock.CopiesInStore < new QueryParam(2))
                                .OrderByDesc(s => s.CopiesInStore).Take(4);

        [Fact]
        public async Task GreaterThanLessThanTakeAsync()
        {
            List<StoreStock> stocks = await cachedDB.GetMultipleAsync(query1, 10, 95);

            stocks.ShouldNotBeNull();
            stocks.Count.ShouldBe(4);

            stocks.Select(stock => stock.CopiesInStore).ShouldBeSubsetOf(new int?[] { 94, 92, 79, 53 });

            DebugLogger.Log.ShouldContain(s => s.Contains("GetHashAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

        }

        public static Query<StoreStock> query2 = new Query<StoreStock>(
                               stock => stock.CopiesInStore > new QueryParam(1)
                                        && stock.CopiesInStore < new QueryParam(2))
                                .OrderBy(s => s.CopiesInStore).Take(4);

        [Fact]
        public async Task GreaterThanLessThanTakeInvalidateAsync()
        {
            List<StoreStock> stocks = await cachedDB.GetMultipleAsync(query2, 10, 95);

            stocks.ShouldNotBeNull();
            stocks.Count.ShouldBe(4);

            stocks.Select(stock => stock.CopiesInStore).ShouldBeSubsetOf(new int?[] { 21, 41, 53, 79 });

            DebugLogger.Log.ShouldContain(s => s.Contains("GetHashAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            StoreStock stock = stocks.FirstOrDefault(s => s.CopiesInStore == 41);
            stock.ShouldNotBeNull();
            stock.CopiesInStore = 3;
            await cachedDB.UpdateAsync(stock);

            DebugLogger.Log.ShouldContain(s => s.Contains("RemoveHashItem") && s.Contains("query2") && s.Contains("REMOVED"));
            DebugLogger.ClearLogData();

        }

        public static Query<StoreStock> query3 = new Query<StoreStock>(
                               stock => stock.CopiesInStore > new QueryParam(1)
                                        && stock.CopiesInStore < new QueryParam(2))
                                .OrderBy(s => s.CopiesInStore).Take(4);

        [Fact]
        public async Task GreaterThanLessThanTakeIntoListInvalidateAsync()
        {
            List<StoreStock> stocks = await cachedDB.GetMultipleAsync(query3, 10, 95);

            stocks.ShouldNotBeNull();
            stocks.Count.ShouldBe(4);

            stocks.Select(stock => stock.CopiesInStore).ShouldBeSubsetOf(new int?[] { 21, 41, 53, 79 });

            DebugLogger.Log.ShouldContain(s => s.Contains("GetHashAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            StoreStock stock1 = await cachedDB.GetByKeyAsync<StoreStock>(7);  //copies=5
            stock1.ShouldNotBeNull();
            stock1.CopiesInStore = 11;
            await cachedDB.UpdateAsync(stock1);

            DebugLogger.Log.ShouldContain(s => s.Contains("RemoveHashItem") && s.Contains("query3") && s.Contains("REMOVED"));
            DebugLogger.ClearLogData();

        }

        public static Query<StoreStock> query4 = new Query<StoreStock>(
                               stock => stock.CopiesInStore > new QueryParam(1)
                                        && stock.CopiesInStore < new QueryParam(2))
                                .OrderBy(s => s.CopiesInStore).Take(4);

        [Fact]
        public async Task GreaterThanLessThanTakeIntoListEndInvalidateAsync()
        {
            List<StoreStock> stocks = await cachedDB.GetMultipleAsync(query4, 10, 95);

            stocks.ShouldNotBeNull();
            stocks.Count.ShouldBe(4);

            stocks.Select(stock => stock.CopiesInStore).ShouldBeSubsetOf(new int?[] { 21, 41, 53, 79 });

            DebugLogger.Log.ShouldContain(s => s.Contains("GetHashAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            StoreStock stock1 = await cachedDB.GetByKeyAsync<StoreStock>(2);  //CopiesInStore = 100
            stock1.ShouldNotBeNull();
            stock1.CopiesInStore = 78;
            await cachedDB.UpdateAsync(stock1);

            DebugLogger.Log.ShouldContain(s => s.Contains("RemoveHashItem") && s.Contains("query4") && s.Contains("REMOVED"));
            DebugLogger.ClearLogData();

        }

        public static Query<StoreStock> query5 = new Query<StoreStock>(
                               stock => stock.CopiesInStore > new QueryParam(1)
                                        && stock.CopiesInStore < new QueryParam(2))
                                .OrderBy(s => s.CopiesInStore).Take(4);

        [Fact]
        public async Task GreaterThanLessThanTakeIntoListEnd2InvalidateAsync()
        {
            List<StoreStock> stocks = await cachedDB.GetMultipleAsync(query5, 10, 95);

            stocks.ShouldNotBeNull();
            stocks.Count.ShouldBe(4);

            stocks.Select(stock => stock.CopiesInStore).ShouldBeSubsetOf(new int?[] { 21, 41, 53, 79 });

            DebugLogger.Log.ShouldContain(s => s.Contains("GetHashAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            StoreStock stock1 = await cachedDB.GetByKeyAsync<StoreStock>(2);  //CopiesInStore = 100
            stock1.ShouldNotBeNull();
            stock1.CopiesInStore = 80;                //This will still invalidate, because we keep 5 in cache
            await cachedDB.UpdateAsync(stock1);

            DebugLogger.Log.ShouldContain(s => s.Contains("RemoveHashItem") && s.Contains("query5") && s.Contains("REMOVED"));
            DebugLogger.ClearLogData();
        }

        public static Query<StoreStock> query6 = new Query<StoreStock>(
                               stock => stock.CopiesInStore > new QueryParam(1)
                                        && stock.CopiesInStore < new QueryParam(2))
                                .OrderBy(s => s.CopiesInStore).Take(4);

        [Fact]
        public async Task GreaterThanLessThanTakeIntoListEnd3DontInvalidateAsync()
        {
            List<StoreStock> stocks = await cachedDB.GetMultipleAsync(query6, 10, 95);

            stocks.ShouldNotBeNull();
            stocks.Count.ShouldBe(4);

            stocks.Select(stock => stock.CopiesInStore).ShouldBeSubsetOf(new int?[] { 21, 41, 53, 79 });

            DebugLogger.Log.ShouldContain(s => s.Contains("GetHashAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            StoreStock stock1 = await cachedDB.GetByKeyAsync<StoreStock>(2);  //CopiesInStore = 100
            stock1.ShouldNotBeNull();
            stock1.CopiesInStore = 93; 
            await cachedDB.UpdateAsync(stock1);

            DebugLogger.Log.ShouldContain(s => s.Contains("RemoveAsync(List)") && s.Contains("[]"));
            DebugLogger.ClearLogData();
        }
    }
}

