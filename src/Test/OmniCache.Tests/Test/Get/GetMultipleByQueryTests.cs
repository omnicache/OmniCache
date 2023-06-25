using System;
using System.Reflection;
using OmniCache.KeyProviders;
using OmniCache.IntegrationTests.Model;
using OmniCache.IntegrationTests.Seed;
using OmniCache.IntegrationTests.Core;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Shouldly;
using Xunit;
using Xunit.Abstractions;
using OmniCache;
using OmniCache.EntityFramework;

namespace OmniCache.IntegrationTests.Test.Get
{
    [Collection("Tests")]
    public class GetMultipleByQueryTests : BaseTest
    {		
        public GetMultipleByQueryTests(ITestOutputHelper output) : base(output)
        {
            new SimpleSeed(cachedDB).Seed();           
            CachedDatabase.LoadAllQueries(this.GetType());
            
        }

        public static Query<Movie> query1 = new Query<Movie>(
                               movie => movie.Active == new QueryParam(1));

        [Fact]
        public async Task GetMultipleByFieldAsync()
        {            
            List<Movie> movies = await cachedDB.GetMultipleAsync(query1, 'A');

            movies.ShouldNotBeNull();
            
            movies.Count.ShouldBe(2);
            DebugLogger.Log.ShouldContain(s => s.Contains("query1") && s.Contains("GetAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();


            movies = await cachedDB.GetMultipleAsync(query1, 'A');
            
            movies.ShouldNotBeNull();
            ;
            movies.Count.ShouldBe(2);
            DebugLogger.Log.ShouldContain(s => s.Contains("query1") && s.Contains("GetAsync") && !s.Contains("NULL"));
        }

        public static Query<Movie> query2 = new Query<Movie>(
                               movie => movie.Category == new QueryParam(1));

        [Fact]
        public async Task GetMultipleByEnumFieldAsync()
        {            
            List<Movie> movies = await cachedDB.GetMultipleAsync(query2, Category.Thriller);

            movies.ShouldNotBeNull();
            
            movies.Count.ShouldBe(2);
            movies.ShouldAllBe(movie => movie.Category == Category.Thriller);
            DebugLogger.Log.ShouldContain(s => s.Contains("query2") && s.Contains("GetAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            movies = await cachedDB.GetMultipleAsync(query2, Category.Thriller);
            movies.ShouldNotBeNull();
            ;
            movies.Count.ShouldBe(2);
            movies.ShouldAllBe(movie => movie.Category == Category.Thriller);
            DebugLogger.Log.ShouldContain(s => s.Contains("query2") && s.Contains("GetAsync") && !s.Contains("NULL"));

        }

        public static Query<Movie> query3 = new Query<Movie>(
                               movie => (int)movie.Category == new QueryParam(1));

        [Fact]
        public async Task GetMultipleByEnumFieldCastedAsync()
        {
            List<Movie> movies = await cachedDB.GetMultipleAsync(query3, (int)Category.Thriller);

            movies.ShouldNotBeNull();            
            movies.Count.ShouldBe(2);
            movies.ShouldAllBe(movie => movie.Category == Category.Thriller);
            DebugLogger.Log.ShouldContain(s => s.Contains("query3") && s.Contains("GetAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            movies = await cachedDB.GetMultipleAsync(query3, (int)Category.Thriller);

            movies.ShouldNotBeNull();            
            movies.Count.ShouldBe(2);
            movies.ShouldAllBe(movie => movie.Category == Category.Thriller);
            DebugLogger.Log.ShouldContain(s => s.Contains("query3") && s.Contains("GetAsync") && !s.Contains("NULL"));
            DebugLogger.ClearLogData();
        }
    }
}

