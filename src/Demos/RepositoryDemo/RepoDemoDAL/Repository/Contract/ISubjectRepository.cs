using System;
using RepoDemo.Data;
using RepoDemo.Data.Model;

namespace RepoDemo.Data.Repository.Contract
{
	public interface ISubjectRepository : ICachedDBRepository<Subject>
    {
        Task<List<Subject>> GetAllSubjectsAsync();
        Task<List<Subject>> GetSubjectsAsync(List<long> subjectIds);
    }
}

