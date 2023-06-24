using System;
using OmniCache;
using OmniCache.EntityFramework;
using OmniCache.IntegrationTests.Core;
using OmniCache.IntegrationTests.Model;

namespace OmniCache.IntegrationTests.Seed
{
	public class SimpleSeed
	{
        protected CachedDatabase cachedDB;

        public SimpleSeed(CachedDatabase cachedDb)
		{
            cachedDB = cachedDb;
        }

        public void Seed()
        {
            Movie[] movies =
            {
                new Movie { Id = 100, Name = "Jaws", ReleaseDate = new DateTime(1976, 1, 27), IsNewRelease = false, Category = Category.Thriller, Active = 'N', AgeRestriction = 18 },
                new Movie { Id = 101, Name = "Jaws 2", ReleaseDate = new DateTime(1978, 2, 7), IsNewRelease = false, Category = Category.Thriller, Active = 'A', AgeRestriction = 18 },
                new Movie { Id = 102, Name = "Matrix", ReleaseDate = new DateTime(2000, 11, 17), IsNewRelease = true, Category = Category.SciFi, Active = 'A', AgeRestriction = 18 }                
            };

            RentalStore[] stores =
            {                
                new RentalStore { StoreCode = "STORE1", Name = "Store 1", Address = "Store Address 1", State = State.VIC },
                new RentalStore { StoreCode = "STORE2", Name = "Store 2", Address = "Store Address 2", State = State.VIC },
                new RentalStore { StoreCode = "STORE3", Name = "Store 3", Address = null, State = null }            
            };

            StoreStock[] stocks =
            {                
                new StoreStock { Id = 1, StoreCode = stores[0].StoreCode, MovieId = movies[0].Id, TotalEarnings = 1000.0f, CopiesInStore = 10, TotalRentals = 50, DailyRentPrice = 2.99m, OverdueFees = 20.0 },
                new StoreStock { Id = 2, StoreCode = stores[0].StoreCode, MovieId = movies[1].Id, TotalEarnings = 1500.0f, CopiesInStore = 8, TotalRentals = 40, DailyRentPrice = 3.49m, OverdueFees = 15.0},
                new StoreStock { Id = 3, StoreCode = stores[0].StoreCode, MovieId = movies[2].Id, TotalEarnings = 2000.0f, CopiesInStore = 12, TotalRentals = 60, DailyRentPrice = 2.99m, OverdueFees = 25.0},
                new StoreStock { Id = 4, StoreCode = stores[1].StoreCode, MovieId = movies[0].Id, TotalEarnings = 1200.0f, CopiesInStore = 6, TotalRentals = 30, DailyRentPrice = 2.49m, OverdueFees = 18.0},
                new StoreStock { Id = 5, StoreCode = stores[1].StoreCode, MovieId = movies[1].Id, TotalEarnings = 1800.0f, CopiesInStore = 9, TotalRentals = 45, DailyRentPrice = 3.99m, OverdueFees = 22.0}                
            };


            List<object> all = new List<object>();
            all.AddRange(movies);
            all.AddRange(stores);
            all.AddRange(stocks);

            cachedDB.DbContext.AddRange(all);
            cachedDB.DbContext.SaveChanges();

            foreach(var item in all)
            {
                cachedDB.DbContext.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }

            DebugLogger.ClearLogData();
        }
    }
}

