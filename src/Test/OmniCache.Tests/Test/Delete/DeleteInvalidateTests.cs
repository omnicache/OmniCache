using System;
using OmniCache;
using OmniCache.EntityFramework;
using OmniCache.IntegrationTests.Core;
using OmniCache.IntegrationTests.Model;
using OmniCache.IntegrationTests.Seed;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace OmniCache.IntegrationTests.Test.Delete
{	
    [Collection("Tests")]
    public class DeleteInvalidateTests : BaseTest
    {
        public DeleteInvalidateTests(ITestOutputHelper output) : base(output)
        {
            new SimpleSeed(cachedDB).Seed();
            CachedDatabase.LoadAllQueries(this.GetType());
        }


        public static Query<Movie> query1 = new Query<Movie>(
                               movie => movie.Category == new QueryParam(1)
                               && movie.IsNewRelease == new QueryParam(2)).OrderBy(m => m.Id);


        [Fact]
        public async Task InvalidateQueryAsync()
        {
            List<Movie> movies = await cachedDB.GetMultipleAsync(query1, Category.Thriller, false);

            movies.Count.ShouldBe(2);
            movies[0].Id.ShouldBe(100);
            movies[1].Id.ShouldBe(101);
            DebugLogger.Log.ShouldContain(s => s.Contains("GetAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();
            
            Movie movie = movies[1];            
            await cachedDB.DeleteAsync(movie);

            List<Movie> movies3 = await cachedDB.GetMultipleAsync(query1, Category.Thriller, false);
            movies3.Count.ShouldBe(1);

            movies3[0].Id.ShouldBe(100);
            DebugLogger.Log.ShouldContain(s => s.Contains("GetAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

        }
    }
}

