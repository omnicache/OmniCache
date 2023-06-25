using System;
using OmniCache.IntegrationTests.Model;
using OmniCache.IntegrationTests.Seed;
using System.Reflection;
using OmniCache.IntegrationTests.Core;
using Xunit;
using Xunit.Abstractions;
using OmniCache.KeyProviders;
using Shouldly;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OmniCache;
using OmniCache.EntityFramework;

namespace OmniCache.IntegrationTests.Test.Update
{
    [Collection("Tests")]
    public class UpdateInvalidateTests : BaseTest
    {		
        public UpdateInvalidateTests(ITestOutputHelper output) : base(output)
        {
            new SimpleSeed(cachedDB).Seed();            
            CachedDatabase.LoadAllQueries(this.GetType());
        }


        public static Query<Movie> query1 = new Query<Movie>(
                               movie => movie.Category == new QueryParam(1)
                               && movie.IsNewRelease == new QueryParam(2)).OrderBy(m => m.Id);

        [Fact]
        public async Task NoTrackingTestAsync()
        {
            List<Movie> movies = await cachedDB.DbContext.Set<Movie>().Where(m => m.Category == Category.Thriller).ToListAsync();

            Movie movie1 = movies.Find(m => m.Id == 101);

            movie1.Name = "test";

            Movie movie2 = await cachedDB.GetByKeyAsync<Movie>(101);
            
            movie1.Name = "test2";

            cachedDB.DbContext.Update(movie1);
            cachedDB.DbContext.SaveChanges();

            movie1.ShouldNotBeNull();
        }

            
        [Fact]
        public async Task InvalidateQueryAsync()
        {
            List<Movie> movies = await cachedDB.GetMultipleAsync(query1, Category.Thriller, false);
            
            movies.Count.ShouldBe(2);
            movies[0].Id.ShouldBe(100);
            movies[1].Id.ShouldBe(101);
            DebugLogger.Log.ShouldContain(s => s.Contains("query1") && s.Contains("GetAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            List<Movie> movies2 = await cachedDB.GetMultipleAsync(query1, Category.Thriller, false);
            movies2.Count.ShouldBe(2);            

            Movie movie = movies[1];
            movie.IsNewRelease = true;
            await cachedDB.UpdateAsync(movie, true);

            List<Movie> movies3 = await cachedDB.GetMultipleAsync(query1, Category.Thriller, false);
            movies3.Count.ShouldBe(1);
            
            movies3[0].Id.ShouldBe(100);
            DebugLogger.Log.ShouldContain(s => s.Contains("query1") && s.Contains("GetAsync") && s.Contains("NULL"));
            DebugLogger.ClearLogData();

            movie = await cachedDB.GetByKeyAsync<Movie>(100);

            movie.ShouldNotBeNull();
            movie.Name.ShouldBe("Jaws");                      
            DebugLogger.Log.ShouldContain(s => s.Contains("100") && s.Contains("GetAsync") && !s.Contains("NULL"));
            DebugLogger.ClearLogData();

            movie.Name = "Terminator";

            await cachedDB.UpdateAsync<Movie>(movie);

            movie = await cachedDB.GetByKeyAsync<Movie>(100);

            movie.ShouldNotBeNull();
            movie.Name.ShouldBe("Terminator");            
            DebugLogger.Log.ShouldContain(s => s.Contains("100") && s.Contains("GetAsync") && !s.Contains("NULL"));
            DebugLogger.ClearLogData();
        }
    }
}

