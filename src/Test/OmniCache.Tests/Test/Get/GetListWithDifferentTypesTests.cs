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
    public class GetListWithDifferentTypesTests : BaseTest
    {
        public GetListWithDifferentTypesTests(ITestOutputHelper output) : base(output)
        {
            new StoreStockSeed(cachedDB).Seed();
            CachedDatabase.LoadAllQueries(this.GetType());

        }

        public static Query<StoreStock> query1 = new Query<StoreStock>(
                               stock => new QueryParam(1).Contains(stock.CopiesInStore));

        [Fact]
        public async Task SameTypeAsync()
        {
            var list = new List<int?> { 21, 100, 41, 53 };
            List<StoreStock> stock = await cachedDB.GetMultipleAsync(query1, list);

            stock.ShouldNotBeNull();
            stock.Count().ShouldBe(4);
        }

        public static Query<StoreStock> query2 = new Query<StoreStock>(
                               stock => new QueryParam(1).Contains(stock.CopiesInStore));

        [Fact]
        public async Task NonNullableTypeAsync()
        {
            var list = new List<int> { 21, 100, 41, 53 };
            List<StoreStock> stock = await cachedDB.GetMultipleAsync(query2, list);

            stock.ShouldNotBeNull();
            stock.Count().ShouldBe(4);
        }

        public static Query<StoreStock> query3 = new Query<StoreStock>(
                               stock => new QueryParam(1).Contains(stock.MovieId));

        [Fact]
        public async Task NullableTypeAsync()
        {
            var list = new List<long?> { 100, 101, 102, 103 };
            List<StoreStock> stock = await cachedDB.GetMultipleAsync(query3, list);

            stock.ShouldNotBeNull();
            stock.Count().ShouldBe(4);
        }


        public static Query<StoreStock> query4 = new Query<StoreStock>(
                               stock => new QueryParam(1).Contains(stock.CopiesInStore));

        [Fact]
        public async Task NonNullableDifferentTypeAsync()
        {
            var list = new List<long> { 21, 100, 41, 53 };
            List<StoreStock> stock = await cachedDB.GetMultipleAsync(query4, list);

            stock.ShouldNotBeNull();
            stock.Count().ShouldBe(4);
        }

        public static Query<StoreStock> query5 = new Query<StoreStock>(
                               stock => new QueryParam(1).Contains(stock.MovieId));

        [Fact]
        public async Task NullableDifferentTypeAsync()
        {
            var list = new List<int?> { 100, 101, 102, 103 };
            List<StoreStock> stock = await cachedDB.GetMultipleAsync(query5, list);

            stock.ShouldNotBeNull();
            stock.Count().ShouldBe(4);
        }
    }
}
