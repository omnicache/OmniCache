using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Runtime.InteropServices;
using OmniCache.Utils;

namespace OmniCache.CacheProvider
{
	public class LocalCacheProvider: ICacheProvider
    {
        public const string CACHENAME = "DataStore";
        private static MemoryCache cache = new MemoryCache(CACHENAME);        

        public LocalCacheProvider()
		{
		}

        public async Task ClearAllAsync()
        {
            await Task.CompletedTask;
        }

        public async Task<CacheItem<T>> GetAsync<T>(string key) where T : class
		{            
            CacheItem<T> item = (CacheItem<T>)cache.Get(key);
            CacheItem<T> cloned = item.Copy();
                            
            DebugLogger.Debug(DebugLogSource.LocalCache, "GetAsync", key, cloned);
            
            return cloned;
        }

        public async Task<CacheItem<T>> GetHashAsync<T>(string key) where T : class
        {
            string[] parts = key.Split(OmniCacheConstants.KEY_HASH_DELIM);

            if (parts.Length != 2)
            {
                throw new Exception("Expecting two parts for key:" + key);
            }

            string hashName = parts[0];
            string hashKey = parts[1];

            CacheItem<T> cloned = null;

            CacheItem<Dictionary<string, CacheItem<T>>> item = (CacheItem<Dictionary<string, CacheItem<T>>>)cache.Get(hashName);
            if (item != null && item.Value != null)
            {

                var hash = item.Value;
                if (hash.ContainsKey(hashKey))
                {
                    var hashItem = hash[hashKey];
                    cloned = hashItem.Copy();
                }

            }

            DebugLogger.Debug(DebugLogSource.LocalCache, "GetHashAsync:" + hashName, hashKey, cloned);

            return cloned;
        }


        public async Task<List<string>> GetAllHashKeysAsync<T>(string key) where T : class
        {
            string[] parts = key.Split(OmniCacheConstants.KEY_HASH_DELIM);

            if (parts.Length != 2)
            {
                throw new Exception("Expecting two parts for key:" + key);
            }

            string hashName = parts[0];
            string hashKey = parts[1];

            CacheItem<Dictionary<string, CacheItem<T>>> item = (CacheItem<Dictionary<string, CacheItem<T>>>)cache.Get(hashName);

            List<string> allKeys=null;

            if (item != null && item.Value != null)
            {
                allKeys = new List<string>(item.Value.Keys);
            }
            
            DebugLogger.Debug(DebugLogSource.LocalCache, "GetAllHashKeysAsync", hashName, allKeys);
            
            return allKeys;
        }

        //For non local, should get in one go
        public async Task<List<CacheItem<T>>> GetAsync<T>(List<string> keys) where T : class //, new()
        {
            List<CacheItem<T>> ret = new List<CacheItem<T>>();

            for (int i = 0; i < keys.Count; i++)
            {
                string key = keys[i];
                CacheItem<T> cloned = null;

               
                if (key != null)
                {
                    CacheItem<T> obj = (CacheItem<T>)cache.Get(key);
                    cloned = obj.Copy();
                }

                ret.Add(cloned);
            }
            
            DebugLogger.Debug(DebugLogSource.LocalCache, "GetAsync(List)", keys, ret);            

            return ret;
        }


        public async Task SetAsync<T>(string key, T obj) where T : class
        {            
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.Priority = CacheItemPriority.Default;
            
            policy.AbsoluteExpiration = DateTimeOffset.MaxValue; //DateTimeOffset.UtcNow.AddHours(1);

            T cloned = GetClonedObject<T>(obj);

            cache.Set(key, new CacheItem<T>(cloned), policy);

            
            DebugLogger.Debug(DebugLogSource.LocalCache, "SetAsync", key, obj);            

            await Task.CompletedTask;
        }

        public async Task SetHashAsync<T>(string key, T obj) where T : class
        {
            string[] parts = key.Split(OmniCacheConstants.KEY_HASH_DELIM);

            if (parts.Length != 2)
            {
                throw new Exception("Expecting two parts for key:" + key);
            }

            string hashName = parts[0];
            string hashKey = parts[1];

            CacheItem<Dictionary<string, CacheItem<T>>> hash = (CacheItem<Dictionary<string, CacheItem<T>>>)cache.Get(hashName);
            if (hash == null || hash.Value == null)
            {
                hash = new CacheItem<Dictionary<string, CacheItem<T>>>();
                hash.Value = new Dictionary<string, CacheItem<T>>();
            }

            T cloned = GetClonedObject<T>(obj);
            var cacheItem = new CacheItem<T>(cloned);
            hash.Value[hashKey] = cacheItem;


            CacheItemPolicy policy = new CacheItemPolicy();
            policy.Priority = CacheItemPriority.Default;

            policy.AbsoluteExpiration = DateTimeOffset.MaxValue; //DateTimeOffset.UtcNow.AddHours(1);

            cache.Set(hashName, hash, policy);
            
            DebugLogger.Debug(DebugLogSource.LocalCache, "SetHashAsync:" + hashName, hashKey, obj);
            
            await Task.CompletedTask;
        }

        public async Task SetAsync<T>(List<string> keys, List<T> objs) where T : class
        {
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.Priority = CacheItemPriority.Default;

            policy.AbsoluteExpiration = DateTimeOffset.MaxValue;// DateTimeOffset.UtcNow.AddHours(1);

            for (int i = 0; i < keys.Count; i++)
            {
                string key = keys[i];
                T obj = objs[i];

                T cloned = GetClonedObject<T>(obj);

                cache.Set(key, new CacheItem<T>(cloned), policy);

            }
            
            DebugLogger.Debug(DebugLogSource.LocalCache, "SetAsync(List)", keys, objs);
            
            await Task.CompletedTask;
        }

        public async Task RemoveItemAsync<T>(string key) where T : class
        {

            cache.Remove(key);

            DebugLogger.Debug(DebugLogSource.LocalCache, "RemoveItem", key);

            await Task.CompletedTask;
        }

        public async Task RemoveHashItemAsync<T>(string key) where T : class
        {
            string[] parts = key.Split(OmniCacheConstants.KEY_HASH_DELIM);
            if (parts.Length != 2)
            {
                throw new Exception($"Hash key error - cannot extract parts from key:{key}");
            }

            string hashName = parts[0];
            string hashKey = parts[1];
            bool removed = false;

            CacheItem<Dictionary<string, CacheItem<T>>> hash = (CacheItem<Dictionary<string, CacheItem<T>>>)cache.Get(hashName);
            if (hash != null && hash.Value != null)
            {
                Dictionary<string, CacheItem<T>> dict = hash.Value;

                if (dict.ContainsKey(hashKey))
                {
                    dict.Remove(hashKey);
                    removed = true;

                    CacheItemPolicy policy = new CacheItemPolicy();
                    policy.Priority = CacheItemPriority.Default;
                    policy.AbsoluteExpiration = DateTimeOffset.MaxValue;
                    cache.Set(hashName, hash, policy);
                }
            }

            DebugLogger.Debug(DebugLogSource.LocalCache, "RemoveHashItem:" + hashName, hashKey, removed ? "REMOVED" : "NOT FOUND");
            await Task.CompletedTask;
        }

        public async Task RemoveHashItemsAsync<T>(List<string> keys) where T : class
        {
            for (int i = 0; i < keys.Count; i++)
            {
                string key = keys[i];

                await RemoveHashItemAsync<T>(key);                
            }
            DebugLogger.Debug(DebugLogSource.LocalCache, "RemoveHashItems(List)", keys, "COUNT:" + keys.Count);
        }

        public async Task RemoveItemsAsync<T>(List<string> keys) where T : class
        {
            for (int i = 0; i < keys.Count; i++)
            {
                string key = keys[i];

                await RemoveItemAsync<T>(key);                
            }

            DebugLogger.Debug(DebugLogSource.LocalCache, "RemoveAsync(List)", keys);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            bool exists = cache.Contains(key);
            
            DebugLogger.Debug(DebugLogSource.LocalCache, "ExistsAsync", key, exists);
            
            return exists;
        }

        
        private static T GetClonedObject<T>(T obj) where T : class
        {
            if (typeof(T) == typeof(object))
            {
                throw new Exception("Cannot pass in object. Must be concrete class");
            }

            return obj.Copy();
        }
    }
}

