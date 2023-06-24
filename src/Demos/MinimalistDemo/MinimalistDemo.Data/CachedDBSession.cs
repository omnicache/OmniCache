using System;
using OmniCache;
using OmniCache.EntityFramework;

namespace MinimalistDemo.Data
{
	public class CachedDBSession: ICachedDBSession
    {		
        protected CachedDatabase cachedDB;

        public CachedDBSession(ApplicationDbContext dbContext)
        {
            cachedDB = new CachedDatabase(dbContext);
        }


        public async Task<T> GetByKeyAsync<T>(object key) where T : class, new()
        {
            return await cachedDB.GetByKeyAsync<T>(key);
        }

        public async Task<T> GetAsync<T>(Query<T> query, params object[] queryParams) where T : class, new()
        {
            return await cachedDB.GetAsync<T>(query, queryParams);
        }

        public async Task<List<T>> GetMultipleAsync<T>(Query<T> query, params object[] queryParams) where T : class, new()
        {
            return await cachedDB.GetMultipleAsync<T>(query, queryParams);
        }

        public async Task AddAsync<T>(T obj) where T : class, new()
        {
            await cachedDB.AddAsync<T>(obj);
        }

        public async Task UpdateAsync<T>(T obj) where T : class, new()
        {
            await cachedDB.UpdateAsync(obj);
        }

        public async Task DeleteAsync<T>(T obj) where T : class, new()
        {
            await cachedDB.DeleteAsync(obj);
        }
    }
}

