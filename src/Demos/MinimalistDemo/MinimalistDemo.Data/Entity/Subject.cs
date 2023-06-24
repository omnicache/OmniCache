using System;
using System.ComponentModel.DataAnnotations;
using OmniCache;

namespace MinimalistDemo.Data.Entity
{
    [Cacheable]
    public class Subject : BaseEntity<Subject>
    {
        [Key]
        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }


        public Subject()
		{
		}


        public static Query<Subject> getAllSubjectsQuery = new Query<Subject>()
                                .OrderByDesc(b => b.Name);

        public static async Task<List<Subject>> GetAllSubjectsAsync(ICachedDBSession db)
        {
            return await db.GetMultipleAsync(getAllSubjectsQuery);
        }


        public static Query<Subject> getSubjectsQuery = new Query<Subject>(
                                b => new QueryParam(1).Contains(b.Id)
                                )
                                .OrderByDesc(b => b.Name);

        public static async Task<List<Subject>> GetSubjectsAsync(ICachedDBSession db, List<long> subjectIds)
        {
            return await db.GetMultipleAsync(getSubjectsQuery, subjectIds);
        }

    }
}

