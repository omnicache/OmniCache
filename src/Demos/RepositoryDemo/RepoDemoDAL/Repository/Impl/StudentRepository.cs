using System;
using OmniCache;
using RepoDemo.Data.Repository.Contract;
using RepoDemo.Data.Model;

namespace RepoDemo.Data.Repository.Impl
{
	public class StudentRepository: CachedDBRepository<Student>, IStudentRepository
	{
		public StudentRepository(ApplicationDbContext context): base(context)
		{
		}

        public static Query<Student> getAllQuery = new Query<Student>(
                                s => s.Status == StudentStatus.Active                                
                                ).OrderByDesc(b => b.Id);


        public async Task<List<Student>> GetAllAsync()
		{
			return await GetMultipleAsync(getAllQuery);

        }


        public static Query<Student> getStudentsQuery = new Query<Student>(
                                s => new QueryParam(1).Contains(s.Id)
                                )
                                .OrderByDesc(b => b.Name);

        public async Task<List<Student>> GetStudentsAsync(List<long> studentIds)
        {
            return await GetMultipleAsync(getStudentsQuery, studentIds);
        }
    }
}

