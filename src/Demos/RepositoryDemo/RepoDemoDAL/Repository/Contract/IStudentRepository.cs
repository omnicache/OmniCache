using System;
using RepoDemo.Data.Model;

namespace RepoDemo.Data.Repository.Contract
{
	public interface IStudentRepository: ICachedDBRepository<Student>
	{
        Task<List<Student>> GetAllAsync();
        Task<List<Student>> GetStudentsAsync(List<long> studentIds);
    }
}

