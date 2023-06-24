using System;
using System.Collections.Concurrent;
using System.Reflection;
using OmniCache.CacheProvider;
using OmniCache.KeyProviders;
using OmniCache.Reflect;
using OmniCache.Utils;

namespace OmniCache
{
	public class CacheStore
	{
        public static bool CacheOn = true;
        
        private CacheProviderService cacheProviderService;
        private KeyProvider keyProvider;
        private InvalidKeyProvider invalidKeyProvider;

        public CacheStore()
		{
            cacheProviderService = new CacheProviderService();
            keyProvider = new KeyProvider();
            invalidKeyProvider = new InvalidKeyProvider(keyProvider, cacheProviderService);
        }

        public static void SetConfig<T>(T config) where T : class
        {
            ConfigStorage.SetConfig<T>(config);
        }

        public async Task ClearCacheAsync()
        {
            await cacheProviderService.ClearAllASync();
        }

        public static void ClearAllQueries()
        {
            ReflectStorage.ClearAllQueries();
        }

        public static void LoadAllQueries()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            Assembly thisAssembly = typeof(ReflectStorage).Assembly;

            foreach (Assembly assembly in assemblies)
            {
                if(assembly!=thisAssembly)
                {
                    ReflectStorage.LoadAllQueries(assembly);
                }                
            }
        }

        public static void LoadAllQueries(Assembly assembly)
        {
            ReflectStorage.LoadAllQueries(assembly);
        }

        public static void LoadAllQueries(Type type)
        {
            ReflectStorage.LoadAllQueries(type);
        }

        public object GetKeyFromObject<T>(object obj) where T : class, new()
        {
            ReflectClass? cls = ReflectStorage.GetClass(typeof(T));
            if (cls == null)
            {
                return null;
            }

            return cls.KeyField.GetValue(obj);
        }

        public async Task<CacheItem<T>> GetByKeyAsync<T>(object key) where T : class, new()
        {
            ReflectClass? cls = ReflectStorage.GetClass(typeof(T));
            if (cls == null)
            {
                return null;
            }

            string cacheKey = keyProvider.GetCacheKeyFromKey<T>(key);
            return await cacheProviderService.GetAsync<T>(cacheKey);
        }

       

        public async Task<CacheItem<T>> GetAsync<T>(Query<T> query, params object[] queryParams) where T : class
        {
            ReflectClass? cls = ReflectStorage.GetClass(typeof(T));
            if (cls == null)
            {
                return null;
            }

            string cacheKey = keyProvider.GetCacheKey(query, queryParams);

            CacheItem<string> item = await cacheProviderService.GetAsync<string>(cacheKey);

            if (item == null)
            {
                return null;    //not in cache
            }

            string objectKey = (string)item.Value;
            if (objectKey == null)
            {                
                return new CacheItem<T>(null);
            }

            return await cacheProviderService.GetAsync<T>(objectKey);
        }

        public async Task<CacheItem<List<T>>> GetMultipleAsync<T>(Query<T> query, object[] queryParams) where T : class, new()
        {
            ReflectClass? cls = ReflectStorage.GetClass(typeof(T));
            if (cls == null)
            {
                return null;
            }

            string cacheKey = keyProvider.GetCacheKey(query, queryParams);
            
            CacheItem<List<string>> item = await cacheProviderService.GetAsync<List<string>>(cacheKey);
            if (item == null)
            {
                return null;    //not in cache
            }

            List<string> objectKeys = item.Value;

            if (objectKeys == null)
            {
                return new CacheItem<List<T>>(null);
            }

            List<CacheItem<T>> objs = await cacheProviderService.GetAsync<T>(objectKeys);

            for (int i = 0; i < objs.Count; i++)
            {
                CacheItem<T> obj = objs[i];
                if (obj == null)
                {
                    await cacheProviderService.RemoveAsync<T>(cacheKey);   //invalidate whole query if one is missing.
                    return null;       
                }
            }

            return new CacheItem<List<T>>(objs.Unbox());                
            
        }

        public async Task SetAsync<T>(T obj) where T : class, new()
        {
            ReflectClass? cls = ReflectStorage.GetClass(typeof(T));
            if (cls == null)
            {
                return;
            }

            string cacheKey = keyProvider.GetCacheKey<T>(obj);
            await cacheProviderService.SetAsync<T>(cacheKey, obj);

        }

        public async Task SetAsync<T>(Query<T> query, object[] queryParams, T obj) where T : class, new()
        {
            ReflectClass? cls = ReflectStorage.GetClass(typeof(T));
            if (cls == null)
            {
                return;
            }

            string cacheKey = keyProvider.GetCacheKey(query, queryParams);

            if (obj != null)
            {
                string objKey = keyProvider.GetCacheKey<T>(obj);
                await cacheProviderService.SetAsync<T>(objKey, obj);
                await cacheProviderService.SetAsync<string>(cacheKey, objKey);
            }
            else
            {
                await cacheProviderService.SetAsync<string>(cacheKey, null);
            }
        }


        public async Task SetMultipleAsync<T>(Query<T> query, object[] queryParams, List<T> objs) where T : class, new()
        {
            ReflectClass? cls = ReflectStorage.GetClass(typeof(T));
            if (cls == null)
            {
                return;
            }

            string cacheKey = keyProvider.GetCacheKey(query, queryParams);

            List<string> keys = new List<string>();
            for (int i = 0; i < objs.Count; i++)
            {
                T obj = objs[i];
                string objectKey = keyProvider.GetCacheKey<T>(obj);
                keys.Add(objectKey);                
            }

            await cacheProviderService.SetAsync<T>(keys, objs);
            await cacheProviderService.SetAsync<List<string>>(cacheKey, keys);

        }

        public async Task UpdateAsync<T>(T previousObj, T obj) where T : class, new()
        {
            ReflectClass? cls = ReflectStorage.GetClass(typeof(T));
            if (cls == null)
            {
                return;
            }

            if (obj == null)
            {
                return;
            }

            string cacheKey = keyProvider.GetCacheKey<T>(obj);
            await cacheProviderService.SetAsync<T>(cacheKey, obj);

            List<string> invalidatedKeys = await invalidKeyProvider.GetInvalidatedKeysAsync(previousObj, obj);
            await cacheProviderService.RemoveAsync<List<string>>(invalidatedKeys);
        }

        public async Task RemoveAsync<T>(T obj) where T : class, new()
        {
            ReflectClass? cls = ReflectStorage.GetClass(typeof(T));
            if (cls == null)
            {
                return;
            }

            if (obj == null)
            {
                return;
            }

            string cacheKey = keyProvider.GetCacheKey<T>(obj);
            await cacheProviderService.RemoveAsync<T>(cacheKey);

            List<string> invalidatedKeys = await invalidKeyProvider.GetInvalidatedKeysAsync(obj, null);
            await cacheProviderService.RemoveAsync<List<string>>(invalidatedKeys);

        }
    }
}

