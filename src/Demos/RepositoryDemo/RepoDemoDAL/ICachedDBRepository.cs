using System;
using System.Linq.Expressions;
using OmniCache;

namespace RepoDemo.Data
{
    public interface ICachedDBRepository<T> where T : class
    {

        Task<T> GetByKeyAsync(object key);
        Task<T> GetAsync(Query<T> query, params object[] queryParams);
        Task<List<T>> GetMultipleAsync(Query<T> query, params object[] queryParams);
        Task AddAsync(T obj);
        Task UpdateAsync(T obj);
        Task DeleteAsync(T obj);
    }
}

