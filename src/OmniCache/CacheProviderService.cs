using System;
using OmniCache.CacheProvider;
using StackExchange.Redis;

namespace OmniCache
{
	public class CacheProviderService
	{
        private ICacheProvider cacheProvider;

        public CacheProviderService()
		{
            CacheProviderType cacheProviderType = CacheProviderType.LocalMemory;

            OmniCacheConfig omniConfig = ConfigStorage.GetConfig<OmniCacheConfig>();
            if(omniConfig!=null)
            {
                cacheProviderType = omniConfig.CacheProvider;
            }

            if(cacheProviderType==CacheProviderType.LocalMemory)
            {
                cacheProvider = new LocalCacheProvider();
            }
            else
            {
                cacheProvider = new RedisCacheProvider();
            }
            
        }

        public async Task ClearAllASync()
        {
            await cacheProvider.ClearAllAsync();
        }

        public async Task<CacheItem<T>> GetAsync<T>(string key) where T : class
        {
            if (key.Contains(OmniCacheConstants.KEY_HASH_DELIM))
            {
                return await cacheProvider.GetHashAsync<T>(key);
            }
            else
            {
                return await cacheProvider.GetAsync<T>(key);
            }
            
        }

        public async Task<List<CacheItem<T>>> GetAsync<T>(List<string> keys) where T : class
        {
            return await cacheProvider.GetAsync<T>(keys);
        }

        public async Task<List<string>> GetAllHashKeysAsync<T>(string key) where T : class
        {
            return await cacheProvider.GetAllHashKeysAsync<T>(key);
        }

        public async Task SetAsync<T>(string key, T obj) where T : class
        {
            if (key.Contains(OmniCacheConstants.KEY_HASH_DELIM))
            {
                await cacheProvider.SetHashAsync<T>(key, obj);
                return;
            }
            else
            {
                await cacheProvider.SetAsync<T>(key, obj);
            }
            
        }

        public async Task SetAsync<T>(List<string> keys, List<T> objs) where T : class
        {
            await cacheProvider.SetAsync<T>(keys, objs);
        }

        public async Task RemoveAsync<T>(string key) where T : class
        {
            if (key.Contains(OmniCacheConstants.KEY_HASH_DELIM))
            {
                await cacheProvider.RemoveHashItemAsync<T>(key);
            }
            else
            {
                await cacheProvider.RemoveItemAsync<T>(key);
            }

        }

        public async Task RemoveAsync<T>(List<string> keys) where T : class
        {
            if (!keys.Any(k => k.Contains(OmniCacheConstants.KEY_HASH_DELIM)))
            {
                await cacheProvider.RemoveItemsAsync<T>(keys);
                return;
            }
            

            List<string> hashKeys = keys.Where(k => k.Contains(OmniCacheConstants.KEY_HASH_DELIM)).ToList();
            List<string> nonHashKeys = keys.Where(k => !k.Contains(OmniCacheConstants.KEY_HASH_DELIM)).ToList();

            await cacheProvider.RemoveHashItemsAsync<T>(hashKeys);
            await cacheProvider.RemoveItemsAsync<T>(nonHashKeys);


        }

        public async Task<bool> ExistsAsync(string key)
        {
            return await cacheProvider.ExistsAsync(key);
        }

    }
}

