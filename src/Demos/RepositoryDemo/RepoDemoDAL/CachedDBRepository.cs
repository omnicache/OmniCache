using System;
using System.Linq.Expressions;
using RepoDemo.Data.Model;
using OmniCache;
using OmniCache.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using OmniCache.EntityFramework;
using System.Reflection;

namespace RepoDemo.Data
{
	public class CachedDBRepository<T>: ICachedDBRepository<T> where T : class, new()
    {
        protected CachedDatabase cachedDB;

        public CachedDBRepository(ApplicationDbContext dbContext)
        {
            cachedDB = new CachedDatabase(dbContext);

        }

        public async Task<T> GetByKeyAsync(object key) 
        {
            return await cachedDB.GetByKeyAsync<T>(key);
        }
        
        public async Task<T> GetAsync(Query<T> query, params object[] queryParams) 
        {
            return await cachedDB.GetAsync<T>(query, queryParams);
        }

        public async Task<List<T>> GetMultipleAsync(Query<T> query, params object[] queryParams) 
        {
            return await cachedDB.GetMultipleAsync<T>(query, queryParams);
        }

        public async Task AddAsync(T obj) 
        {
            await cachedDB.AddAsync<T>(obj);
        }

        public async Task UpdateAsync(T obj) 
        {
            await cachedDB.UpdateAsync(obj);
        }

        public async Task DeleteAsync(T obj)
        {
            await cachedDB.DeleteAsync(obj);
        }

    }
}

