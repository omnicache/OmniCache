using System;
using OmniCache.IntegrationTests.Model;
using OmniCache.IntegrationTests.Seed;
using OmniCache.IntegrationTests.Core;
using Shouldly;
using Xunit;
using Xunit.Abstractions;
using OmniCache;
using OmniCache.EntityFramework;

namespace OmniCache.IntegrationTests.Test.Get
{
    [Collection("Tests")]
    public class GetNullableEnumTests : BaseTest
    {		
        public GetNullableEnumTests(ITestOutputHelper output) : base(output)
        {
            new SimpleSeed(cachedDB).Seed();
            CachedDatabase.LoadAllQueries(this.GetType());
        }
        
        public static Query<RentalStore> query1 = new Query<RentalStore>(
                               store => store.State == new QueryParam(1));
        
        [Fact]
        public async Task GetByNullQueryParam()
        {
            RentalStore store = await cachedDB.GetAsync(query1, null);

            store.ShouldNotBeNull();            
            DebugLogger.Log.ShouldContain(s => s.Contains("query1") && s.Contains("GetAsync") && s.Contains("NULL"));
            
        }

        public static Query<RentalStore> query2 = new Query<RentalStore>(
                               store => store.State == null);

        [Fact]
        public async Task GetByNullParam()
        {
            RentalStore store = await cachedDB.GetAsync(query2);

            store.ShouldNotBeNull();            
            DebugLogger.Log.ShouldContain(s => s.Contains("query2") && s.Contains("GetAsync") && s.Contains("NULL"));
            
        }
    }
}

