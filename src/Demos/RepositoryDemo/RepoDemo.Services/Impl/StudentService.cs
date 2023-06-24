using System;
using RepoDemo.Data.Model;
using RepoDemo.Data.Repository.Contract;
using RepoDemo.Services.Contract;

namespace RepoDemo.Services.Impl
{
	public class StudentService: IStudentService
    {
        private IStudentRepository _studentRepository;

        public StudentService(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<List<Student>> GetAllAsync()
        {
            return await _studentRepository.GetAllAsync();
        }

        public async Task AddAsync(Student student)
        {
            await _studentRepository.AddAsync(student);
        }

        public async Task UpdateAsync(Student student)
        {
            await _studentRepository.UpdateAsync(student);
        }
        
        public async Task<Student> GetById(long id)
        {
            return await _studentRepository.GetByKeyAsync(id);
        }
    }
}

