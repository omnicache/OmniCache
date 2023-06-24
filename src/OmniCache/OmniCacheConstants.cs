using System;
namespace OmniCache
{
	
	public static class OmniCacheConstants
	{
		public const string KEY_HASH_DELIM = "<|>";
		public const string KEY_PARAM_DELIM = ":::";

		public const string PARAM_PROPNAME_CONSTANT = "_CONSTANT_";
		
	}

    public enum CacheProviderType
	{
		LocalMemory,
		Redis
	}
}

