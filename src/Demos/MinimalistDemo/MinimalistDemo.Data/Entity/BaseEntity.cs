using System;
namespace MinimalistDemo.Data.Entity
{
	public abstract class BaseEntity<T> where T : BaseEntity<T>, new()
    {
		public BaseEntity()
		{
		}

        public async Task InsertAsync(ICachedDBSession db) 
        {
            await db.AddAsync((T)this);
        }

        public async Task UpdateAsync(ICachedDBSession db)
        {
            await db.UpdateAsync((T)this);
        }

        public static async Task<T> GetByKeyAsync(ICachedDBSession db, long id)
        {
            return await db.GetByKeyAsync<T>(id);
        }

        public async Task DeleteAsync(ICachedDBSession db)
        {
            await db.DeleteAsync((T)this);
        }
    }
}

