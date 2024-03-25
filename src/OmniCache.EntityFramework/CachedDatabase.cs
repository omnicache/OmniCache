using System;
using System.Data;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using OmniCache;
using OmniCache.CacheProvider;
using OmniCache.Reflect;
using OmniCache.Utils;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;
using System.Collections.Generic;

namespace OmniCache.EntityFramework
{
	public class CachedDatabase
	{		
        protected CacheStore cacheStore = new CacheStore();
        public DbContext DbContext { get; set; }

        public CachedDatabase(DbContext dbContext)
        {

            DbContext = dbContext;
            DbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            
        }

        public static void SetConfig<T>(T config) where T : class
        {
            ConfigStorage.SetConfig<T>(config);
        }

        public async Task ClearCacheAsync()
        {
            await cacheStore.ClearCacheAsync();
        }

        public static void ClearAllQueries()
        {
            CacheStore.ClearAllQueries();
        }

        public static void LoadAllQueries()
        {
            CacheStore.LoadAllQueries();
        }

        public static void LoadAllQueries(Assembly assembly)
        {
            CacheStore.LoadAllQueries(assembly);
        }

        public static void LoadAllQueries(Type type)
        {
            CacheStore.LoadAllQueries(type);
        }

        public async Task<T> GetByKeyAsync<T>(object key, bool addToCache = true) where T : class, new()
        {
            CacheItem<T> cachedItem = await cacheStore.GetByKeyAsync<T>(key);
            if (cachedItem != null)
            {
                return cachedItem.Value;
            }

            key = ReflectionUtils.ConvertKeyFieldType<T>(key);
            
            T val = await DbContext.Set<T>().FindAsync(key);            

            if (addToCache)
            {
                await cacheStore.SetAsync<T>(val);
            }

            return val;
        }

        private object[] fixQueryParams(object[] queryParams)
        {
            if (queryParams == null)
            {
                return new object[] { null };
            }

            if (queryParams.Length == 0)
            {
                return null;
            }
            return queryParams;
        }

        public async Task<T> GetAsync<T>(Query<T> query, params object[] queryParams) where T : class, new()
        {

            if (query._Take == 1)
            {
                List<T> takeList = await GetMultipleAsync<T>(query, queryParams);
                if(takeList==null || takeList.Count==0)
                {
                    return null;
                }
                return takeList[0];
            }
            else if(query._Take > 1)
            {
                throw new Exception("Use GetMultipleAsync for Take greater than 1");
            }

            queryParams = fixQueryParams(queryParams);


            CacheItem<T> cachedItem = await cacheStore.GetAsync<T>(query, queryParams);

            if (cachedItem != null)
            {
                return cachedItem.Value;
            }

            T val = null;
            List<T> list = await GenerateDBQuerable(query, queryParams).ToListAsync();

            if (list.Count > 1)
            {
                throw new Exception($"Only expecing on item for query {query._QueryName}, but got {list.Count}. Params:{(queryParams==null?"":string.Join(",", queryParams))}");
            }
            if(list.Count==1)
            {
                val = list[0];
            }

            await cacheStore.SetAsync<T>(query, queryParams, val);
            
            return val;
        }


        public async Task<List<T>> GetMultipleAsync<T>(Query<T> query, params object[] queryParams) where T : class, new()
        {
            queryParams = fixQueryParams(queryParams);

            CacheItem<List<T>> cachedItems = await cacheStore.GetMultipleAsync(query, queryParams);

            if (cachedItems != null)
            {
                List<T> cachedVal = cachedItems.Value;
                UpdateListTaken(query, cachedVal);
                return cachedVal;
            }

            var generatedQuery = GenerateDBQuerable(query, queryParams);
            List<T> val = await generatedQuery.ToListAsync();

            await cacheStore.SetMultipleAsync<T>(query, queryParams, val);

            UpdateListTaken(query, val);
            return val;
        }

        public async Task AddAsync<T>(T obj, bool saveChanges = true) where T : class, new()
        {
            await DbContext.Set<T>().AddAsync(obj);
            
            await cacheStore.UpdateAsync(null, obj);

            if (saveChanges)
            {
                await DbContext.SaveChangesAsync();
            }

            DbContext.Entry(obj).State = EntityState.Detached;
        }

        public async Task UpdateAsync<T>(T obj, bool saveChanges = true) where T : class, new()
        {
            object key = cacheStore.GetKeyFromObject<T>(obj);

            T previous = await GetByKeyAsync<T>(key, false);            

            DbContext.Set<T>().Update(obj);
            
            await cacheStore.UpdateAsync(previous, obj);

            if (saveChanges)
            {
                await DbContext.SaveChangesAsync();             
            }

            DbContext.Entry(obj).State = EntityState.Detached;
        }


        protected IQueryable<T> GenerateDBQuerable<T>(Query<T> query, params object[] queryParams) where T : class, new()
        {
            var expressionToRun = query.Generate(queryParams);

            var queryable = DbContext.Set<T>().AsNoTracking();

            if(expressionToRun!=null)
            {
                queryable = queryable.Where(expressionToRun);
            }
            
            if (query._OrderBy != null)
            {
                if (query._OrderByDesc)
                {
                    queryable = queryable.OrderByDescending(query._OrderBy);
                }
                else
                {
                    queryable = queryable.OrderBy(query._OrderBy);
                }
            }
            if (query._Take > 0)
            {
                queryable = queryable.Take(query._Take + 1);        //get an extra one so we can check for invalidation
            }
            return queryable;
        }

        protected void UpdateListTaken<T>(Query<T> query, List<T> list) where T : class, new()
        {
            if (query._Take > 0)
            {
                int count = list.Count;
                if(count > query._Take)
                {
                    list.RemoveAt(list.Count - 1);
                }
                
            }         
        }

        public async Task DeleteAsync<T>(T obj) where T : class, new()
        {            
            DbContext.Set<T>().Remove(obj);

            await cacheStore.RemoveAsync(obj);

            await DbContext.SaveChangesAsync();

        }

    }
}

