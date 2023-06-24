using System;
using OmniCache;
using OmniCache.EntityFramework;
using OmniCache.IntegrationTests.Core;
using OmniCache.IntegrationTests.Model;
using OmniCache.IntegrationTests.Seed;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace OmniCache.IntegrationTests.Test.Delete
{	
    [Collection("Tests")]
    public class DeleteTests : BaseTest
    {
        public DeleteTests(ITestOutputHelper output) : base(output)
        {
            new SimpleSeed(cachedDB).Seed();
            CachedDatabase.LoadAllQueries(typeof(Movie));
        }

        [Fact]
        public async Task DeleteAsync()
        {

            Movie movie = await cachedDB.GetByKeyAsync<Movie>(100);

            movie.ShouldNotBeNull();
            movie.Name.ShouldBe("Jaws");
            DebugLogger.Log.ShouldContain(s => s.Contains("GetAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();
            
            await cachedDB.DeleteAsync(movie);

            movie = await cachedDB.GetByKeyAsync<Movie>(100);

            movie.ShouldBeNull();            
            DebugLogger.Log.ShouldContain(s => s.Contains("GetAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();
        }
    }
}

