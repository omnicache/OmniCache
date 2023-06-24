using System;
using OmniCache.IntegrationTests.Model;
using OmniCache.IntegrationTests.Seed;
using System.Reflection;
using OmniCache.IntegrationTests.Core;
using Shouldly;
using Xunit;
using Xunit.Abstractions;
using OmniCache.KeyProviders;
using OmniCache;
using OmniCache.EntityFramework;

namespace OmniCache.IntegrationTests.Test.Join
{
    [Collection("Tests")]
    public class JoinSetsTests : BaseTest
    {		
        public JoinSetsTests(ITestOutputHelper output) : base(output)
        {
            new SimpleSeed(cachedDB).Seed();            
            CachedDatabase.LoadAllQueries(this.GetType());            
        }

        public static Query<Movie> query1 = new Query<Movie>(
                               movie => movie.Active == new QueryParam(1));

        public static Query<StoreStock> query2 = new Query<StoreStock>(
                               stock => stock.CopiesInStore > 0);

        [Fact]
        public async Task JoinAsync()
        {
            List<Movie> movies = await cachedDB.GetMultipleAsync(query1, 'A');

            List<StoreStock> stock = await cachedDB.GetMultipleAsync(query2);


            var joinedData = from m in movies
                             join s in stock on m.Id equals s.MovieId
                             select new
                             {
                                 Movie = m,
                                 Stock = s
                             };

            joinedData.Count().ShouldBe(3);
        }


    }
}

