using System;
using System.Reflection;
using OmniCache.IntegrationTests.Model;
using OmniCache.IntegrationTests.Seed;
using OmniCache.IntegrationTests.Core;
using Shouldly;
using Xunit;
using Xunit.Abstractions;
using OmniCache;
using OmniCache.EntityFramework;

namespace OmniCache.IntegrationTests.Test.Add
{
    [Collection("Tests")]
    public class AddTests : BaseTest
    {		
        public AddTests(ITestOutputHelper output) : base(output)
        {
            new SimpleSeed(cachedDB).Seed();            
            CachedDatabase.LoadAllQueries(typeof(Movie));

        }

        [Fact]
        public async Task AddAsync()
        {
            
            Movie movie1 = new Movie()
            {
                Id = 200,
                Name = "Toy Story",
                ReleaseDate = new DateTime(1996, 5, 17),
                IsNewRelease = true,
                Category = Category.Kids,
                Active = 'A'                
            };

            await cachedDB.AddAsync<Movie>(movie1);
            

            Movie movie = await cachedDB.GetByKeyAsync<Movie>(200);

            movie.ShouldNotBeNull();            
            DebugLogger.Log.ShouldContain(s => s.Contains("GetAsync") && !s.Contains("NULL"));
            DebugLogger.ClearLogData();
        }
    }
}

