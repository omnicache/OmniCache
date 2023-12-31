﻿using System;
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
    public class GreaterThanUnorderedListTests : BaseTest
    {
        public GreaterThanUnorderedListTests(ITestOutputHelper output) : base(output)
        {
            new SimpleSeed(cachedDB).Seed();
            CachedDatabase.LoadAllQueries(this.GetType());

        }

        public static Query<StoreStock> query1 = new Query<StoreStock>(
                               s => s.CopiesInStore > new QueryParam(1));

        [Fact]
        public async Task UpdateGreaterThanUnorderedListAsync()
        {
            List<StoreStock> stock = await cachedDB.GetMultipleAsync(query1, 7);

            stock.ShouldNotBeNull();
            stock.Count.ShouldBe(4);
            DebugLogger.Log.ShouldContain(s =>s.Contains("query1") &&  s.Contains("GetHashAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            stock = await cachedDB.GetMultipleAsync(query1, 7);
            DebugLogger.Log.ShouldContain(s => s.Contains("query1") && s.Contains("GetHashAsync") && !s.Contains("NULL"));
            DebugLogger.ClearLogData();

            StoreStock stock1 = stock.Where(s => s.Id == 1).FirstOrDefault();
            stock1.ShouldNotBeNull();
            stock1.CopiesInStore = 5;
            await cachedDB.UpdateAsync(stock1);
            
            DebugLogger.Log.ShouldContain(s => s.Contains("query1") && s.Contains("RemoveHashItems(List)") && !s.Contains("NULL"));
            DebugLogger.ClearLogData();

            stock = await cachedDB.GetMultipleAsync(query1, 7);
            DebugLogger.Log.ShouldContain(s => s.Contains("query1") && s.Contains("GetHashAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

        }

        public static Query<StoreStock> query2 = new Query<StoreStock>(
                               s => s.CopiesInStore > new QueryParam(1));

        [Fact]
        public async Task UpdateGreaterThanUnorderedListNoChangeAsync()
        {
            List<StoreStock> stock = await cachedDB.GetMultipleAsync(query2, 7);

            stock.ShouldNotBeNull();
            stock.Count.ShouldBe(4);
            DebugLogger.Log.ShouldContain(s => s.Contains("query2") && s.Contains("GetHashAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            stock = await cachedDB.GetMultipleAsync(query2, 7);
            DebugLogger.Log.ShouldContain(s => s.Contains("query2") && s.Contains("GetHashAsync") && !s.Contains("NULL"));
            DebugLogger.ClearLogData();

            StoreStock stock1 = stock.Where(s => s.Id == 1).FirstOrDefault();
            stock1.ShouldNotBeNull();
            stock1.CopiesInStore = 100;
            await cachedDB.UpdateAsync(stock1);

            DebugLogger.Log.ShouldNotContain(s => s.Contains("query2") && s.Contains("RemoveHashItems(List)"));
            DebugLogger.ClearLogData();

            stock = await cachedDB.GetMultipleAsync(query2, 7);
            DebugLogger.Log.ShouldContain(s => s.Contains("query2") && s.Contains("GetHashAsync") && !s.Contains("NULL"));
            DebugLogger.ClearLogData();

        }

        public static Query<StoreStock> query3 = new Query<StoreStock>(
                               s => s.CopiesInStore > new QueryParam(1));

        [Fact]
        public async Task UpdateGreaterThanUnorderedListAddAsync()
        {
            List<StoreStock> stock = await cachedDB.GetMultipleAsync(query3, 7);

            stock.ShouldNotBeNull();
            stock.Count.ShouldBe(4);
            DebugLogger.Log.ShouldContain(s => s.Contains("query3") && s.Contains("GetHashAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            stock = await cachedDB.GetMultipleAsync(query3, 7);
            DebugLogger.Log.ShouldContain(s => s.Contains("query3") && s.Contains("GetHashAsync") && !s.Contains("NULL"));
            DebugLogger.ClearLogData();

            StoreStock stock1 = new StoreStock
            {                
                StoreCode = "STORE1",
                MovieId = 100,
                TotalEarnings = 1000.0f,
                CopiesInStore = 13,
                TotalRentals = 50,
                DailyRentPrice = 2.99m,
                OverdueFees = 20.0
            };

            await cachedDB.AddAsync(stock1);
            
            DebugLogger.Log.ShouldContain(s => s.Contains("query3") && s.Contains("RemoveHashItems(List)") && !s.Contains("NULL"));
            DebugLogger.ClearLogData();

            stock = await cachedDB.GetMultipleAsync(query3, 7);
            stock.Count.ShouldBe(5);
            DebugLogger.ClearLogData();

        }


        public static Query<StoreStock> query4 = new Query<StoreStock>(
                               s => s.CopiesInStore > new QueryParam(1));

        [Fact]
        public async Task UpdateGreaterThanUnorderedListDeleteAsync()
        {
            List<StoreStock> stock = await cachedDB.GetMultipleAsync(query4, 7);

            stock.ShouldNotBeNull();
            stock.Count.ShouldBe(4);
            DebugLogger.Log.ShouldContain(s => s.Contains("query4") && s.Contains("GetHashAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            stock = await cachedDB.GetMultipleAsync(query4, 7);
            DebugLogger.Log.ShouldContain(s => s.Contains("query4") && s.Contains("GetHashAsync") && !s.Contains("NULL"));
            DebugLogger.ClearLogData();

            StoreStock stock1 = stock.Where(s => s.Id == 1).FirstOrDefault();
            stock1.ShouldNotBeNull();
            await cachedDB.DeleteAsync(stock1);

            DebugLogger.Log.ShouldContain(s => s.Contains("query4") && s.Contains("RemoveHashItems(List)") && !s.Contains("NULL"));
            DebugLogger.ClearLogData();

            stock = await cachedDB.GetMultipleAsync(query4, 7);
            stock.Count.ShouldBe(3);
            DebugLogger.ClearLogData();

        }

        public static Query<StoreStock> query5 = new Query<StoreStock>(
                               s => s.CopiesInStore > new QueryParam(1));

        [Fact]
        public async Task UpdateGreaterThanUnorderedListDeleteNoChangeAsync()
        {
            List<StoreStock> stock = await cachedDB.GetMultipleAsync(query5, 7);

            stock.ShouldNotBeNull();
            stock.Count.ShouldBe(4);
            DebugLogger.Log.ShouldContain(s => s.Contains("query5") && s.Contains("GetHashAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            stock = await cachedDB.GetMultipleAsync(query5, 7);
            DebugLogger.Log.ShouldContain(s => s.Contains("query5") && s.Contains("GetHashAsync") && !s.Contains("NULL"));
            DebugLogger.ClearLogData();

            StoreStock stock1 = await cachedDB.GetByKeyAsync<StoreStock>(4);
            stock1.ShouldNotBeNull();
            stock1.CopiesInStore = 1;
            await cachedDB.UpdateAsync(stock1);

            DebugLogger.Log.ShouldNotContain(s => s.Contains("query5") && s.Contains("RemoveHashItems(List)"));
            DebugLogger.ClearLogData();

            stock = await cachedDB.GetMultipleAsync(query5, 7);
            stock.Count.ShouldBe(4);
            DebugLogger.Log.ShouldContain(s => s.Contains("query5") && s.Contains("GetHashAsync") && !s.Contains("NULL"));
            DebugLogger.ClearLogData();


        }

        public static Query<StoreStock> query6 = new Query<StoreStock>(
                               s => s.CopiesInStore > new QueryParam(1));

        [Fact]
        public async Task UpdateGreaterThanUnorderedListAddNoChangeAsync()
        {
            List<StoreStock> stock = await cachedDB.GetMultipleAsync(query6, 7);

            stock.ShouldNotBeNull();
            stock.Count.ShouldBe(4);
            DebugLogger.Log.ShouldContain(s => s.Contains("query6") && s.Contains("GetHashAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            stock = await cachedDB.GetMultipleAsync(query6, 7);
            stock.Count.ShouldBe(4);
            DebugLogger.Log.ShouldContain(s => s.Contains("query6") && s.Contains("GetHashAsync") && !s.Contains("NULL"));
            DebugLogger.ClearLogData();

            StoreStock stock1 = new StoreStock
            {
                StoreCode = "STORE1",
                MovieId = 100,
                TotalEarnings = 1000.0f,
                CopiesInStore = 3,
                TotalRentals = 50,
                DailyRentPrice = 2.99m,
                OverdueFees = 20.0
            };

            await cachedDB.AddAsync(stock1);

            DebugLogger.Log.ShouldNotContain(s => s.Contains("query6") && s.Contains("RemoveHashItems(List)"));
            DebugLogger.ClearLogData();

            stock = await cachedDB.GetMultipleAsync(query6, 7);
            stock.Count.ShouldBe(4);
            DebugLogger.Log.ShouldContain(s => s.Contains("query6") && s.Contains("GetHashAsync") && !s.Contains("NULL"));            
            DebugLogger.ClearLogData();

        }
    }
}

