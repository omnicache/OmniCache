using System;
using OmniCache;
using OmniCache.EntityFramework;
using OmniCache.IntegrationTests.Core;
using OmniCache.IntegrationTests.Model;
using OmniCache.IntegrationTests.Seed;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace OmniCache.IntegrationTests.Test.MultiCondition
{	
    [Collection("Tests")]
    public class GreaterThanEqualTests : BaseTest
    {
        public GreaterThanEqualTests(ITestOutputHelper output) : base(output)
        {
            new StoreStockSeed(cachedDB).Seed();
            CachedDatabase.LoadAllQueries(this.GetType());

        }

        public static Query<StoreStock> query1 = new Query<StoreStock>(
                               stock => stock.CopiesInStore > new QueryParam(2)
                                        && stock.DailyRentPrice == new QueryParam(1))
                                .OrderByDesc(s => s.CopiesInStore).Take(4);

        [Fact]
        public async Task GreaterThanEqualOutAsync()
        {
            List<StoreStock> stocks = await cachedDB.GetMultipleAsync(query1, 4m, 30);

            stocks.ShouldNotBeNull();
            stocks.Count.ShouldBe(2);

            stocks.Select(stock => stock.MovieId).SequenceEqual(new long[] { 94, 92 });

            DebugLogger.Log.ShouldContain(s => s.Contains("GetHashAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();
            
            StoreStock stock = stocks.FirstOrDefault(s => s.CopiesInStore == 94);
            stock.ShouldNotBeNull();
            stock.CopiesInStore = 28;
            await cachedDB.UpdateAsync(stock);

            DebugLogger.Log.ShouldContain(s => s.Contains("RemoveHashItem") && s.Contains("query1") && s.Contains("REMOVED"));
            DebugLogger.ClearLogData();

        }

        public static Query<StoreStock> query2 = new Query<StoreStock>(
                               stock => stock.CopiesInStore > new QueryParam(2)
                                        && stock.DailyRentPrice == new QueryParam(1))
                                .OrderByDesc(s => s.CopiesInStore).Take(4);

        [Fact]
        public async Task GreaterThanEqualInAsync()
        {
            List<StoreStock> stocks = await cachedDB.GetMultipleAsync(query2, 4m, 30);

            stocks.ShouldNotBeNull();
            stocks.Count.ShouldBe(2);

            stocks.Select(stock => stock.MovieId).SequenceEqual(new long[] { 94, 92 });

            DebugLogger.Log.ShouldContain(s => s.Contains("GetHashAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            StoreStock stock1 = await cachedDB.GetByKeyAsync<StoreStock>(3); 
            stock1.ShouldNotBeNull();

            DebugLogger.ClearLogData();
            stock1.DailyRentPrice = 4;
            await cachedDB.UpdateAsync(stock1);

            DebugLogger.Log.ShouldContain(s => s.Contains("RemoveHashItem") && s.Contains("query2") && s.Contains("REMOVED"));
            DebugLogger.ClearLogData();

            stocks = await cachedDB.GetMultipleAsync(query2, 4m, 30);

            stocks.ShouldNotBeNull();
            stocks.Count.ShouldBe(3);

        }

        public static Query<StoreStock> query3 = new Query<StoreStock>(
                               stock => stock.CopiesInStore > new QueryParam(1)
                                        && stock.DailyRentPrice == 4m)
                                .OrderByDesc(s => s.CopiesInStore).Take(4);

        [Fact]
        public async Task GreaterThanEqualInWithConstantAsync()
        {
            List<StoreStock> stocks = await cachedDB.GetMultipleAsync(query3, 30);

            stocks.ShouldNotBeNull();
            stocks.Count.ShouldBe(2);

            stocks.Select(stock => stock.MovieId).SequenceEqual(new long[] { 94, 92 });

            DebugLogger.Log.ShouldContain(s => s.Contains("GetHashAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            StoreStock stock1 = await cachedDB.GetByKeyAsync<StoreStock>(3);
            stock1.ShouldNotBeNull();

            DebugLogger.ClearLogData();
            stock1.DailyRentPrice = 4;
            await cachedDB.UpdateAsync(stock1);

            DebugLogger.Log.ShouldContain(s => s.Contains("RemoveHashItem") && s.Contains("query3") && s.Contains("REMOVED"));
            DebugLogger.ClearLogData();

            stocks = await cachedDB.GetMultipleAsync(query3, 30);

            stocks.ShouldNotBeNull();
            stocks.Count.ShouldBe(3);

        }

        public static Query<StoreStock> query4 = new Query<StoreStock>(
                               stock => stock.CopiesInStore > 30
                                        && stock.DailyRentPrice == new QueryParam(1))
                                .OrderByDesc(s => s.CopiesInStore).Take(4);

        [Fact]
        public async Task GreaterThanEqualInWithConstant2Async()
        {
            List<StoreStock> stocks = await cachedDB.GetMultipleAsync(query4, 4m);

            stocks.ShouldNotBeNull();
            stocks.Count.ShouldBe(2);

            stocks.Select(stock => stock.MovieId).SequenceEqual(new long[] { 94, 92 });

            DebugLogger.Log.ShouldContain(s => s.Contains("GetHashAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            StoreStock stock1 = await cachedDB.GetByKeyAsync<StoreStock>(3);
            stock1.ShouldNotBeNull();

            DebugLogger.ClearLogData();
            stock1.DailyRentPrice = 4;
            await cachedDB.UpdateAsync(stock1);

            DebugLogger.Log.ShouldContain(s => s.Contains("RemoveHashItem") && s.Contains("query4") && s.Contains("REMOVED"));
            DebugLogger.ClearLogData();

            stocks = await cachedDB.GetMultipleAsync(query4, 4m);

            stocks.ShouldNotBeNull();
            stocks.Count.ShouldBe(3);

        }
    }
}

