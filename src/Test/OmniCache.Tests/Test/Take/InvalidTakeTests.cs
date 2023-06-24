using System;
using OmniCache;
using OmniCache.EntityFramework;
using OmniCache.IntegrationTests.Core;
using OmniCache.IntegrationTests.Model;
using OmniCache.IntegrationTests.Seed;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace OmniCache.IntegrationTests.Test.Take
{	
    [Collection("Tests")]
    public class InvalidTakeTests : BaseTest
    {
        public InvalidTakeTests(ITestOutputHelper output) : base(output)
        {
            new SimpleSeed(cachedDB).Seed();
            CachedDatabase.LoadAllQueries(this.GetType());

        }

        [Fact]
        public async Task TakeWithoutOrderByAsync()
        {

            Should.Throw<Exception>(() =>
            {
                Query<Movie> query1 = new Query<Movie>(
                               movie => movie.AgeRestriction == new QueryParam(1)).Take(2);
            });

        }

    }
}

