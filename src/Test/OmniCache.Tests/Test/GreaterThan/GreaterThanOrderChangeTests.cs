using System;
using System.Linq;
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
    public class GreaterThanOrderChangeTests : BaseTest
    {		
        public GreaterThanOrderChangeTests(ITestOutputHelper output) : base(output)
        {
            new StoreStockSeed(cachedDB).Seed();
            CachedDatabase.LoadAllQueries(this.GetType());

        }


        public static Query<StoreStock> query1 = new Query<StoreStock>(
                               stock => stock.MovieId > new QueryParam(1)).OrderByDesc(s => s.CopiesInStore).Take(4);

        [Fact]
        public async Task GreaterThanOrderChangeAsync()
        {
            List<StoreStock> stocks = await cachedDB.GetMultipleAsync(query1, 101);

            stocks.ShouldNotBeNull();
            stocks.Count.ShouldBe(4);
            
            stocks.Select(stock => stock.CopiesInStore).ShouldBeSubsetOf(new int?[] { 94, 92, 79, 53 });

            DebugLogger.Log.ShouldContain(s => s.Contains("query1") && s.Contains("GetHashAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            StoreStock stock1 = stocks.FirstOrDefault(s => s.CopiesInStore == 92);
            stock1.CopiesInStore = 20;
            await cachedDB.UpdateAsync(stock1);

            DebugLogger.Log.ShouldContain(s => s.Contains("query1") && s.Contains("RemoveHashItem") && s.Contains("query1") && s.Contains("REMOVED"));
            DebugLogger.ClearLogData();
        }
    }
}

