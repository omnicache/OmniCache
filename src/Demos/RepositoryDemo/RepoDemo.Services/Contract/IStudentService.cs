using System;
using RepoDemo.Data.Model;

namespace RepoDemo.Services.Contract
{
	public interface IStudentService
	{
        Task<List<Student>> GetAllAsync();
        Task AddAsync(Student student);
        Task<Student> GetById(long id);
        Task UpdateAsync(Student student);
    }
}

