using System;
using System.ComponentModel.DataAnnotations;
using OmniCache;

namespace MinimalistDemo.Data.Entity
{
    public enum StudentStatus
    {
        Active = 1,
        Inactive
    }

    [Cacheable]
    public class Student : BaseEntity<Student>
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public String Name { get; set; }

        [Required]
        public String Email { get; set; }

        public DateTime DOB { get; set; }

        public StudentStatus Status { get; set; }


        public Student()
		{
		}

        public static Query<Student> getAllQuery = new Query<Student>(
                                s => s.Status == StudentStatus.Active
                                ).OrderByDesc(b => b.Id);


        public static async Task<List<Student>> GetAllAsync(ICachedDBSession db)
        {
            return await db.GetMultipleAsync(getAllQuery);

        }


        public static Query<Student> getStudentsQuery = new Query<Student>(
                                s => new QueryParam(1).Contains(s.Id)
                                )
                                .OrderByDesc(b => b.Name);

        public static async Task<List<Student>> GetStudentsAsync(ICachedDBSession db, List<long> studentIds)
        {
            return await db.GetMultipleAsync(getStudentsQuery, studentIds);
        }
    }
}

