using System;
using Xunit;

namespace OmniCache.IntegrationTests.Core
{
    [CollectionDefinition("Tests")]
    public class DatabaseCollection : ICollectionFixture<TestsFixture>
    {
    }

    public class TestsFixture : IDisposable
    {
		public TestsFixture()
		{
            //code to run once before all tests
		}

        public void Dispose()
        {
            //code to run once after all tests
        }
    }
}

