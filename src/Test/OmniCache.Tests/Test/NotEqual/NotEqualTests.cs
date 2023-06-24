using System;
using OmniCache.IntegrationTests.Core;
using OmniCache.IntegrationTests.Model;
using OmniCache.IntegrationTests.Seed;
using OmniCache.IntegrationTests.Core;
using Shouldly;
using Xunit;
using Xunit.Abstractions;
using OmniCache;
using OmniCache.EntityFramework;

namespace OmniCache.IntegrationTests.Test.NotEqual
{	
    [Collection("Tests")]
    public class NotEqualTests : BaseTest
    {
        public NotEqualTests(ITestOutputHelper output) : base(output)
        {
            new SimpleSeed(cachedDB).Seed();
            CachedDatabase.LoadAllQueries(this.GetType());
        }

        public static Query<RentalStore> query1 = new Query<RentalStore>(
                               store => store.State != new QueryParam(1));

        [Fact]
        public async Task GetStateNotEqualQueryParamAsync()
        {
            List<RentalStore> store = await cachedDB.GetMultipleAsync(query1, State.VIC);

            store.ShouldNotBeNull();            
            store.Count.ShouldBe(1);
            DebugLogger.Log.ShouldContain(s => s.Contains("GetAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();


            store = await cachedDB.GetMultipleAsync(query1, State.VIC);

            store.ShouldNotBeNull();           
            store.Count.ShouldBe(1);
        }

        public static Query<RentalStore> query2 = new Query<RentalStore>(
                               store => store.State != new QueryParam(1));

        [Fact]
        public async Task GetStateNotEqualNullQueryParamAsync()
        {
            List<RentalStore> store = await cachedDB.GetMultipleAsync(query2, null);

            store.ShouldNotBeNull();            
            store.Count.ShouldBe(2);
            DebugLogger.Log.ShouldContain(s => s.Contains("GetAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            store = await cachedDB.GetMultipleAsync(query2, null);

            store.ShouldNotBeNull();            
            store.Count.ShouldBe(2);

        }

        public static Query<RentalStore> query3 = new Query<RentalStore>(
                               store => store.State != null);

        [Fact]
        public async Task GetStateNotEqualNullConstantAsync()
        {
            List<RentalStore> store = await cachedDB.GetMultipleAsync(query3);

            store.ShouldNotBeNull();            
            store.Count.ShouldBe(2);
            DebugLogger.Log.ShouldContain(s => s.Contains("GetAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            store = await cachedDB.GetMultipleAsync(query3);

            store.ShouldNotBeNull();            
            store.Count.ShouldBe(2);
        }

        public static Query<RentalStore> query4 = new Query<RentalStore>(
                               store => store.State != State.VIC);

        [Fact]
        public async Task GetStateNotEqualConstantAsync()
        {
            List<RentalStore> store = await cachedDB.GetMultipleAsync(query4);

            store.ShouldNotBeNull();            
            store.Count.ShouldBe(1);
            DebugLogger.Log.ShouldContain(s => s.Contains("GetAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            store = await cachedDB.GetMultipleAsync(query4);

            store.ShouldNotBeNull();            
            store.Count.ShouldBe(1);
            DebugLogger.Log.ShouldContain(s => s.Contains("GetAsync") && !s.Contains("NULL"));
            DebugLogger.ClearLogData();
        }
    }
}

