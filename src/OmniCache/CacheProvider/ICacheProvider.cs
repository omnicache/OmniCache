using System;
namespace OmniCache.CacheProvider
{
	public interface ICacheProvider
	{
        Task ClearAllAsync();
        
        Task<CacheItem<T>> GetAsync<T>(string key) where T : class;
        Task<List<CacheItem<T>>> GetAsync<T>(List<string> keys) where T : class;
        Task<CacheItem<T>> GetHashAsync<T>(string key) where T : class;
        Task<List<string>> GetAllHashKeysAsync<T>(string key) where T : class;

        Task SetAsync<T>(string key, T obj) where T : class;
        Task SetAsync<T>(List<string> keys, List<T> objs) where T : class;
        Task SetHashAsync<T>(string key, T obj) where T : class;

        Task RemoveItemAsync<T>(string key) where T : class;
        Task RemoveItemsAsync<T>(List<string> keys) where T : class;
        Task RemoveHashItemAsync<T>(string key) where T : class;
        Task RemoveHashItemsAsync<T>(List<string> keys) where T : class;
        
        Task<bool> ExistsAsync(string key);
        
    }
}

