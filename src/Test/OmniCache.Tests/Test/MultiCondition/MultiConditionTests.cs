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
    public class MultiConditionTests : BaseTest
    {
        public MultiConditionTests(ITestOutputHelper output) : base(output)
        {
            new StoreStockSeed(cachedDB).Seed();
            CachedDatabase.LoadAllQueries(this.GetType());

        }

        public static Query<StoreStock> query1 = new Query<StoreStock>(
                               stock => stock.CopiesInStore > new QueryParam(1)
                                        && stock.TotalEarnings < new QueryParam(2))
                                .OrderByDesc(s => s.MovieId).Take(4);

        [Fact]
        public async Task MultiConditionOutAsync()
        {
            List<StoreStock> stocks = await cachedDB.GetMultipleAsync(query1, 40, 800);

            stocks.ShouldNotBeNull();
            stocks.Count.ShouldBe(4);

            stocks.Select(stock => stock.MovieId).ShouldBeSubsetOf(new long[] { 107, 104, 102, 101 });

            DebugLogger.Log.ShouldContain(s => s.Contains("GetHashAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            StoreStock stock = stocks.FirstOrDefault(s => s.MovieId == 104);
            stock.ShouldNotBeNull();
            stock.CopiesInStore = 38;
            await cachedDB.UpdateAsync(stock);

            DebugLogger.Log.ShouldContain(s => s.Contains("RemoveHashItem") && s.Contains("query1") && s.Contains("REMOVED"));
            DebugLogger.ClearLogData();

            stocks = await cachedDB.GetMultipleAsync(query1, 40, 800);

            stocks.ShouldNotBeNull();
            stocks.Count.ShouldBe(3);
            DebugLogger.Log.ShouldContain(s => s.Contains("GetHashAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

        }
        
        public static Query<StoreStock> query2 = new Query<StoreStock>(
                               stock => stock.CopiesInStore > new QueryParam(1)
                                        && stock.TotalEarnings < new QueryParam(2))
                                .OrderByDesc(s => s.MovieId);

        [Fact]
        public async Task MultiConditionInAsync()
        {
            List<StoreStock> stocks = await cachedDB.GetMultipleAsync(query2, 40, 800);

            stocks.ShouldNotBeNull();
            stocks.Count.ShouldBe(4);

            stocks.Select(stock => stock.MovieId).ShouldBeSubsetOf(new long[] { 107, 104, 102, 101 });

            DebugLogger.Log.ShouldContain(s => s.Contains("GetHashAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            StoreStock stock1 = await cachedDB.GetByKeyAsync<StoreStock>(1);  //CopiesInStore = 21            
            stock1.ShouldNotBeNull();

            DebugLogger.ClearLogData();
            stock1.CopiesInStore = 41;
            await cachedDB.UpdateAsync(stock1);

            DebugLogger.Log.ShouldContain(s => s.Contains("RemoveHashItem") && s.Contains("query2") && s.Contains("REMOVED"));
            DebugLogger.ClearLogData();

            stocks = await cachedDB.GetMultipleAsync(query2, 40, 800);

            stocks.ShouldNotBeNull();
            stocks.Count.ShouldBe(5);
            DebugLogger.Log.ShouldContain(s => s.Contains("GetHashAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

        }


        public static Query<StoreStock> query3 = new Query<StoreStock>(
                               stock => stock.CopiesInStore > new QueryParam(1)
                                        && stock.TotalEarnings < new QueryParam(2))
                                .OrderByDesc(s => s.MovieId);

        [Fact]
        public async Task MultiConditionHalfInAsync()
        {
            List<StoreStock> stocks = await cachedDB.GetMultipleAsync(query3, 40, 800);

            stocks.ShouldNotBeNull();
            stocks.Count.ShouldBe(4);

            stocks.Select(stock => stock.MovieId).ShouldBeSubsetOf(new long[] { 107, 104, 102, 101 });

            DebugLogger.Log.ShouldContain(s => s.Contains("GetHashAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            StoreStock stock1 = await cachedDB.GetByKeyAsync<StoreStock>(1);  //CopiesInStore = 21            
            stock1.ShouldNotBeNull();

            DebugLogger.ClearLogData();
            stock1.CopiesInStore = 41;
            stock1.TotalEarnings = 810;
            await cachedDB.UpdateAsync(stock1);

            DebugLogger.Log.ShouldContain(s => s.Contains("RemoveAsync(List)") && s.Contains("[]"));
            DebugLogger.ClearLogData();

            stocks = await cachedDB.GetMultipleAsync(query3, 40, 800);

            stocks.ShouldNotBeNull();
            stocks.Count.ShouldBe(4);
            DebugLogger.Log.ShouldContain(s => s.Contains("GetHashAsync") && !s.Contains("NULL"));
            DebugLogger.ClearLogData();

        }

        public static Query<StoreStock> query4 = new Query<StoreStock>(
                               stock => stock.CopiesInStore > new QueryParam(1)
                                        && stock.TotalEarnings < new QueryParam(2))
                                .OrderByDesc(s => s.MovieId);

        [Fact]
        public async Task MultiConditionAlreadyHalfInAsync()
        {
            List<StoreStock> stocks = await cachedDB.GetMultipleAsync(query4, 60, 800);

            stocks.ShouldNotBeNull();
            stocks.Count.ShouldBe(3);

            stocks.Select(stock => stock.MovieId).ShouldBeSubsetOf(new long[] { 107, 104, 101 });

            DebugLogger.Log.ShouldContain(s => s.Contains("GetHashAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            StoreStock stock1 = await cachedDB.GetByKeyAsync<StoreStock>(4);  //CopiesInStore = 53
            stock1.ShouldNotBeNull();

            DebugLogger.ClearLogData();
            stock1.CopiesInStore = 61;            
            await cachedDB.UpdateAsync(stock1);

            DebugLogger.Log.ShouldContain(s => s.Contains("RemoveAsync(List)") && s.Contains("[]"));
            DebugLogger.ClearLogData();

            stocks = await cachedDB.GetMultipleAsync(query4, 60, 800);

            stocks.ShouldNotBeNull();
            stocks.Count.ShouldBe(3);
            DebugLogger.Log.ShouldContain(s => s.Contains("GetHashAsync") && !s.Contains("NULL"));
            DebugLogger.ClearLogData();

        }
    }
}

