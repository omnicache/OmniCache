using System;
using OmniCache.EntityFramework;

namespace RepoDemo.Api
{
	public class ApplicationInitialiser
	{
        private readonly DatabaseSeeder _seeder;

        public ApplicationInitialiser(DatabaseSeeder seeder)
		{
            _seeder = seeder;
        }

        public async Task InitialiseAsync()
        {

            CachedDatabase.LoadAllQueries();

            await _seeder.SeedAsync();

        }
    }
}

