using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.Caching;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using OmniCache.Utils;
using StackExchange.Redis;

namespace OmniCache.CacheProvider
{
    
	public class RedisCacheProvider: ICacheProvider
    {
        private ConnectionMultiplexer redis;

        public RedisCacheProvider()
		{

            ConnectionMultiplexer.SetFeatureFlag("preventthreadtheft", true);

            ConfigurationOptions redisConfig = ConfigStorage.GetConfig<ConfigurationOptions>();

            if(redisConfig==null)
            {
                throw new Exception("Redis config not provided");
            }

            redis = ConnectionMultiplexer.Connect(redisConfig);
        }


        //only for local machine testing
        public async Task ClearAllAsync()
        {
            await Task.CompletedTask;
            
            IDatabase db = redis.GetDatabase();
            var server = redis.GetServer(redis.GetEndPoints().First());            

            var keys = server.Keys();
            foreach (var key in keys)
            {                
                db.KeyDelete(key);
            }
            
        }

        public async Task<CacheItem<T>> GetAsync<T>(string key) where T : class
        {
            IDatabase db = redis.GetDatabase();

            string value = await db.StringGetAsync(key);
            CacheItem<T> cacheObj = JsonUtils.FromJsonString<CacheItem<T>>(value);


            DebugLogger.Debug(DebugLogSource.Redis, "GetAsync", key, cacheObj);// (cacheObj == null ? "not in cache" : value));


            return cacheObj;
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

            IDatabase db = redis.GetDatabase();
            string item = await db.HashGetAsync(hashName, hashKey);

            CacheItem<T> cacheObj = JsonUtils.FromJsonString<CacheItem<T>>(item);

            DebugLogger.Debug(DebugLogSource.Redis, "GetHashAsync:" + hashName, hashKey, cacheObj);

            return cacheObj;
        }

        public async Task<List<CacheItem<T>>> GetAsync<T>(List<string> keys) where T : class
        {
            IDatabase db = redis.GetDatabase();

            int[] mapping = new int[keys.Count];

            List<RedisKey> redisKeys = new List<RedisKey>();
            for (int i = 0; i < keys.Count; i++)
            {
                string key = keys[i];
                if (key != null)
                {
                    mapping[i] = redisKeys.Count;

                    RedisKey rk = key;
                    redisKeys.Add(rk);

                }
                else
                {
                    mapping[i] = -1;
                }
            }

            RedisValue[] redisVals = await db.StringGetAsync(redisKeys.ToArray());

            List<CacheItem<T>> ret = new List<CacheItem<T>>();

            for (int i = 0; i < keys.Count; i++)
            {
                CacheItem<T> cacheObj = null;
                string value = null;

                int index = mapping[i];
                if (index >= 0)
                {
                    value = redisVals[index];

                    cacheObj = JsonUtils.FromJsonString<CacheItem<T>>(value);
                }
                ret.Add(cacheObj);

            }

            DebugLogger.Debug(DebugLogSource.Redis, "GetAsync(List)", keys, ret);

            return ret;
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


            IDatabase db = redis.GetDatabase();

            List<string> ret = new List<string>();

            var items = await db.HashKeysAsync(hashName);
            if(items != null)
            {
                for(int i=0;i< items.Length;i++)
                {
                    string item = items[i];
                    ret.Add(item);
                }
            }            

            DebugLogger.Debug(DebugLogSource.Redis, "GetAllHashKeysAsync", hashKey, ret);
            return ret;
        }

        public async Task SetAsync<T>(string key, T obj) where T : class
        {
            IDatabase db = redis.GetDatabase();


            CacheItem<T> cacheObj = new CacheItem<T>(obj);
            string json = JsonUtils.ToJsonString(cacheObj);


            await db.StringSetAsync(key, json);
            

            DebugLogger.Debug(DebugLogSource.Redis, "SetAsync", key, json);
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

            IDatabase db = redis.GetDatabase();

            CacheItem<T> cacheObj = new CacheItem<T>(obj);
            string json = JsonUtils.ToJsonString(cacheObj);


            await db.HashSetAsync(hashName, hashKey, json);

            DebugLogger.Debug(DebugLogSource.Redis, "SetHashAsync:" + hashName, hashKey, obj);
        }

        public async Task SetAsync<T>(List<string> keys, List<T> objs) where T : class
        {
            IDatabase db = redis.GetDatabase();

            var batch = new List<KeyValuePair<RedisKey, RedisValue>>();

            for (int i = 0; i < keys.Count; i++)
            {
                string key = keys[i];
                T obj = objs[i];

                CacheItem<T> cacheObj = new CacheItem<T>(obj);
                string json = JsonUtils.ToJsonString(cacheObj);

                batch.Add(new KeyValuePair<RedisKey, RedisValue>(key, json));
 
            }

            DebugLogger.Debug(DebugLogSource.Redis, "SetAsync(List)", keys, batch);

            await db.StringSetAsync(batch.ToArray());

        }

        public async Task RemoveItemAsync<T>(string key) where T : class
        {
            IDatabase db = redis.GetDatabase();

            
            await db.KeyDeleteAsync(key);

            DebugLogger.Debug(DebugLogSource.Redis, "RemoveItem", key);
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


            IDatabase db = redis.GetDatabase();

            bool removed = await db.HashDeleteAsync(hashName, hashKey);
            DebugLogger.Debug(DebugLogSource.Redis, "RemoveHashItem:" + hashName, hashKey, removed ? "REMOVED" : "NOT FOUND");

        }

        public async Task RemoveHashItemsAsync<T>(List<string> keys) where T : class
        {
            Dictionary<string, List<string>> all = new Dictionary<string, List<string>>();

            for (int i = 0; i < keys.Count; i++)
            {
                string key = keys[i];

                string[] parts = key.Split(OmniCacheConstants.KEY_HASH_DELIM);
                if (parts.Length != 2)
                {
                    throw new Exception($"Hash key error - cannot extract parts from key:{key}");
                }

                string hashName = parts[0];
                string hashKey = parts[1];

                List<string> hashKeys;

                if(all.ContainsKey(hashName))
                {
                    hashKeys = all[hashName];
                }
                else
                {
                    hashKeys = new List<string>();
                }

                if(!hashKeys.Contains(hashKey))
                {
                    hashKeys.Add(hashKey);
                }
                all[hashName] = hashKeys;
            }

            IDatabase db = redis.GetDatabase();

            foreach (KeyValuePair<string, List<string>> kvp in all)
            {
                string hashName = kvp.Key;
                List<string> hashKeys = kvp.Value;

                List<RedisValue> redisKeys = new List<RedisValue>();
                for (int i = 0; i < hashKeys.Count; i++)
                {
                    string hashKey = hashKeys[i];
                    RedisValue rv = hashKey;
                    redisKeys.Add(rv);
                    DebugLogger.Debug(DebugLogSource.Redis, "RemoveHashItem:" + hashName, hashKey);
                }

                long removedCount = await db.HashDeleteAsync(hashName, redisKeys.ToArray());

                DebugLogger.Debug(DebugLogSource.Redis, "RemoveHashItems(List)", keys, "COUNT:" + removedCount);
                bool removed = removedCount == hashKeys.Count;
                for (int i = 0; i < hashKeys.Count; i++)
                {
                    string hashKey = hashKeys[i];                 
                    DebugLogger.Debug(DebugLogSource.Redis, "RemoveHashItem:" + hashName, hashKey, removed ? "REMOVED" : "NOT FOUND?");
                }
            }
            
        }

        public async Task RemoveItemsAsync<T>(List<string> keys) where T : class
        {
            IDatabase db = redis.GetDatabase();

            List<RedisKey> redisKeys = new List<RedisKey>();
            for (int i = 0; i < keys.Count; i++)
            {
                RedisKey rk = keys[i];
                redisKeys.Add(rk);
            }

            DebugLogger.Debug(DebugLogSource.Redis, "RemoveAsync(List)", keys);


            await db.KeyDeleteAsync(redisKeys.ToArray());
        }

        public async Task<bool> ExistsAsync(string key)
        {
            IDatabase db = redis.GetDatabase();
            bool exists = await db.KeyExistsAsync(key);


            DebugLogger.Debug(DebugLogSource.Redis, "ExistsAsync", key, exists);

            return exists;
        }
    }
}

