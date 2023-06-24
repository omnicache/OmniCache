using System;
using OmniCache.IntegrationTests.Model;
using OmniCache.IntegrationTests.Seed;
using OmniCache.IntegrationTests.Test.Add;
using System.Reflection;
using OmniCache.IntegrationTests.Core;
using Xunit;
using Xunit.Abstractions;
using OmniCache.KeyProviders;
using Shouldly;
using OmniCache;
using OmniCache.EntityFramework;

namespace OmniCache.IntegrationTests.Test.Update
{
    [Collection("Tests")]
    public class UpdateTests : BaseTest
    {		
        public UpdateTests(ITestOutputHelper output) : base(output)
        {
            new SimpleSeed(cachedDB).Seed();            
            CachedDatabase.LoadAllQueries(typeof(Movie));
        }

        [Fact]
        public async Task UpdateAsync()
        {

            Movie movie = await cachedDB.GetByKeyAsync<Movie>(100);

            movie.ShouldNotBeNull();            
            movie.Name.ShouldBe("Jaws");
            DebugLogger.Log.ShouldContain(s => s.Contains("GetAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            movie.Name = "Terminator";
            await cachedDB.UpdateAsync<Movie>(movie);

            movie = await cachedDB.GetByKeyAsync<Movie>(100);

            movie.ShouldNotBeNull();            
            movie.Name.ShouldBe("Terminator");
            DebugLogger.Log.ShouldContain(s => s.Contains("GetAsync") && !s.Contains("NULL"));
            DebugLogger.ClearLogData();
        }
    }
}

