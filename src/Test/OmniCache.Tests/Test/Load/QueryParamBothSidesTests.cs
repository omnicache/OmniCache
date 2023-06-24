using System;
using OmniCache.IntegrationTests.Model;
using System.Reflection;
using OmniCache.IntegrationTests.Core;
using Shouldly;
using Xunit;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Xunit.Abstractions;
using OmniCache.IntegrationTests.Seed;
using OmniCache;
using OmniCache.EntityFramework;

namespace OmniCache.IntegrationTests.Test.Load
{
    [Collection("Tests")]
    public class QueryParamBothSidesTests : BaseTest
    {
		public QueryParamBothSidesTests(ITestOutputHelper output) : base(output)
        {
            new SimpleSeed(cachedDB).Seed();            
        }

        
        [Fact]
        public async Task CompareTwoQueryParamsAsync()
        {

            Should.Throw<Exception>(() =>
            {
                Query<Movie> query2 = new Query<Movie>(
                               movie => (object)new QueryParam(1) == new QueryParam(2));

                query2.Init("query2");
            });

        }
    }
}

