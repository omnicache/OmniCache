using System;
using OmniCache.IntegrationTests.Model;
using OmniCache.IntegrationTests.Seed;
using OmniCache.IntegrationTests.Core;
using Shouldly;
using Xunit;
using Xunit.Abstractions;
using OmniCache;
using OmniCache.EntityFramework;
using System.Linq;

namespace OmniCache.IntegrationTests.Test.NotEqual
{    
    [Collection("Tests")]
    public class NotEqualInvalidateTests : BaseTest
    {
        public NotEqualInvalidateTests(ITestOutputHelper output) : base(output)
        {
            new SimpleSeed(cachedDB).Seed();
            CachedDatabase.LoadAllQueries(this.GetType());
        }

        
        public static Query<RentalStore> query1 = new Query<RentalStore>(
                               store => store.State != new QueryParam(1));        

        [Fact]
        public async Task InvalidateOriginalQueryAsync()
        {
            List<RentalStore> stores = await cachedDB.GetMultipleAsync(query1, State.VIC);

            stores.ShouldNotBeNull();            
            stores.Count.ShouldBe(1);
            DebugLogger.Log.ShouldContain(s => s.Contains("GetAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            RentalStore store3 = await cachedDB.GetByKeyAsync<RentalStore>("STORE3");

            store3.ShouldNotBeNull();            
            store3.State = State.VIC;
            await cachedDB.UpdateAsync(store3, true);

            stores = await cachedDB.GetMultipleAsync(query1, State.VIC);

            stores.ShouldNotBeNull();            
            stores.Count.ShouldBe(0);
            DebugLogger.Log.ShouldContain(s => s.Contains("GetAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

        }

        public static Query<RentalStore> query2 = new Query<RentalStore>(
                               store => store.State != State.VIC);

        public static Query<RentalStore> query3 = new Query<RentalStore>(
                               store => store.State != null);

        [Fact]
        public async Task InvalidateOtherQuerySameFieldAsync()
        {
            List<RentalStore> stores2 = await cachedDB.GetMultipleAsync(query2);
            List<RentalStore> stores3 = await cachedDB.GetMultipleAsync(query3);

            stores2.ShouldNotBeNull();
            stores2.Count.ShouldBe(1);            
            DebugLogger.Log.ShouldContain(s => s.Contains("GetAsync") && s.Contains("query2") && s.Contains("NULL"));            

            stores3.ShouldNotBeNull();
            stores3.Count.ShouldBe(2);            
            DebugLogger.Log.ShouldContain(s => s.Contains("GetAsync") && s.Contains("query3") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            RentalStore toUpdate = stores2[0];
            toUpdate.State = State.VIC;
            await cachedDB.UpdateAsync(toUpdate, true);

            stores2 = await cachedDB.GetMultipleAsync(query2);
            stores3 = await cachedDB.GetMultipleAsync(query3);

            stores2.ShouldNotBeNull();
            stores2.Count.ShouldBe(0);            
            DebugLogger.Log.ShouldContain(s => s.Contains("GetAsync") && s.Contains("query2") && s.Contains("NULL"));            

            stores3.ShouldNotBeNull();
            stores3.Count.ShouldBe(3);            
            DebugLogger.Log.ShouldContain(s => s.Contains("GetAsync") && s.Contains("query3") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

        }
        

        public static Query<RentalStore> query4 = new Query<RentalStore>(
                               store => store.State != State.VIC).OrderBy(s=>s.StoreCode);

        public static Query<RentalStore> query5 = new Query<RentalStore>(
                               store => store.Name == new QueryParam(1)).OrderBy(s => s.StoreCode);

        [Fact]
        public async Task DontInvalidateOtherQueryDiffFieldAsync()
        {
            List<RentalStore> stores4 = await cachedDB.GetMultipleAsync(query4);
            List<RentalStore> stores5 = await cachedDB.GetMultipleAsync(query5, "Store 3");

            stores4.ShouldNotBeNull();
            stores4.Count.ShouldBe(1);            
            DebugLogger.Log.ShouldContain(s => s.Contains("GetAsync") && s.Contains("query4") && s.Contains("NULL"));            

            stores5.ShouldNotBeNull();
            stores5.Count.ShouldBe(1);            
            DebugLogger.Log.ShouldContain(s => s.Contains("GetAsync") && s.Contains("query5") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            RentalStore toUpdate = stores4[0];
            toUpdate.State = State.VIC;
            await cachedDB.UpdateAsync(toUpdate);

            stores4 = await cachedDB.GetMultipleAsync(query4);
            stores5 = await cachedDB.GetMultipleAsync(query5, "Store 3");

            stores4.ShouldNotBeNull();
            stores4.Count.ShouldBe(0);            
            DebugLogger.Log.ShouldContain(s => s.Contains("GetAsync") && s.Contains("query4") && s.Contains("NULL"));            

            stores5.ShouldNotBeNull();
            stores5.Count.ShouldBe(1);            
            stores5[0].State.ShouldBe(State.VIC);
            DebugLogger.Log.ShouldContain(s => s.Contains("GetAsync") && s.Contains("query5") && !s.Contains("NULL"));
            DebugLogger.ClearLogData();
        }
    }
}

