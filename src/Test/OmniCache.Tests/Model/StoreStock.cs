using System;
using OmniCache;

namespace OmniCache.IntegrationTests.Model
{
    [Cacheable]
    public class StoreStock
	{
        public long Id { get; set; }

        public string StoreCode { get; set; }
        public long MovieId { get; set; }

        public float TotalEarnings { get; set; }
        public int? CopiesInStore { get; set; }
        public int TotalRentals { get; set; }
        public decimal DailyRentPrice { get; set; }
        public double OverdueFees { get; set; }


        public StoreStock()
		{
		}
	}
}

