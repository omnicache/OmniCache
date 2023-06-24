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
    public class GreaterThanInvalidateTests : BaseTest
    {
		
        public GreaterThanInvalidateTests(ITestOutputHelper output) : base(output)
        {
            new SimpleSeed(cachedDB).Seed();
            CachedDatabase.LoadAllQueries(this.GetType());

        }

        public static Query<StoreStock> query1 = new Query<StoreStock>(
                               stock => stock.CopiesInStore > new QueryParam(1));

        [Fact]
        public async Task InvalidateGreaterThan()
        {
            List<StoreStock> stocks = await cachedDB.GetMultipleAsync(query1, 6);

            stocks.ShouldNotBeNull();            
            stocks.Count.ShouldBe(4);
            DebugLogger.Log.ShouldContain(s => s.Contains("GetHashAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            List<StoreStock> stocks2 = await cachedDB.GetMultipleAsync(query1, 9);

            stocks2.ShouldNotBeNull();            
            stocks2.Count.ShouldBe(2);
            DebugLogger.Log.ShouldContain(s => s.Contains("GetHashAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            StoreStock stock = stocks2.FirstOrDefault(s => s.Id == 3);
            stock.ShouldNotBeNull();
            stock.CopiesInStore = 3;                                              //Update from 12 -> 3
            await cachedDB.UpdateAsync(stock, true);                              //Will invalidate keys :12
                                                                                    //                   :3

            stocks2 = await cachedDB.GetMultipleAsync(query1, 9);
            stocks2.ShouldNotBeNull();            
            stocks2.Count.ShouldBe(1);
            DebugLogger.Log.ShouldContain(s => s.Contains("GetHashAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

        }
    }
}

