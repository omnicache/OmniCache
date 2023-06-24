using System;
using System.ComponentModel.DataAnnotations;
using OmniCache;

namespace OmniCache.IntegrationTests.Model
{
    public enum State
    {
        VIC,
        NSW,
        QLD,
        ACT,
        NT,
        SA,
        TAS,
        WA
    }

    [Cacheable]
    public class RentalStore
	{
		[Key]
		public string StoreCode { get; set; }
        public string Name { get; set; }
        public string? Address { get; set; }
		public State? State { get; set; }
		
		public RentalStore()
		{
		}

    }
}

