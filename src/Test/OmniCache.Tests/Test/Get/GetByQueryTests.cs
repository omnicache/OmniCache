using System;
using System.Reflection;
using System.Xml.Linq;
using OmniCache.KeyProviders;
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
    public class GetByQueryTests : BaseTest
    {		
        public GetByQueryTests(ITestOutputHelper output) : base(output)
        {
            new SimpleSeed(cachedDB).Seed();            
            CachedDatabase.LoadAllQueries(this.GetType());            
        }

        public static Query<Movie> query1 = new Query<Movie>(
                               movie => movie.Name == new QueryParam(1));

        [Fact]
        public async Task GetByFieldAsync()
        {         
            Movie movie = await cachedDB.GetAsync(query1, "Matrix");
            
            movie.ShouldNotBeNull();            
            DebugLogger.Log.ShouldContain(s => s.Contains("query1") && s.Contains("GetAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();


            movie = await cachedDB.GetAsync(query1, "Matrix");

            movie.ShouldNotBeNull();            
            DebugLogger.Log.ShouldContain(s => s.Contains("query1") && s.Contains("GetAsync") && !s.Contains("NULL"));
            DebugLogger.ClearLogData();
        }

        
       
    }
}

