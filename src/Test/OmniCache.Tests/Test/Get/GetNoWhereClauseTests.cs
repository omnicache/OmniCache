using System;
using OmniCache;
using OmniCache.EntityFramework;
using OmniCache.IntegrationTests.Core;
using OmniCache.IntegrationTests.Model;
using OmniCache.IntegrationTests.Seed;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace OmniCache.IntegrationTests.Test.Get
{	
    [Collection("Tests")]
    public class GetNoWhereClauseTests : BaseTest
    {
        public GetNoWhereClauseTests(ITestOutputHelper output) : base(output)
        {
            new SimpleSeed(cachedDB).Seed();
            CachedDatabase.LoadAllQueries(this.GetType());
        }

        public static Query<RentalStore> query1 = new Query<RentalStore>().OrderBy(s=>s.Address).Take(2);


        [Fact]
        public async Task GetByNullQueryParam()
        {
            List<RentalStore> stores = await cachedDB.GetMultipleAsync(query1);

            stores.ShouldNotBeNull();
            stores.Count().ShouldBe(2);

            stores[0].StoreCode.ShouldBe("STORE3");
            stores[1].StoreCode.ShouldBe("STORE1");

            DebugLogger.Log.ShouldContain(s => s.Contains("query1") && s.Contains("GetAsync") && s.Contains("NULL"));

        }

        [Fact]
        public async Task InvalidateNullQueryParam()
        {
            List<RentalStore> stores = await cachedDB.GetMultipleAsync(query1);
            DebugLogger.Log.ShouldContain(s => s.Contains("query1") && s.Contains("GetAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            RentalStore rental = new RentalStore
            {
                StoreCode = "STORE1a",
                Name = "Store 1a",
                Address = "Store Address 1a",
                State = State.VIC
            };

            await cachedDB.AddAsync(rental);

            DebugLogger.Log.ShouldContain(s => s.Contains("query1") && s.Contains("RemoveAsync(List)") && !s.Contains("NULL"));
            DebugLogger.ClearLogData();
        }

    }
}

