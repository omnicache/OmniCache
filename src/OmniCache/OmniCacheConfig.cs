using System;
namespace OmniCache
{
	
	public class OmniCacheConfig
	{
		public CacheProviderType CacheProvider { get; set; } = CacheProviderType.LocalMemory;

        public OmniCacheConfig()
		{
		}

		
	}
}

