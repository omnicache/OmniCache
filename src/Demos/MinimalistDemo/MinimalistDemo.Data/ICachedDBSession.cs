using System;
using OmniCache;

namespace MinimalistDemo.Data
{
	public interface ICachedDBSession
	{
        Task<T> GetByKeyAsync<T>(object key) where T : class, new();

        Task<T> GetAsync<T>(Query<T> query, params object[] queryParams) where T : class, new();

        Task<List<T>> GetMultipleAsync<T>(Query<T> query, params object[] queryParams) where T : class, new();

        Task AddAsync<T>(T obj) where T : class, new();

        Task UpdateAsync<T>(T obj) where T : class, new();

        Task DeleteAsync<T>(T obj) where T : class, new();
    }
}

