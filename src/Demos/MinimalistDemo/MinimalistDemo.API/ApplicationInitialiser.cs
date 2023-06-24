using System;
using OmniCache.EntityFramework;

namespace MinimalistDemo.API
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

