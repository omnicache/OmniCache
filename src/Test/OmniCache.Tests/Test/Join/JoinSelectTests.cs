using System;
using OmniCache.IntegrationTests.Model;
using OmniCache.IntegrationTests.Seed;
using System.Reflection;
using OmniCache.IntegrationTests.Core;
using Shouldly;
using Xunit;
using Xunit.Abstractions;
using System.Linq.Expressions;
using OmniCache.KeyProviders;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using OmniCache.Extension;
using OmniCache;
using OmniCache.EntityFramework;

namespace OmniCache.IntegrationTests.Test.Join
{
    [Collection("Tests")]
    public class JoinSelectTests : BaseTest
    {

        public JoinSelectTests(ITestOutputHelper output) : base(output)
        {
            new SimpleSeed(cachedDB).Seed();            
            CachedDatabase.LoadAllQueries(this.GetType());
        }

        public static Query<Movie> query1 = new Query<Movie>(
                               movie => movie.Active == new QueryParam(1));

        public static Query<StoreStock> query2 = new Query<StoreStock>(
                               stock => new QueryParam(1).Contains(stock.MovieId));


        [Fact]
        public async Task JoinAsync()
        {
            
            List<Movie> movies = await cachedDB.GetMultipleAsync(query1, 'A');
            List<StoreStock> movieStock = await cachedDB.GetMultipleAsync(query2, movies.GetList(m=>m.Id));


            var joinedData = from m in movies
                             join s in movieStock on m.Id equals s.MovieId
                             select new
                             {
                                 Movie = m,
                                 Stock = s
                             };

            joinedData.Count().ShouldBe(3);


            DebugLogger.Log.ShouldContain(s => s.Contains("GetAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

        }

    }
}

