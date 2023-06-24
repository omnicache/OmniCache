using System;
using OmniCache;
using OmniCache.EntityFramework;
using OmniCache.IntegrationTests.Model;

namespace OmniCache.IntegrationTests.Seed
{
	public class StoreStockSeed
	{		
        protected CachedDatabase cachedDB;

        public StoreStockSeed(CachedDatabase cachedDb)
        {
            cachedDB = cachedDb;
        }

        public void Seed()
        {
            StoreStock[] stocks =
            {
                new StoreStock { Id = 1, StoreCode="1", MovieId = 100, TotalEarnings = 200, CopiesInStore = 21, TotalRentals = 240, DailyRentPrice = 3, OverdueFees = 0},
                new StoreStock { Id = 2, StoreCode="2", MovieId = 101, TotalEarnings = 220, CopiesInStore = 100, TotalRentals = 503, DailyRentPrice = 3, OverdueFees = 0},
                new StoreStock { Id = 3, StoreCode="3", MovieId = 102, TotalEarnings = 420, CopiesInStore = 41, TotalRentals = 1300, DailyRentPrice = 3, OverdueFees = 0},
                new StoreStock { Id = 4, StoreCode="4", MovieId = 103, TotalEarnings = 3020, CopiesInStore = 53, TotalRentals = 6550, DailyRentPrice = 3, OverdueFees = 0},
                new StoreStock { Id = 5, StoreCode="5", MovieId = 104, TotalEarnings = 120, CopiesInStore = 92, TotalRentals = 2520, DailyRentPrice = 4, OverdueFees = 0},
                new StoreStock { Id = 6, StoreCode="6", MovieId = 105, TotalEarnings = 820, CopiesInStore = 94, TotalRentals = 8350, DailyRentPrice = 4, OverdueFees = 0},
                new StoreStock { Id = 7, StoreCode="7", MovieId = 106, TotalEarnings = 220, CopiesInStore = 5, TotalRentals = 650, DailyRentPrice = 4, OverdueFees = 0},
                new StoreStock { Id = 8, StoreCode="8", MovieId = 107, TotalEarnings = 720, CopiesInStore = 79, TotalRentals = 450, DailyRentPrice = 3, OverdueFees = 0}                
            };

            cachedDB.DbContext.AddRange(stocks);
            cachedDB.DbContext.SaveChanges();

            foreach (var item in stocks)
            {
                cachedDB.DbContext.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }

            DebugLogger.ClearLogData();
        }
    }
}

