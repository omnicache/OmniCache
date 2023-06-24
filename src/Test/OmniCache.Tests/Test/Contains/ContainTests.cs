using System;
using OmniCache.Extension;
using OmniCache.IntegrationTests.Model;
using OmniCache.IntegrationTests.Seed;
using System.Reflection;
using OmniCache.IntegrationTests.Core;
using Shouldly;
using Xunit;
using Xunit.Abstractions;
using System.Linq;
using OmniCache;
using OmniCache.EntityFramework;

namespace OmniCache.IntegrationTests.Test.Contains
{
    [Collection("Tests")]
    public class ContainTests : BaseTest
    {
		
        public ContainTests(ITestOutputHelper output) : base(output)
        {
            new SimpleSeed(cachedDB).Seed();            
            CachedDatabase.LoadAllQueries(this.GetType());
        }

        
        public static Query<Movie> movieIdQuery = new Query<Movie>(
                               movie => new QueryParam(1).Contains(movie.Id));

        
        public static Query<Movie> categoryQuery = new Query<Movie>(
                               movie => new QueryParam(1).Contains(movie.Category));
        
        public static Query<Movie> newReleaseQuery = new Query<Movie>(
                               movie => new QueryParam(1).Contains(movie.IsNewRelease));
        
        public static Query<Movie> categoryNoParamQuery = new Query<Movie>(
                               movie => new List<Category>() { Category.SciFi, Category.Documentary}.Contains(movie.Category));
        
        public static Query<Movie> constantQuery = new Query<Movie>(
                               movie => new QueryParam(1).Contains('A'));
        
        public static Query<Movie> constantListQuery = new Query<Movie>(
                               movie => new List<Category>() { Category.SciFi, Category.Documentary }.Contains((Category)(int)new QueryParam(1)));
        
        public static Query<Movie> twoQueryParamsQuery = new Query<Movie>(
                               movie => new QueryParam(1).Contains((Category)(int)new QueryParam(2)));
        

        
        [Fact]
        public async Task ContainsLongAsync()
        {
            var movieIDs = new List<long>() { 100, 101 };
                        
            List<Movie> movies = await cachedDB.GetMultipleAsync(movieIdQuery, movieIDs);

            movies.Count().ShouldBe(2);
        }
        
        [Fact]
        public async Task ContainsEnumAsync()
        {
            var categories = new List<Category>() { Category.Thriller, Category.Action };

            List<Movie> movies = await cachedDB.GetMultipleAsync(categoryQuery, categories);

            movies.Count().ShouldBe(2);
        }
        
        [Fact]
        public async Task ContainsBoolAsync()
        {
            var bools = new List<bool>() { true };

            List<Movie> movies = await cachedDB.GetMultipleAsync(newReleaseQuery, bools);

            movies.Count().ShouldBe(1);
        }

        [Fact]
        public async Task NoParamsContainsCategoryAsync()
        {            
            List<Movie> movies = await cachedDB.GetMultipleAsync(categoryNoParamQuery);

            movies.Count().ShouldBe(1);
        }
        
        [Fact]
        public async Task ContainsConstantAsync()
        {

            var activeList = new List<char>() { 'A', 'B' };
           
            List<Movie> movies = await cachedDB.GetMultipleAsync(constantQuery, activeList);

            movies.Count().ShouldBe(3);
        }
        
        [Fact]
        public async Task ConstantListContainsAsync()
        {

            List<Movie> movies = await cachedDB.GetMultipleAsync(constantListQuery, Category.SciFi);

            movies.Count().ShouldBe(3);
        }
        
        [Fact]
        public async Task QueryParamContainsQueryParamAsync()
        {
            var categories = new List<Category>() { Category.SciFi, Category.Action };

            List<Movie> movies = await cachedDB.GetMultipleAsync(twoQueryParamsQuery, categories, Category.SciFi);

            movies.Count().ShouldBe(3);
        }
        
    }
}

