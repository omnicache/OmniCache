using System;
using OmniCache.EntityFramework;
using OmniCache.IntegrationTests.Model;

namespace OmniCache.IntegrationTests.Seed
{
	public class UnseedAll
	{		
        protected CachedDatabase cachedDB;

        public UnseedAll(CachedDatabase cachedDb)
        {
            cachedDB = cachedDb;
        }

        public void Run()
        {

            cachedDB.DbContext.Set<Movie>().RemoveRange(cachedDB.DbContext.Set<Movie>());
            cachedDB.DbContext.Set<RentalStore>().RemoveRange(cachedDB.DbContext.Set<RentalStore>());
            cachedDB.DbContext.Set<StoreStock>().RemoveRange(cachedDB.DbContext.Set<StoreStock>());

            cachedDB.DbContext.SaveChanges();

        }
    }
}

