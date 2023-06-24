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
    public class GetByMultipleParamQueryTests : BaseTest
    {		
        public GetByMultipleParamQueryTests(ITestOutputHelper output) : base(output)
        {
            new SimpleSeed(cachedDB).Seed();            
            CachedDatabase.LoadAllQueries(this.GetType());
            
        }

        public static Query<Movie> query1 = new Query<Movie>(
                               movie => movie.IsNewRelease == new QueryParam(1)
                               && movie.Active == new QueryParam(2));

        [Fact]
        public async Task GetByMultipleParamsAsync()
        {
            

            List<Movie> movies = await cachedDB.GetMultipleAsync(query1, false, 'A');

            movies.ShouldNotBeNull();
            
            movies.Count.ShouldBe(1);
            movies.ShouldContain(m => m.Id == 101);
            DebugLogger.Log.ShouldContain(s => s.Contains("GetAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();


            movies = await cachedDB.GetMultipleAsync(query1, false, 'A');

            movies.ShouldNotBeNull();            
            movies.Count.ShouldBe(1);
            movies.ShouldContain(m => m.Id == 101);
            DebugLogger.Log.ShouldContain(s => s.Contains("GetAsync") && !s.Contains("NULL"));
            
        }

        public static Query<Movie> query2 = new Query<Movie>(
                               movie => movie.IsNewRelease == false
                               && movie.Active == 'A');

        [Fact]
        public async Task GetByMultipleConditionsAsync()
        {
            List<Movie> movies = await cachedDB.GetMultipleAsync(query2);

            movies.ShouldNotBeNull();            
            movies.Count.ShouldBe(1);
            movies.ShouldContain(m => m.Id == 101);
            DebugLogger.Log.ShouldContain(s => s.Contains("GetAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            movies = await cachedDB.GetMultipleAsync(query2);
            movies.ShouldNotBeNull();            
            movies.Count.ShouldBe(1);
            movies.ShouldContain(m => m.Id == 101);
            DebugLogger.Log.ShouldContain(s => s.Contains("GetAsync") && !s.Contains("NULL"));
            DebugLogger.ClearLogData();
        }
    }
}

